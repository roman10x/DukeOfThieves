using RotaryHeart.Lib.SerializableDictionaryPro;
using UnityEngine;

namespace DukeOfThieves.StaticData
{
   
        [CreateAssetMenu(fileName = "LevelStorage", menuName = "Data/Create level Storage")]
        [System.Serializable]
        public partial class LevelStorage : ScriptableObject
        {
            [SerializeField]
            private SerializableDictionary<int, LevelStaticData> _windowByKey = new SerializableDictionary<int, LevelStaticData>();

            public LevelStaticData GetLevelByIndex(int index)
            {
                var level = _windowByKey[index];
                return level;
            }
        }
    
}