using System;

using UnityEngine;

namespace TF.Runtime
{
    /// <summary>
    /// 带电物体
    /// </summary>
    public class ElectricItem : SceneItem
    {
        public enum EElecState
        {
            None,
            Discharge,
        }
    
        //放电特效
        public GameObject chargeEffect;

        public EElecState state;
        public override string GetState()
        {
            return state.ToString();
        }
    
        protected override ECommandReply CheckSetState(StateCommand cmd, bool undo)
        {
            var state = cmd.TargetState.ToEnum<EElecState>();
            if (undo)
            {
                state = cmd.OriginState.ToEnum<EElecState>();
            }
            SetState(state);
        
            return ECommandReply.Y;
        }

        public void SetState(EElecState state)
        {
            this.state = state;
            switch (state)
            {
                case EElecState.None:
                    chargeEffect.SetActive(false);
                    break;
                case EElecState.Discharge:
                    AudioManager.Instance.PlayEffect("2011");
                    chargeEffect.SetActive(true);
                    ObservableUtil.Delay(3, () =>
                    {
                        chargeEffect.SetActive(false);
                    });
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }

    }
}