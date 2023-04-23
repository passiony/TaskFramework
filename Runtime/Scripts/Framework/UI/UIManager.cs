using System;
using System.Collections.Generic;
using UnityEngine;

namespace TF.Runtime
{
    public enum WindowType
    {
        SYSTEM,
        WINDOW,
        DIALOG,
        FLYWORD,
        MSGTIP,
        Top,
    }

    public class UIManager : Singleton<UIManager>
    {
        public Dictionary<string, BaseWindow> mOpenWindows = new Dictionary<string, BaseWindow>();
        public Dictionary<WindowType, int> mMinDepths = new Dictionary<WindowType, int>();
        public List<BaseWindow> mOpenWinStack = new List<BaseWindow>();
        public Dictionary<int, List<BaseWindow>> typeWindows = new Dictionary<int, List<BaseWindow>>();

        public const string UIPath = "Prefabs/UI/";
        public Transform UIRoot;
        private Vector3 UISCALE = new Vector3(0.001f, 0.001f, 0.001f);

        public override void Init()
        {
            mMinDepths[WindowType.SYSTEM] = 1000;
            mMinDepths[WindowType.WINDOW] = 2000;
            mMinDepths[WindowType.DIALOG] = 3000;
            mMinDepths[WindowType.FLYWORD] = 4000;
            mMinDepths[WindowType.MSGTIP] = 5000;
            mMinDepths[WindowType.Top] = 6000;

            UIRoot = GameObject.Find("Main/UIRoot").transform;
        }

        public T OpenWindow<T>() where T : BaseWindow
        {
            var windowName = typeof(T).Name;

            var window = GetWindow<T>();
            if (window == null)
            {
                window = Load<T>(windowName);
                window.Init();
                mOpenWindows.Add(windowName, window);
                AddTypeWindow(window.winType, window);
            }

            window.RefreshVRCanvas();
            Transform trans = window.transform;
            if (trans == null)
                return null;
            if (window.winType == WindowType.WINDOW)
            {
                CloseAllWindowByType(WindowType.DIALOG);
            }

            window.Active();
            return window;
        }

        public BaseWindow OpenWindow(Type type)
        {
            var windowName = type.Name;

            var window = GetWindow(type);
            if (window == null)
            {
                window = Load(windowName);
                window.Init();
                mOpenWindows.Add(windowName, window);
                AddTypeWindow(window.winType, window);
            }

            window.RefreshVRCanvas();
            Transform trans = window.transform;
            if (trans == null)
                return null;
            if (window.winType == WindowType.WINDOW)
            {
                CloseAllWindowByType(WindowType.DIALOG);
            }

            window.Active();
            return window;
        }

        private T Load<T>(string windowName)
        {
            var mResPath = UIPath + windowName;
            GameObject prefab = Resources.Load<GameObject>(mResPath);
            if (prefab == null)
            {
                Debug.LogError(string.Format("加载Window资源失败:{0}", mResPath));
                return default;
            }

            GameObject go = GameObject.Instantiate(prefab);
            go.transform.SetParent(UIRoot, false);
            go.transform.localScale = UISCALE;
            go.SetActive(false);
            return go.GetComponent<T>();
        }

        private BaseWindow Load(string windowName)
        {
            var mResPath = UIPath + windowName;
            GameObject prefab = Resources.Load<GameObject>(mResPath);
            if (prefab == null)
            {
                Debug.LogError(string.Format("加载Window资源失败:{0}", mResPath));
                return default;
            }

            GameObject go = GameObject.Instantiate(prefab);
            go.transform.SetParent(UIRoot, false);
            go.transform.localScale = UISCALE;
            go.SetActive(false);
            return go.GetComponent<BaseWindow>();
        }

        public void CloseWindow<T>()
        {
            var windowName = typeof(T).Name;
            BaseWindow window = null;
            mOpenWindows.TryGetValue(windowName, out window);
            if (window != null)
            {
                window.Close();
                if (!window.IsResident())
                {
                    mOpenWindows.Remove(windowName);
                    RemoveTypeWindow(window.winType, window);
                }
            }

            if (mOpenWindows.ContainsKey(windowName))
            {
                mOpenWindows.Remove(windowName);
            }
        }

        public void CloseWindow(BaseWindow window)
        {
            var windowName = window.GetType().Name;
            window.Close();
            if (!window.IsResident())
            {
                mOpenWindows.Remove(windowName);
                RemoveTypeWindow(window.winType, window);
            }
        }

        public T HideWindow<T>() where T : BaseWindow
        {
            var windowName = typeof(T).Name;
            BaseWindow window = null;
            mOpenWindows.TryGetValue(windowName, out window);
            if (window != null)
            {
                window.gameObject.SetActive(false);
            }

            return window as T;
        }

        public T ShowWindow<T>() where T : BaseWindow
        {
            var windowName = typeof(T).Name;
            BaseWindow window = null;
            mOpenWindows.TryGetValue(windowName, out window);
            if (window != null)
            {
                window.gameObject.SetActive(true);
            }

            return window as T;
        }

        public void CloseAllWindowByType(WindowType type)
        {
            List<BaseWindow> windows;
            if (!typeWindows.TryGetValue((int)type, out windows))
            {
                windows = new List<BaseWindow>();
            }

            for (int i = 0; i < windows.Count; i++)
            {
                CloseWindow(windows[i]);
            }
        }

        public void CloseAllWindow()
        {
            foreach (var pair in mOpenWindows)
            {
                var windoName = pair.Key;
                var window = pair.Value;
                if (window)
                    window.Close();
            }

            mOpenWindows.Clear();
            typeWindows.Clear();
        }

        public T GetWindow<T>() where T : BaseWindow
        {
            var windowName = typeof(T).Name;
            BaseWindow window = null;
            mOpenWindows.TryGetValue(windowName, out window);
            return window as T;
        }

        public BaseWindow GetWindow(Type type)
        {
            var windowName = type.Name;
            BaseWindow window = null;
            mOpenWindows.TryGetValue(windowName, out window);
            return window;
        }

        void AddTypeWindow(WindowType type, BaseWindow window)
        {
            List<BaseWindow> windows;
            if (!typeWindows.TryGetValue((int)type, out windows))
            {
                windows = new List<BaseWindow>();
                typeWindows.Add((int)type, windows);
            }

            windows.Add(window);
        }

        void RemoveTypeWindow(WindowType type, BaseWindow window)
        {
            List<BaseWindow> windows;
            if (!typeWindows.TryGetValue((int)type, out windows))
            {
                windows = new List<BaseWindow>();
                typeWindows.Add((int)type, windows);
            }

            windows.Remove(window);
        }

        public void Clear()
        {
            foreach (var pair in mOpenWindows)
            {
                pair.Value.Close();
            }

            mOpenWindows.Clear();
            mOpenWinStack.Clear();
        }

        public void CloseAll()
        {
            foreach (var pair in mOpenWindows)
            {
                pair.Value.Close();
            }
        }

        public override void Dispose()
        {
            Clear();
        }
    }
}