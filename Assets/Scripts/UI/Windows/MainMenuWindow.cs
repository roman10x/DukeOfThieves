using DukeOfThieves.Services;
using TMPro;
using UICore;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuWindow : Window
{
    [SerializeField] 
    private Button _startGameButton;
    [SerializeField] 
    private TMP_Text _collectedGoldAmountLabel;
    public override void OnPush()
    {
        _startGameButton.onClick.AddListener(HandleStartGameButton);
        var totalCoinsCollected = AllServices.Container.Single<IPersistentProgressService>().Progress.PlayerData.TotalCoinsCollected;
        _collectedGoldAmountLabel.text = $"{totalCoinsCollected} Coins Stolen";
        PushFinished();
    }

    public override void OnPop()
    {
        _startGameButton.onClick.RemoveAllListeners();
        PopFinished();
    }

    private void HandleStartGameButton()
    {
        AllServices.Container.Single<UIManager>().QueuePush(WindowKeys.StartLevelPopUp);
        AllServices.Container.Single<ISaveLoadService>().SaveProgress();
    }
}
