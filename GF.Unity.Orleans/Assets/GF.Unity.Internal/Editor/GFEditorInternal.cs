using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml;

public class GFEditorInternal : EditorWindow
{
    //-------------------------------------------------------------------------
    [MenuItem("GF.Unity/导出GF.Unity.Orleans.unitypackage")]
    static void exportGFJsonPackage()
    {
        string[] arr_assetpathname = new string[1];
        arr_assetpathname[0] = "Assets/GF.Unity.Orleans";
        AssetDatabase.ExportPackage(arr_assetpathname, "GF.Unity.Orleans.unitypackage", ExportPackageOptions.Recurse);

        Debug.Log("Export GF.Unity.Orleans.unitypackage Finished!");
    }
}
