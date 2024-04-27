using UnityEngine;

namespace RotaryHeart.Lib.SerializableDictionaryPro
{
    [CreateAssetMenu(fileName = "DataBaseOrdered.asset", menuName = "Data Base (Ordered)")]
    public class OrderedExample : ScriptableObject
    {
        public enum EnumExample
        {
            None,
            Value1,
            Value2,
            Value3,
            Value4,
            Value5,
            Value6
        }

        [SerializeField]
        private Enum_String _enumString;

        [System.Serializable]
        public class Enum_String : SerializableOrderedDictionary<EnumExample, string> { };

        [ContextMenu("Print")]
        public void Print()
        {
            Debug.Log(JsonUtility.ToJson(_enumString));
        }
    }
}