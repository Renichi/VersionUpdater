namespace VU
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using UnityEditor;
    using Unity.EditorCoroutines.Editor;
    using UnityEngine.Networking;
    using UnityEngine;
    using System.Collections;

    #region AppleStore.

    [System.Serializable]
    public class AppleStoreData
    {
        public string version;
        public string bundleId;
    }

    public class LoadASD
    {
        public AppleStoreData[] results;
    }

    #endregion

    public class VersionUpdater : EditorWindow
    {
        private EditorCoroutine _coroutine;
        private LoadASD inputAppleStoreJson;
        private int selector = 0;
        private string appid = "";
        private string version = "";


        private string updateversion;
        private string updatebundleid;
        private string updatebuild;

        [MenuItem("Extra/VersionUpdater")]
        public static void Create()
        {
            EditorWindow wnd = GetWindow<VersionUpdater>();
            wnd.titleContent = new GUIContent("VersionUpdater");
        }

        private void OnGUI()
        {
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            string[] platforms = { "Android", "iOS" };
            selector = EditorGUILayout.Popup("Platform", selector, platforms);
            appid = EditorGUILayout.TextField("AppID", appid);
            if (GUILayout.Button("Search Store"))
            {
                version = "";
                switch (selector)
                {
                    case 0:
                        // コルーチンを開始
                        _coroutine = this.StartCoroutine(GETGooglePlayStore( appid ));
                        break;
                    case 1:
                        _coroutine = this.StartCoroutine(GETAppleStore(appid));
                        break;
                }
            }

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            if (version != "")
            {
                EditorGUILayout.LabelField("ApplicationInfo");
                EditorGUILayout.LabelField("Store Version", version);
                EditorGUILayout.LabelField("bundleId", inputAppleStoreJson.results[0].bundleId);
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("↓");
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Suggest update information");
                var splitversion = version.Split(".");
                splitversion[2] = (int.Parse(splitversion[2]) + 1).ToString(); 
                updateversion = string.Format( "{0}.{1}.{2}",  splitversion[0],splitversion[1],splitversion[2]);
                updatebundleid = inputAppleStoreJson.results[0].bundleId;
                updatebuild = "0";
                updateversion = EditorGUILayout.TextField("Version", updateversion);
                updatebuild = EditorGUILayout.TextField("Build", updatebuild);
                updatebundleid = EditorGUILayout.TextField("bundleId", updatebundleid);
                
                if (GUILayout.Button("Update"))
                {

                }
            }
            else
            {
                EditorGUILayout.LabelField("Nothing...");
            }
        }

        #region Coroutine.

        private IEnumerator GETGooglePlayStore( string appPackageId )
        {
            var playStoreUrl = $"https://play.google.com/store/apps/details?id={appPackageId}";
            Debug.Log(playStoreUrl);
            using (var req = UnityWebRequest.Get(playStoreUrl))
            {
                yield return req.SendWebRequest();
                if (req.isNetworkError)
                {
                    Debug.Log(req.error);
                }
                else if (req.isHttpError)
                {
                    Debug.Log(req.error);
                }
                else
                {
                    //resultText.text = req.downloadHandler.text;
                    Debug.Log(req.downloadHandler.text);
                    var match = Regex.Match(req.downloadHandler.text, @"<script class=""ds:\d+"" nonce=""[^""]+"">AF_initDataCallback\(.+?\[\[\[""(?\d+\.\d[\d\.]*)""\]\].+?\);</script>");
                   
                   // var ver = match.Groups["ver"].Value;
                   // Debug.Log(ver);
                }
            }
        }

        private IEnumerator GETAppleStore(string appPackageId)
        {
            var playStoreUrl = $"https://itunes.apple.com/lookup?id={appPackageId}&country=JP";
            Debug.Log(playStoreUrl);
            using (var req = UnityWebRequest.Get(playStoreUrl))
            {
                yield return req.SendWebRequest();
                if (req.isNetworkError)
                {
                    Debug.Log(req.error);
                }
                else if (req.isHttpError)
                {
                    Debug.Log(req.error);
                }
                else
                {
                    //resultText.text = req.downloadHandler.text;
                    Debug.Log(req.downloadHandler.text);
                    inputAppleStoreJson = JsonUtility.FromJson<LoadASD>(req.downloadHandler.text);
                    version = inputAppleStoreJson.results[0].version;
                }
            }
        }

        #endregion
    }
}
    