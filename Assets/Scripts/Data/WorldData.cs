using System;
using System.Collections.Generic;
using DukeOfThieves.StaticData;
using Newtonsoft.Json;

namespace DukeOfThieves.Data
{
    [Serializable]
    public class WorldData
    {
        [JsonProperty(PropertyName = "lootDataByLevelIndex")]
        private List<LootData> _lootDataByLevelIndex;

        public LootData LootDataForLevel(int index)
        {
            return _lootDataByLevelIndex[index];
        }

        public void AddLootValueForLevel(int levelIndex, int lootValue)
        {
            var loot = new Loot(lootValue);
            _lootDataByLevelIndex[levelIndex].Collect(loot);
        }

        public WorldData(LevelStorage levelStorage)
        {
            _lootDataByLevelIndex = new List<LootData>(levelStorage.GetTotalLevelAmount());
            SetDefaultLootDataForLevels(levelStorage);
        }

        private void SetDefaultLootDataForLevels(LevelStorage levelStorage)
        {
            var totalLevelsCount = levelStorage.GetTotalLevelAmount();
            for (int i = 0; i < totalLevelsCount; i++)
            {
                _lootDataByLevelIndex.Insert(i, new LootData());
            }
        }

        [JsonConstructor]
        private WorldData(List<LootData> lootDataByLevelIndex)
        {
            _lootDataByLevelIndex = lootDataByLevelIndex;
        }
    }
}