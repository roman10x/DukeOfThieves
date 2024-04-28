using System;
using DukeOfThieves.StaticData;
using Newtonsoft.Json;
using UnityEngine;

namespace DukeOfThieves.Data
{
  [Serializable]
  public class PlayerProgress
  {
    [JsonProperty(PropertyName = "worldData")]
    private WorldData _worldData;

    [JsonProperty(PropertyName = "playerData")]
    private PlayerData _playerData;

    [JsonIgnore] 
    public WorldData WorldData => _worldData;

    public PlayerProgress(LevelStorage levelStorage)
    {
      _worldData = new WorldData(levelStorage);
      _playerData = new PlayerData();
    }
    
    // Private constructor for deserialization
    [JsonConstructor]
    private PlayerProgress(WorldData worldData, PlayerData playerData)
    {
      _worldData = worldData;
      _playerData = playerData;
    }
  }
}