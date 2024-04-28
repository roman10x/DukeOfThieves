

using System.Threading.Tasks;
using DukeOfThieves.Data;
using DukeOfThieves.Infrastructure.AssetManagement;
using DukeOfThieves.Services;
using DukeOfThieves.StaticData;

namespace DukeOfThieves.Infrastructure
{
  public class LoadProgressState : IState
  {
    private readonly GameStateMachine _gameStateMachine;
    private readonly IPersistentProgressService _progressService;
    private readonly ISaveLoadService _saveLoadProgress;

    public LoadProgressState(GameStateMachine gameStateMachine, IPersistentProgressService progressService, ISaveLoadService saveLoadProgress)
    {
      _gameStateMachine = gameStateMachine;
      _progressService = progressService;
      _saveLoadProgress = saveLoadProgress;
    }

    public void Enter()
    {
      LoadProgressOrInitNew();
      
    }

    public void Exit()
    {
    }

    private async Task LoadProgressOrInitNew()
    {
      var playerProgress = _saveLoadProgress.LoadProgress();
      if (playerProgress != null)
      {
        _progressService.Progress = playerProgress;
      }
      else
      {
        _progressService.Progress = await NewProgress();
        _saveLoadProgress.SaveProgress();
      }
      _gameStateMachine.Enter<MainMenuState>();
    }

    private async Task<PlayerProgress> NewProgress()
    {
      var assetService = AllServices.Container.Single<IAssetProvider>();
      var levelStorage = await assetService.Load<LevelStorage>(AssetAddress.LevelStorage);
      var progress =  new PlayerProgress(levelStorage);
      return progress;
    }
  }
}