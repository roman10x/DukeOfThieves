using System;
using System.Threading.Tasks;
using DukeOfThieves.Logic;
using DukeOfThieves.Services;
using DukeOfThieves.StaticData;
using UICore;
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
    private readonly UIManager _uiManager;

    private int _levelToLoad;
    private string _sceneName;

    public LoadLevelState(GameStateMachine gameStateMachine, SceneLoader sceneLoader, LoadingCurtain loadingCurtain, UIManager uiManager, IGameFactory gameFactory, IPersistentProgressService progressService, IStaticDataService staticDataService)
    {
      _stateMachine = gameStateMachine;
      _sceneLoader = sceneLoader;
      _loadingCurtain = loadingCurtain;
      _gameFactory = gameFactory;
      _progressService = progressService;
      _staticData = staticDataService;
      _uiManager = uiManager;
    }

    public void Enter(int levelIndex)
    {
      _levelToLoad = levelIndex;
      _loadingCurtain.Show();
      _uiManager.QueuePop(WindowKeys.StartLevelPopUp);
      _uiManager.QueuePop(WindowKeys.MainMenuWindow);
      _gameFactory.Cleanup();
      _gameFactory.WarmUp(OnWarmed);
    }

    private void OnWarmed()
    {
      OnLoaded();
    }

    public void Exit()
    {
      Debug.Log("Level load exit");
      _loadingCurtain.Hide();
    }

    private async Task OnLoaded()
    {
      await InitGameWorld();
      await Task.Delay(TimeSpan.FromSeconds(0.5)); // To avoid loading blink on fast devices
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
      LevelStaticData levelData = _gameFactory.PrepareLevel(_levelToLoad);

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
   
  }
}