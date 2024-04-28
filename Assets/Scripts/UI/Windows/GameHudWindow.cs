using DukeOfThieves.Infrastructure;
using DukeOfThieves.Services;
using TMPro;
using UICore;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Windows
{
    public class GameHudWindow : Window
    {
        [SerializeField] 
        private Button _pauseGameButton;
        [SerializeField] 
        private TMP_Text _totalCollectedGoldAmountLabel;
        [SerializeField] 
        private TMP_Text _sessionCollectedGoldAmountLabel;
        [SerializeField] 
        private TMP_Text _timerLabel;

        private ILevelSessionDataService _sessionService;
        private IPersistentProgressService _progressService;
        public override void OnPush()
        {
            _sessionService = AllServices.Container.Single<ILevelSessionDataService>();
            _progressService = AllServices.Container.Single<IPersistentProgressService>();
            _pauseGameButton.onClick.AddListener(HandlePauseGameButton);
            _totalCollectedGoldAmountLabel.text = $"Record: {_progressService.Progress.PlayerData.TotalCoinsCollected}";
            UpdateCurrentLootLabel(0);
            LevelSessionDataService.OnLootCollected += UpdateCurrentLootLabel;
            LevelTimerController.OnTimerUpdated += UpdateTimerCountdown;
            PausePopUp.OnLevelQuit += HandleLevelQuit;
            PushFinished();
        }

        public override void OnPop()
        {
            PausePopUp.OnLevelQuit -= HandleLevelQuit;
            LevelSessionDataService.OnLootCollected -= UpdateCurrentLootLabel;
            LevelTimerController.OnTimerUpdated -= UpdateTimerCountdown;
            _pauseGameButton.onClick.RemoveAllListeners();
            PopFinished();
        }
        
        private void HandlePauseGameButton()
        {
            AllServices.Container.Single<UIManager>().QueuePush(WindowKeys.PausePopUp);
        }

        private void UpdateCurrentLootLabel(int lootValue)
        {
            _sessionCollectedGoldAmountLabel.text = $"Collected: {lootValue}";
        }
        
        private void UpdateTimerCountdown(float countdown)
        {
            _timerLabel.text = countdown.ToString("00.00");
        }

        private void HandleLevelQuit(bool state)
        {
            Hide();
        }
    }
}