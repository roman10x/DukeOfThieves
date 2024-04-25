
using DukeOfThieves.Infrastructure.AssetManagement;
using DukeOfThieves.Services;
using DukeOfThieves.Services.Randomizer;
using UI;
using UnityEngine;

namespace DukeOfThieves.Infrastructure
{
  public class BootstrapState : IState
  {
    private const string Initial = "InitialScene";
    
    private readonly GameStateMachine _stateMachine;
    private readonly SceneLoader _sceneLoader;
    private readonly AllServices _services;

    public BootstrapState(GameStateMachine stateMachine, SceneLoader sceneLoader, AllServices services)
    {
      _stateMachine = stateMachine;
      _sceneLoader = sceneLoader;
      _services = services;

      RegisterServices();
    }

    public void Enter() =>
      _sceneLoader.Load(Initial, onLoaded: EnterLoadLevel);

    public void Exit()
    {
    }

    private void RegisterServices()
    {
      RegisterStaticDataService();

      _services.RegisterSingle<IGameStateMachine>(_stateMachine);
      //RegisterAssetProvider();
      _services.RegisterSingle<IRandomService>(new RandomService());
      _services.RegisterSingle<IPersistentProgressService>(new PersistentProgressService());

      _services.RegisterSingle<UIManager>(new UIManager());
      
      _services.RegisterSingle<IGameFactory>(new GameFactory(
        _services.Single<IAssetProvider>(),
        _services.Single<IStaticDataService>(),
        _services.Single<IRandomService>(),
        _services.Single<IPersistentProgressService>(),
        _services.Single<IGameStateMachine>()
        ));
      
      _services.RegisterSingle<ISaveLoadService>(new SaveLoadService(
        _services.Single<IPersistentProgressService>(),
        _services.Single<IGameFactory>()));
    }

    private void RegisterAssetProvider()
    {
      AssetProvider assetProvider = new AssetProvider();
      _services.RegisterSingle<IAssetProvider>(assetProvider);
      assetProvider.Initialize();
    }

    private void RegisterStaticDataService()
    {
      IStaticDataService staticData = new StaticDataService();
      staticData.Load();
      _services.RegisterSingle(staticData);
    }

    private void EnterLoadLevel() =>
      _stateMachine.Enter<LoadProgressState>();
  }
}