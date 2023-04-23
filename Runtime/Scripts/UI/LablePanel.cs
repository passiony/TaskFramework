using TMPro;

namespace TF.Runtime
{
    public class LablePanel : BaseTalkPanel
    {
        private TextMeshProUGUI lableTxt;

        private int fontSize = 50;

        protected override void InitWidget()
        {
            lableTxt = transform.Find("LableTxt").GetComponent<TextMeshProUGUI>();
        }

        private void OnEnable()
        {
            EventManager.AddHandler<string>(EventID.FinishTask, OnFinishTask);
        }

        private void OnDisable()
        {
            EventManager.RemoveHandler<string>(EventID.FinishTask, OnFinishTask);
        }

        private void OnFinishTask(string taskId)
        {
            UIManager.Instance.CloseWindow(this);
        }

        public override BaseWindow SetArgs(params string[] args)
        {
            fontSize = args.TryGetIntValue(0);
            return this;
        }

        protected override void UpdateView()
        {
            this.Speak(taskData.id, content_str, 0, () =>
            {
                EventManager.FireEvent(EventID.FinishTalkGoal);
            });
            
            lableTxt.text = content_str;
            if (fontSize > 1)
            {
                lableTxt.fontSize = fontSize;
            }
        }
    }
}