using TMPro;
using UnityEngine.Events;
using UnityEngine.UI;

namespace TF.Runtime
{
    public class EndPanel : BaseWindow
    {
        private TextMeshProUGUI titleTxt;
        private TextMeshProUGUI contentTxt;
        private TextMeshProUGUI wrongTxt;
        private Button btn1;
        private Button btn2;
        private TextMeshProUGUI btn1Txt;
        private TextMeshProUGUI btn2Txt;

        protected override void InitWidget()
        {
            base.InitWidget();
            titleTxt = transform.Find("title").GetComponent<TextMeshProUGUI>();
            contentTxt = transform.Find("content/Text").GetComponent<TextMeshProUGUI>();
            wrongTxt = transform.Find("wrong/Text").GetComponent<TextMeshProUGUI>();
            btn1 = transform.Find("bottom/btn1").GetComponent<Button>();
            btn2 = transform.Find("bottom/btn2").GetComponent<Button>();
            btn1Txt = btn1.GetComponentInChildren<TextMeshProUGUI>();
            btn2Txt = btn2.GetComponentInChildren<TextMeshProUGUI>();
        }

        public override BaseWindow Open(params object[] args)
        {
            AudioManager.Instance.PlayEffect("1003");

            UpdateView();
            return this;
        }

        protected void UpdateView()
        {
            string content = @"本次任务完成。";
            string wrong = "";
            if (TaskManager.Instance.mode == TaskMode.Practice)
            {
                content += "本次得分为：" + ScoreManager.Instance.GetScore();
                wrong = ScoreManager.Instance.GetWrongTxt();
            }

            this.Show("提示", content, wrong, "退出", "重新开始", () =>
                {
                    SceneManager.Instance.EnterScene("MainScene");
                },
                () =>
                {
                    ScoreManager.Instance.Reset();
                    TaskManager.Instance.Dispose();
                    TaskManager.Instance.ResetTask();
                    SceneManager.Instance.ReEnterScene();
                });
        }

        private void Show(string title, string content, string wrong, string txt1, string txt2, UnityAction btn1Handler,
            UnityAction btn2Handler)
        {
            this.titleTxt.text = title;
            this.contentTxt.text = content;
            this.wrongTxt.text = wrong;
            this.btn1Txt.text = txt1;
            this.btn2Txt.text = txt2;
            btn1.onClick.AddListener(btn1Handler);
            btn2.onClick.AddListener(btn2Handler);

            this.Speak(TaskModel.Instance.cur_task.id, content, 0, null);
        }
    }
}