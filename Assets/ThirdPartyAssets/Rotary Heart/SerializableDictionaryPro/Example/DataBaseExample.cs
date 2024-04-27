using UnityEngine;

namespace RotaryHeart.Lib.SerializableDictionaryPro
{
    [CreateAssetMenu(fileName = "DataBase.asset", menuName = "Data Base")]
    public class DataBaseExample : ScriptableObject
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

        [System.Serializable]
        public class ChildTest
        {
            public Color myChildColor;
            public bool myChildBool;
            public Gradient test;
        }

        [System.Serializable]
        public class ClassTest
        {
            public string id;
            public float test;
            public string test2;
            public Quaternion quat;
            public ChildTest[] childTest;
        }

        [System.Serializable]
        public class ArrayTest
        {
            public int[] myArray;
        }

        [System.Serializable]
        public class AdvancedGenericClass
        {
            [Range(0, 100)]
            public float value;

            public bool Equals(AdvancedGenericClass other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return other.value == value;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != typeof(AdvancedGenericClass)) return false;
                return Equals((AdvancedGenericClass)obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return value.GetHashCode();
                }
            }
        }

        [SerializeField]
        public Generic_String _genericString;

        [SerializeField]
        public Generic_Generic _genericGeneric;

        [SerializeField, Id("id")]
        private S_GenericDictionary _stringGeneric;

        [SerializeField]
        private I_GenericDictionary _intGeneric;

        [SerializeField]
        private I_GO _intGameobject;
        [SerializeField]
        private GO_I _gameobjectInt;

        [SerializeField]
        private S_GO _stringGameobject;
        [SerializeField]
        private GO_S _gameobjectString;

        [SerializeField]
        private S_Mat _stringMaterial;
        [SerializeField]
        private Mat_S _materialString;

        [SerializeField]
        private V3_Q _vector3Quaternion;
        [SerializeField]
        private Q_V3 _quaternionVector3;

        [SerializeField]
        private S_AC _stringAudioClip;
        [SerializeField]
        private AC_S _audioClipString;

        [SerializeField]
        private C_Int _charInt;
        [SerializeField]
        private G_Int _gradientInt;

        [SerializeField]
        private Int_IntArray _intArray;

        [SerializeField]
        private Enum_String _enumString;

        [SerializeField, DrawKeyAsProperty]
        private AdvanGeneric_String _advancedGenericKey;

        [System.Serializable]
        public class Generic_String : SerializableDictionary<ClassTest, string> { }

        [System.Serializable]
        public class Generic_Generic : SerializableDictionary<ClassTest, ClassTest> { }

        [System.Serializable]
        public class C_Int : SerializableDictionary<char, int> { }
        [System.Serializable]
        public class G_Int : SerializableDictionary<Gradient, int> { }

        [System.Serializable]
        public class I_GO : SerializableDictionary<int, GameObject> { }
        [System.Serializable]
        public class GO_I : SerializableDictionary<GameObject, int> { }

        [System.Serializable]
        public class S_GO : SerializableDictionary<string, GameObject> { }
        [System.Serializable]
        public class GO_S : SerializableDictionary<GameObject, string> { }

        [System.Serializable]
        public class S_Mat : SerializableDictionary<string, Material> { }
        [System.Serializable]
        public class Mat_S : SerializableDictionary<Material, string> { }

        [System.Serializable]
        public class S_AC : SerializableDictionary<string, AudioClip> { }
        [System.Serializable]
        public class AC_S : SerializableDictionary<AudioClip, string> { }

        [System.Serializable]
        public class S_Sprite : SerializableDictionary<string, Sprite> { }

        [System.Serializable]
        public class V3_Q : SerializableDictionary<Vector3, Quaternion> { }
        [System.Serializable]
        public class Q_V3 : SerializableDictionary<Quaternion, Vector3> { }

        [System.Serializable]
        public class S_GenericDictionary : SerializableDictionary<string, ClassTest> { }

        [System.Serializable]
        public class I_GenericDictionary : SerializableDictionary<int, ClassTest> { }

        [System.Serializable]
        public class Int_IntArray : SerializableDictionary<int, ArrayTest> { }

        [System.Serializable]
        public class Enum_String : SerializableDictionary<EnumExample, string> { };

        [System.Serializable]
        public class AdvanGeneric_String : SerializableDictionary<AdvancedGenericClass, string> { };

        [ContextMenu("Print")]
        public void Print()
        {
            Debug.Log(JsonUtility.ToJson(_enumString));
        }
    }
}
