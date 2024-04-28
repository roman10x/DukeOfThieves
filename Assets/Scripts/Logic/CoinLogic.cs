using System.Collections;
using UnityEngine;

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
            
        public void Initialize()
        {
            
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                OnCollected();
            }

         
        }
        public int OnCollected()
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
            yield return new WaitForSeconds(countdown);
            _coinSpriteObj.SetActive(true);
            _collider.enabled = true;
            yield return null;
        }
    }
}