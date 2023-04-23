using System;
using System.Linq;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace TF.Runtime
{
    /// <summary>
    /// 提示框（提示+内容+多个按钮选择）
    /// </summary>
    public class AlertPanel : BaseTalkPanel
    {
        private RectTransform root;
        private RectTransform bg;

        private TextMeshProUGUI titleTxt;
        private TextMeshProUGUI content;
        private Button[] buttons;
        private TextMeshProUGUI[] buttonTxts;

        protected override void InitWidget()
        {
            this.mResident = true;
            root = this.transform as RectTransform;
            bg = this.transform.Find("bg") as RectTransform;
            titleTxt = transform.Find("bg/title").GetComponent<TextMeshProUGUI>();
            content = transform.Find("bg/content/Text").GetComponent<TextMeshProUGUI>();

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

        void RebuildLayout()
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(bg);
            LayoutRebuilder.ForceRebuildLayoutImmediate(root);
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

            this.titleTxt.text = title_str;
            this.content.text = this.content_str;
            RebuildLayout();

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