using System;
using System.Collections.Generic;
using DukeOfThieves.StaticData;
using Newtonsoft.Json;
using RotaryHeart.Lib.SerializableDictionaryPro;
using UnityEngine;

namespace DukeOfThieves.Data
{
    [Serializable]
    public class WorldData
    {
        [JsonProperty(PropertyName = "lootDataByLevelIndex")]
        private List<LootData> _lootDataByLevelIndex;

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