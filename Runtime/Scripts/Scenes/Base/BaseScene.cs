
using UnityEngine;

namespace TF.Runtime
{
    public abstract class BaseScene
    {
        public int TaskId;

        public virtual void OnEnter()
        {
            ObjectManager.Instance.ResetSceneData();

            var projectId = Main.Instance.projectId;
            ConfigManager.Instance.LoadTaskCfg(projectId, TaskId);
            TaskManager.Instance.AutoInit();

            UIManager.Instance.UIRoot = GameObject.Find("Main/UIRoot").transform;
        }

        public virtual void OnLeave()
        {
            UIManager.Instance.CloseAllWindow();
        }

        public virtual void LoadTutorial(string title, string content)
        {
            var point = ObjectManager.Instance.GetPoint("p1");
            var tutorial = UIManager.Instance.OpenWindow<TutorialPanel>().AddToPoint(point);
            tutorial.Show(title, content, "重新开始", "退出场景",
                () => { SceneManager.Instance.ReEnterScene(); },
                () => { SceneManager.Instance.EnterScene("MainScene"); });
        }
    }
}