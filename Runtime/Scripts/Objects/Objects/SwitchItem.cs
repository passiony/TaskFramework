using System;

using UnityEngine;

namespace TF.Runtime
{
    /// <summary>
    /// 开关
    /// </summary>
    public class SwitchItem : SceneItem
    {
        public enum ESwitchState
        {
            Close,
            Open
        }

        [Serializable]
        public class SwitchData
        {
            public bool pos;
            public Vector3 position;
            public bool rot;
            public Vector3 rotation;
        }

        public Transform swithObject;
        public SwitchData closeData;
        public SwitchData openData;

        public ESwitchState currentState;

        protected override void Awake()
        {
            base.Awake();
            SetState(currentState);
        }

        public override string GetState()
        {
            return currentState.ToString();
        }
    
        protected override ECommandReply CheckSetState(StateCommand cmd, bool undo)
        {
            var state = cmd.TargetState.ToEnum<ESwitchState>();
            if (undo)
            {
                state = cmd.OriginState.ToEnum<ESwitchState>();
            }
            SetState(state);

            return ECommandReply.Y;
        }

        public void SetState(ESwitchState state)
        {
            currentState = state;
            switch (state)
            {
                case ESwitchState.Close:
                    if (closeData.pos)
                    {
                        swithObject.localPosition = closeData.position;
                    }
                
                    if (closeData.rot)
                    {
                        swithObject.localEulerAngles = closeData.rotation;
                    }
                    break;
                case ESwitchState.Open:
                    if (openData.pos)
                    {
                        swithObject.localPosition = openData.position;
                    }
                
                    if (openData.rot)
                    {
                        swithObject.localEulerAngles = openData.rotation;
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }
    }
}