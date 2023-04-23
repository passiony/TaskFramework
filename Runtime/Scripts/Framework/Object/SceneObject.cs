using Quick;
using UnityEngine;

namespace TF.Runtime
{

    /// <summary>
    /// 所有场景物品的基类
    /// </summary>
    public abstract class SceneObject : ISceneObject
    {
        protected Outline Outline { get; set; }
        protected Animator Animator { get; set; }
        public Transform UIRoot { get; set; }

        private PickItem itemLable;

        protected override void Awake()
        {
            base.Awake();
            this.UIRoot = transform.Find("UIRoot");
            this.Animator = transform.GetComponent<Animator>();
        }

        public void InitCmd()
        {
            AddCommands();
        }

        protected override void AddCommands()
        {
            this.Receiver.AddCommand<StateCommand>(ECommand.SetState, CheckSetState);
            this.Receiver.AddCommand<AnimateCommand>(ECommand.PlayAnim, CheckPlayAnim);
            this.Receiver.AddCommand<PositionCommand>(ECommand.SetPosition, CheckSetPosition);
            this.Receiver.AddCommand<ActiveCommand>(ECommand.SetActive, CheckSetActive);
            this.Receiver.AddCommand<ScaleCommand>(ECommand.SetScale, CheckSetScale);
            this.Receiver.AddCommand<ActiveChildCommand>(ECommand.SetChildActive, CheckChildActive);
            this.Receiver.AddCommand<HilightCommand>(ECommand.Hilight, CheckHilight);
            this.Receiver.AddCommand<MoveCommand>(ECommand.Move, CheckMove);
            this.Receiver.AddCommand<MoveToCommand>(ECommand.MoveTo, CheckMoveTo);
            this.Receiver.AddCommand<LableCommand>(ECommand.ShowLable, CheckSetLable);
            this.Receiver.AddCommand<LableMarkCommand>(ECommand.ShowLable, CheckSetLableMark);
            this.Receiver.AddCommand<ColliderCommand>(ECommand.SetCollider, CheckSetCollider);
        }

        private ECommandReply CheckSetCollider(ColliderCommand command, bool undo)
        {
            if (undo)
            {
                this.gameObject.GetComponent<Collider>().enabled = command.OriginEnable;
            }
            else
            {
                this.gameObject.GetComponent<Collider>().enabled = command.TargetEnable;
            }
            return ECommandReply.Y;
        }

        private ECommandReply CheckSetLableMark(LableMarkCommand cmd, bool undo)
        {
            if (undo)
            {
                gameObject.SetActive(cmd.OriginActive);
                ShowLable(false, true);
            }
            else
            {
                gameObject.SetActive(true);
                ShowLable(true, true);
            }

            return ECommandReply.Y;
        }

        protected virtual ECommandReply CheckSetLable(LableCommand cmd, bool undo)
        {
            if (undo)
            {
                gameObject.SetActive(cmd.OriginActive);
                ShowLable(false, false);
            }
            else
            {
                gameObject.SetActive(true);
                ShowLable(true, false);
            }

            return ECommandReply.Y;
        }

        private ECommandReply CheckMoveTo(MoveToCommand cmd, bool undo)
        {
            // DOTween
            // cmd.Owner.DOKill(true);
            // if (undo)
            // {
            //     cmd.Owner.transform.DOMove(cmd.Origin.Position, cmd.Duration);
            //     cmd.Owner.transform.DORotate(cmd.Origin.Rotation, cmd.Duration);
            // }
            // else
            // {
            //     cmd.Owner.transform.DOMove(cmd.Target.Position, cmd.Duration);
            //     cmd.Owner.transform.DORotate(cmd.Target.Rotation, cmd.Duration);
            // }

            return ECommandReply.Y;
        }

        protected virtual ECommandReply CheckSetScale(ScaleCommand cmd, bool undo)
        {
            if (undo)
            {
                transform.localScale = cmd.Origin;
            }
            else
            {
                transform.localScale = cmd.Target;
            }

            return ECommandReply.Y;
        }

        protected virtual ECommandReply CheckMove(MoveCommand cmd, bool undo)
        {
            // DOTween
            // if (undo)
            // {
            //     cmd.Owner.transform.position = cmd.Origin;
            // }
            // else
            // {
            //     cmd.Owner.DOKill(true);
            //
            //     var endPos = cmd.Owner.transform.position + cmd.Velocity * cmd.Duration;
            //     cmd.Owner.transform.DOMove(endPos, cmd.Duration).SetEase(Ease.Linear);
            // }

            return ECommandReply.Y;
        }

        protected virtual ECommandReply CheckSetState(StateCommand command, bool undo)
        {
            return ECommandReply.Y;
        }

        protected virtual ECommandReply CheckPlayAnim(AnimateCommand cmd, bool undo)
        {
            if (undo)
            {
                this.PlayAnim(cmd.OriginAnimName);
            }
            else
            {
                this.PlayAnim(cmd.TargetAnimName);
            }

            return ECommandReply.Y;
        }

        protected virtual ECommandReply CheckChildActive(ActiveChildCommand cmd, bool undo)
        {
            var child = transform.Find(cmd.ChildName);
            if (child == null)
            {
                Debug.LogError("child not found");
                return ECommandReply.N;
            }

            if (undo)
            {
                child.gameObject.SetActive(!cmd.Enable);
            }
            else
            {
                child.gameObject.SetActive(cmd.Enable);
            }

            return ECommandReply.Y;
        }

        protected virtual ECommandReply CheckSetActive(ActiveCommand cmd, bool undo)
        {
            if (undo)
            {
                gameObject.SetActive(!cmd.Enable);
            }
            else
            {
                gameObject.SetActive(cmd.Enable);
            }

            return ECommandReply.Y;
        }

        protected virtual ECommandReply CheckSetPosition(PositionCommand cmd, bool undo)
        {
            if (undo)
            {
                cmd.Owner.SetPosition(cmd.Origin.Position);
                cmd.Owner.SetRotation(cmd.Origin.Rotation);
            }
            else
            {
                cmd.Owner.SetPosition(cmd.Target.Position);
                cmd.Owner.SetRotation(cmd.Target.Rotation);
            }

            return ECommandReply.Y;
        }


        protected virtual ECommandReply CheckHilight(HilightCommand cmd, bool undo)
        {
            if (undo)
            {
                SetHilight(!cmd.Enable, cmd.Width, cmd.Color);
            }
            else
            {
                SetHilight(cmd.Enable, cmd.Width, cmd.Color);
            }

            return ECommandReply.Y;
        }


        public virtual void PlayAnim(string animName)
        {
            Animator.Play(animName);
        }

        public virtual void SetHilight(bool enable, int width = 10, Color color = default)
        {
            if (enable)
            {
                if (Outline == null)
                {
                    Outline = gameObject.AddComponent<Outline>();
                }

                Outline.enabled = true;
                Outline.OutlineWidth = width;
                Outline.OutlineColor = color;
            }
            else
            {
                if (Outline != null)
                    Outline.enabled = false;
            }
        }

        public void ShowLable(bool show, bool hasMark)
        {
            if (this.UIRoot == null)
            {
                Debug.Log(this.name + "的 UIRoot节点为空，无法添加Lable");
                return;
            }

            //销毁
            if (!show)
            {
                foreach (var child in transform)
                {
                    Destroy(child as GameObject);
                }

                return;
            }

            //已创建
            if (itemLable != null)
            {
                return;
            }

            //显示
            var lableName = hasMark ? "LableToogle" : "Lable";
            var prefab = Resources.Load<GameObject>(UIManager.UIPath + lableName);
            if (prefab == null)
            {
                Debug.LogError(lableName + " not found");
            }

            var go = Instantiate(prefab, UIRoot);
            go.SetActive(true);
            go.transform.localPosition = Vector3.zero;
            go.transform.localEulerAngles = Vector3.zero;
            go.transform.localScale = Vector3.one * 0.001f;

            itemLable = go.GetComponent<PickItem>();
            itemLable.Init(hasMark, this);
        }

        public virtual float GetAlpha()
        {
            return 1;
        }

        public virtual Color GetColor()
        {
            return Color.white;
        }

        public virtual string GetState()
        {
            return "None";
        }

        public virtual string GetAnimName()
        {
            return "Idle";
        }

        public virtual string GetChildAnim(string child)
        {
            return "Idle";
        }

        public virtual void KillTween()
        {
            // DOTween
            // transform.DOKill(true);
        }
    }
}