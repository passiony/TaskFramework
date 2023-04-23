using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace TF.Runtime
{
    /// <summary>
    /// 提示框（提示+内容+多个按钮选择）
    /// </summary>
    public class ExamPanel : BaseTalkPanel
    {
        private readonly string[] optionBtns = { "A", "B", "C", "D" }; //选项s

        private RectTransform root;
        private RectTransform bg;

        private TextMeshProUGUI titleTxt;
        private TextMeshProUGUI content;
        private Button[] buttons;
        private TextMeshProUGUI[] optionTxts;
        private TextMeshProUGUI[] buttonTxts;

        private int questionId;
        private db_ExamConfig config;
        private bool finish = false;
        
        protected override void InitWidget()
        {
            this.mResident = true;
            root = this.transform as RectTransform;
            bg = this.transform.Find("bg") as RectTransform;
            titleTxt = transform.Find("bg/title").GetComponent<TextMeshProUGUI>();
            content = transform.Find("bg/content/Text").GetComponent<TextMeshProUGUI>();

            optionTxts = new TextMeshProUGUI[4];
            for (int i = 0; i < 4; i++)
            {
                var index = i + 1;
                optionTxts[i] = transform.Find($"bg/content/Option{index}").GetComponent<TextMeshProUGUI>();
            }

            buttons = new Button[4];
            buttonTxts = new TextMeshProUGUI[4];
            for (int i = 0; i < 4; i++)
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

        protected override void Show(string[] talks)
        {
            this.talks = talks;
            this.title_str = talks[1];
            this.finish = false;
            
            var ids = talks[2].SplitToInt('-');
            var id = Main.Instance.projectId * 1000 + Random.Range(ids[0], ids[1] + 1);
            this.questionId = id;
            this.config = ConfigManager.Instance.GetTable<db_ExamConfig>(id.ToString());
            this.content_str = config.content;
        
            if (string.IsNullOrEmpty(config.optionD))
            {
                this.options = new[] { config.optionA, config.optionB, config.optionC };
            }
            else
            {
                this.options = new[] { config.optionA, config.optionB, config.optionC, config.optionD };
            }

            corrects = new[] { this.config.answer - 1 };

            UpdateView();
        }

    
        protected override void UpdateView()
        {
            this.gameObject.SetActive(true);

            for (int i = 0; i < 4; i++)
            {
                optionTxts[i].gameObject.SetActive(false);
                buttons[i].gameObject.SetActive(false);
            }

            for (int i = 0; i < options.Length; i++)
            {
                buttons[i].gameObject.SetActive(true);
                optionTxts[i].gameObject.SetActive(true);
                optionTxts[i].text = options[i];
                buttonTxts[i].text = optionBtns[i];
            }

            this.titleTxt.text = title_str;
            this.content.text = content_str;
            RebuildLayout();

            SpeakerManager.Instance.Speak(content_str, 0, TalkCompelete);
        }

        private void OnConfirmClick(int index)
        {
            if (finish) return;

            finish = true;
            if (corrects.IsNullOrEmpty() || corrects.Contains(index))
            {
                SpeakerManager.PlaySelectSuccess(() => { FinishTalk(); });
            }
            else
            {
                ScoreManager.Instance.AddScore(-2);
                ScoreManager.Instance.AddWrongQuestion(questionId);

                SpeakerManager.PlaySelectError(optionBtns[corrects[0]], () =>
                {
                    FinishTalk();
                });
            }
        }

    }
}