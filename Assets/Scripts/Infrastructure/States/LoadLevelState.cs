using System.Threading.Tasks;
using DukeOfThieves.Logic;
using DukeOfThieves.Services;
using DukeOfThieves.StaticData;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DukeOfThieves.Infrastructure
{
  public class LoadLevelState : IPayloadedState<int>
  {
    private readonly GameStateMachine _stateMachine;
    private readonly SceneLoader _sceneLoader;
    private readonly LoadingCurtain _loadingCurtain;
    private readonly IGameFactory _gameFactory;
    private readonly IPersistentProgressService _progressService;
    private readonly IStaticDataService _staticData;

    private string _sceneName;

    public LoadLevelState(GameStateMachine gameStateMachine, SceneLoader sceneLoader, LoadingCurtain loadingCurtain, IGameFactory gameFactory, IPersistentProgressService progressService, IStaticDataService staticDataService)
    {
      _stateMachine = gameStateMachine;
      _sceneLoader = sceneLoader;
      _loadingCurtain = loadingCurtain;
      _gameFactory = gameFactory;
      _progressService = progressService;
      _staticData = staticDataService;
    }

    public void Enter(int levelIndex)
    {
      Debug.Log("level started");
      _loadingCurtain.Show();
      _gameFactory.Cleanup();
      _gameFactory.WarmUp(OnWarmed);
    }

    private void OnWarmed()
    {
      _sceneLoader.Load(_sceneName, OnLoaded);
    }

    public void Exit() =>
      _loadingCurtain.Hide();

    private async void OnLoaded()
    {
      await InitGameWorld();
      InformProgressReaders();

      _stateMachine.Enter<GameLoopState>();
    }

    private void InformProgressReaders()
    {
      foreach (ISavedProgressReader progressReader in _gameFactory.ProgressReaders)
        progressReader.LoadProgress(_progressService.Progress);
    }

    private async Task InitGameWorld()
    {
      LevelStaticData levelData = _gameFactory.PrepareLevel(0);

      await InitSpawners(levelData);
      await InitLootPieces();
      GameObject hero = await InitHero(levelData);
    }

    private async Task InitSpawners(LevelStaticData levelStaticData)
    {
      /*foreach (EnemySpawnerStaticData spawnerData in levelStaticData.EnemySpawners)
        await _gameFactory.CreateSpawner(spawnerData.Id, spawnerData.Position, spawnerData.MonsterTypeId);*/
    }

    private async Task InitLootPieces()
    {
      
    }

    private async Task<GameObject> InitHero(LevelStaticData levelStaticData) => 
      await _gameFactory.CreateHero(levelStaticData.InitialHeroPosition);

   

    private LevelStaticData LevelStaticData() => 
      _staticData.ForLevel(SceneManager.GetActiveScene().name);
  }
}