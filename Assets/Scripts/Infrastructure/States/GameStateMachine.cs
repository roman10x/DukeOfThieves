using System;
using System.Collections.Generic;
using DukeOfThieves.Common;
using DukeOfThieves.Logic;
using DukeOfThieves.Services;
using UICore;


namespace DukeOfThieves.Infrastructure
{
  public class GameStateMachine : IGameStateMachine
  {
    private Dictionary<Type, IExitableState> _states;
    private IExitableState _activeState;

    public GameStateMachine(SceneLoader sceneLoader, LoadingCurtain loadingCurtain, UIManager uiManager, InputListener inputListener, AllServices services)
    {
      _states = new Dictionary<Type, IExitableState>
      {
        [typeof(BootstrapState)] = new BootstrapState(this, sceneLoader, uiManager, inputListener, services),
        [typeof(MainMenuState)] = new MainMenuState(this, sceneLoader, loadingCurtain, uiManager),
        [typeof(LoadLevelState)] = new LoadLevelState(this, sceneLoader, loadingCurtain, uiManager, services.Single<IGameFactory>(),
          services.Single<IPersistentProgressService>(), services.Single<IStaticDataService>(), services.Single<ILevelSessionDataService>()),
        [typeof(LoadProgressState)] = new LoadProgressState(this, services.Single<IPersistentProgressService>(), services.Single<ISaveLoadService>()),
        
        [typeof(GameLoopState)] = new GameLoopState(this, services.Single<IPersistentProgressService>(), services.Single<ILevelSessionDataService>(), services.Single<IGameFactory>()),
        
      };
    }
    
    public void Enter<TState>() where TState : class, IState
    {
      IState state = ChangeState<TState>();
      state.Enter();
    }

    public void Enter<TState, TPayload>(TPayload payload) where TState : class, IPayloadedState<TPayload>
    {
      TState state = ChangeState<TState>();
      state.Enter(payload);
    }

    private TState ChangeState<TState>() where TState : class, IExitableState
    {
      _activeState?.Exit();
      
      TState state = GetState<TState>();
      _activeState = state;
      
      return state;
    }

    private TState GetState<TState>() where TState : class, IExitableState => 
      _states[typeof(TState)] as TState;
  }
}