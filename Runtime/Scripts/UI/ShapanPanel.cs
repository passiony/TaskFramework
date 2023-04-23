
using UnityEngine;

namespace TF.Runtime
{
    /// <summary>
    /// 沙盘显示UI
    /// </summary>
    public class ShapanPanel : BaseWindow
    {
        private ShapanItem[] itmes;
        private MenuPanel window;


        protected override void InitWidget()
        {
            base.InitWidget();
            itmes = transform.GetComponentsInChildren<ShapanItem>();
            for (int i = 0; i < itmes.Length; i++)
            {
                itmes[i].OnClick = (x) => { };
            }

            var point = ObjectManager.Instance.GetPoint("p3");
            window = UIManager.Instance.OpenWindow<MenuPanel>()
                .AddToPoint(point)
                .SetOptions("一级漏保", "二级漏保", "三级漏保") as MenuPanel;

            window.Show("供电所综合业务VR实训系统")
                .OnCallback(index => { LoadScene(index); });
        }

        void LoadScene(int index)
        {
            if (index < 2)
            {
                SpeakerManager.Instance.Speak("暂未开放", 0, null);
                return;
            }

            Main.Instance.projectId = index + 1;
            Debug.Log($"加载第{Main.Instance.projectId}个故障模拟");

            SceneManager.Instance.EnterScene("OfficeScene");
        }

        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.B))
            {
                LoadScene(2);
            }
        }
    }
}