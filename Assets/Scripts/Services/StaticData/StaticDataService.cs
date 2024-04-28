using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DukeOfThieves.Infrastructure.AssetManagement;
using DukeOfThieves.StaticData;
using UnityEngine;

namespace DukeOfThieves.Services
{
  public class StaticDataService : IStaticDataService
  {
    private LevelStorage _levelStorage;

    public LevelStorage LevelStorage => _levelStorage;
    public void Load(AssetProvider assetProvider)
    {
      SetLevelStorage(assetProvider);
    }

    private async Task SetLevelStorage(AssetProvider assetProvider)
    {
      _levelStorage = await assetProvider.Load<LevelStorage>(AssetAddress.LevelStorage);
    }
  }
}