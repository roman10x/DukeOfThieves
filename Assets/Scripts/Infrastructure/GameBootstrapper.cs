using DukeOfThieves.Common;
using DukeOfThieves.Logic;
using UICore;
using UnityEngine;

namespace DukeOfThieves.Infrastructure
{
    public class GameBootstrapper : MonoBehaviour, ICoroutineRunner
    {
        [SerializeField]
        private LoadingCurtain _curtainPrefab;
        [SerializeField] 
        private UIManager _uiManager;
        [SerializeField] 
        private InputListener _inputListener;
        
        private Game _game;

        private void Awake()
        {
            var loadingCurtain = Instantiate(_curtainPrefab);
            var uiManager = Instantiate(_uiManager, parent: this.transform);
            var inputListener = Instantiate(_inputListener, parent: this.transform);
            
            _game = new Game(this, loadingCurtain, uiManager, inputListener);
            _game.StateMachine.Enter<BootstrapState>();

            DontDestroyOnLoad(this);
        }
    }
}