using System;
using UnityEngine;

namespace TF.Runtime
{

    public class Role : SceneObject
    {
        public enum ERoleState
        {
            None,
            Manual, //玩家手动操作
            Animate, //动画模式
            Discharge, //触电
            Alive, //复活
            Fall,
            Dead, //死亡
        }

        private ERoleState roleState;

        public override string GetState()
        {
            return roleState.ToString();
        }

        protected override ECommandReply CheckSetState(StateCommand cmd, bool undo)
        {
            var state = cmd.TargetState.ToEnum<ERoleState>();
            if (undo)
            {
                state = cmd.OriginState.ToEnum<ERoleState>();
            }

            SetState(state);
            return ECommandReply.Y;
        }

        public virtual void SetState(ERoleState state)
        {
            roleState = state;
            switch (state)
            {
                case ERoleState.Discharge:
                    SetElectricMode();
                    break;
                case ERoleState.Dead:
                    SetDeadMode();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }

        private void SetElectricMode()
        {
            var rigid = this.GetComponent<Rigidbody>();
            rigid.useGravity = true;
            rigid.AddForceAtPosition(Vector3.right * 2, new Vector3(0, 2, 0), ForceMode.Impulse);

            this.GetComponent<Collider>().isTrigger = false;
        }

        public virtual void SetDeadMode()
        {}
    }
}