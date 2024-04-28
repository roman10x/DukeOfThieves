using System;
using System.Collections;
using System.Threading.Tasks;
using DukeOfThieves.Logic;
using DukeOfThieves.Services;
using UICore;
using UnityEngine;

namespace DukeOfThieves.Infrastructure
{
    public class MainMenuState : IState
    {
        private const string MainSceneName = "GameScene";

        private readonly GameStateMachine _stateMachine;
        private readonly SceneLoader _sceneLoader;
        private readonly LoadingCurtain _loadingCurtain;
        private readonly UIManager _uiManager;

        public MainMenuState(GameStateMachine gameStateMachine, 
            SceneLoader sceneLoader, 
            LoadingCurtain loadingCurtain, 
            UIManager uiManager
            )
        {
            _stateMachine = gameStateMachine;
            _loadingCurtain = loadingCurtain;
            _sceneLoader = sceneLoader;
            _uiManager = uiManager;
        }

        public void Exit()
        {
        }

        public void Enter()
        {
            _loadingCurtain.Show();
            _uiManager.QueuePush(WindowKeys.MainMenuWindow, callback: OnMainWindowPushed);
        }

        private void OnMainWindowPushed(Window window)
        {
            if (window.Key == WindowKeys.MainMenuWindow)
                _sceneLoader.Load(MainSceneName, OnLoaded);
        }

        private void OnLoaded()
        {
            _loadingCurtain.Hide();
        }
    }
}