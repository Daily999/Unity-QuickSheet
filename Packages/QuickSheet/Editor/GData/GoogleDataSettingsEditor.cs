///////////////////////////////////////////////////////////////////////////////
///
/// GoogleDataSettingsEditor.cs
/// 
/// (c)2013 Kim, Hyoun Woo
///
///////////////////////////////////////////////////////////////////////////////
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace UnityQuickSheet
{
    /// <summary>
    /// Resolve TlsException error.
    /// </summary>
    public class UnsafeSecurityPolicy
    {
        public static bool Validator(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors policyErrors)
        {
            return true;
        }

        public static void Instate()
        {
            ServicePointManager.ServerCertificateValidationCallback = Validator;
        }
    }

    /// <summary>
    /// Editor script class for GoogleDataSettings scriptable object to hide password of google account.
    /// </summary>
    [CustomEditor(typeof(GoogleDataSettings))]
    public class GoogleDataSettingsEditor : Editor
    {
        private VisualElement inputGroup;
        private TextField templatePath;
        private TextField runtimePath;
        private TextField editorPath;

        public void OnEnable()
        {
            // resolve TlsException error
            UnsafeSecurityPolicy.Instate();
        }

        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();

            var quickToolVisualTree = Resources.Load<VisualTreeAsset>("GoogleSetting");
            quickToolVisualTree.CloneTree(root);
            inputGroup = root.Q("Input");

            var client = root.Q<TextField>("Client_ID");
            client.RegisterCallback<ChangeEvent<string>>((evt) => OnClientIDChanged(evt.newValue));
            client.value = GoogleDataSettings.Instance.OAuth2Data.client_id;

            var secret = root.Q<TextField>("Client_Secret");
            secret.RegisterCallback<ChangeEvent<string>>((evt) => OnClientSecretChanged(evt.newValue));
            secret.value = GoogleDataSettings.Instance.OAuth2Data.client_secret;

            var access = root.Q<TextField>("AccessCode");
            access.RegisterCallback<ChangeEvent<string>>((evt) => OnAccessCodeChanged(evt.newValue));
            access.value = GoogleDataSettings.Instance._AccessCode;

            templatePath = root.Q<TextField>("TemplatePath");
            templatePath.RegisterCallback<ChangeEvent<string>>((evt) => OnTemplatePathChanged(evt.newValue));
            templatePath.value = GoogleDataSettings.Instance.TemplatePath;

            runtimePath = root.Q<TextField>("RuntimePath");
            runtimePath.RegisterCallback<ChangeEvent<string>>((evt) => OnRuntimePathChanged(evt.newValue));
            runtimePath.value = GoogleDataSettings.Instance.RuntimePath;

            editorPath = root.Q<TextField>("EditorPath");
            editorPath.RegisterCallback<ChangeEvent<string>>((evt) => OnEditorPathChanged(evt.newValue));
            editorPath.value = GoogleDataSettings.Instance.EditorPath;

            var webBT = root.Q<Button>("OpenWeb");
            webBT.clicked += OpenGoogleAPIInBrowser;

            var start = root.Q<Button>("Start_Auth");
            start.clicked += () =>
            {
                GDataDB.Impl.GDataDBRequestFactory.InitAuthenticate();
            };

            var finish = root.Q<Button>("Finish_Auth");
            finish.clicked += () =>
            {
                try
                {
                    GDataDB.Impl.GDataDBRequestFactory.FinishAuthenticate();
                }
                catch (Exception e)
                {
                    EditorUtility.DisplayDialog("Error", e.Message, "OK");
                }
            };

            var reset = root.Q<Button>("Reset");
            reset.clicked += () =>
            {
                GoogleDataSettings.Instance.OAuth2Data.client_id = string.Empty;
                GoogleDataSettings.Instance.OAuth2Data.client_secret = string.Empty;
                GoogleDataSettings.Instance._AccessCode = string.Empty;
                GoogleDataSettings.Instance._RefreshToken = string.Empty;
                GoogleDataSettings.Instance._AccessToken = string.Empty;
            };

            var templateSelect = root.Q<Button>("TemplateSelect");
            templateSelect.clicked += OnTemplateSelect;
            var runtimeSelect = root.Q<Button>("RuntimeSelect");
            runtimeSelect.clicked += OnRuntimeSelect;
            var editorSelect = root.Q<Button>("EditorSelect");
            editorSelect.clicked += OnEditorSelect;

            OnClientIDChanged(GoogleDataSettings.Instance.OAuth2Data.client_id);
            OnClientSecretChanged(GoogleDataSettings.Instance.OAuth2Data.client_secret);
            OnAccessCodeChanged(GoogleDataSettings.Instance._AccessCode);
            OnEditorPathChanged(GoogleDataSettings.Instance.EditorPath);
            OnRuntimePathChanged(GoogleDataSettings.Instance.RuntimePath);
            OnTemplatePathChanged(GoogleDataSettings.Instance.TemplatePath);

            return root;
        }
        private void OpenGoogleAPIInBrowser()
        {
            if (EditorUtility.DisplayDialog("Message", "Open Web in browser?", "Yes", "No"))
            {
                Application.OpenURL("https://kimsama.gitbooks.io/unity-quicksheet/content/google-oauth2/");
                Application.OpenURL("http://console.developers.google.com/");
            }
        }
        private void OnClientIDChanged(string value)
        {
            if (value.Length == 0) return;
            GoogleDataSettings.Instance.OAuth2Data.client_id = value;
            EditorUtility.SetDirty(GoogleDataSettings.Instance);
            AssetDatabase.SaveAssets();
        }
        private void OnClientSecretChanged(string value)
        {
            if (value.Length == 0) return;
            GoogleDataSettings.Instance.OAuth2Data.client_secret = value;
            EditorUtility.SetDirty(GoogleDataSettings.Instance);
            AssetDatabase.SaveAssets();
        }
        private void OnAccessCodeChanged(string value)
        {
            if (value.Length == 0) return;
            GoogleDataSettings.Instance._AccessCode = value;
            EditorUtility.SetDirty(GoogleDataSettings.Instance);
            AssetDatabase.SaveAssets();
        }
        private void OnEditorPathChanged(string value)
        {
            if (value.Length == 0) return;
            GoogleDataSettings.Instance.EditorPath = value;
            EditorUtility.SetDirty(GoogleDataSettings.Instance);
            AssetDatabase.SaveAssets();
        }
        private void OnRuntimePathChanged(string value)
        {
            if (value.Length == 0) return;
            GoogleDataSettings.Instance.RuntimePath = value;
            EditorUtility.SetDirty(GoogleDataSettings.Instance);
            AssetDatabase.SaveAssets();
        }
        private void OnTemplatePathChanged(string value)
        {
            if (value.Length == 0) return;
            GoogleDataSettings.Instance.TemplatePath = value;
            EditorUtility.SetDirty(GoogleDataSettings.Instance);
            AssetDatabase.SaveAssets();
        }
        private void OnTemplateSelect()
        {
            var path = SelectPath("Select TemplatePath");
            if (path.Length > 0)
                OnTemplatePathChanged(path);
            templatePath.value = GoogleDataSettings.Instance.TemplatePath;
        }
        private void OnRuntimeSelect()
        {
            var path = (SelectPath("Select RuntimePath"));
            if (path.Length > 0)
                OnRuntimePathChanged(path);
            runtimePath.value = GoogleDataSettings.Instance.RuntimePath;
        }
        private void OnEditorSelect()
        {
            var path = SelectPath("Select EditorPath");
            if (path.Length > 0)
                OnEditorPathChanged(path);
            editorPath.value = GoogleDataSettings.Instance.EditorPath;
        }
        private string SelectPath(string title)
        {
            return EditorUtility.OpenFolderPanel(title, Application.dataPath, "");
        }

    }
}