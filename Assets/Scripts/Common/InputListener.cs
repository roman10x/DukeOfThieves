using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DukeOfThieves.Common
{
    public class InputListener : MonoBehaviour, IPointerDownHandler
    {
        private Action _onTapAction;
        public void Initialize(Action onTapAction)
        {
            _onTapAction = onTapAction;
        }
        public void OnPointerDown(PointerEventData eventData)
        {
            _onTapAction?.Invoke();
        }
    }
}
