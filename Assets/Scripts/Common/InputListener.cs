using System;
using DukeOfThieves.Services;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DukeOfThieves.Common
{
    public class InputListener : MonoBehaviour, IPointerDownHandler, IService
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
