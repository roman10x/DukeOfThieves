using DukeOfThieves.Infrastructure;
using DukeOfThieves.Services;
using TMPro;
using UICore;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Windows
{
    public class StartLevelPopUp : Window
    {
        [SerializeField] 
        private Button _startGameButton;
        [SerializeField] 
        private TMP_Text _collectedGoldAmountLabel;

        private int _selectedLevelIndex;
        
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
            AllServices.Container.Single<IGameStateMachine>().Enter<LoadLevelState, int>(_selectedLevelIndex);
        }
    }
}