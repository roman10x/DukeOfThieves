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
