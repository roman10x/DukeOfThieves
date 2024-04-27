using System.Collections.Generic;
using System.Threading.Tasks;
using DukeOfThieves.Infrastructure;
using DukeOfThieves.Infrastructure.AssetManagement;
using DukeOfThieves.Services;
using UnityEngine;
using UnityEngine.UI;

namespace UICore
{
    /// <summary>
    ///   <para> UI manager showing and hiding UI windows and popups.</para>
    /// </summary>
    public class UIManager : MonoBehaviour, IService
    {
        private SceneLoader _sceneLoader; 
        private CanvasScaler _rootCanvasScaler;
        private const int WindowSortingStartingInt = 50;
        public static UIManager Instance { get; private set; }

        private abstract class WindowStack
        {
            public WindowKeys key;
        }

        private class WindowStackPush : WindowStack
        {
            public Window.Context context;
            public WindowKeys key;
            public PushedDelegate callback;
            public Window customWindowPrefab;
            public bool useCameraOverlay;
            public Canvas customCanvas;
            public int customSorting;
        }

        private class WindowStackPop : WindowStack
        {
            public PoppedDelegate callback;
        }

        public delegate void PushedDelegate(Window window);

        public delegate void PoppedDelegate(WindowKeys key);

        [SerializeField] private Canvas _rootCanvas;
        

        private CanvasScaler _rootCanvasScalar;
        private Dictionary<WindowKeys, Window> _cache;
        private Queue<WindowStack> _queue;
        private List<Window> _stack;
        private State _state;
        private PushedDelegate _activePushCallback;
        private PoppedDelegate _activePopCallback;
        private WindowStorage _windowStorage;
        private WindowKeys _currentWindowKey = WindowKeys.Empty;
       
        public SceneLoader SceneLoader => _sceneLoader; // TODO update classes who is using this loader

        private readonly Vector2 _zeroVector2 = new Vector2(0, 0);
        private readonly Vector2 _oneVector2 = new Vector2(1, 1);
        private readonly Vector2 _halfVector2 = new Vector2(0.5f, 0.5f);
        private readonly Vector3 _zeroVector3 = new Vector3(0, 0, 0);
        private readonly Vector3 _oneVector3 = new Vector3(1, 1, 1);
        
        private AssetProvider _assetSystem;

        private enum State
        {
            Ready,
            Push,
            Pop
        }

        public void Init(AssetProvider assetSystem)
        {
            _assetSystem = assetSystem;

            _rootCanvasScaler = _rootCanvas.GetComponent<CanvasScaler>();

            _cache = new Dictionary<WindowKeys, Window>();
            _queue = new Queue<WindowStack>();
            _stack = new List<Window>();
            SetState(State.Ready);

            // Remove any objects that left under the root.
            foreach (Transform child in _rootCanvas.transform)
            {
                Destroy(child.gameObject);
            }
           
            SetWindowStorage();
        }

      

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }
        private async Task SetWindowStorage()
        {
            _windowStorage = await _assetSystem.Load<WindowStorage>(AssetAddress.WindowStorage);
        }
        /// <summary>
        /// Queue the window to be pushed onto the window stack. 
        /// Callback will be invoked when the window is pushed to the stack.
        /// </summary>
        public void QueuePush(WindowKeys key, Window.Context context = null, PushedDelegate callback = null,
            Window customWindowPrefab = null, bool useCameraOverlay = false, Canvas customCanvas = null, int customSorting = -1)
        {
            if (GetWindow(key) != null)
            {
                return;
            }

            WindowStackPush push = new WindowStackPush
            {
                context = context,
                key = key,
                callback = callback,
                customWindowPrefab = customWindowPrefab,
                useCameraOverlay = useCameraOverlay,
                customCanvas = customCanvas,
                customSorting = customSorting
            };

            _queue.Enqueue(push);

            if (CanExecuteNextQueueItem())
                ExecuteNextQueueItem();
        }

        /// <summary>
        /// Queue the window to be popped from the stack. This will pop all windows on top of it as well.
        /// Callback will be invoked when the window is reached, or popped if 'include' is true.
        /// </summary>
        public void QueuePopTo(WindowKeys key, bool include, PoppedDelegate callback = null)
        {
            bool found = false;

            for (int i = 0; i < _stack.Count; i++)
            {
                var window = _stack[i];

                if (window.Key != key)
                {
                    var queuedPop = new WindowStackPop
                    {
                        key = window.Key
                    };

                    _queue.Enqueue(queuedPop);
                }
                else
                {
                    if (include)
                    {
                        var queuedPop = new WindowStackPop
                        {
                            key = window.Key,
                            callback = callback
                        };

                        _queue.Enqueue(queuedPop);
                    }

                    if (callback != null)
                        callback(window.Key);

                    found = true;
                    break;
                }
            }

            if (!found)
                Debug.LogWarning($"[UIManager] {key.ToString()} was not in the stack. All windows have been popped.");

            if (CanExecuteNextQueueItem())
                ExecuteNextQueueItem();
        }

        private void QueuePop()
        {
            QueuePop(null);
        }

        /// <summary>
        /// Queue the top-most window to be popped from the window stack.
        /// Callback will be invoked when the window is popped from the stack.
        /// </summary>
        public void QueuePop(PoppedDelegate callback = null)
        {
            var topWindow = GetTopWindowInStack();

            var pop = new WindowStackPop
            {
                key = topWindow.Key,
                callback = callback
            };

            _queue.Enqueue(pop);

            if (CanExecuteNextQueueItem())
                ExecuteNextQueueItem();
        }

        /// <summary>
        /// Queue the window based on key.
        /// Callback will be invoked when the window is popped from the stack.
        /// </summary>
        public void QueuePop(WindowKeys key, PoppedDelegate callback = null)
        {
            var windowToPop = GetWindow(key);

            var pop = new WindowStackPop
            {
                key = windowToPop.Key,
                callback = callback
            };

            _queue.Enqueue(pop);

            if (CanExecuteNextQueueItem())
                ExecuteNextQueueItem();
        }

        /// <summary>
        /// Queue the window.
        /// Callback will be invoked when the window is popped from the stack.
        /// </summary>
        public void QueuePop(Window window, PoppedDelegate callback = null)
        {
            var windowToPop = window;

            var pop = new WindowStackPop
            {
                key = windowToPop.Key,
                callback = callback
            };

            _queue.Enqueue(pop);

            if (CanExecuteNextQueueItem())
                ExecuteNextQueueItem();
        }

        public void OnUpdate()
        {
            if (CanExecuteNextQueueItem())
                ExecuteNextQueueItem();
        }

        public Window GetTopWindow()
        {
            if (_stack.Count <= 0)
                return null;

            for (int i = 0; i < _stack.Count; i++)
            {
                var topWindow = _stack[i];
                if (!topWindow.PushedAsPopUp)
                    return topWindow;
            }

            return _stack[_stack.Count - 1];
        }

        public Window GetTopWindowInStack()
        {
            return _stack.Count <= 0 ? null : _stack[0];
        }

        public Window GetTopPopUp()
        {
            var topWindoInStack = GetTopWindowInStack();
            if (topWindoInStack.PushedAsPopUp)
                return topWindoInStack;

            return null;
        }
        

        public Window GetWindow(WindowKeys key)
        {
            int count = _stack.Count;
            for (int i = 0; i < count; i++)
            {
                if (_stack[i].Key == key)
                    return _stack[i];
            }

            return null;
        }

        public T GetWindow<T>(WindowKeys key) where T : Window
        {
            var window = GetWindow(key);
            return (T)window;
        }

        public void SetVisibility(bool visible)
        {
            var canvasGroup = _rootCanvas.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = _rootCanvas.gameObject.AddComponent<CanvasGroup>();
            }

            canvasGroup.alpha = visible ? 1.0f : 0.0f;
            canvasGroup.interactable = visible;
            canvasGroup.blocksRaycasts = visible;
        }

        public bool IsVisible()
        {
            var canvasGroup = _rootCanvas.GetComponent<CanvasGroup>();

            if (canvasGroup == null)
            {
                return true;
            }

            bool isVisible = canvasGroup.alpha > 0.0f &&
                             canvasGroup.interactable == true &&
                             canvasGroup.blocksRaycasts == true;

            return isVisible;
        }

        private bool CanExecuteNextQueueItem()
        {
            if (_state == State.Ready)
            {
                if (_queue.Count > 0)
                {
                    return true;
                }
            }

            return false;
        }

        private void ExecuteNextQueueItem()
        {
            // Get next queued item.
            var queuedWindow = _queue.Dequeue();

            if (queuedWindow is WindowStackPush queuedPush)
            {
                ShowWindow(queuedPush);
            }
            else
            {
                // Pop window.
                var queuedPop = (WindowStackPop)queuedWindow;
                var windowToPop = GetTopWindowInStack();

                if (windowToPop.Key != queuedWindow.key)
                {
                    RemoveWindow(queuedWindow);
                    SetState(State.Ready);
                    return;
                }

                windowToPop.OnFocusLost();

                SetState(State.Pop);
                _stack.RemoveAt(0);

                // Tell new top wind that it is getting focus.
                var newTopWindow = GetTopWindowInStack();
                if (newTopWindow != null)
                {
                    if (_queue.Count == 0)
                    {
                        // Window get focus when it is on top of the stack and no other items in the queue.
                        newTopWindow.OnFocus();
                    }
                }

                _activePopCallback = queuedPop.callback;
                windowToPop.OnWindowClose?.Invoke();
                windowToPop.OnPopFinished += HandlePopFinished;
                windowToPop.OnPop();
            }
        }

        private async Task ShowWindow(WindowStackPush windowData)
        {
            // Push window.
            var windowKeys = windowData.key;
            if (_cache.TryGetValue(windowKeys, out var windowInstance))
            {
                // Use cached instance of the window.
                _cache.Remove(windowKeys);
                windowInstance.Setup(windowData.key, windowData.context);

                // Move cached to the front of the transform hierarchy to sort properly.
                windowInstance.transform.SetAsLastSibling();
                windowInstance.gameObject.SetActive(true);
                SetupWindow(windowData,windowInstance);
                return;
            }
            
            var window = await CreateWindow(windowData,_rootCanvas.transform);
            SetupWindow(windowData, window);
        }

        private async Task<Window> CreateWindow(WindowStackPush windowData, Transform parent)
        {

            var prefab = windowData.customWindowPrefab;
            if (prefab == null)
            {
                prefab = GetWindowFromStorage(windowData.key);
                if (prefab == null)
                {
                    prefab = await GetWindowFromAdressables(windowData.key);
                }
            }
          
            if (prefab == null)
            {
                Debug.Log($"Window is absent:{windowData.key}");
                return null;
            }

            var window = Instantiate(prefab, parent);
            // Resetting rect transform settings of the window prefab
            ResetRectTransform(window);
            //_container.Inject(window);
           
            
            
            return window;
        }

        private async Task<Window> GetWindowFromAdressables(WindowKeys key)
        {
            var go = await _assetSystem.Load<GameObject>(key.ToString());
            return go != null ? go.GetComponent<Window>() : null;
        }

        private Window GetWindowFromStorage(WindowKeys key)
        {
            return _windowStorage == null ? null : _windowStorage.GetWindowByKey(key);
        }

        private void SetupWindow(WindowStackPush queuedPush, Window window)
        {
            window.Setup(queuedPush.key,queuedPush.context);
            UpdateSortOrderOverrides(customSorting: queuedPush.customSorting);

            // Telling previous top window that it's losing focus.
            var topWindow = GetTopWindowInStack();
            if (topWindow != null)
            {
                topWindow.OnFocusLost();
            }

            // Insert new window at the top of the stack.
            SetState(State.Push);
            _stack.Insert(0, window);

            _activePushCallback = queuedPush.callback;

            window.OnPushFinished += HandlePushFinished;
            window.OnPush();

            if (_queue.Count == 0)
            {
                // Window get focus when it is on top of the stack and no other items in the queue.
                window.OnFocus();
            }
        }

      


        private void RemoveWindow(WindowStack queuedWindow)
        {
            SetState(State.Pop);

            var windowToPop = GetWindow(queuedWindow.key);

            if (windowToPop == null)
                return;

            _stack.Remove(windowToPop);
            windowToPop.OnPopFinished += HandlePopFinished;
            windowToPop.OnPop();
        }

        private void HandlePushFinished(Window window)
        {
            window.OnPushFinished -= HandlePushFinished;

            SetState(State.Ready);

            if (_activePushCallback != null)
            {
                _activePushCallback(window);
                _activePushCallback = null;
            }

            if (CanExecuteNextQueueItem())
                ExecuteNextQueueItem();
        }

        private void HandlePopFinished(Window window)
        {
            window.OnPopFinished -= HandlePopFinished;

            UpdateSortOrderOverrides(true, window);

            if (window.KeepCached)
            {
                // Store in the cache for later use.
                window.gameObject.SetActive(false);
                var windowKey = window.Key;

                if (!_cache.ContainsKey(windowKey))
                {
                    _cache.Add(windowKey, window);
                }
            }
            else
            {
                // Destroy window.
                Destroy(window.gameObject);
            }

            SetState(State.Ready);

            if (_activePopCallback != null)
            {
                _activePopCallback(window.Key);
                _activePopCallback = null;
            }

            if (CanExecuteNextQueueItem())
                ExecuteNextQueueItem();
        }

        // Fixed issue with sorting
        private void UpdateSortOrderOverrides(bool isOnPopSorting = false, Window windowToPop = null, int customSorting = -1)
        {
            int managedOrder = WindowSortingStartingInt;
            var extraLayerForTopPanels = 3;
            var topWindowCanvasOrder = managedOrder;
            var topPopUpCanvasOrder = managedOrder;


            int childCount = _rootCanvas.transform.childCount;

            for (int i = 0; i < childCount; i++)
            {
                var window = _rootCanvas.transform.GetChild(i).GetComponent<Window>();
                if (window != null)
                {
                    var canvas = window.GetComponent<Canvas>();
                    if (canvas != null)
                    {
                        if (!isOnPopSorting) // Sorting only on push
                        {
                            canvas.overrideSorting = true;
                            canvas.sortingOrder = managedOrder;
                        }

                        var ignoreWindow =
                            windowToPop != null &&
                            isOnPopSorting &&
                            window.Key == windowToPop.Key; // Ignoring window if it's in process of popping

                        if (!ignoreWindow)
                        {
                            if (window.PushedAsPopUp)
                                topPopUpCanvasOrder = managedOrder;
                            else topWindowCanvasOrder = managedOrder;
                        }

                        managedOrder += extraLayerForTopPanels;
                    }
                }
            }
        }
       
       
       
        private void ResetRectTransform(Window window)
        {
            var panelRectTransform = window.GetComponent<RectTransform>();
            panelRectTransform.anchorMin = _zeroVector2;
            panelRectTransform.anchorMax = _oneVector2;
            panelRectTransform.pivot = _halfVector2;
            panelRectTransform.position = _zeroVector3;
            panelRectTransform.offsetMax = _zeroVector2;
            panelRectTransform.offsetMin = _zeroVector2;
            panelRectTransform.localScale = _oneVector3;
            panelRectTransform.localPosition = _zeroVector3;
            panelRectTransform.ForceUpdateRectTransforms();
        }

        
        private void SetState(State state)
        {
            var oldState = _state;
            _state = state;
            if (_state != oldState)
                OnStateChanged(oldState, state);
        }

        private void OnStateChanged(State oldState, State newState)
        {
            if (newState == State.Ready && _queue.Count > 0)
                ExecuteNextQueueItem();
        }
    }
}