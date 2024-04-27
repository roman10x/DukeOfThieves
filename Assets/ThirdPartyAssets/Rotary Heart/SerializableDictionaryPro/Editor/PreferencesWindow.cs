using UnityEditor;
using UnityEngine;

namespace RotaryHeart.Lib.SerializableDictionaryPro
{
    public class PreferencesWindow
    {
        #region GUIContent
        static readonly GUIContent M_GuiPagesTitle = new GUIContent("Pages", "Section that has all the pages settings for the drawer");
        static readonly GUIContent M_GuiShowPages = new GUIContent("Show Pages", "Should the drawer be divided in pages?");
        static readonly GUIContent M_GuiShowSizes = new GUIContent("Show Sizes", "Should the dictionary show the size on the title bar?");
        static readonly GUIContent M_GuiPageCount = new GUIContent("Page Count", "How many elements per page are going to be drawn");
        #endregion

        // Have we loaded the prefs yet
        static bool M_prefsLoaded;

        //Default values
        static bool M_showPages;
        static bool M_showSize;
        static int M_pageCount;

#if UNITY_2018_3_OR_NEWER
        private class MyPrefSettingsProvider : SettingsProvider
        {
            public MyPrefSettingsProvider(string path, SettingsScope scopes = SettingsScope.Project)
            : base(path, scopes)
            { }

            public override void OnGUI(string searchContext)
            {
                PreferencesGUI();
            }
        }

        [SettingsProvider]
        static SettingsProvider MyNewPrefCode()
        {
            return new MyPrefSettingsProvider("Preferences/RHSD");
        }
#else
        // Add preferences section named "My Preferences" to the Preferences Window
        [PreferenceItem("RHSD")]
#endif
        public static void PreferencesGUI()
        {
            if (!M_prefsLoaded)
            {
                M_showPages = Constants.ShowPages;
                M_showSize = Constants.ShowSize;
                M_pageCount = Constants.PageCount;

                M_prefsLoaded = true;
            }

            EditorGUILayout.LabelField(M_GuiPagesTitle, EditorStyles.boldLabel);
            EditorGUILayout.Space();

            M_showSize = EditorGUILayout.Toggle(M_GuiShowSizes, M_showSize);
            M_showPages = EditorGUILayout.Toggle(M_GuiShowPages, M_showPages);

            GUI.enabled = M_showPages;

            M_pageCount = Mathf.Clamp(EditorGUILayout.IntField(M_GuiPageCount, M_pageCount), 5, int.MaxValue);

            GUI.enabled = true;

            if (GUI.changed)
            {
                Constants.ShowPages = M_showPages;
                Constants.ShowSize = M_showSize;
                Constants.PageCount = M_pageCount;
            }

            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Restore Default"))
            {
                Constants.RestoreDefaults();

                M_prefsLoaded = false;
            }
        }
    }
}