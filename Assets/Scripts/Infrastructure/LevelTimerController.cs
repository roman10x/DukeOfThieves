using System;
using UI.Windows;
using UnityEngine;

namespace DukeOfThieves.Infrastructure
{
    public class LevelTimerController : MonoBehaviour
    {
        [SerializeField]
        private float _levelPlayTime = 60.0f; // TODO add time to level prefab if custom time will be required

        private bool _levelStarted;
        private float _levelTimeLeft;

        private Action<bool> _onTimerFinished;

        public static Action<float> OnTimerUpdated;

        public void StartTimer(Action<bool> onLevelFinished)
        {
            GameLoopState.OnGameStop += StopTimer;
            PausePopUp.OnPauseTap += HandlePause;
            TapToStartPopUp.OnClose += HandlePopUpClose;
            _levelTimeLeft = _levelPlayTime;
            _onTimerFinished = onLevelFinished;
        }

        public void StopTimer()
        {
            TapToStartPopUp.OnClose -= HandlePopUpClose;
            GameLoopState.OnGameStop -= StopTimer;
            PausePopUp.OnPauseTap -= HandlePause;
        }
        
        private void Update()
        {
            if (!_levelStarted)
            {
                return;
            }

            _levelTimeLeft -= Time.deltaTime;
            if (_levelTimeLeft <= 0.0f)
            {
                _levelStarted = false;
                _levelTimeLeft = 0.0f;
                StopTimer();
                _onTimerFinished?.Invoke(false);
            }
            
            OnTimerUpdated?.Invoke(_levelTimeLeft);
        }

        private void HandlePause(bool pauseState)
        {
            _levelStarted = !pauseState;
        }
        
        private void HandlePopUpClose()
        {
            _levelStarted = true;
        }
    }
}