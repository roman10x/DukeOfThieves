using System;
using DukeOfThieves.Services;
using TMPro;
using UICore;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Windows
{
    public class TapToStartPopUp : Window
    {
        [SerializeField] 
        private Button _startGameButton;

        public static Action OnClose;
        
        public override void OnPush()
        {
            _startGameButton.onClick.AddListener(HandleStartGameButton);
            PushFinished();
        }

        public override void OnPop()
        {
            _startGameButton.onClick.RemoveAllListeners();
            PopFinished();
        }
        
        private void HandleStartGameButton()
        {
            OnClose?.Invoke();
            Hide();
        }
    }
}