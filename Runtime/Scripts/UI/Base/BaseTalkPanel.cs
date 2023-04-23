using System;
using UnityEngine;

namespace TF.Runtime
{
    public class BaseTalkPanel : BaseWindow
    {
        // protected string[] multTab;
        // protected int multIndex;
        protected string[] talks;
        protected string title_str;
        protected string content_str;
        protected string npcId;

        protected string[] options;
        protected int[] corrects; //正确选项[0-n]
        protected TaskData taskData;

        public override BaseWindow SetOptions(params string[] args)
        {
            options = args;
            return this;
        }

        public override BaseWindow SetCorrect(params string[] args)
        {
            corrects = args.ToIntArr();
            return this;
        }

        public override BaseWindow Open(params object[] args)
        {
            taskData = args[0] as TaskData;
            Show(taskData.config.talk);
            AudioManager.Instance.PlayEffect("1003");

            return this;
        }

        protected virtual void Show(string[] talks)
        {
            this.talks = talks;
            // this.multTab = talks;
            // this.multIndex = -1;
            this.npcId = talks[0];
            this.title_str = talks[1];
            this.content_str = talks[2].Replace("\\n", "\n");

            UpdateView();
        }

        protected virtual void UpdateView()
        {
        }

        protected virtual void TalkCompelete()
        {
            //自动关闭
            if (options.IsNullOrEmpty())
            {
                FinishTalk();
            }
        }

        protected virtual void FinishTalk()
        {
            // UIManager.Instance.CloseWindow(this);
            EventManager.FireEvent(EventID.FinishTalkGoal);
        }

        protected override void OnHide()
        {
            options = null;
            corrects = null;
        }
    }
}