using UnityEngine;

namespace TF.Runtime
{
    public class Main : MonoSingleton<Main>
    {
        public int projectId = 0;

        void Start()
        {
            Debug.Log("项目启动...");
            DontDestroyOnLoad(this);

            ScoreManager.Instance.StartUp();
            ConfigManager.Instance.StartUp();
            TaskManager.Instance.StartUp();
            UIManager.Instance.StartUp();
            AudioManager.Instance.StartUp();
            SceneManager.Instance.StartUp();
        }

    }

}