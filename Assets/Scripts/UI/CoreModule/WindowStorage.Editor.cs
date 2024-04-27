#if UNITY_EDITOR
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace UICore
{
  public partial class WindowStorage 
    {
        public void EditorOnGui(WindowStorage windowDataStorage) => _editor.OnGui(windowDataStorage);
        
        private readonly Editor _editor = new Editor();
        
        
        public class Editor
        {
            private WindowStorage _dataStorage;
            private GUIStyle _styleModule = null;
            private Vector2 _scrollPos = Vector2.zero;
            
            public void OnGui(WindowStorage windowDataStorage)
            {
                if(windowDataStorage == null)
                {
                    Debug.Log("Access to window storage is not provided");
                    return;
                }
                
                _dataStorage = windowDataStorage;
                DrawData();
            }
            
            private void DrawData()
            {
                SetModuleStyle();
                EditorGUILayout.BeginHorizontal();
                DrawKeys();
                EditorGUILayout.EndHorizontal();
            }
            
            private void DrawKeys()
            {
                var windowKeys = Enum.GetValues(typeof(WindowKeys)).Cast<WindowKeys>().OrderBy(x => x.ToString()).ToList();

                EditorGUILayout.BeginVertical(_styleModule,GUILayout.ExpandHeight(true));
                _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
                foreach (WindowKeys windowKey in windowKeys)
                {
                    if (windowKey != WindowKeys.Empty)
                    {
                        if (!_dataStorage._windowByKey.ContainsKey(windowKey))
                        {
                            _dataStorage._windowByKey[windowKey] = null;
                        }
                        EditorGUILayout.BeginHorizontal("box");
                        GUILayout.Label(windowKey.ToString(), GUILayout.Width(220));
                        var window = _dataStorage._windowByKey[windowKey];
                        _dataStorage._windowByKey[windowKey] =
                            (Window)EditorGUILayout.ObjectField(_dataStorage._windowByKey[windowKey], typeof(Window), allowSceneObjects: true, GUILayout.Width(440));
                        EditorGUILayout.EndHorizontal();
                    }
                }
                EditorGUILayout.EndScrollView();
                EditorGUILayout.EndVertical();
            }
            
            private void SetModuleStyle()
            {
                _styleModule = new GUIStyle(EditorStyles.helpBox);
                _styleModule.alignment = TextAnchor.UpperRight;
            }
        }
    }
}
#endif