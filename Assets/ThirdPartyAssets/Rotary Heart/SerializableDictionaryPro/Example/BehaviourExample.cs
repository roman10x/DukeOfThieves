using UnityEngine;

namespace RotaryHeart.Lib.SerializableDictionaryPro
{
    public class BehaviourExample : MonoBehaviour
    {
        [System.Serializable]
        public class MyDict : SerializableDictionary<int, bool>
        {
            public MyDict() : base(false, false, false, true) { }
        }
        [System.Serializable]
        public class MyDict2 : SerializableOrderedDictionary<int, bool>
        {
            public MyDict2() : base(false, false, false, true) { }
        }

        [System.Serializable]
        public class MyDict3 : SerializableSortedDictionary<int, bool>
        {
            public MyDict3() : base(false, false, false, true) { }
        }

        [System.Serializable]
        public class MyDict4 : SerializableBiDictionary<int, string>
        {
            public MyDict4() : base(false, false, false, true) { }
        }

        [System.Serializable]
        public class MyDict5 : SerializableLinkedList<int>
        {
            public MyDict5() : base(false, false, false, true) { }
        }

        public MyDict dictionary;
        public MyDict2 orderedDictionary;
        public MyDict3 sortedDictionary;
        public MyDict4 bidirectionalDictionary;
        public MyDict5 linkedList;

        // Use this for initialization
        void Start()
        {
            for (int i = 0; i < 15; i++)
            {
                dictionary.Add(i, false);
                orderedDictionary.Add(i, false);
                sortedDictionary.Add(i, false);
                bidirectionalDictionary.Add(i, i.ToString());
                linkedList.AddLast(i);
            }

            for (int i = 0; i < 4; i++)
            {
                int removal = Random.Range(0, 15);

                dictionary.Remove(removal);
                orderedDictionary.Remove(removal);
                sortedDictionary.Remove(removal);
                bidirectionalDictionary.Remove(i);
                linkedList.Remove(i);
            }

            for (int i = 15; i < 20; i++)
            {
                dictionary.Add(i, true);
                orderedDictionary.Add(i, true);
                sortedDictionary.Add(i, true);
                bidirectionalDictionary.Add(i, i.ToString());
                linkedList.AddLast(i);
            }

            Debug.Log("For Key: 14 Value is " + bidirectionalDictionary[14]);
            Debug.Log("For Value: \"14\" Key is " + bidirectionalDictionary["14"]);
        }
    }
}