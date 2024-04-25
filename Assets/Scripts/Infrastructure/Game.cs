using DukeOfThieves.Logic;
using DukeOfThieves.Services;
using UnityEngine;

namespace DukeOfThieves.Infrastructure
{
    public class Game
    {
        private GameStateMachine _stateMachine;

        public GameStateMachine StateMachine => _stateMachine;

        public Game(ICoroutineRunner coroutineRunner, LoadingCurtain curtain)
        {
            _stateMachine = new GameStateMachine(new SceneLoader(coroutineRunner), curtain, AllServices.Container);
        }
    }
}