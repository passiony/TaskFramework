using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TF.Runtime
{
    public class SceneManager : MonoSingleton<SceneManager>
    {
        private Dictionary<string, BaseScene> scenes = new Dictionary<string, BaseScene>();
        private string sceneName;
        private BaseScene curScene;
        public event Action<string> OnSceneChange;

        void Awake()
        {
            var assembly = typeof(BaseScene).Assembly; //获取当前父类所在的程序集``
            var types = assembly.GetTypes(); //获取该程序集中的所有类型
            foreach (var type in types)
            {
                var menu = type.GetCustomAttribute<SceneAttribute>();
                if (menu == null)
                {
                    continue;
                }

                var scene = Activator.CreateInstance(type) as BaseScene;
                scene.TaskId = menu.taskId;
                scenes.Add(type.Name, scene);
            }
        }

        void OnEnable()
        {
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
            UnityEngine.SceneManagement.SceneManager.sceneUnloaded += OnSceneUnLoaded;
        }

        void OnDisable()
        {
            UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
            UnityEngine.SceneManagement.SceneManager.sceneUnloaded -= OnSceneUnLoaded;
        }

        private void OnSceneUnLoaded(Scene scene)
        {
            if (scenes.TryGetValue(scene.name, out BaseScene sc))
            {
                sc.OnLeave();
            }
            else
            {
                Debug.LogError("找不到Scene类:" + scene.name);
            }
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            sceneName = scene.name;
            if (scenes.TryGetValue(sceneName, out BaseScene sc))
            {
                sc.OnEnter();
            }
            else
            {
                Debug.LogError("找不到Scene类:" + sceneName);
            }
        }

        public void EnterScene(string sceneName)
        {
            Debug.Log("进入场景：" + sceneName);

            OnSceneChange?.Invoke(sceneName);
            TaskManager.Instance.Dispose();
            UIManager.Instance.CloseAll();

#if VRTK_DEFINE_SDK_STEAMVR
            SteamVR_LoadLevel.Begin(sceneName);
#else
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
#endif
            EventManager.FireEvent(EventID.SwitchToScene, sceneName);
        }

        public void ReEnterScene()
        {
            EnterScene(sceneName);
        }

        public override void Dispose()
        {
        }
    }
}