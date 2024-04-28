using System;
using System.Collections.Generic;
using UnityEngine;

namespace UICore
{
    public class UIHelper : MonoBehaviour
    {
        /// <summary>
        /// Initiates widgets in scroll or grid
        /// </summary>
        public static void InitWidgets<T>(List<T> widgetsList, int count, Action<T, int> initAction) where T: UIForm
        {
            if(widgetsList == null || widgetsList.Count == 0)
                return;
            
            // Checking if quantity of widgets and init count is equal is invoking action in each widget
            if (widgetsList.Count == count)
            {
                for (var i = 0; i < widgetsList.Count; i++)
                {
                    initAction.Invoke(widgetsList[i], i);
                }
            }
            
            // If only one widget in grid - filling the grid with new widgets
            else if(widgetsList.Count == 1 && count > 1 && count > widgetsList.Count)
            {
                var widget = widgetsList[0];
                var widgetGo = widget.gameObject;
                
                var widgetParent = widgetGo.transform.parent.gameObject;
                
                var childWidgets = GetChildWidgets<T>(widgetParent);
                
                if(childWidgets.Count != count) // Cleaning parent from extra childs if count is changed
                {
                    var widgetsToRemove = new List<GameObject>();
                    for (var i = 1; i < childWidgets.Count; i++)
                    {
                        var widgetToRemove = widgetParent.transform.GetChild(i);
                        widgetsToRemove.Add(widgetToRemove.gameObject);
                    }
                    
                    if(widgetsToRemove.Count > 0)
                        foreach (var gameObject in widgetsToRemove)
                        {
                            DestroyImmediate(gameObject);
                        }
                    
                    childWidgets = GetChildWidgets<T>(widgetParent);
                }
                
                if(childWidgets.Count == widgetsList.Count)
                {
                    for (var i = 1; i < count; i++)
                    {
                        var newWidget = widgetGo;
                        var newWidgetGo = Instantiate(newWidget, widgetParent.transform, true);
                        newWidgetGo.transform.localScale = Vector3.one;
                        newWidgetGo.name = $"{i}_{widget.name}";
                    }
                }

                
                var widgets = GetChildWidgets<T>(widgetParent);

                // Making actions in each widget
                for (int j = 0; j < widgets.Count; j++)
                {
                    initAction.Invoke(widgets[j], j);
                    widgets[j].SetActive(true);
                }
            }
        }
        
        // Getting the list of all widgets in grid
        private static List<T> GetChildWidgets<T>(GameObject widgetParent) where T : UIForm
        {
            var widgets = new List<T>();
            foreach (Transform item in widgetParent.transform)
            {
                var widgetComponent = item.GetComponent<T>();
                if (widgetComponent != null)
                    widgets.Add(item.GetComponent<T>());
            }

            return widgets;
        }
    }
}