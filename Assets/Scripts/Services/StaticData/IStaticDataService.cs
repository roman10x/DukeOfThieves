
using DukeOfThieves.Infrastructure.AssetManagement;
using DukeOfThieves.StaticData;
using UnityEngine;

namespace DukeOfThieves.Services
{
  public interface IStaticDataService : IService
  {
    LevelStorage LevelStorage { get; }
    void Load(AssetProvider assetProvider);
  }
}