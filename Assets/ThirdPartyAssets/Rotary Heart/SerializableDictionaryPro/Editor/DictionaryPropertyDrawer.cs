using System.Collections;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace RotaryHeart.Lib.SerializableDictionaryPro
{
    [CustomPropertyDrawer(typeof(DrawableDictionary), true)]
    public class DictionaryPropertyDrawer : PropertyDrawer
    {
        #region Fields
        
        SerializedProperty m_keyAdd;
        SerializedProperty m_keysComparingValues;

        DrawableDictionary m_target;
        ReorderableList m_list;
        SerializedProperty m_settingsVal;
        SerializedProperty m_keysProp;
        SerializedProperty m_valuesProp;

        GUIContent m_title = new GUIContent();
        bool m_canAdd = true;
        bool m_canRemove = true;
        bool m_canReorder = true;
        bool m_readOnly = false;
        bool m_checkedIdAttribute = false;
        bool m_checkedDefaultKeyAttribute = false;
        bool? m_containsAttribute = null;
        bool m_drawErrorOnAdd;
        string m_keyArrayElementType;
        string m_addingPath;
        object m_addValue = null;
        FieldInfo[] m_fieldInfos = null;
        IdAttribute m_idAttribute = null;
        DefaultKeyAttribute m_defaultKeyAttribute = null;
        
        readonly System.Type[] m_typesNative =
        {
                typeof(bool),
                typeof(byte),
                typeof(float),
                typeof(int),
                typeof(string),
                typeof(Vector2),
                typeof(Vector2Int),
                typeof(Vector3),
                typeof(Vector3Int),
                typeof(Vector4),
                typeof(Quaternion),
                typeof(Matrix4x4),
                typeof(Color),
                typeof(Rect),
                typeof(RectInt),
                typeof(LayerMask)
        };
        
        Dictionary<string, object> m_keyMap = new Dictionary<string, object>(1);

        #endregion Fields

        #region Properties

        /// <summary>
        /// Check if it contains the new key attribute
        /// </summary>
        bool ContainsAttribute
        {
            get
            {
                if (m_containsAttribute == null)
                {
                    m_containsAttribute = fieldInfo.GetCustomAttributes(typeof(DrawKeyAsPropertyAttribute), true).Any();
                }

                return m_containsAttribute.Value;
            }
        }
        FieldInfo[] KeyFields
        {
            get
            {
                if (m_fieldInfos == null)
                {
                    FieldInfo field =
                        fieldInfo.FieldType.BaseType.GetField(
                            "m_keys", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                    System.Type fieldType = field.FieldType;
                    System.Type elementType = fieldType.GetGenericArguments()[0];
                    m_fieldInfos =
                        elementType.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                }

                return m_fieldInfos;
            }
        }
        /// <summary>
        /// Default space between entries
        /// </summary>
        public float VerticalSpace
        {
            get { return m_list == null ? 2 : m_list.verticalSpacing * 2; }
        }
        public IdAttribute IdAttribute
        {
            get
            {
                if (!m_checkedIdAttribute)
                {
                    m_checkedIdAttribute = true;
                    m_idAttribute = System.Attribute.GetCustomAttribute(fieldInfo, typeof(IdAttribute)) as IdAttribute;
                }
                
                return m_idAttribute;
            }
        }
        public DefaultKeyAttribute DefaultKeyAttribute
        {
            get
            {
                if (!m_checkedDefaultKeyAttribute)
                {
                    m_checkedDefaultKeyAttribute = true;
                    m_defaultKeyAttribute = System.Attribute.GetCustomAttribute(fieldInfo, typeof(DefaultKeyAttribute)) as DefaultKeyAttribute;
                }

                return m_defaultKeyAttribute;
            }
        }

        #endregion Properties

        static class Style
        {
            public static GUIStyle headerBackground;
            public static GUIStyle boxBackground;
            public static GUIStyle tooTipStyle;

            public static GUIContent keyAddContent;
            public static GUIContent idContent;
            public static GUIContent valueContent;
            public static GUIContent idEmptyContent;
            public static GUIContent duplicateIdContent;

            static Style()
            {
                headerBackground = new GUIStyle("RL Header");
                boxBackground = new GUIStyle("RL Background")
                {
                    border = new RectOffset(6, 3, 3, 6)
                };
                tooTipStyle = new GUIStyle("Tooltip");
                
                idContent = new GUIContent("Id");
                valueContent = new GUIContent("Value");
                idEmptyContent = new GUIContent(EditorGUIUtility.IconContent("console.warnicon.sml").image,
                                                                      "ID cannot be left empty");
                duplicateIdContent = new GUIContent(EditorGUIUtility.IconContent("console.erroricon.sml").image,
                                                                          "Dictionary already has this id, this id cannot be used");
                keyAddContent = new GUIContent("Key");
            }
        }

        /// <summary>
        /// Used to get the required references, returns the reference to the ReorderableList
        /// </summary>
        /// <param name="property">This property</param>
        void GetReferences(SerializedProperty property)
        {
            m_settingsVal = property.FindPropertyRelative("m_val");
            int val = m_settingsVal.intValue;
            m_canAdd = (val & 1) == 1;
            m_canRemove = (val & 2) == 2;
            m_canReorder = (val & 4) == 4;
            m_readOnly = (val & 8) == 8;

            m_keysProp = property.FindPropertyRelative("m_keys");
            m_valuesProp = property.FindPropertyRelative("m_values");

            //Clone the serialized object and use it for the key values (used for comparing values)
            SerializedProperty clonedProperty = new SerializedObject(property.serializedObject.targetObject).FindProperty(property.propertyPath);
            m_keysComparingValues = clonedProperty.FindPropertyRelative("m_keys");
            
            //Reset dictionary to avoid having unused data on memory
            if (m_valuesProp != null)
            {
                clonedProperty.FindPropertyRelative("m_values").arraySize = 0;
            }
            
            //Clone the serialized object and use it for the key for new elements
            clonedProperty = new SerializedObject(property.serializedObject.targetObject).FindProperty(property.propertyPath);
            m_keyAdd = clonedProperty.FindPropertyRelative("m_keys");

            m_keyAdd.arraySize = 1;
            m_keyAdd = m_keyAdd.GetArrayElementAtIndex(0);
            
            //Reset dictionary to avoid having unused data on memory
            if (m_valuesProp != null)
            {
                clonedProperty.FindPropertyRelative("m_values").arraySize = 0;
            }
            
            m_target = (DrawableDictionary)GetTargetObjectOfProperty(property);
            m_list = m_target.reorderableList;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            //Make sure references are ready
            GetReferences(property);

            //Default header height
            float height = EditorGUIUtility.singleLineHeight;

            if (m_settingsVal.isExpanded)
            {
                //Default height for the bottom section
                height += EditorGUIUtility.singleLineHeight;

                int keysSize = m_keysProp.arraySize;

                if (Constants.ShowPages)
                {
                    //Extra space for top section pages
                    height += EditorGUIUtility.singleLineHeight;
                    keysSize = Mathf.Min(keysSize, Constants.PageCount);
                }

                if (keysSize > 0)
                {
                    //Iterate through all the keys
                    for (int keyIndex = 0; keyIndex < keysSize; keyIndex++)
                    {
                        //Should only happen with pages
                        if (keyIndex >= m_keysProp.arraySize)
                            break;

                        SerializedProperty keyProp = m_keysProp.GetArrayElementAtIndex(keyIndex);

                        //Use the same element height calculations
                        height += List_onGetElementHeight(keyProp, keyIndex) + VerticalSpace;
                    }
                }
                else
                {
                    //Default height for empty list
                    height += EditorGUIUtility.singleLineHeight + VerticalSpace * 3;
                }
            }

            float offset = 0;

            //Extra space when adding a new element
            if (m_addingPath == m_keysProp.propertyPath)
            {
                offset = EditorGUIUtility.singleLineHeight * 5;
                
                if (m_keyAdd.isExpanded)
                {
                    offset += EditorGUI.GetPropertyHeight(m_keyAdd);
                }
            }
            
            return height + VerticalSpace + offset;
        }

        #region Helpers

        object GetTargetObjectOfProperty(SerializedProperty prop)
        {
            string path = prop.propertyPath.Replace(".Array.data[", "[");
            object obj = prop.serializedObject.targetObject;
            string[] elements = path.Split('.');
            foreach (string element in elements)
            {
                if (element.Contains("["))
                {
                    string elementName = element.Substring(0, element.IndexOf("["));
                    int index = System.Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[", "").Replace("]", ""));
                    obj = GetValue_Imp(obj, elementName, index);
                }
                else
                {
                    obj = GetValue_Imp(obj, element);
                }
            }
            return obj;
        }

        void SetTargetObjectOfProperty(SerializedProperty prop, object value, bool custom = false)
        {
            string path = prop.propertyPath.Replace(".Array.data[", "[");
            object obj = prop.serializedObject.targetObject;
            string[] elements = path.Split('.');
            foreach (string element in elements.Take(elements.Length - 1))
            {
                if (element.Contains("["))
                {
                    string elementName = element.Substring(0, element.IndexOf("["));
                    int index = System.Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[", "").Replace("]", ""));
                    obj = GetValue_Imp(obj, elementName, index);
                }
                else
                {
                    obj = GetValue_Imp(obj, element);
                }
            }

            if (Object.ReferenceEquals(obj, null)) return;

            try
            {
                string element = elements.Last();
                System.Type tp = obj.GetType();

                if (custom)
                    tp = tp.BaseType;

                if (element.Contains("["))
                {
                    string elementName = element.Substring(0, element.IndexOf("["));
                    int index = System.Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[", "").Replace("]", ""));
                    FieldInfo field = tp.GetField(elementName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    IList arr = field.GetValue(obj) as System.Collections.IList;
                    arr[index] = value;
                }
                else
                {
                    FieldInfo field = tp.GetField(element, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    if (field != null)
                    {
                        field.SetValue(obj, value);
                    }
                }

            }
            catch
            {
                return;
            }
        }

        object GetValue_Imp(object source, string name)
        {
            if (source == null)
                return null;
            System.Type type = source.GetType();

            while (type != null)
            {
                FieldInfo f = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                if (f != null)
                    return f.GetValue(source);

                PropertyInfo p = type.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (p != null)
                    return p.GetValue(source, null);

                type = type.BaseType;
            }
            return null;
        }

        object GetValue_Imp(object source, string name, int index)
        {
            IEnumerable enumerable = GetValue_Imp(source, name) as IEnumerable;
            if (enumerable == null) return null;
            IEnumerator enm = enumerable.GetEnumerator();
            //while (index-- >= 0)
            //    enm.MoveNext();
            //return enm.Current;

            for (int i = 0; i <= index; i++)
            {
                if (!enm.MoveNext()) return null;
            }
            return enm.Current;
        }

        bool IsUnitySerialized(FieldInfo fieldInfo)
        {
            object[] customAttributes = fieldInfo.GetCustomAttributes(true);
            if (customAttributes.Any(x => x is System.NonSerializedAttribute))
            {
                return false;
            }
            if (fieldInfo.IsPrivate && !customAttributes.Any(x => x is SerializeField))
            {
                return false;
            }
            return IsUnitySerialized(fieldInfo.FieldType);
        }

        bool IsUnitySerialized(System.Type type)
        {
            if (type.IsGenericType)
            {
                if (type.GetGenericTypeDefinition() == typeof(List<>))
                {
                    return IsUnitySerialized(type.GetGenericArguments()[0]);
                }
                return false;
            }
            if (type.IsEnum)
            {
                return true;
            }
            if (type.IsValueType)
            {
                return true;
            }
            if (type.IsAssignableFrom(typeof(Object)))
            {
                return true;
            }
            if (m_typesNative.Contains(type) || (type.IsArray && m_typesNative.Contains(type.GetElementType())))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Converts a Vector4 to Quaternion
        /// </summary>
        /// <param name="v4">Vector to convert</param>
        Quaternion ConvertToQuaternion(Vector4 v4)
        {
            return new Quaternion(v4.x, v4.y, v4.z, v4.w);
        }

        /// <summary>
        /// Converts a Quaternion to Vector4
        /// </summary>
        /// <param name="q">Quaternion to convert</param>
        Vector4 QuaternionToVector4(Quaternion q)
        {
            return new Vector4(q.x, q.y, q.z, q.w);
        }

        #endregion

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            //Make sure references are ready
            GetReferences(property);

            property.isExpanded = false;

            m_title.text = label.text;
            TooltipAttribute tooltipAttribute = fieldInfo.GetCustomAttribute<TooltipAttribute>(true);

            if (tooltipAttribute != null)
            {
                m_title.tooltip = tooltipAttribute.tooltip;
            }

            float offset = 0;

            Rect nextRect = GetNextRect(ref position);

            //Fix values size based on the keys size
            if (m_valuesProp != null && m_valuesProp.arraySize != m_keysProp.arraySize)
            {
                m_valuesProp.arraySize = m_keysProp.arraySize;
            }
            
            //Make sure both comparing and original values array are the same (this should only happen on remove)
            if (m_keysComparingValues.arraySize != m_keysProp.arraySize)
            {
                m_keysComparingValues.arraySize = m_keysProp.arraySize;
            }

            float height = EditorGUIUtility.singleLineHeight * 5;

            //Extra offset when adding a new element
            if (m_addingPath == m_keysProp.propertyPath)
            {
                offset += height;
                
                if (m_keyAdd.isExpanded)
                {
                    offset += EditorGUI.GetPropertyHeight(m_keyAdd);
                }
            }

            if (m_list == null || !m_list.HasList)
            {
                CreateList();
            }
            
            m_list.List = m_keysProp;
            m_list.isExpanded = m_settingsVal.isExpanded;
            m_list.canAdd = m_canAdd;
            m_list.canRemove = m_canRemove;
            m_list.draggable = m_canReorder;

            m_list.DoList(new Rect(nextRect.x, nextRect.y, nextRect.width, GetPropertyHeight(property, label) - offset), label, Constants.ShowPages, Constants.PageCount);
           
            if (m_addingPath == m_keysProp.propertyPath)
            {
                //Calculate all positions
                float keyHeight = EditorGUI.GetPropertyHeight(m_keyAdd);

                if (m_keyAdd.propertyType == SerializedPropertyType.Quaternion)
                {
                    keyHeight -= EditorGUIUtility.singleLineHeight * 4;
                }
                
                Rect rect = new Rect(position.x, position.y + GetPropertyHeight(property, label) - (height + keyHeight), position.width, EditorGUIUtility.singleLineHeight);
                
                if (m_keyAdd.propertyType == SerializedPropertyType.Quaternion)
                {
                    rect.y -= EditorGUIUtility.singleLineHeight * 4;
                }
                
                //Draw add window backjground
                if (Event.current.type == EventType.Repaint)
                {
                    Rect tRect = rect;
                    tRect.height += (EditorGUIUtility.singleLineHeight * 2) + 8;
                    
                    if (m_keyAdd.isExpanded)
                    {
                        tRect.height += keyHeight - EditorGUIUtility.singleLineHeight;
                    }
                    
                    Style.boxBackground.Draw(tRect, false, false, false, false);
                    Style.headerBackground.Draw(rect, false, false, false, false);
                }
                
                EditorGUI.LabelField(rect, "Element to add");

                rect.y = rect.yMax + 8;
                Rect keyRect = rect;
                keyRect.x += 16;
                keyRect.width -= 16;

                //Indicate that the key cannot be added
                if (m_drawErrorOnAdd)
                {
                    GUI.Button(new Rect(keyRect.x - 15, keyRect.y, 30, 30), Style.duplicateIdContent, GUIStyle.none);
                }

                //Make sure that the key has the correct saved value
                if (m_addValue == null)
                {
                    m_addValue = GetPropertyValue(m_keyAdd);
                }
                            
                SetPropertyValue(m_keyAdd, m_addValue);

                switch (m_keyAdd.propertyType)
                {
                    case SerializedPropertyType.Enum:
                        SerializedProperty keyToUse = m_keyAdd;

                        string[] names = keyToUse.enumDisplayNames;

                        if (m_addValue == null)
                        {
                            m_addValue = GetNotUsedEnumValue(m_keysProp);
                        }

                        int castedValue = (int)m_addValue;
                        
                        string selectedVal = names[castedValue];

                        //Draw button with dropdown style
                        Rect adjustedRect = EditorGUI.PrefixLabel(keyRect, new GUIContent("Key"));
                        if (GUI.Button(adjustedRect, selectedVal, EditorStyles.layerMaskField))
                        {
                            List<string> usedNames = new List<string>(m_keysProp.arraySize);
                            GenericMenu menu = new GenericMenu();

                            //Add all the used values
                            for (int i = 0; i < m_keysProp.arraySize; i++)
                            {
                                usedNames.Add(names[m_keysProp.GetArrayElementAtIndex(i).enumValueIndex]);
                            }

                            //Add all the menu items
                            for (int i = 0; i < names.Length; i++)
                            {
                                int nameIndex = i;

                                //If the value is being used, show it disabled
                                if (usedNames.Contains(names[nameIndex]) && !names[nameIndex].Equals(selectedVal))
                                {
                                    menu.AddDisabledItem(new GUIContent(names[nameIndex]));
                                }
                                else
                                {
                                    menu.AddItem(new GUIContent(names[nameIndex]), selectedVal == names[nameIndex], () =>
                                    {
                                        keyToUse.enumValueIndex = nameIndex;
                                        m_addValue = nameIndex;
                                    });
                                }
                            }

                            //Show menu under mouse position
                            menu.ShowAsContext();

                            Event.current.Use();
                        }
                        break;
                    
                    case SerializedPropertyType.Quaternion:
                        //Used to draw quaternion as Vector4Field
                        using (EditorGUI.ChangeCheckScope changeCheckScope = new EditorGUI.ChangeCheckScope())
                        {
                            Vector4 newV4 = EditorGUI.Vector4Field(
                                new Rect(keyRect.x + 45, keyRect.y, keyRect.width - 45, keyRect.height),
                            GUIContent.none, QuaternionToVector4(m_keyAdd.quaternionValue));

                            if (changeCheckScope.changed)
                            {
                                m_addValue = ConvertToQuaternion(newV4);
                                SetPropertyValue(m_keyAdd, m_addValue);
                            }
                        }
                        break;
                    
                    default:
                        using (EditorGUI.ChangeCheckScope changeCheckScope = new EditorGUI.ChangeCheckScope())
                        {
                            EditorGUI.PropertyField(keyRect, m_keyAdd, Style.keyAddContent, true);

                            if (changeCheckScope.changed)
                            {
                                m_addValue = GetPropertyValue(m_keyAdd);
                            }
                        }

                        break;
                }

                if (m_keyAdd.isExpanded)
                {
                    rect.y += keyHeight;
                }
                else
                {
                    rect.y = rect.yMax;
                }

                rect.width /= 2;
                
                //Add a new key using default value
                int index = m_keysComparingValues.arraySize;

                int count;
                m_drawErrorOnAdd = ContainsId(m_addValue, index, out count);

                if (m_drawErrorOnAdd)
                    GUI.enabled = false;
                
                if (GUI.Button(rect, "Add") && m_addValue != null)
                {
                    m_keysComparingValues.arraySize = ++m_keysProp.arraySize;

                    if (m_valuesProp != null)
                        m_valuesProp.arraySize = m_keysComparingValues.arraySize;

                    m_keysComparingValues.serializedObject.ApplyModifiedProperties();
            
                    if (m_valuesProp != null)
                        m_valuesProp.serializedObject.ApplyModifiedProperties();
                    
                    m_keysProp.serializedObject.ApplyModifiedProperties();

                    SetPropertyValue(m_keysComparingValues.GetArrayElementAtIndex(index), m_addValue);
                    SetPropertyValue(m_keysProp.GetArrayElementAtIndex(index), m_addValue);

                    if (m_valuesProp != null)
                    {
                        object propObj = GetTargetObjectOfProperty(m_valuesProp.GetArrayElementAtIndex(index));
                        if (propObj != null)
                        {
                            System.Type test = propObj.GetType();

                            while (test != null && test != typeof(DrawableDictionary))
                            {
                                test = test.BaseType;
                            }

                            if (test == typeof(DrawableDictionary))
                            {
                                SerializedProperty element = m_valuesProp.GetArrayElementAtIndex(index);

                                element.FindPropertyRelative("m_val").intValue = 7;
                            }
                        }
                    }

                    GUI.FocusControl("");

                    //Call action
                    if (m_target.EditorOnAdded != null)
                    {
                        m_target.EditorOnAdded.Invoke(m_keysProp.GetArrayElementAtIndex(index), m_valuesProp != null ? m_valuesProp.GetArrayElementAtIndex(index) : null);
                    }
                    
                    m_addingPath = null;
                }

                GUI.enabled = true;
                
                rect.x = rect.xMax;
                
                if (GUI.Button(rect, "Cancel"))
                {
                    m_addingPath = null;
                    m_drawErrorOnAdd = false;
                    GUI.FocusControl("");
                    m_addValue = null;
                }
            }
        }

        void CreateList()
        {
            m_list = new ReorderableList(m_keysProp, m_canAdd, m_canRemove, m_canReorder);

            //Required callbacks
            m_list.onRemoveCallback -= List_onRemove;
            m_list.onAddCallback -= List_onAdd;
            m_list.drawElementCallback -= List_onDrawElement;
            m_list.drawHeaderCallback -= List_onDrawHeader;
            m_list.getElementHeightCallback -= List_onGetElementHeight;
            m_list.onElementsReorder -= List_onElementsReorder;
            m_list.headerExpand -= List_onHeaderExpand;

            m_list.onRemoveCallback += List_onRemove;
            m_list.onAddCallback += List_onAdd;
            m_list.drawElementCallback += List_onDrawElement;
            m_list.drawHeaderCallback += List_onDrawHeader;
            m_list.getElementHeightCallback += List_onGetElementHeight;
            m_list.onElementsReorder += List_onElementsReorder;
            m_list.headerExpand += List_onHeaderExpand;

            //Set the list object value
            m_target.reorderableList = m_list;
        }

        #region List callbacks

        void List_onRemove(ReorderableList list)
        {
            Remove(list.Selected);
        }

        void List_onAdd(ReorderableList list)
        {
            m_keyAdd.isExpanded = false;
            m_addingPath = m_keysProp.propertyPath;
            m_addValue = null;

            if (m_keyAdd.propertyType == SerializedPropertyType.Enum)
            {
                m_keyAdd.enumValueIndex = GetNotUsedEnumValue(m_keysComparingValues);
            }
        }

        void List_onDrawElement(Rect rect, SerializedProperty element, GUIContent label, int index, bool selected, bool focused)
        {
            GUI.enabled = !m_readOnly;

            SerializedProperty keyValueProp = m_keysComparingValues.GetArrayElementAtIndex(index);
            SerializedProperty keyProp = m_keysProp.GetArrayElementAtIndex(index);
            SerializedProperty valueProp = m_valuesProp == null ? null : m_valuesProp.GetArrayElementAtIndex(index);
            SerializedProperty keyToUse;

            if (keyProp.propertyType == SerializedPropertyType.Generic)
            {
                keyToUse = keyProp;
                rect.x -= 10;
                rect.width += 10;
            }
            else
            {
                keyToUse = keyValueProp;
            }

            //Only draw the color if this entry is not selected
            if (!selected && Event.current.type == EventType.Repaint)
            {
                Style.tooTipStyle.Draw(rect, false, false, false, false);
            }

            rect.height = EditorGUIUtility.singleLineHeight;
            
            Rect keyRect = new Rect(rect.x + 50, rect.y + 4, rect.width - 52, rect.height);
            Rect valueRect = new Rect(keyRect);

            EditorGUI.BeginChangeCheck();
            
            #region Key Field
            SerializedPropertyType keyPropType = keyToUse.propertyType;

            string propName = "";
            if (ContainsAttribute)
            {
                foreach (FieldInfo fi in KeyFields)
                {
                    if (!IsUnitySerialized(fi)) continue;
                    
                    propName = fi.Name;
                    break;
                }
            }

            bool hasPropName = string.IsNullOrEmpty(propName);

            //Draw only if its not a generic type or can be draw as property
            if ((ContainsAttribute && !hasPropName) || keyPropType != SerializedPropertyType.Generic)
            {
                if (ContainsAttribute)
                {
                    keyRect.height = EditorGUI.GetPropertyHeight(keyProp, GUIContent.none, true) -
                                     (keyProp.isExpanded ? EditorGUIUtility.singleLineHeight : 0);
                }

                if (valueProp != null)
                {
                    keyProp.isExpanded = EditorGUI.Foldout(new Rect(rect.x + 15, keyRect.y, 20, rect.height),
                        keyProp.isExpanded, Style.idContent, true);
                }
                else
                {
                    keyRect.x -= 30;
                    keyRect.width += 30;
                }
            }

            GUI.SetNextControlName("CheckGenericFocus" + index);
            bool keyChanged = false;
            
            //Fix for enum key modified affecting values
            object enumKeyValue;
            if (m_keyMap.TryGetValue(keyToUse.propertyPath, out enumKeyValue))
            {
                SetPropertyValue(keyValueProp, enumKeyValue);
                keyChanged = true;
                m_keyMap.Clear();
            }
            
            switch (keyPropType)
            {
                case SerializedPropertyType.Quaternion:
                    using (EditorGUI.ChangeCheckScope changeCheckScope = new EditorGUI.ChangeCheckScope())
                    {
                        Vector4 newV4 = EditorGUI.Vector4Field(keyRect, GUIContent.none,
                                                   QuaternionToVector4(keyToUse.quaternionValue));

                        if (changeCheckScope.changed)
                        {
                            keyChanged = true;
                            keyToUse.quaternionValue = ConvertToQuaternion(newV4);
                        }
                    }

                    break;

                case SerializedPropertyType.Enum:
                    string[] names = keyToUse.enumDisplayNames;
                    
                    //Safe check to remove if wrong data is stored
                    if (names.Length <= keyToUse.enumValueIndex || keyToUse.enumValueIndex < 0)
                    {
                        Remove(index);
                        return;
                    }
                    
                    string selectedVal = names[keyToUse.enumValueIndex];

                    //Draw button with dropdown style
                    if (GUI.Button(keyRect, selectedVal, EditorStyles.layerMaskField))
                    {
                        List<string> usedNames = new List<string>();
                        GenericMenu menu = new GenericMenu();

                        //Add all the used values
                        for (int i = 0; i < m_keysProp.arraySize; i++)
                        {
                            usedNames.Add(names[m_keysProp.GetArrayElementAtIndex(i).enumValueIndex]);
                        }

                        //Add all the menu items
                        for (int i = 0; i < names.Length; i++)
                        {
                            int nameIndex = i;

                            //If the value is being used, show it disabled
                            if (usedNames.Contains(names[nameIndex]) && !names[nameIndex].Equals(selectedVal))
                            {
                                menu.AddDisabledItem(new GUIContent(names[nameIndex]));
                            }
                            else
                            {
                                menu.AddItem(new GUIContent(names[nameIndex]), selectedVal == names[nameIndex], () =>
                                {
                                    m_keyMap.Add(keyToUse.propertyPath, nameIndex);
                                });
                            }
                        }

                        //Show menu under mouse position
                        menu.ShowAsContext();

                        Event.current.Use();
                    }
                    break;

                case SerializedPropertyType.Generic:

                    using (EditorGUI.ChangeCheckScope changeCheckScope = new EditorGUI.ChangeCheckScope())
                    {
                        //Only draw as property if values are correct
                        if (ContainsAttribute && !hasPropName)
                        {
                            EditorGUI.PropertyField(keyRect, keyToUse.FindPropertyRelative(propName), GUIContent.none,
                                false);
                        }
                        else
                        {
                            if (valueProp == null)
                                Style.idContent.text = "Value";

                            keyRect.height = EditorGUI.GetPropertyHeight(keyToUse, Style.idContent);
                            EditorGUI.PropertyField(
                                new Rect(rect.x + 15, keyRect.y, keyRect.width + 35, keyRect.height), keyToUse,
                                Style.idContent, true);
                        }

                        if (changeCheckScope.changed)
                        {
                            keyChanged = true;
                        }
                    }

                    break;

                default:
                    using (EditorGUI.ChangeCheckScope changeCheckScope = new EditorGUI.ChangeCheckScope())
                    {
                        EditorGUI.PropertyField(keyRect, keyToUse, GUIContent.none, false);
                        
                        if (changeCheckScope.changed)
                        {
                            keyChanged = true;
                        }
                    }

                    break;
            }

            //Not used for generic type
            if (keyPropType != SerializedPropertyType.Generic && keyChanged)
            {
                //Old key value
                object oldId = GetPropertyValue(keyProp);
                //New key value
                object newId = GetPropertyValue(keyValueProp);

                //Notify if the key is empty or null
                if ((keyPropType == SerializedPropertyType.String && string.IsNullOrEmpty(newId.ToString())) || newId == null)
                {
                    GUI.Button(new Rect(keyRect.x - 15, keyRect.y, 30, 30), Style.idEmptyContent, GUIStyle.none);
                }
                //Check if the key value has been changed
                else
                {
                    int count;
                    if ((oldId == null && newId != null) || !oldId.Equals(newId))
                    {
                        //Be sure that the dictionary doesn't contain an element with this key
                        if (ContainsId(newId, index, out count))
                        {
                            //Check if this key is still focused
                            if (GUI.GetNameOfFocusedControl().Equals("CheckGenericFocus" + index))
                            {
                                //Notify the user that this key already exists
                                GUI.Button(new Rect(keyRect.x - 15, keyRect.y, 30, 30), Style.duplicateIdContent, GUIStyle.none);
                            }
                            else
                            {
                                //If it's not, set the correct key back. This is to avoid having multiple errors with ids
                                SetPropertyValue(keyValueProp, oldId);
                            }
                        }
                        else
                        {
                            //Set the value
                            SetGenericValue(keyProp, valueProp, newId);
                        }
                    }
                    else if (ContainsId(newId, index, out count))
                    {
                        //Notify the user that this key already exists
                        GUI.Button(new Rect(keyRect.x - 15, keyRect.y, 30, 30), Style.duplicateIdContent, GUIStyle.none);
                    }
                }
            }
            
            #endregion Key Field

            valueRect.y = keyRect.yMax + 3 - (ContainsAttribute ? 2 : 0);
            valueRect.x -= 20;
            valueRect.width += 20;

            #region Value Field

            if (valueProp != null)
            {
                SerializedPropertyType valPropType = valueProp.propertyType;

                //Value field
                if (keyProp.isExpanded)
                {
                    using (EditorGUI.ChangeCheckScope changeCheckScope = new EditorGUI.ChangeCheckScope())
                    {
                        switch (valPropType)
                        {
                            case SerializedPropertyType.Generic:
                                if (keyPropType != SerializedPropertyType.Generic)
                                {
                                    valueRect.y -= 3;
                                }

                                EditorGUI.PropertyField(valueRect, valueProp, Style.valueContent, true);
                                
                                if (changeCheckScope.changed && m_valuesProp != null)
                                {
                                    //This is used to apply the modified changes
                                    m_valuesProp.serializedObject.ApplyModifiedProperties();
                                }
                                
                                break;

                            case SerializedPropertyType.Quaternion:
                                valueRect.x -= 10;
                                valueRect.width += 10;
                                
                                Vector4 newV4 = EditorGUI.Vector4Field(
                                    new Rect(valueRect.x + 45, valueRect.y, valueRect.width - 45, valueRect.height),
                                    GUIContent.none, QuaternionToVector4(valueProp.quaternionValue));

                                if (changeCheckScope.changed)
                                {
                                    valueProp.quaternionValue = ConvertToQuaternion(newV4);
                                }
                                break;

                            default:
                                EditorGUI.PropertyField(
                                    new Rect(valueRect.x + 45, valueRect.y, valueRect.width - 45, valueRect.height),
                                    valueProp, GUIContent.none, true);
                                break;
                        }
                    }
                }
            }

            #endregion Value Field
            
            if (EditorGUI.EndChangeCheck())
            {
                if (m_target.EditorOnEdit != null)
                    m_target.EditorOnEdit.Invoke(keyToUse, valueProp);
            }

            GUI.enabled = true;

            if (m_target.EditorOnDrawElement != null)
                m_target.EditorOnDrawElement.Invoke(rect, element);
        }

        void List_onDrawHeader(Rect rect, GUIContent label)
        {
            rect.x += 6;

            using (EditorGUI.ChangeCheckScope changeCheckScope = new EditorGUI.ChangeCheckScope())
            {
                m_settingsVal.isExpanded = EditorGUI.Foldout(rect, m_settingsVal.isExpanded, string.Empty, true);
                
                if (changeCheckScope.changed)
                {
                    m_settingsVal.serializedObject.ApplyModifiedProperties();
                }
            }
            m_title.text += (Constants.ShowSize ? " [" + m_keysComparingValues.arraySize.ToString("N0") + "]" : string.Empty);
            EditorGUI.LabelField(rect, m_title);
        }

        float List_onGetElementHeight(SerializedProperty element, int index)
        {
            if (element == null)
            {
                return EditorGUIUtility.singleLineHeight;
            }
            
            float height = EditorGUI.GetPropertyHeight(element, GUIContent.none, true) + VerticalSpace * 4;

            //Value height
            if (!element.isExpanded || m_valuesProp == null || m_valuesProp.arraySize <= index)
            {
                return height;
            }

            SerializedProperty valueProp = m_valuesProp.GetArrayElementAtIndex(index);

            height += EditorGUI.GetPropertyHeight(valueProp, GUIContent.none, true) + VerticalSpace -
                      (ContainsAttribute ? EditorGUIUtility.singleLineHeight : 0);
            
            //Special check to reduce quaternion vertical space
            if (element.propertyType == SerializedPropertyType.Quaternion)
            {
                height -= EditorGUIUtility.singleLineHeight * 5;
            }

            return height;
        }

        void List_onElementsReorder(int startIndex, int newIndex)
        {
            m_keysComparingValues.MoveArrayElement(startIndex, newIndex);
            
            if (m_valuesProp != null)
                m_valuesProp.MoveArrayElement(startIndex, newIndex);

            if (m_target.EditorOnReorder != null)
                m_target.EditorOnReorder.Invoke(startIndex, newIndex);
        }

        void List_onHeaderExpand(bool expand)
        {
            m_settingsVal.isExpanded = expand;

            for (int i = 0; i < m_keysComparingValues.arraySize; i++)
            {
                m_keysProp.GetArrayElementAtIndex(i).isExpanded = expand;
                
                if (m_valuesProp != null)
                {
                    m_valuesProp.GetArrayElementAtIndex(i).isExpanded = expand;
                }
            }

            m_settingsVal.serializedObject.ApplyModifiedProperties();
        }

        void Remove(params int[] selected)
        {
            for (int i = selected.Length - 1; i >= 0; i--)
            {
                int index = selected[i];

                int last = m_keysProp.arraySize - 1;

                m_keysComparingValues.MoveArrayElement(index, last);
                m_keysProp.MoveArrayElement(index, last);
                
                if (m_valuesProp != null)
                {
                    m_valuesProp.MoveArrayElement(index, last);
                }

                m_keysComparingValues.arraySize--;
                m_keysProp.arraySize--;
                
                if (m_valuesProp != null)
                {
                    m_valuesProp.arraySize--;
                }
            }

            if (m_valuesProp != null)
            {
                m_valuesProp.serializedObject.ApplyModifiedProperties();
                m_valuesProp.serializedObject.Update();
            }

            if (m_target.EditorOnRemove != null)
            {
                m_target.EditorOnRemove.Invoke(selected);
            }
        }

        #endregion

        /// <summary>
        /// Checks if the <paramref name="m_keysProp"/> contains the id
        /// </summary>
        /// <param name="obj">Id to check</param>
        /// <param name="index">Property index on the array</param>
        /// <param name="count">How many times the Id is found</param>
        /// <returns>True if an element is already using the id; otherwise, false</returns>
        bool ContainsId(object obj, int index, out int count)
        {
            count = 0;
            bool returnValue = false;
            for (int i = 0; i < m_keysProp.arraySize; i++)
            {
                if (index == i)
                {
                    continue;
                }

                object val = GetPropertyValue(m_keysProp.GetArrayElementAtIndex(i));

                if (!val.Equals(obj)) continue;
                
                count++;
                returnValue = true;

                if (count >= 2)
                    break;
            }
            return returnValue;
        }

        /// <summary>
        /// Returns the current property value
        /// </summary>
        /// <param name="prop">Property to check</param>
        /// <returns>object representation of the property value</returns>
        object GetPropertyValue(SerializedProperty prop)
        {
            switch (prop.propertyType)
            {
                case SerializedPropertyType.Integer:
                case SerializedPropertyType.LayerMask:
                    return prop.intValue;
                case SerializedPropertyType.Boolean:
                    return prop.boolValue;
                case SerializedPropertyType.Float:
                    return prop.floatValue;
                case SerializedPropertyType.String:
                    return prop.stringValue;
                case SerializedPropertyType.Color:
                    return prop.colorValue;
                case SerializedPropertyType.ObjectReference:
                    return prop.objectReferenceValue;
                case SerializedPropertyType.Enum:
                    return prop.enumValueIndex < 0 ? 0 : prop.enumValueIndex;
                case SerializedPropertyType.Vector2:
                    return prop.vector2Value;
                case SerializedPropertyType.Vector2Int:
                    return prop.vector2IntValue;
                case SerializedPropertyType.Vector3:
                    return prop.vector3Value;
                case SerializedPropertyType.Vector3Int:
                    return prop.vector3IntValue;
                case SerializedPropertyType.Vector4:
                    return prop.vector4Value;
                case SerializedPropertyType.Rect:
                    return prop.rectValue;
                case SerializedPropertyType.RectInt:
                    return prop.rectIntValue;
                case SerializedPropertyType.ArraySize:
                    return prop.arraySize;
                case SerializedPropertyType.Character:
                    return (char)prop.intValue;
                case SerializedPropertyType.AnimationCurve:
                    return prop.animationCurveValue;
                case SerializedPropertyType.Bounds:
                    return prop.boundsValue;
                case SerializedPropertyType.Gradient:
                    return GetGradientValue(prop);
                case SerializedPropertyType.Quaternion:
                    return prop.quaternionValue;
                case SerializedPropertyType.Generic:
                    return GetTargetObjectOfProperty(prop);
                default:
                    Debug.LogError("Key Type not implemented: " + prop.propertyType);
                    return null;
            }
        }

        /// <summary>
        /// Sets the property value
        /// </summary>
        /// <param name="prop">Property to modify</param>
        /// <param name="obj">Value</param>
        void SetPropertyValue(SerializedProperty prop, object obj)
        {
            switch (prop.propertyType)
            {
                case SerializedPropertyType.Integer:
                case SerializedPropertyType.LayerMask:
                    prop.intValue = (int)obj;
                    break;
                case SerializedPropertyType.Boolean:
                    prop.boolValue = (bool)obj;
                    break;
                case SerializedPropertyType.Float:
                    prop.floatValue = (float)obj;
                    break;
                case SerializedPropertyType.String:
                    prop.stringValue = (string)obj;
                    break;
                case SerializedPropertyType.Color:
                    prop.colorValue = (Color)obj;
                    break;
                case SerializedPropertyType.ObjectReference:
                    prop.objectReferenceValue = (Object)obj;
                    break;
                case SerializedPropertyType.Enum:
                    prop.enumValueIndex = (int)obj;
                    break;
                case SerializedPropertyType.Vector2:
                    prop.vector2Value = (Vector2)obj;
                    break;
                case SerializedPropertyType.Vector2Int:
                    prop.vector2IntValue = (Vector2Int)obj;
                    break;
                case SerializedPropertyType.Vector3:
                    prop.vector3Value = (Vector3)obj;
                    break;
                case SerializedPropertyType.Vector3Int:
                    prop.vector3IntValue = (Vector3Int)obj;
                    return;
                case SerializedPropertyType.Vector4:
                    prop.vector4Value = (Vector4)obj;
                    break;
                case SerializedPropertyType.Rect:
                    prop.rectValue = (Rect)obj;
                    break;
                case SerializedPropertyType.RectInt:
                    prop.rectIntValue = (RectInt)obj;
                    break;
                case SerializedPropertyType.ArraySize:
                    prop.arraySize = (int)obj;
                    break;
                case SerializedPropertyType.Character:
                    prop.intValue = (char)obj;
                    break;
                case SerializedPropertyType.AnimationCurve:
                    prop.animationCurveValue = (AnimationCurve)obj;
                    break;
                case SerializedPropertyType.Bounds:
                    prop.boundsValue = (Bounds)obj;
                    break;
                case SerializedPropertyType.Gradient:
                    SetGradientValue(prop, (Gradient)obj);
                    break;
                case SerializedPropertyType.Quaternion:
                    prop.quaternionValue = (Quaternion)obj;
                    break;
                case SerializedPropertyType.Generic:
                    SetTargetObjectOfProperty(prop, null);
                    break;
                default:
                    Debug.Log("Type not implemented: " + prop.propertyType);
                    break;
            }
        }

        /// <summary>
        /// Tries to get the Gradient value of the <paramref name="prop"/> using reflection, if it fails returns null
        /// </summary>
        /// <param name="prop">SerializedProperty to get the value from</param>
        /// <returns>Gradient value, or null if it fails</returns>
        Gradient GetGradientValue(SerializedProperty prop)
        {
            PropertyInfo propertyInfo =
                typeof(SerializedProperty).GetProperty("gradientValue",
                                                       BindingFlags.Public | BindingFlags.NonPublic |
                                                       BindingFlags.Instance);

            if (propertyInfo == null)
                return null;

            return propertyInfo.GetValue(prop, null) as Gradient;
        }

        /// <summary>
        /// Tries to set the Gradient value of the <paramref name="prop"/> using reflection, if it fails nothing is saved
        /// </summary>
        /// <param name="prop">SerializedProperty to get the value from</param>
        /// <param name="gradient">Gradient value to save</param>
        void SetGradientValue(SerializedProperty prop, Gradient gradient)
        {
            PropertyInfo propertyInfo =
                typeof(SerializedProperty).GetProperty("gradientValue",
                                                       BindingFlags.Public | BindingFlags.NonPublic |
                                                       BindingFlags.Instance);

            if (propertyInfo == null)
                return;

            propertyInfo.SetValue(prop, gradient, null);
        }

        /// <summary>
        /// Special check for a dictionary with a generic value type. This tries to set the key to its id field
        /// </summary>
        /// <param name="keyProp">Key property</param>
        /// <param name="valueProp">Value property</param>
        /// <param name="obj">Key value to set</param>
        void SetGenericValue(SerializedProperty keyProp, SerializedProperty valueProp, object obj)
        {
            SetPropertyValue(keyProp, obj);

            if (IdAttribute == null)
            {
                //This generic dictionary doesn't contain an id attribute
                return;
            }

            SerializedProperty id = valueProp.FindPropertyRelative(IdAttribute.Id);

            if (id == null)
            {
                Debug.LogError("Couldn't find any id field with name '" + IdAttribute.Id + "' on field: " + fieldInfo.Name);
                return;
            }

            SetPropertyValue(id, obj);
        }

        /// <summary>
        /// Returns the default value to use as key, this includes the new DefaultKeyAttribute
        /// </summary>
        /// <param name="prop">Key property</param>
        object GetDefaultKeyValue(SerializedProperty prop)
        {
            if (DefaultKeyAttribute != null)
            {
                return DefaultKeyAttribute.DefaultValue;
            }
            
            return GetPropertyValue(prop);
        }

        /// <summary>
        /// Returns the last available element of an enum if available; otherwise, 0
        /// </summary>
        /// <param name="prop">Property that has the enum array</param>
        int GetNotUsedEnumValue(SerializedProperty prop)
        {
            //TODO: Speed optimization - shouldn't be doing two loops and a Contains
            int index = 0;

            List<int> indexUsed = new List<int>(prop.arraySize);

            for (int i = 0; i < prop.arraySize; i++)
            {
                indexUsed.Add(prop.GetArrayElementAtIndex(i).enumValueIndex);
            }

            while(true)
            {
                if (!indexUsed.Contains(index))
                {
                    break;
                }

                index++;
            }

            if (prop.arraySize == 0 || index >= prop.GetArrayElementAtIndex(0).enumNames.Length)
                index = 0;

            return index;
        }
        
        /// <summary>
        /// Returns the next rect forcing the height to be of a single line
        /// </summary>
        /// <param name="position">Position reference</param>
        /// <returns>Next rect</returns>
        Rect GetNextRect(ref Rect position)
        {
            float h = EditorGUIUtility.singleLineHeight;
            Rect r = new Rect(position.x, position.y, position.width, h);
            position = new Rect(position.x, position.y + h, position.width, h);
            return r;
        }
    }
}
