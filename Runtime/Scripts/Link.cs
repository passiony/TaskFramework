using System;
using UniRx;
using UnityEngine;

namespace TF.Runtime
{
    public class Link : Singleton<Link>
    {
        private ICommandReceiver _receiver;

        protected ICommandReceiver Receiver
        {
            get
            {
                if (_receiver == null)
                {
                    _receiver = new ICommandReceiver();
                }

                return _receiver;
            }
        }

        private FlyWord _flyWord;

        public ECommandReply Command<T>(T cmd, bool undo = false) where T : ICommand
        {
            return Receiver.Command(cmd, undo);
        }

        public override void Init()
        {
            AddCommands();
        }

        private void AddCommands()
        {
            this.Receiver.AddCommand<FlyWordCommand>(ECommand.ShowFlyWord, CheckShowText);
            this.Receiver.AddCommand<TaskJumpCommand>(ECommand.TaskJump, CheckTaskJump);
            this.Receiver.AddCommand<CreateObjCommand>(ECommand.CreateObject, CheckCreateObject);
            this.Receiver.AddCommand<LoadSceneCommand>(ECommand.LoadScene, CheckLoadScene);
            this.Receiver.AddCommand<ProjectIdCommand>(ECommand.ProjectId, CheckSetProjectId);
        }

        private ECommandReply CheckSetProjectId(ProjectIdCommand cmd, bool undo)
        {
            Main.Instance.projectId = cmd.Id;
            return ECommandReply.Y;
        }

        private ECommandReply CheckLoadScene(LoadSceneCommand cmd, bool undo)
        {
            Observable.TimerFrame(1, FrameCountType.EndOfFrame).Subscribe((x) =>
            {
                SceneManager.Instance.EnterScene(cmd.SceneName);
            });
            return ECommandReply.Y;
        }

        public ECommandReply CheckCreateObject(CreateObjCommand cmd, bool undo)
        {
            CreateObject(cmd);
            return ECommandReply.Y;
        }

        private ECommandReply CheckTaskJump(TaskJumpCommand cmd, bool undo)
        {
            if (undo)
            {
                JumpTaskId(cmd.OriginId);
            }
            else
            {
                JumpTaskId(cmd.TargetId);
            }

            return ECommandReply.Y;
        }


        protected virtual ECommandReply CheckShowText(FlyWordCommand cmd, bool undo)
        {
            if (undo)
            {
                if (_flyWord)
                {
                    GameObject.Destroy(_flyWord.gameObject);
                }
            }
            else
            {
                var prefab = Resources.Load<GameObject>(UIManager.UIPath + "ItemText");
                var go = GameObject.Instantiate(prefab);
                go.transform.localPosition = Vector3.zero;
                go.transform.localRotation = Quaternion.identity;
                _flyWord = go.GetComponent<FlyWord>();
                _flyWord.AddToPoint(cmd.pointData).ShowText(cmd.Content, cmd.DelayTime);
            }

            return ECommandReply.Y;
        }

        public static ECommandReply SendCommand(SceneObject obj, string cmdKey, string[] args, bool undo = false)
        {
            if (string.IsNullOrEmpty(cmdKey))
            {
                return ECommandReply.N;
            }

            string cmdName = "TF.Runtime." + cmdKey + "Command";
            Type cmdClass = Type.GetType(cmdName);
            if (cmdClass == null)
            {
                Debug.LogError($"找不到CMD命令类：{cmdName}");
                return ECommandReply.N;
            }

            var ctors = cmdClass.GetConstructors();
            var cmd = (ICommand)ctors[0].Invoke(new object[2] { obj, args });
            if (cmd == null)
            {
                Debug.LogError($"CMD命令类,初始化错误：{cmdName}");
                return ECommandReply.N;
            }

            return SendCommand(cmd, undo);
        }


        public static ECommandReply SendCommand(ICommand cmd, bool undo = false)
        {
            if (cmd.Owner == null)
            {
                return Instance.Command(cmd, undo);
            }

            if (!undo) TaskManager.Instance.PushStack(cmd);

            return cmd.Owner.Command(cmd, undo);
        }

        public static ECommandReply UndoCommand(TaskConmand tc)
        {
            var cmd = tc.command;
            Debug.Log($"回滚任务{tc.taskId} Obj:{cmd.Owner.name},命令:{cmd.Command}");
            if (cmd.Owner)
            {
                return cmd.Owner.Command(cmd, true);
            }

            return Instance.Command(cmd, true);
        }

        protected void JumpTaskId(string taskId)
        {
            TaskModel.Instance.JumpToTask(taskId);
        }

        public void CreateObject(CreateObjCommand cmd)
        {
            string path = "Prefab/Tools/" + cmd.ItemId;
            var prefab = Resources.Load<GameObject>(path);
            var go = GameObject.Instantiate(prefab);

            var item = go.GetComponent<SceneItem>();
            item.AddToPoint(cmd.pointData);
            ObjectManager.Instance.GetSceneData().AddItem(item);
        }

        public override void Dispose()
        {
        }
    }
}