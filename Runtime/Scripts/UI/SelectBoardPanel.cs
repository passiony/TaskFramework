using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using System.Linq;

namespace TF.Runtime
{
    public class SelectBoardPanel : BaseTalkPanel
    {
        private TextMeshProUGUI titleTxt;
        private TextMeshProUGUI contentTxt;
        private List<Button> menus;

        protected override void InitWidget()
        {
            titleTxt = transform.Find("title").GetComponent<TextMeshProUGUI>();
            contentTxt = transform.Find("content").GetComponent<TextMeshProUGUI>();

            for (int i = 0; i < 4; i++)
            {
                var path = "bottom/menu" + i;
                var btn = transform.Find(path).GetComponent<Button>();
                var index = i;
                btn.onClick.AddListener(() => { this.OnButtonClick(index); });
            }
        }

        protected override void UpdateView()
        {
            this.titleTxt.text = title_str;
            this.contentTxt.text = content_str;

            this.Speak(taskData.id, content_str, 0, null);
        }

        private void OnButtonClick(int index)
        {
            AudioManager.Instance.PlayEffect("1002");

            if (!corrects.IsNullOrEmpty())
            {
                if (corrects.Contains(index))
                {
                    UIManager.Instance.CloseWindow(this);
                    SpeakerManager.PlaySelectSuccess(() => { EventManager.FireEvent(EventID.FinishTalkGoal); });
                }
                else
                {
                    SpeakerManager.PlaySelectError();
                }
            }
        }
    }
}