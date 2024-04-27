using UnityEngine;

namespace RotaryHeart.Lib.SerializableDictionaryPro
{
    /// <summary>
    /// This class is only used to be able to draw the custom property drawer
    /// </summary>
    [System.Serializable]
    public abstract class DrawableDictionary
    {
#if UNITY_EDITOR
        /// <summary>
        /// Called when an item is removed from the dictionary. It will send the indexes of all the removed items.
        /// </summary>
        [System.NonSerialized]
        public System.Action<int[]> EditorOnRemove;
        /// <summary>
        /// Called when a new item is added. It will send the added SerializedProperty key and SerializedProperty value.
        /// </summary>
        [System.NonSerialized]
        public System.Action<UnityEditor.SerializedProperty, UnityEditor.SerializedProperty> EditorOnAdded;
        /// <summary>
        /// Called when an element is drawn. It will send the position and element drawn.
        /// </summary>
        [System.NonSerialized]
        public System.Action<Rect, UnityEditor.SerializedProperty> EditorOnDrawElement;
        /// <summary>
        /// Called when the items are reordered. It will send the previous and new index for the relocated item.
        /// </summary>
        [System.NonSerialized]
        public System.Action<int, int> EditorOnReorder;
        /// <summary>
        /// Called when an element is edited. It will send both the key and value as SerializedProperty
        /// </summary>
        [System.NonSerialized]
        public System.Action<UnityEditor.SerializedProperty, UnityEditor.SerializedProperty> EditorOnEdit;
        
        [System.NonSerialized]
        public ReorderableList reorderableList = null;

        [HideInInspector, SerializeField]
        byte m_val = 7;
#endif

        public DrawableDictionary(bool canAdd = true, bool canRemove = true, bool canReorder = true, bool readOnly = false)
        {
#if UNITY_EDITOR
            m_val = 0;

            if (canAdd)
                m_val += 1;
            if (canRemove)
                m_val += 2;
            if (canReorder)
                m_val += 4;
            if (readOnly)
                m_val += 8;
#endif
        }
    }
}