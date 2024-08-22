namespace VU
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using UnityEditor;
    using UnityEngine;

    public class VersionUpdater : EditorWindow
    {
        [MenuItem("Extra/VersionUpdater")]
        public static void Create()
        {
            EditorWindow wnd = GetWindow<VersionUpdater>();
            wnd.titleContent = new GUIContent("VersionUpdater");
        }
    }
}