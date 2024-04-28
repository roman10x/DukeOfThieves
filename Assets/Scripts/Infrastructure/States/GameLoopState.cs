using System;
using DukeOfThieves.Services;
using UI.Windows;

namespace DukeOfThieves.Infrastructure
{
  public class GameLoopState : IState
  {
    private readonly IPersistentProgressService _persistentProgressService;
    private readonly IGameStateMachine _stateMachine;
    private readonly ILevelSessionDataService _sessionDataService;
    private readonly IGameFactory _gameFactory;

    public static Action OnGameStop;
    public GameLoopState(GameStateMachine stateMachine, IPersistentProgressService persistentProgressService, ILevelSessionDataService sessionDataService, IGameFactory gameFactory)
    {
      _stateMachine = stateMachine;
      _persistentProgressService = persistentProgressService;
      _sessionDataService = sessionDataService;
      _gameFactory = gameFactory;
    }

    public void Exit()
    {
      var levelIndex = _sessionDataService.CurrentLevelIndex;
      var collectedLootAmount = _sessionDataService.CollectedLootValue;
      
      if(collectedLootAmount > 0)
      {
        var playerProgress = _persistentProgressService.Progress;
        playerProgress.WorldData.AddLootValueForLevel(levelIndex, collectedLootAmount);
        playerProgress.PlayerData.AddTotalCoins(collectedLootAmount);
        playerProgress.PlayerData.SetLastFinishedLevel(levelIndex);
      }
      OnGameStop?.Invoke();
      
      AllServices.Container.Single<ISaveLoadService>().SaveProgress();
      _gameFactory.StopGame();
      PausePopUp.OnLevelQuit -= OnLevelFinished;
    }

    public void Enter()
    {
      PausePopUp.OnLevelQuit += OnLevelFinished;
    }
    
    private void OnLevelFinished(bool isQuit)
    {
      if(isQuit)
        _sessionDataService.CleanupProgress();
            
      _stateMachine.Enter<MainMenuState>();
    }
  }
}