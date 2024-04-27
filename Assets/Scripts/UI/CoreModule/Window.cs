using System;
using UnityEngine;

namespace UICore
{
    /// <summary>
    /// <para> Window is a base class for UI windows.</para>
    /// </summary>
    [Serializable]
    public class Window : UIForm
    {
        /// <summary>
        /// Context container that is passed along to Window that are being pushed. Windows can use these to setup
        /// themselves up with custom data provided at run-time.
        /// </summary>
        public abstract class Context { }

        [SerializeField] private bool _createBackgroundCollider; //TODO implement creation of the collider in order to close popups by tapping in the zone located outside of the window
        [SerializeField] private bool _keepCached;
        [SerializeField] private bool _pushedAsPopUp;
        
        public WindowKeys Key { get; private set; }
        
        public bool KeepCached 
        {
            get { return _keepCached; }
            set { _keepCached = value; }
        }

        protected Context _context;
        public bool PushedAsPopUp => _pushedAsPopUp;

        public delegate void WindowDelegate (Window window);

        public event WindowDelegate OnPushFinished;
        public event WindowDelegate OnPopFinished;

        public Action OnWindowClose;
        
        public void Setup(WindowKeys key, Context context)
        {
            Key = key;
            
            if(context != null)
                _context = context;
            
            OnSetup();
        }

        /// <summary>
        /// Called by the UIManager when this Window is being pushed to the Window stack.
        /// Be sure to call PushPopFinished when your Window is done pushing. Delaying the PushPopFinished call
        /// allows the Window to delay execution of the UIManager's Window queue.
        /// !!Be sure to call PushFinished to signal the end of the push.!!
        /// </summary>
        public virtual void OnPush()
        {
            
        }

        /// <summary>
        /// Called by the UIManager when this Window is being popped from the Window stack.
        /// Be sure to call PopFinished when your Window is done popping. Delaying the PushPopFinished call
        /// allows the Window to delay execution of the UIManager's Window queue.
        /// !!Be sure to call PopFinished to signal the end of the pop.!!
        /// </summary>
        public virtual void OnPop()
        {
            
        }

        /// <summary>
        /// Called by the UIManager when this Window becomes the top most Window in the stack.
        /// </summary>
        public virtual void OnFocus()
        {
            
        }

        /// <summary>
        /// Called by the UIManager when this Window is no longer the top most Window in the stack.
        /// </summary>
        public virtual void OnFocusLost()
        {
            
        }
        
        /// <summary>
        /// Closing current window.
        /// </summary>
        public virtual void Hide()
        {
            UIManager.Instance.QueuePop(this);
        }

        
        /// <summary>
        /// Setup is called after instantiating a Window prefab. It is only called once for the lifecycle of the Window.
        /// Run one-time setup operations here.
        /// </summary>
        protected virtual void OnSetup()
        {
            
        }

        protected void PushFinished ()
        {
            if (OnPushFinished != null)
                OnPushFinished(this);
        }

        protected void PopFinished ()
        {
            if (OnPopFinished != null)
                OnPopFinished(this);
        }
        
        
        /// <summary>
        /// Checking if local context (inherited from Window.Context class) exist.
        /// </summary>
        protected bool TryGetLocalContext<T>(out T localContext) where T : Context
        {
            localContext = null;
            var context = _context;
            var contextExist = TryGetLocalContext(context, out localContext);

            return contextExist;
        }
        protected bool TryGetLocalContext<T>(Context context, out T localContext) where T : Context
        {
            localContext = null;
            
            if(context != null)
                localContext = context as T;

            if (localContext == null)
            {
                return false;
            }
            
            return true;
        }
    }
}
