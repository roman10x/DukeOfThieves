using DukeOfThieves.Common;
using DukeOfThieves.Logic;
using DukeOfThieves.Services;
using UICore;
using UnityEngine;

namespace DukeOfThieves.Infrastructure
{
    public class Game
    {
        private GameStateMachine _stateMachine;

        public GameStateMachine StateMachine => _stateMachine;

        public Game(ICoroutineRunner coroutineRunner, LoadingCurtain curtain, UIManager uiManager, InputListener inputListener, LevelLayoutController levelLayoutController)
        {
            _stateMachine = new GameStateMachine(new SceneLoader(coroutineRunner), curtain, uiManager, inputListener, levelLayoutController, AllServices.Container);
        }
    }
}