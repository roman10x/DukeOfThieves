using UnityEngine;

namespace RotaryHeart.Lib.SerializableDictionaryPro
{
    [CreateAssetMenu(fileName = "HashSetExample.asset", menuName = "Data Base (HashSet)")]
    public class HashSetExample : ScriptableObject
    {
        [SerializeField]
        private _String _string;

        [System.Serializable]
        public class _String : SerializableHashSet<string> { };

        [ContextMenu("Print")]
        public void Print()
        {
            Debug.Log(JsonUtility.ToJson(_string));
        }
    }
}