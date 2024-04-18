using System;
using UnityEngine;

namespace DukeOfThieves.Common
{
    public class CollisionEventData
    {
        public Action<Transform> OnCollisionEnter;
        public Action<Transform> OnCollisionExit;
    }
    public class CollisionEventHandler : MonoBehaviour
    {
        private CollisionEventData _data;
        private bool _initialized;
        
        public void Initialize(CollisionEventData data)
        {
            _data = data;
            _initialized = true;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (!_initialized)
                return;
            
            _data.OnCollisionEnter?.Invoke(collision.transform);
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            if (!_initialized)
                return;
            
            _data.OnCollisionExit?.Invoke(collision.transform);
        }
    }
}