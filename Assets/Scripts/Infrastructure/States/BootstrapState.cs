
using DukeOfThieves.Common;
using DukeOfThieves.Infrastructure.AssetManagement;
using DukeOfThieves.Services;
using DukeOfThieves.Services.Randomizer;
using UICore;
using UnityEngine;

namespace DukeOfThieves.Infrastructure
{
  public class BootstrapState : IState
  {
    private const string Initial = "InitialScene";
    
    private readonly GameStateMachine _stateMachine;
    private readonly SceneLoader _sceneLoader;
    private readonly UIManager _uiManager;
    private readonly InputListener _inputListener;
    private readonly AllServices _services;

    private AssetProvider _assetProvider;

    public BootstrapState(GameStateMachine stateMachine, SceneLoader sceneLoader, UIManager uiManager,
      InputListener inputListener,
      AllServices services)
    {
      _stateMachine = stateMachine;
      _sceneLoader = sceneLoader;
      _uiManager = uiManager;
      _services = services;
      _inputListener = inputListener;
      RegisterServices();
    }

    public void Enter()
    {
      _sceneLoader.Load(Initial, onLoaded: EnterMainMenu);
    }

    public void Exit()
    {
    }

    private void RegisterServices()
    {
      _services.RegisterSingle<IGameStateMachine>(_stateMachine);
      RegisterAssetProvider();
      _services.RegisterSingle<IRandomService>(new RandomService());
      _services.RegisterSingle<IPersistentProgressService>(new PersistentProgressService());

      _services.RegisterSingle<IGameFactory>(new GameFactory(
        _services.Single<IAssetProvider>(),
        _services.Single<IStaticDataService>(),
        _services.Single<IRandomService>(),
        _services.Single<IPersistentProgressService>(),
        _services.Single<IGameStateMachine>()
        ));
      
      _services.RegisterSingle<InputListener>(_inputListener);
      
      _services.RegisterSingle<ISaveLoadService>(new SaveLoadService(
        _services.Single<IPersistentProgressService>(),
        _services.Single<IGameFactory>()));
      
      RegisterStaticDataService();
      
      _services.RegisterSingle<UIManager>(_uiManager);
      _uiManager.Init(_assetProvider);
    }

    private void RegisterAssetProvider()
    { 
      _assetProvider = new AssetProvider();
      _services.RegisterSingle<IAssetProvider>(_assetProvider);
      _assetProvider.Initialize();
    }

    private void RegisterStaticDataService()
    {
      IStaticDataService staticData = new StaticDataService();
      staticData.Load(_assetProvider);
      _services.RegisterSingle(staticData);
    }

    private void EnterMainMenu() =>
      _stateMachine.Enter<LoadProgressState>();
  }
}