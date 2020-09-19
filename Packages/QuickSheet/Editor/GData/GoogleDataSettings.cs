///////////////////////////////////////////////////////////////////////////////
///
/// GoogleDataSettings.cs
/// 
/// (c)2013 Kim, Hyoun Woo
///
///////////////////////////////////////////////////////////////////////////////
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace UnityQuickSheet
{
    /// <summary>
    /// A class manages google account setting.
    /// </summary>
    // [CreateAssetMenu(menuName = "QuickSheet/Setting/GoogleData Setting")]
    public class GoogleDataSettings : SingletonScriptableObject<GoogleDataSettings>
    {
        static int wasPreferencesDirCreated = 0;
        static int wasPreferencesAssetCreated = 0;
        public const string GOOGLEDATA_SETTINGS_ASSET_PATH = "Assets/Editor/GoogleDataSetting.asset";

        // A flag which indicates use local installed oauth2 json file for authentication or not.
        static public bool useOAuth2JsonFile = false;

        public string JsonFilePath
        {
            get { return jsonFilePath; }
            set
            {
                if (string.IsNullOrEmpty(value) == false)
                    jsonFilePath = value;
            }
        }
        private string jsonFilePath = string.Empty;

        /// <summary>
        /// A default path where .txt template files are.
        /// </summary>
        public string TemplatePath = "Packages/com.daily.quicksheet/Runtime/GData/Templates";

        /// <summary>
        /// A path where generated ScriptableObject derived class and its data class script files are to be put.
        /// </summary>
        public string RuntimePath = string.Empty;

        /// <summary>
        /// A path where generated editor script files are to be put.
        /// </summary>
        public string EditorPath = string.Empty;

        [System.Serializable]
        public struct OAuth2JsonData
        {
            public string client_id;
            public string auth_uri;
            public string token_uri;
            public string auth_provider_x509_cert_url;
            public string client_secret;
            public List<string> redirect_uris;
        };

        public OAuth2JsonData OAuth2Data;

        // enter Access Code after getting it from auth url
        public string _AccessCode = "Paste AcecessCode here!";

        // enter Auth 2.0 Refresh Token and AccessToken after succesfully authorizing with Access Code
        public string _RefreshToken = "";

        public string _AccessToken = "";

        /// <summary>
        /// Select currently exist account setting asset file.
        /// </summary>
        [MenuItem("Tools/QuickSheet/Select Google Data Setting")]
        public static void Edit()
        {
            Selection.activeObject = Instance;
            if (Selection.activeObject == null)
            {
                LoadOrCreate();
            }
        }

        public static GoogleDataSettings GetSettings()
        {
            if (Instance == null)
            {
                LoadOrCreate();
            }
            return Instance;
        }

        public static void LoadOrCreate()
        {
            var settings = AssetDatabase.LoadAssetAtPath<GoogleDataSettings>(GOOGLEDATA_SETTINGS_ASSET_PATH);
            if (settings == null)
            {
                settings = ScriptableObject.CreateInstance<GoogleDataSettings>();

                if (!AssetDatabase.IsValidFolder("Assets/Editor") && System.Threading.Interlocked.Exchange(ref wasPreferencesDirCreated, 1) == 0)
                    AssetDatabase.CreateFolder("Assets", "Editor");
                if (System.Threading.Interlocked.Exchange(ref wasPreferencesAssetCreated, 1) == 0)
                    AssetDatabase.CreateAsset(settings, GOOGLEDATA_SETTINGS_ASSET_PATH);
                Debug.Log($"GoogleDataSettings.asset file has created at {GOOGLEDATA_SETTINGS_ASSET_PATH}.");
            }
        }
    }
}