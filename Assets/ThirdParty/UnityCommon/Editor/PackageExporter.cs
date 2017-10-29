// Copyright 2012-2017 Elringus (Artyom Sovetnikov). All Rights Reserved.

namespace UnityCommon
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using UnityEditor;
    using UnityEngine;
    
    [InitializeOnLoad]
    public class PackageExporter : EditorWindow
    {
        protected static string PackageName { get { return PlayerPrefs.GetString(PREFS_PREFIX + "PackageName"); } set { PlayerPrefs.SetString(PREFS_PREFIX + "PackageName", value); } }
        protected static string Copyright { get { return PlayerPrefs.GetString(PREFS_PREFIX + "Copyright"); } set { PlayerPrefs.SetString(PREFS_PREFIX + "Copyright", value); } }
        protected static string AssetsPath { get { return "Assets/" + PackageName; } }
        protected static string OutputPath { get { return PlayerPrefs.GetString(PREFS_PREFIX + "OutputPath"); } set { PlayerPrefs.SetString(PREFS_PREFIX + "OutputPath", value); } }
        protected static string OutputFileName { get { return PackageName; } }
        protected static string NamespaceToWrap { get { return PackageName; } }
        protected static bool IsReadyToExport { get { return !string.IsNullOrEmpty(OutputPath) && !string.IsNullOrEmpty(OutputFileName); } }
    
        private const string PREFS_PREFIX = "PackageExporter.";
        private const string TAB_CHARS = "    ";
    
        private static Dictionary<string, string> modifiedScripts = new Dictionary<string, string>();
    
        private void Awake ()
        {
            if (string.IsNullOrEmpty(PackageName))
                PackageName = Application.productName;
        }
    
        [MenuItem("Edit/Project Settings/Package Exporter")]
        private static void OpenSettingsWindow ()
        {
            var window = GetWindow<PackageExporter>();
            window.Show();
        }
    
        [MenuItem("Assets/+ Export Package", priority = 20)]
        private static void ExportPackage ()
        {
            if (IsReadyToExport)
                Export();
        }
    
        [MenuItem("Assets/+ Export Package (Wrap)", priority = 20)]
        private static void ExportPackageStore ()
        {
            if (IsReadyToExport)
                Export(true);
        }
    
        private void OnGUI ()
        {
            EditorGUILayout.LabelField("Package Exporter Settings", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Settings are stored in editor's PlayerPrefs and won't be exposed in builds or project assets.", EditorStyles.miniLabel);
            EditorGUILayout.Space();
            PackageName = EditorGUILayout.TextField("Package Name", PackageName);
            Copyright = EditorGUILayout.TextField("Copyright Notice", Copyright);
            using (new EditorGUILayout.HorizontalScope())
            {
                OutputPath = EditorGUILayout.TextField("Output Path", OutputPath);
                if (GUILayout.Button("Select", EditorStyles.miniButton, GUILayout.Width(65)))
                    OutputPath = EditorUtility.OpenFolderPanel("Output Path", "", "");
            }
        }
    
        private static void Export (bool wrapNamespace = false)
        {
            if (!string.IsNullOrEmpty(NamespaceToWrap))
            {
                foreach (var path in AssetDatabase.GetAllAssetPaths())
                {
                    if (!path.StartsWith(AssetsPath)) continue;
                    if (!path.EndsWith(".cs")) continue;
    
                    var fullpath = Application.dataPath.Replace("Assets", "") + path;
                    var originalScriptText = File.ReadAllText(fullpath, Encoding.UTF8);
    
                    string scriptText = string.Empty;
                    var isImportedScript = path.Contains("ThirdParty");
                    var copyright = isImportedScript || string.IsNullOrEmpty(Copyright) ? string.Empty : "// " + Copyright;
                    if (!isImportedScript || wrapNamespace) scriptText += copyright + Environment.NewLine + Environment.NewLine + "namespace " + NamespaceToWrap + Environment.NewLine + "{" + Environment.NewLine;
                    scriptText += isImportedScript ? originalScriptText : TAB_CHARS + originalScriptText.Replace(Environment.NewLine, Environment.NewLine + TAB_CHARS);
                    if (!isImportedScript || wrapNamespace) scriptText += Environment.NewLine + "}" + Environment.NewLine;
                    File.WriteAllText(fullpath, scriptText, Encoding.UTF8);
    
                    modifiedScripts.Add(fullpath, originalScriptText);
                }
            }
    
            AssetDatabase.ExportPackage(AssetsPath, OutputPath + "/" + OutputFileName + ".unitypackage", ExportPackageOptions.Recurse);
    
            if (!string.IsNullOrEmpty(NamespaceToWrap))
            {
                foreach (var modifiedScript in modifiedScripts)
                    File.WriteAllText(modifiedScript.Key, modifiedScript.Value, Encoding.UTF8);
            }
        }
    }
    
}
