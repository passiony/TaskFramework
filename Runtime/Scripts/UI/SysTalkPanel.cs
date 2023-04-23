using System.Linq;
using TMPro;
using UnityEngine.UI;

namespace TF.Runtime
{
    public class SysTalkPanel : BaseTalkPanel
    {
        private TextMeshProUGUI title;
        private TextMeshProUGUI content;
        private Button[] buttons;
        private TextMeshProUGUI[] buttonTxts;

        protected override void InitWidget()
        {
            this.mResident = true;
            title = this.transform.Find("bg/title").GetComponent<TextMeshProUGUI>();
            content = this.transform.Find("bg/content").GetComponent<TextMeshProUGUI>();

            buttons = new Button[3];
            buttonTxts = new TextMeshProUGUI[3];
            for (int i = 0; i < 3; i++)
            {
                var index = i;
                buttons[i] = transform.Find($"bg/bottom/btn{index + 1}").GetComponent<Button>();
                buttonTxts[i] = transform.Find($"bg/bottom/btn{index + 1}/Text").GetComponent<TextMeshProUGUI>();

                buttons[i].onClick.AddListener(() => { this.OnConfirmClick(index); });
            }
        }

        public override BaseWindow SetOptions(params string[] args)
        {
            base.SetOptions(args);
            HideAllButtons();
            for (int i = 0; i < args.Length; i++)
            {
                var option = args[i];
                buttons[i].gameObject.SetActive(true);
                buttonTxts[i].text = option;
            }

            return this;
        }

        protected override void UpdateView()
        {
            this.gameObject.SetActive(true);
            if (options.IsNullOrEmpty())
            {
                HideAllButtons();
            }

            var npcCfg = ConfigManager.Instance.GetTable<db_NpcConfig>((f) => f.name == npcId);
            this.title.text = npcCfg.name + ":";
            this.content.text = this.content_str;

            this.Speak(taskData.id, content_str, 0, TalkCompelete);
        }

        private void HideAllButtons()
        {
            foreach (var btn in buttons)
            {
                btn.gameObject.SetActive(false);
            }
        }

        private void OnConfirmClick(int index)
        {
            AudioManager.Instance.PlayEffect("1002");
            if (corrects.IsNullOrEmpty())
            {
                UIManager.Instance.CloseWindow(this);
                EventManager.FireEvent(EventID.MakeChoice, index);
                return;
            }
            
            if (corrects.Contains(index))
            {
                UIManager.Instance.CloseWindow(this);
                SpeakerManager.PlaySelectSuccess(() => { EventManager.FireEvent(EventID.MakeChoice, index);});
            }
            else
            {
                SpeakerManager.PlaySelectError();
            }
        }

    }
}