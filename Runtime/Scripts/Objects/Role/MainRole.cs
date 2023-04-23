using System;


using UnityEngine;

namespace TF.Runtime
{

    public class MainRole : Role
    {
        protected ERoleState curState;

        protected override void AddCommands()
        {
            base.AddCommands();
            this.Receiver.AddCommand<DressCommand>(ECommand.Dress, CheckDress);
            this.Receiver.AddCommand<UndressCommand>(ECommand.UnDress, CheckUnDress);
            this.Receiver.AddCommand<LoadEquipCommand>(ECommand.LoadEquip, CheckLoadEquip);
            this.Receiver.AddCommand<UnLoadEquipCommand>(ECommand.UnLoadEquip, CheckUnloadEquip);
            this.Receiver.AddCommand<ChildAnimCommand>(ECommand.PlayChildAnim, CheckPlayHandAnim);
        }

        public virtual HandItem LeftHand => null;
        public virtual HandItem RightHand => null;

        public virtual MainRole Init()
        {
            return this;
        }

        protected override ECommandReply CheckChildActive(ActiveChildCommand cmd, bool undo)
        {
            var child = transform.Find(cmd.ChildName);
            if (child == null)
            {
                Debug.LogError("child not found");
                return ECommandReply.N;
            }

            child.gameObject.SetActive(cmd.Enable);
            return ECommandReply.Y;
        }

        protected virtual ECommandReply CheckDress(DressCommand command, bool undo)
        {
            return ECommandReply.Y;
        }

        protected virtual ECommandReply CheckUnDress(UndressCommand command, bool undo)
        {
            return ECommandReply.Y;
        }

        protected virtual ECommandReply CheckLoadEquip(LoadEquipCommand cmd, bool undo)
        {
            return ECommandReply.Y;
        }

        protected virtual ECommandReply CheckUnloadEquip(UnLoadEquipCommand cmd, bool undo)
        {
            return ECommandReply.Y;
        }

        protected virtual ECommandReply CheckPlayHandAnim(ChildAnimCommand cmd, bool undo)
        {
            return ECommandReply.Y;
        }

        protected override ECommandReply CheckSetPosition(PositionCommand cmd, bool undo)
        {
            if (undo)
            {
                if (cmd.Rotate)
                {
                    this.SetPosition(cmd.Origin.Position, cmd.Origin.Rotation);
                }
                else
                {
                    this.SetPosition(cmd.Origin.Position);
                }
            }
            else
            {
                if (cmd.Rotate)
                {
                    this.SetPosition(cmd.Target.Position, cmd.Target.Rotation);
                }
                else
                {
                    this.SetPosition(cmd.Target.Position);

                }
            }

            return ECommandReply.Y;
        }

        public virtual Vector3 GetPosition()
        {
            return transform.position;
        }

        public virtual void SetPosition(Vector3 pos)
        {
            transform.position = pos;
        }

        public virtual void SetPosition(Vector3 pos, Vector3 rot)
        {
            transform.position = pos;
            transform.eulerAngles = rot;
        }

        public override void PlayAnim(string animName)
        {
            SetState(ERoleState.Animate);
            Animator.Play(animName);
        }

        public override void SetState(ERoleState state)
        {
            if (curState == state)
            {
                return;
            }

            curState = state;
        }

        private void Update()
        {
            if (Input.GetMouseButton(0))
            {
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hitinfo, 100))
                {
                    var item = hitinfo.collider.GetComponent<SceneItem>();
                    if (item != null)
                    {
                        item.EnterTouch(ITEM_ID.RightHand);
                    }
                }
            }
#if UNITY_EDITOR
            if (Input.GetKeyUp(KeyCode.T))
            {
                TaskModel.Instance.FinishCurTask();
            }
#endif
        }

    }
}