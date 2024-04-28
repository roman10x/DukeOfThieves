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

        [JsonIgnore]
        public int TotalCoinsCollected => _totalCoinsCollected;

        public PlayerData()
        {
            _totalCoinsCollected = 0;
        }
        
        [JsonConstructor]
        private PlayerData(int totalCoinsCollected)
        {
            _totalCoinsCollected = totalCoinsCollected;
        }
    }
}