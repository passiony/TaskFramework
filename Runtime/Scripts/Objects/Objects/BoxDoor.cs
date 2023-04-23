using System;
using UnityEngine;

namespace TF.Runtime
{
    /// <summary>
    /// 门 
    /// </summary>
    public class BoxDoor : SceneItem
    {
        [Serializable]
        public class ADoor
        {
            public Transform go;
            public Vector3 openAngle;
            public Vector3 closeAngle;
            public float duration = 2;
        }

        public bool open;
        public ADoor[] doors;
        private ESwitchState openState;
    
        protected override void Awake()
        {
            base.Awake();
            OpenDoor(open);
        }

        public override string GetState()
        {
            return openState.ToString();
        }
    
        protected override ECommandReply CheckSetState(StateCommand cmd, bool undo)
        {
            var state = cmd.TargetState.ToEnum<ESwitchState>();
            if (undo)
            {
                state = cmd.OriginState.ToEnum<ESwitchState>();
            }

            openState = state;
            open = state == ESwitchState.Open;
            OpenDoor(open);

            return ECommandReply.Y;
        }


        protected virtual void OpenDoor(bool value)
        {
            var collider = this.GetComponent<Collider>();
            if (collider != null)
            {
                collider.enabled = !value;
            }

            // DOTween
            // foreach (var door in doors)
            // {
            //     if (value) //开
            //     {
            //         door.go.DOLocalRotate(door.openAngle, door.duration);
            //     }
            //     else //关
            //     {
            //         door.go.DOLocalRotate(door.closeAngle, door.duration);
            //     }
            // }
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                OpenDoor(true);
            }
            if (Input.GetKeyDown(KeyCode.B))
            {
                OpenDoor(false);
            }
        }
    }
}