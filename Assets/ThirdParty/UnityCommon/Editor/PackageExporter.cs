using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace UnityCommon
{
    [InitializeOnLoad]
    public class PackageExporter : EditorWindow
    {
        public interface IProcessor
        {
            void OnPackagePreProcess ();
            void OnPackagePostProcess ();
        }

        private static string PackageName { get { return PlayerPrefs.GetString(prefsPrefix + "PackageName"); } set { PlayerPrefs.SetString(prefsPrefix + "PackageName", value); } }
        private static string Copyright { get { return PlayerPrefs.GetString(prefsPrefix + "Copyright"); } set { PlayerPrefs.SetString(prefsPrefix + "Copyright", value); } }
        private static string LicenseFilePath { get { return PlayerPrefs.GetString(prefsPrefix + "LicenseFilePath"); } set { PlayerPrefs.SetString(prefsPrefix + "LicenseFilePath", value); } }
        private static string LicenseAssetPath { get { return AssetsPath + "/" + defaultLicenseFileName + ".txt"; } }
        private static string AssetsPath { get { return "Assets/" + PackageName; } }
        private static string OutputPath { get { return PlayerPrefs.GetString(prefsPrefix + "OutputPath"); } set { PlayerPrefs.SetString(prefsPrefix + "OutputPath", value); } }
        private static string OutputFileName { get { return PackageName; } }
        private static string IgnoredAssetGUIds { get { return PlayerPrefs.GetString(prefsPrefix + "IgnoredAssetGUIds"); } set { PlayerPrefs.SetString(prefsPrefix + "IgnoredAssetGUIds", value); } }
        private static bool IsAnyPathsIgnored { get { return !string.IsNullOrEmpty(IgnoredAssetGUIds); } }
        private static bool IsReadyToExport { get { return !string.IsNullOrEmpty(OutputPath) && !string.IsNullOrEmpty(OutputFileName); } }

        private const string prefsPrefix = "PackageExporter.";
        private const string autoRefreshKey = "kAutoRefresh";
        private const string defaultLicenseFileName = "LICENSE";

        private static Dictionary<string, string> modifiedScripts = new Dictionary<string, string>();
        private static List<UnityEngine.Object> ignoredAssets = new List<UnityEngine.Object>();
        private static SceneSetup[] sceneSetup = null;

        public static void AddIgnoredAsset (string assetPath)
        {
            var guid = AssetDatabase.AssetPathToGUID(assetPath);
            if (!IgnoredAssetGUIds.Contains(guid)) IgnoredAssetGUIds += "," + guid;
        }

        public static void RemoveIgnoredAsset (string assetPath)
        {
            var guid = AssetDatabase.AssetPathToGUID(assetPath);
            if (!IgnoredAssetGUIds.Contains(guid)) IgnoredAssetGUIds = IgnoredAssetGUIds.Replace(guid, string.Empty);
        }

        private void Awake ()
        {
            if (string.IsNullOrEmpty(PackageName))
                PackageName = Application.productName;
            if (string.IsNullOrEmpty(LicenseFilePath))
                LicenseFilePath = Application.dataPath.Replace("Assets", "") + defaultLicenseFileName;
        }

        private void OnEnable ()
        {
            DeserealizeIgnoredAssets();
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
                ExportPackageImpl();
        }

        private void OnGUI ()
        {
            EditorGUILayout.LabelField("Package Exporter Settings", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Settings are stored in editor's PlayerPrefs and won't be exposed in builds or project assets.", MessageType.Info);
            EditorGUILayout.Space();
            PackageName = EditorGUILayout.TextField("Package Name", PackageName);
            Copyright = EditorGUILayout.TextField("Copyright Notice", Copyright);
            LicenseFilePath = EditorGUILayout.TextField("License File Path", LicenseFilePath);
            using (new EditorGUILayout.HorizontalScope())
            {
                OutputPath = EditorGUILayout.TextField("Output Path", OutputPath);
                if (GUILayout.Button("Select", EditorStyles.miniButton, GUILayout.Width(65)))
                    OutputPath = EditorUtility.OpenFolderPanel("Output Path", "", "");
            }
            EditorGUILayout.Space();

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.LabelField("Ignored folders: ");
            for (int i = 0; i < ignoredAssets.Count; i++)
                ignoredAssets[i] = EditorGUILayout.ObjectField(ignoredAssets[i], typeof(UnityEngine.Object), false);
            if (GUILayout.Button("+")) ignoredAssets.Add(null);
            if (EditorGUI.EndChangeCheck()) SerializeIgnoredAssets();
        }

        private static void SerializeIgnoredAssets ()
        {
            var ignoredAseetsGUIDs = new List<string>();
            foreach (var asset in ignoredAssets)
            {
                if (!asset) continue;
                var assetPath = AssetDatabase.GetAssetPath(asset);
                var assetGUID = AssetDatabase.AssetPathToGUID(assetPath);
                ignoredAseetsGUIDs.Add(assetGUID);
            }
            IgnoredAssetGUIds = string.Join(",", ignoredAseetsGUIDs.ToArray());
        }

        private static void DeserealizeIgnoredAssets ()
        {
            ignoredAssets.Clear();
            var ignoredAseetsGUIDs = IgnoredAssetGUIds.Split(',');
            foreach (var guid in ignoredAseetsGUIDs)
            {
                if (string.IsNullOrEmpty(guid)) continue;
                var assetPath = AssetDatabase.GUIDToAssetPath(guid);
                var asset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(assetPath);
                if (asset) ignoredAssets.Add(asset);
            }
        }

        private static bool IsAssetIgnored (string assetPath)
        {
            var guid = AssetDatabase.AssetPathToGUID(assetPath);
            return IgnoredAssetGUIds.Contains(guid);
        }

        private static void ExportPackageImpl ()
        {
            DisplayProgressBar("Preparing for export...", 0f);

            // Disable auto recompile.
            var wasAutoRefreshEnabled = EditorPrefs.GetBool(autoRefreshKey);
            EditorPrefs.SetBool(autoRefreshKey, false);

            // Load a temp scene and unload assets to prevent reference errors.
            sceneSetup = EditorSceneManager.GetSceneManagerSetup();
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene);
            EditorUtility.UnloadUnusedAssetsImmediate(true);

            DisplayProgressBar("Pre-processing assets...", 0f);
            var processors = GetProcessors();
            foreach (var proc in processors)
                proc.OnPackagePreProcess();

            var assetPaths = AssetDatabase.GetAllAssetPaths().Where(p => p.StartsWith(AssetsPath));
            var ignoredPaths = assetPaths.Where(p => IsAssetIgnored(p));
            var unignoredPaths = assetPaths.Where(p => !IsAssetIgnored(p));

            // Temporary hide ignored assets.
            DisplayProgressBar("Hiding ignored assets...", .1f);
            if (IsAnyPathsIgnored)
            {
                foreach (var path in ignoredPaths)
                    File.SetAttributes(path, File.GetAttributes(path) | FileAttributes.Hidden);
                AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            }

            // Add license file.
            var needToAddLicense = File.Exists(LicenseFilePath);
            if (needToAddLicense)
            {
                File.Copy(LicenseFilePath, LicenseAssetPath);
                AssetDatabase.ImportAsset(LicenseAssetPath, ImportAssetOptions.ForceSynchronousImport);
            }

            // Modify scripts (namespace and copyright).
            DisplayProgressBar("Modifying scripts...", .25f);
            modifiedScripts.Clear();
            var needToModify = !string.IsNullOrEmpty(Copyright);
            if (needToModify)
            {
                foreach (var path in unignoredPaths)
                {
                    if (!path.EndsWith(".cs") && !path.EndsWith(".shader") && !path.EndsWith(".cginc")) continue;

                    var fullpath = Application.dataPath.Replace("Assets", string.Empty) + path;
                    var originalScriptText = File.ReadAllText(fullpath, Encoding.UTF8);

                    string scriptText = string.Empty;
                    var isImportedScript = path.Contains("ThirdParty");

                    var copyright = isImportedScript || string.IsNullOrEmpty(Copyright) ? string.Empty : "// " + Copyright;
                    if (!string.IsNullOrEmpty(copyright) && !isImportedScript)
                        scriptText += copyright + "\r\n\r\n";

                    scriptText += originalScriptText;

                    File.WriteAllText(fullpath, scriptText, Encoding.UTF8);

                    modifiedScripts.Add(fullpath, originalScriptText);
                }
            }

            // Export the package.
            DisplayProgressBar("Writing package file...", .5f);
            AssetDatabase.ExportPackage(AssetsPath, OutputPath + "/" + OutputFileName + ".unitypackage", ExportPackageOptions.Recurse);

            // Restore modified scripts.
            DisplayProgressBar("Restoring modified scripts...", .75f);
            if (needToModify)
            {
                foreach (var modifiedScript in modifiedScripts)
                    File.WriteAllText(modifiedScript.Key, modifiedScript.Value, Encoding.UTF8);
            }

            // Remove previously added license asset.
            if (needToAddLicense) AssetDatabase.DeleteAsset(LicenseAssetPath);

            // Un-hide ignored assets.
            DisplayProgressBar("Un-hiding ignored assets...", .95f);
            if (IsAnyPathsIgnored)
            {
                foreach (var path in ignoredPaths)
                    File.SetAttributes(path, File.GetAttributes(path) & ~FileAttributes.Hidden);
                AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            }

            DisplayProgressBar("Post-processing assets...", 1f);
            foreach (var proc in processors)
                proc.OnPackagePostProcess();

            EditorPrefs.SetBool(autoRefreshKey, wasAutoRefreshEnabled);
            EditorSceneManager.RestoreSceneManagerSetup(sceneSetup);

            EditorUtility.ClearProgressBar();
        }

        private static void DisplayProgressBar (string activity, float progress)
        {
            EditorUtility.DisplayProgressBar(string.Format("Exporting {0}", PackageName), activity, progress);
        }

        private static IEnumerable<IProcessor> GetProcessors ()
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => typeof(IProcessor).IsAssignableFrom(t) && t.IsClass)
                .Select(t => (IProcessor)Activator.CreateInstance(t));
        }
    }
}
