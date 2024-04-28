using System;
using DukeOfThieves.Services;
using TMPro;
using UICore;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Windows
{
    public class PausePopUp : Window
    {
        [SerializeField] 
        private Button _resumeGameButton;
        [SerializeField] 
        private Button _quitGameButton;
        
        public static Action<bool> OnPauseTap;
        public static Action<bool> OnLevelQuit;
        
        public override void OnPush()
        {
            OnPauseTap?.Invoke(true);
            _resumeGameButton.onClick.AddListener(HandleResumeGameButton);
            _quitGameButton.onClick.AddListener(HandleQuitGameButton);
            PushFinished();
        }

        public override void OnPop()
        {
            _resumeGameButton.onClick.RemoveAllListeners();
            _quitGameButton.onClick.RemoveAllListeners();
            PopFinished();
        }
        
        private void HandleQuitGameButton()
        {
            OnLevelQuit?.Invoke(true);
            Hide();
        } 
        
        private void HandleResumeGameButton()
        {
            OnPauseTap?.Invoke(false);
            Hide();
        }
    }
}