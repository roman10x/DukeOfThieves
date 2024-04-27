#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace UICore
{
    public class WindowStorageEditor : EditorWindow
    {
        private const string _pathToWindowCollection =  @"Assets/AsyncAssets/UI/WindowStorage.asset";
        private static WindowStorage _dataStorage;
        [MenuItem("DoT/Windows Storage Editor")]
        static void Open()
        {
            _dataStorage = null;
            _dataStorage = AssetDatabase.LoadAssetAtPath<WindowStorage>(_pathToWindowCollection);
            WindowStorageEditor win = GetWindow<WindowStorageEditor>();
            win.titleContent = new GUIContent("Window Storage Editor");
            win.Show();
        }

        private void OnGUI()
        {
            if (_dataStorage == null)
            {
                Debug.Log("Access to window storage is not provided");
                return;
            }
            TopButtons();
            EditorGUILayout.BeginVertical();
            DrawPanels();
            EditorGUILayout.EndVertical();
        }
        
        private void DrawPanels()
        {
            EditorGUILayout.BeginHorizontal();
            
            _dataStorage.EditorOnGui(_dataStorage);

            EditorGUILayout.EndHorizontal();
        }
        
        private void TopButtons()
        {
            if (GUILayout.Button("Save"))
            {
                EditorUtility.SetDirty(_dataStorage);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }
    }
}
#endif