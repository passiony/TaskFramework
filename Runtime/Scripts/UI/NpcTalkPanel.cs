using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

namespace TF.Runtime
{
    public class NpcTalkPanel : BaseTalkPanel
    {
        private RectTransform root;
        private RectTransform bg;
        private TextMeshProUGUI nameTxt;
        private TextMeshProUGUI contentTxt;
        private Button[] buttons;
        private TextMeshProUGUI[] buttonTxts;

        protected override void InitWidget()
        {
            this.mResident = true;
            root = this.transform as RectTransform;
            bg = this.transform.Find("bg") as RectTransform;
            nameTxt = this.transform.Find("bg/name").GetComponent<TextMeshProUGUI>();
            contentTxt = this.transform.Find("bg/content/Text").GetComponent<TextMeshProUGUI>();

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
            if (options.IsNullOrEmpty())
            {
                HideAllButtons();
            }

            var npcCfg = ConfigManager.Instance.GetTable<db_NpcConfig>((f) => f.name == npcId);
            this.nameTxt.text = npcCfg.name + ":";
            this.contentTxt.text = this.content_str;

            LayoutRebuilder.ForceRebuildLayoutImmediate(bg);
            LayoutRebuilder.ForceRebuildLayoutImmediate(root);

            this.Speak(taskData.id, content_str, npcCfg.sex, TalkCompelete);
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
                SpeakerManager.PlaySelectSuccess(() => { EventManager.FireEvent(EventID.MakeChoice, index); });
            }
            else
            {
                SpeakerManager.PlaySelectError();
            }
        }

        void HideAllButtons()
        {
            foreach (var btn in buttons)
            {
                btn.gameObject.SetActive(false);
            }
        }
    }
}