using System;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using UnityEngine;

//-----------------------------------------------------------------------------
[Serializable]
public struct LocalVersionInfo
{
    public string data_version;
    public string remote_version_url;
}

//-----------------------------------------------------------------------------
public struct RemoteVersionInfo
{
    public string bundle_version;
    public string bundle_url;
    public string data_version;
    public string data_url;
    public _eBundleState bundle_state;
    public AutoPatcherServerState server_state;
}

public class VersionInfo
{
    //-------------------------------------------------------------------------
    public string LocalBundleVersion { get; private set; }
    public LocalVersionInfo LocalVersionInfo { get; private set; }
    public RemoteVersionInfo RemoteVersionInfo { get; private set; }
    public Dictionary<string, string> MapLocalAssetInfo { get; private set; }
    public Dictionary<string, string> MapRemoteAssetInfo { get; private set; }
    public string LocalAssetInfoPath { get; private set; }

    //-------------------------------------------------------------------------
    public VersionInfo(string local_assetinfo_path)
    {
        LocalBundleVersion = Application.version;
        LocalAssetInfoPath = local_assetinfo_path;
        MapLocalAssetInfo = new Dictionary<string, string>();
        MapRemoteAssetInfo = new Dictionary<string, string>();

        // Load Local VersionInfo
        string data = PlayerPrefs.GetString(AutoPatcherStringDef.PlayerPrefsKeyLocalVersionInfo);
        if (string.IsNullOrEmpty(data))
        {
            LocalVersionInfo info;
            info.data_version = "";
            info.remote_version_url = "";
            LocalVersionInfo = info;
        }
        else
        {
            LocalVersionInfo = EbTool.jsonDeserialize<LocalVersionInfo>(data);
        }

        // Parse Local DataFileList
        if (File.Exists(LocalAssetInfoPath))
        {
            // 读取本地DataFileList.txt文件成功
            _parseDataFileList(File.ReadAllText(LocalAssetInfoPath), true);
        }
        else
        {
            // 读取本地DataFileList.txt文件失败
            LocalVersionInfo info = LocalVersionInfo;
            info.data_version = "";
            LocalVersionInfo = info;
            data = EbTool.jsonSerialize(LocalVersionInfo);
            PlayerPrefs.SetString(AutoPatcherStringDef.PlayerPrefsKeyLocalVersionInfo, data);
        }
    }

    //-------------------------------------------------------------------------
    public void changeLocalDataVersionToRemote()
    {
        LocalVersionInfo info;
        info.data_version = RemoteVersionInfo.data_version;
        info.remote_version_url = "";
        LocalVersionInfo = info;

        string data = EbTool.jsonSerialize(LocalVersionInfo);

        PlayerPrefs.SetString(AutoPatcherStringDef.PlayerPrefsKeyLocalVersionInfo, data);
    }

    //-------------------------------------------------------------------------
    public void parseRemoteVersionInfo(string remote_version_info_text)
    {
        XmlDocument xml = new XmlDocument();
        xml.LoadXml(remote_version_info_text);

        XmlNode version_node =
#if UNITY_ANDROID
        xml.SelectSingleNode("VersionInfo/Android");
#elif UNITY_IPHONE
        xml.SelectSingleNode("VersionInfo/IOS");
#else
        xml.SelectSingleNode("VersionInfo/PC");
#endif
        XmlNode server_state_node = xml.SelectSingleNode("VersionInfo/ServerState");

        List<RemoteVersionInfo> list_remote_versions = new List<RemoteVersionInfo>();

        _eBundleState local_bundlestate = _eBundleState.Production;
        foreach (XmlNode i in version_node.ChildNodes)
        {
            RemoteVersionInfo server_config;
            string remote_version = i.Attributes["Version"].Value;
            server_config.bundle_version = remote_version;
            server_config.bundle_url = i.Attributes["BundleURL"].Value;
            server_config.data_version = i.Attributes["DataVersion"].Value;
            server_config.data_url = i.Attributes["DataURL"].Value;
            server_config.server_state = (AutoPatcherServerState)int.Parse(server_state_node.Attributes["state"].Value);
            _eBundleState bundle_state = (_eBundleState)int.Parse(i.Attributes["State"].Value);
            server_config.bundle_state = bundle_state;

            if (remote_version.Equals(LocalBundleVersion))
            {
                local_bundlestate = bundle_state;
            }

            if (local_bundlestate == bundle_state)
            {
                list_remote_versions.Add(server_config);
            }
        }

        list_remote_versions.Sort((x, y) =>
        {
            int x_b = int.Parse(x.bundle_version.Replace(".", ""));
            int y_b = int.Parse(y.bundle_version.Replace(".", ""));
            if (x_b > y_b)
            {
                return -1;
            }
            else if (x_b < y_b)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        });

        RemoteVersionInfo = list_remote_versions[0];
    }

    //-------------------------------------------------------------------------
    public void parseRemoteDataFileList(string pack_info)
    {
        _parseDataFileList(pack_info, false);
    }

    //-------------------------------------------------------------------------
    public bool mustUpdateBundleVersion()
    {
#if UNITY_EDITOR
        return false;
#else
         int remote_version = int.Parse(RemoteVersionInfo.bundle_version.Replace(".", ""));
         int local_version = int.Parse(LocalBundleVersion.Replace(".", ""));

        if (remote_version > local_version)
        {
            return true;
        }

        return false;
#endif
    }

    //-------------------------------------------------------------------------
    public bool mustUpdateDataVersion()
    {
        string local_dataversion = LocalVersionInfo.data_version;
        if (string.IsNullOrEmpty(local_dataversion))
        {
            return true;
        }

        int remote_version = int.Parse(RemoteVersionInfo.data_version.Replace(".", ""));
        int local_version = int.Parse(local_dataversion.Replace(".", ""));

        if (remote_version > local_version)
        {
            return true;
        }

        return false;
    }

    //-------------------------------------------------------------------------
    void _parseDataFileList(string data_filelist_text, bool is_local)
    {
        string[] infos = data_filelist_text.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
        foreach (var i in infos)
        {
            if (string.IsNullOrEmpty(i))
            {
                continue;
            }

            string[] info = i.Split(' ');
            if (is_local)
            {
                if (info.Length != 2)
                {
                    //FloatMsgInfo msg_info;
                    //msg_info.msg = "本地资源文件损坏,请修复文件!";
                    //msg_info.color = Color.red;
                    //UiMgr.Instance.FloatMsgMgr.createFloatMsg(msg_info);
                }
                else
                {
                    MapLocalAssetInfo[info[0]] = info[1];
                }
            }
            else
            {
                if (info.Length != 2)
                {
                    //FloatMsgInfo msg_info;
                    //msg_info.msg = "资源文件损坏!";
                    //msg_info.color = Color.red;
                    //UiMgr.Instance.FloatMsgMgr.createFloatMsg(msg_info);
                }
                else
                {
                    MapRemoteAssetInfo[info[0]] = info[1];
                }
            }
        }
    }
}

//-------------------------------------------------------------------------
public enum _eBundleState
{
    Examine = 0,
    Production,
}
