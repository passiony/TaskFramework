using System;
using UniRx;
using UnityEngine;

namespace TF.Runtime
{
    public class Npc : Role
    {
        public db_NpcConfig config { get; private set; }

        protected override void Start()
        {
            config = ConfigManager.Instance.GetTable<db_NpcConfig>((f) => f.name == id);
            UIRoot = transform.Find("UIRoot");
        }

        protected override void AddCommands()
        {
            base.AddCommands();
            this.Receiver.AddCommand<TalkCommand>(ECommand.Talk, CheckShowTalk);
        }

        private ECommandReply CheckShowTalk(TalkCommand cmd, bool undo)
        {
            if (undo)
            {
                return ECommandReply.N;
            }

            ShowTalk(cmd.Task, cmd.Goal);
            return ECommandReply.Y;
        }

        public override void PlayAnim(string animName)
        {
            Debug.Log("PlayAnim:" + animName);
            Animator.CrossFade(animName, 0.2f);
        }

        private void ShowTalk(TaskData task, TaskGoal goal)
        {
            var data = goal.data as panel_goal_data;
            //练习模式才有答题
            if (data.uiName == "Exam")
            {
                if (TaskManager.Instance.mode != TaskMode.Practice)
                {
                    EventManager.FireEvent(EventID.FinishTalkGoal);
                    return;
                }
            }

            var uiname = "TF.Runtime." + data.uiName + "Panel";
            var uiType = Type.GetType(uiname);
            if (uiType == null)
            {
                Debug.LogError("找不到UI：" + data.uiName);
            }

            Transform point = null;
            if (string.IsNullOrEmpty(data.point))
            {
                var npc = ObjectManager.Instance.GetNPC(task.config.talk[0]);
                point = npc.UIRoot;
            }
            else
            {
                point = ObjectManager.Instance.GetPoint(data.point);
            }

            var window = UIManager.Instance.OpenWindow(uiType).AddToPoint(point);
            if (window.winType == WindowType.Top)
            {
                window.AddToPoint(UIManager.Instance.UIRoot);
                window.transform.localScale = Vector3.one;
            }

            foreach (var cmd_data in data.cmds)
            {
                var cmd = cmd_data.name.ToEnum<ECommandUI>();
                switch (cmd)
                {
                    case ECommandUI.SetOptions:
                        var args = task.FilterItem(goal.type, cmd_data.args);
                        window.SetOptions(args);
                        break;
                    case ECommandUI.SetCorrect:
                        window.SetCorrect(cmd_data.args);
                        break;
                    case ECommandUI.SetArgs:
                        window.SetArgs(cmd_data.args);
                        break;
                    case ECommandUI.SetLife:
                        window.SetLife(cmd_data.args);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            window.Open(task);
        }
    }
}