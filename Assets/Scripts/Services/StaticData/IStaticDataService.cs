
using DukeOfThieves.StaticData;
using UnityEngine;

namespace DukeOfThieves.Services
{
  public interface IStaticDataService : IService
  {
    void Load();
    LevelStaticData ForLevel(string sceneKey);
  }
}