using DukeOfThieves.Data;
using RotaryHeart.Lib.SerializableDictionaryPro;
using UnityEngine;

namespace UICore
{
    /// <summary>
    ///   <para> Container to hold data of all Windows in game.</para>
    /// </summary>
    [CreateAssetMenu(fileName = "Window Storage", menuName = "Data/Create window Storage")]
    [System.Serializable]
    public partial class WindowStorage : ScriptableObject
    {
        [SerializeField, HideInInspector]
        private SerializableDictionary<WindowKeys, Window> _windowByKey = new SerializableDictionary<WindowKeys, Window>();

        public Window GetWindowByKey(WindowKeys windowKey)
        {
            var window = _windowByKey[windowKey];
            return window;
        }
    }
}