using System;
using System.Threading.Tasks;
using DukeOfThieves.Logic;
using DukeOfThieves.Services;
using DukeOfThieves.StaticData;
using UI.Windows;
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
        private readonly LevelLayoutController _levelLayoutController;
        private readonly ILevelSessionDataService _levelSessionDataService;

        private int _levelToLoad;
        private string _sceneName;

        public LoadLevelState(GameStateMachine gameStateMachine,
            SceneLoader sceneLoader,
            LoadingCurtain loadingCurtain,
            UIManager uiManager,
            IGameFactory gameFactory,
            IPersistentProgressService progressService,
            IStaticDataService staticDataService, 
            ILevelSessionDataService levelSessionDataService
            )
        {
            _stateMachine = gameStateMachine;
            _sceneLoader = sceneLoader;
            _loadingCurtain = loadingCurtain;
            _gameFactory = gameFactory;
            _progressService = progressService;
            _staticData = staticDataService;
            _uiManager = uiManager;
            _levelSessionDataService = levelSessionDataService;
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
            PausePopUp.OnLevelQuit -= OnLevelFinished;
            _loadingCurtain.Hide();
        }

        private async Task OnLoaded()
        {
            await InitGameWorld();
            await Task.Delay(TimeSpan.FromSeconds(0.5)); // To avoid loading blink on fast devices
            InformProgressReaders();
            OpenHud();
        }

        private void OpenHud()
        {
            _uiManager.QueuePush(WindowKeys.GameHud, callback: OpenStartPopUp);
        }
        
        private void OpenStartPopUp(Window openedWindow)
        {
            _uiManager.QueuePush(WindowKeys.TapToStartPopUp, callback: StartGame);
        }

        private void StartGame(Window openedWindow)
        {
            _stateMachine.Enter<GameLoopState>();
        }

        private void InformProgressReaders()
        {
            foreach (ISavedProgressReader progressReader in _gameFactory.ProgressReaders)
                progressReader.LoadProgress(_progressService.Progress);
        }

        private async Task InitGameWorld()
        {
            var levelData = _gameFactory.PrepareLevel(_levelToLoad);
            _gameFactory.CreateLevel(OnLevelFinished);
            await InitSpawners(levelData);
            await InitLootPieces();
            GameObject hero = await InitHero(levelData);
        }

        private async Task InitSpawners(LevelStaticData levelStaticData)
        {
            
        }

        private async Task InitLootPieces()
        {
            
        }

        private async Task<GameObject> InitHero(LevelStaticData levelStaticData) =>
            await _gameFactory.CreateHero(levelStaticData.InitialHeroPosition);

        private void OnLevelFinished(bool isQuit)
        {
            if(isQuit)
                _levelSessionDataService.CleanupProgress();
            
            _stateMachine.Enter<MainMenuState>();
        }
    }
}