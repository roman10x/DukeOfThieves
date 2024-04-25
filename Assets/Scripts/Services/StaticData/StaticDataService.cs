using System.Collections.Generic;
using System.Linq;
using DukeOfThieves.StaticData;
using UnityEngine;

namespace DukeOfThieves.Services
{
  public class StaticDataService : IStaticDataService
  {
    private const string MonstersDataPath = "Static Data/Monsters";
    private const string LevelsDataPath = "Static Data/Levels";
    private const string StaticDataWindowPath = "Static Data/UI/WindowStaticData";

    
    private Dictionary<string, LevelStaticData> _levels;
  


    public void Load()
    {
      _levels = Resources
        .LoadAll<LevelStaticData>(LevelsDataPath)
        .ToDictionary(x => x.LevelKey, x => x);
    }

    public LevelStaticData ForLevel(string sceneKey) =>
      _levels.TryGetValue(sceneKey, out LevelStaticData staticData)
        ? staticData
        : null;
  }
}