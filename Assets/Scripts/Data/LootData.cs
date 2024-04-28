using System;
using Newtonsoft.Json;
using UnityEngine;

namespace DukeOfThieves.Data
{
  [Serializable]
  public class LootData
  {
    [JsonProperty(PropertyName = "collectedCoins")] 
    private int _collectedCoins;
    
    [JsonIgnore]
    public Action Changed;

    public void Collect(Loot loot)
    {
      _collectedCoins += loot.Value;
      Changed?.Invoke();
    }
    public void Add(int lootValue)
    {
      _collectedCoins += lootValue;
      Changed?.Invoke();
    }

    public LootData()
    {
      _collectedCoins = 0;
    }
    
    [JsonConstructor]
    private LootData(int collectedCoins)
    {
      _collectedCoins = collectedCoins;
    }
  }
}