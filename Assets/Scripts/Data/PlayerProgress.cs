using System;
using UnityEngine;

namespace DukeOfThieves.Data
{
  [Serializable]
  public class PlayerProgress
  {
    [SerializeField]
    private WorldData _worldData;
   

    public WorldData WorldData => _worldData;
    
    
    public PlayerProgress(string initialLevel)
    {
      _worldData = new WorldData(initialLevel);
    }
  }
}