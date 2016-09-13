using System;
using System.IO;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GameCloudEditorInitProjectInfo : EditorWindow
{
    //-------------------------------------------------------------------------    
    string mComponyName;
    string mProductName;
    string mBundleIdentifier;
    string mBundleVersion;
    string mDataVersion;
    string mPatchInfoTargetDirectory;
    string mPatchInfoResouceDirectory;

    //-------------------------------------------------------------------------
    public void copyPatchInfo(string patch_infotargetdirectory, string patch_inforesoucedirectory)
    {
        mPatchInfoTargetDirectory = patch_infotargetdirectory;
        mPatchInfoResouceDirectory = patch_inforesoucedirectory;
    }

    //-------------------------------------------------------------------------
    void _deleteDirectory(string path)
    {
        if (Directory.Exists(path))
        {
            Directory.Delete(path);
        }
    }

    //-------------------------------------------------------------------------
    void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("公司名:");
        mComponyName = EditorGUILayout.TextField(mComponyName);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("产品名(App名称):");
        mProductName = EditorGUILayout.TextField(mProductName);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("BundleIdentify:");
        mBundleIdentifier = EditorGUILayout.TextField(mBundleIdentifier);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("初始程序版本号(请以*.**.***，其中*为数字来设置):");
        mBundleVersion = EditorGUILayout.TextField(mBundleVersion);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("初始资源版本号(请以*.**.***，其中*为数字来设置):");
        mDataVersion = EditorGUILayout.TextField(mDataVersion);
        EditorGUILayout.EndHorizontal();

        bool init = GUILayout.Button("设置", GUILayout.Width(200));
        if (init)
        {
            _initProject();
            ShowNotification(new GUIContent("初始化成功!"));
        }
    }

    //-------------------------------------------------------------------------
    void _initProject()
    {
        try
        {
            Directory.CreateDirectory(mPatchInfoTargetDirectory);
        }
        catch (Exception e)
        {
            _deleteDirectory(mPatchInfoTargetDirectory);
            Debug.LogError("GameCloudEditorInitProjectInfo CreateDirectory Error::" + e.Message);
        }

        try
        {
            GameCloudEditor.copyFile(mPatchInfoResouceDirectory, mPatchInfoTargetDirectory, mPatchInfoResouceDirectory);
        }
        catch (Exception e)
        {
            _deleteDirectory(mPatchInfoTargetDirectory);
            Debug.LogError("GameCloudEditorInitProjectInfo copyFile Error::" + e.Message);
        }

        PlayerSettings.companyName = mComponyName;
        PlayerSettings.productName = mProductName;
        PlayerSettings.bundleIdentifier = mBundleIdentifier;
        GameCloudEditor.changeBundleData(mBundleVersion, true);
        GameCloudEditor.changeDataData(mDataVersion, true);
    }
}
