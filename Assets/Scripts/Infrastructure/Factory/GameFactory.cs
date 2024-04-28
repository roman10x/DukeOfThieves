using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DukeOfThieves.Common;
using DukeOfThieves.Infrastructure.AssetManagement;
using DukeOfThieves.Logic;
using DukeOfThieves.Services;
using DukeOfThieves.Services.Randomizer;
using DukeOfThieves.StaticData;
using UnityEngine;
using UnityEngine.AI;
using Object = UnityEngine.Object;

namespace DukeOfThieves.Infrastructure
{
    public class GameFactory: IGameFactory
  {
    public List<ISavedProgressReader> ProgressReaders { get; } = new List<ISavedProgressReader>();
    public List<ISavedProgress> ProgressWriters { get; } = new List<ISavedProgress>();

    private readonly IAssetProvider _assets;
    private readonly IStaticDataService _staticData;
    private readonly IRandomService _randomService;
    private readonly IPersistentProgressService _persistentProgressService;
    private GameObject _heroGameObject;
    private readonly IGameStateMachine _stateMachine;

    private LevelStorage _levelStorage;
    private LevelLayoutController _layoutController;
    private LevelStaticData _levelData;
    private int _selectedLevelIndex;
    private CoinLogic _coin;
    
    public GameFactory(
      IAssetProvider assets, 
      IStaticDataService staticData, 
      IRandomService randomService, 
      IPersistentProgressService persistentProgressService, 
      IGameStateMachine stateMachine)
    {
      _assets = assets;
      _staticData = staticData;
      _randomService = randomService;
      _persistentProgressService = persistentProgressService;
      _stateMachine = stateMachine;
    }

    public async Task WarmUp(Action onWarmed)
    {
      _levelStorage = await _assets.Load<LevelStorage>(AssetAddress.LevelStorage);
      var coinObject = await _assets.Load<GameObject>(AssetAddress.Coin);
      _coin = coinObject.GetComponent<CoinLogic>();
      onWarmed?.Invoke();
    }

    public void SetLevelLayoutController(LevelLayoutController layoutController)
    {
      _layoutController = layoutController;
    }

    public async Task<GameObject> CreateHero(Vector2 at) =>
      _heroGameObject = await InstantiateRegisteredAsync(AssetAddress.HeroPath, at);

    public void CreateLevel()
    {
      _layoutController.Init(_levelData, _coin, _selectedLevelIndex, _persistentProgressService);
    }

    private void Register(ISavedProgressReader progressReader)
    {
      if (progressReader is ISavedProgress progressWriter)
        ProgressWriters.Add(progressWriter);

      ProgressReaders.Add(progressReader);
    }

    public void Cleanup()
    {
      ProgressReaders.Clear();
      ProgressWriters.Clear();
      
      _assets.Cleanup();
    }

    public LevelStaticData PrepareLevel(int index)
    {
      _selectedLevelIndex = index;
      _levelData = _levelStorage.GetLevelByIndex(index);
      return _levelData;
    }
   

    private async Task<GameObject> InstantiateRegisteredAsync(string prefabPath, Vector2 at)
    {
      GameObject gameObject = await _assets.Instantiate(path: prefabPath, at: at);
      RegisterProgressWatchers(gameObject);

      return gameObject;
    }
   
    private void RegisterProgressWatchers(GameObject gameObject)
    {
      foreach (ISavedProgressReader progressReader in gameObject.GetComponentsInChildren<ISavedProgressReader>())
        Register(progressReader);
    }
  }
}