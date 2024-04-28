using System.Collections;
using UnityEngine;

namespace DukeOfThieves.Logic
{
    public class LoadingCurtain : MonoBehaviour
    {
        private const float _curtainDarkAlpha = 0.03f;

        [SerializeField]
        private CanvasGroup _curtain;

        private WaitForSeconds _waitForSeconds = new WaitForSeconds(0.03f);

        private void Awake()
        {
            DontDestroyOnLoad(this);
        }

        public void Show()
        {
            _curtain.alpha = 1;
            gameObject.SetActive(true);
        }
    
        public void Hide() => StartCoroutine(DoFadeIn());
    
        private IEnumerator DoFadeIn()
        {
            while (_curtain.alpha > 0)
            {
                _curtain.alpha -= _curtainDarkAlpha;
                yield return _waitForSeconds;
            }
      
            gameObject.SetActive(false);
        }
    }
}