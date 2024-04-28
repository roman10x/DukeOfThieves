using System;
using System.Collections;
using UI.Windows;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DukeOfThieves.Logic
{
    public class CoinLogic : MonoBehaviour
    {
        [SerializeField]
        private float _randomSpawnMinTime = 2.0f;
        [SerializeField]
        private float _randomSpawnMaxTime = 5.0f;
        [SerializeField]
        private Collider2D _collider;
        [SerializeField]
        private GameObject _coinSpriteObj;
        [SerializeField] 
        private int _premiumCoinMultiplier;
        
        private bool _isPaused;

        private Action<int> _onCollected;
            
        public void Initialize(Action<int> onCollectedCallback)
        {
            PausePopUp.OnPauseTap -= HandlePause;
            PausePopUp.OnPauseTap += HandlePause;
            _onCollected = onCollectedCallback;
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                _onCollected?.Invoke(OnCollected());
            }

         
        }
        private int OnCollected()
        {
            var amountOfCoinsPerCollect = 1;
            _collider.enabled = false;
            StartCoroutine(Respawn());
            return amountOfCoinsPerCollect;
        }
        
        private IEnumerator Respawn()
        {
            _collider.enabled = false;
            _coinSpriteObj.SetActive(false);
            var countdown = Random.Range(_randomSpawnMinTime, _randomSpawnMaxTime);
            while (countdown > 0f)
            {
                if (!_isPaused)
                {
                    yield return new WaitForSeconds(1f);
                    countdown -= 1f;
                }
                else
                {
                    yield return null; 
                }
            }
            _coinSpriteObj.SetActive(true);
            _collider.enabled = true;
            yield return null;
        }
        
        private void HandlePause(bool pauseState)
        {
            _isPaused = pauseState;
        }
    }
}