
using TMPro;
using UnityEngine;

namespace TF.Runtime
{
    /// <summary>
    /// TextMeshPro 文本物体
    /// </summary>
    public class TextItem : SceneItem
    {
        private TextMeshPro textmesh;
        private Vector3 localScale;

        protected override void Awake()
        {
            base.Awake();
            localScale = transform.localScale;
            textmesh = this.GetComponent<TextMeshPro>();
        }

        protected override ECommandReply CheckSetScale(ScaleCommand cmd, bool undo)
        {
            transform.localScale = localScale * cmd.Target.x;

            return ECommandReply.Y;
        }

        protected override ECommandReply CheckHilight(HilightCommand cmd, bool undo)
        {
            if (cmd.Enable)
            {
                textmesh.fontMaterial.SetColor("_FaceColor", Color.green);
            }
            else
            {
                textmesh.fontMaterial.SetColor("_FaceColor", Color.white);
            }

            return ECommandReply.Y;
        }
    }
}