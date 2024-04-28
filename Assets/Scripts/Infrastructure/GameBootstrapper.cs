using DukeOfThieves.Common;
using DukeOfThieves.Logic;
using DukeOfThieves.Services;
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
        [SerializeField] 
        private LevelLayoutController _levelLayoutControllerPrefab;

        private LevelLayoutController _levelLayoutController;
        
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

        public void SetupLevelLayoutController()
        {
            var levelController = Instantiate(_levelLayoutControllerPrefab);
            _levelLayoutController = levelController;
            AllServices.Container.Single<IGameFactory>().SetLevelLayoutController(_levelLayoutController);
        }
    }
}