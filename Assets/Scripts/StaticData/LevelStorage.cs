using RotaryHeart.Lib.SerializableDictionaryPro;
using UnityEngine;

namespace DukeOfThieves.StaticData
{
    [CreateAssetMenu(fileName = "LevelStorage", menuName = "Data/Create level Storage")]
    [System.Serializable]
    public class LevelStorage : ScriptableObject
    {
        [SerializeField] private SerializableDictionary<int, LevelStaticData> _levelByIndex = new SerializableDictionary<int, LevelStaticData>();

        public LevelStaticData GetLevelByIndex(int index)
        {
            var level = _levelByIndex[index];
            return level;
        }

        public int GetTotalLevelAmount()
        {
            return _levelByIndex.Count;
        }
    }
}