using TMPro;
using UnityEngine.Events;
using UnityEngine.UI;

namespace TF.Runtime
{
    public class FinishPanel : BaseTalkPanel
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
            titleTxt = transform.Find("root/title").GetComponent<TextMeshProUGUI>();
            contentTxt = transform.Find("root/content/Text").GetComponent<TextMeshProUGUI>();
            wrongTxt = transform.Find("root/wrong/Text").GetComponent<TextMeshProUGUI>();
            btn1 = transform.Find("root/bottom/btn1").GetComponent<Button>();
            btn2 = transform.Find("root/bottom/btn2").GetComponent<Button>();
            btn1Txt = btn1.GetComponentInChildren<TextMeshProUGUI>();
            btn2Txt = btn2.GetComponentInChildren<TextMeshProUGUI>();
        }

        protected override void UpdateView()
        {
            string content = content_str;
            string wrong = "";
            if (TaskManager.Instance.mode == TaskMode.Practice)
            {
                content += "本次得分为：" + ScoreManager.Instance.GetScore();
                wrong = ScoreManager.Instance.GetWrongTxt();
            }

            this.Show(title_str, content, wrong, "退出", "重新开始", () => { SceneManager.Instance.EnterScene("MainScene"); }, () =>
            {
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

            this.Speak(taskData.id, content, 0, null);
        }
    }
}