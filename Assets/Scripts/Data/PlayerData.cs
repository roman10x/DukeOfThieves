using System;
using Newtonsoft.Json;
using UnityEngine;

namespace DukeOfThieves.Data
{
    [Serializable]
    public class PlayerData
    {
        [JsonProperty(PropertyName = "totalCoinsCollected")] 
        private int _totalCoinsCollected;
        [JsonProperty(PropertyName = "lastFinishedLevel")] 
        private int _lastFinishedLevel;

        [JsonIgnore]
        public int TotalCoinsCollected => _totalCoinsCollected;
        [JsonIgnore]
        public int LastFinishedLevel => _lastFinishedLevel;

        public PlayerData()
        {
            _totalCoinsCollected = 0;
            _lastFinishedLevel = 0;
        }
        
        [JsonConstructor]
        private PlayerData(int totalCoinsCollected, int lastFinishedLevel)
        {
            _totalCoinsCollected = totalCoinsCollected;
            _lastFinishedLevel = lastFinishedLevel;
        }
    }
}