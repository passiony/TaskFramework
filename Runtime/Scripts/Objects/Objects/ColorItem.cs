
using UnityEngine;

namespace TF.Runtime
{
    /// <summary>
    /// 可变颜色和透明度物品
    /// </summary>
    public class ColorItem : SceneItem
    {
        public GameObject[] models;
        private Material[] materials;

        private Material clearMat;

        protected override void Awake()
        {
            base.Awake();
            clearMat = Resources.Load<Material>("Materials/Clear");

            int count = models.Length;
            materials = new Material[count];
            for (int i = 0; i < models.Length; i++)
            {
                materials[i] = models[i].GetComponent<Renderer>().material;
            }
        }

        protected override void AddCommands()
        {
            base.AddCommands();
            this.Receiver.AddCommand<ColorCommand>(ECommand.SetColor, CheckSetColor);
            this.Receiver.AddCommand<AlphaCommand>(ECommand.SetAlpha, CheckSetAlpha);
        }

        private ECommandReply CheckSetAlpha(AlphaCommand command, bool undo)
        {
            if (undo)
            {
                SetAlpha(command.OriginAlpha);
            }
            else
            {
                SetAlpha(command.TargetAlpha);
            }
            return ECommandReply.Y;
        }

        private ECommandReply CheckSetColor(ColorCommand command, bool undo)
        {
            if (undo)
            {
                SetColor(command.OriginColor);
            }
            else
            {
                SetColor(command.TargetColor);
            }
            return ECommandReply.Y;
        }

        protected void SetColor(Color color)
        {
            foreach (var model in models)
            {
                model.SetActive(true);
                var meshs = model.GetComponentsInChildren<Renderer>();
                foreach (var mesh in meshs)
                {
                    mesh.material.color = color;
                }
            }
        }

        protected void SetAlpha(float alpha)
        {
            gameObject.SetActive(true);
            for (int i = 0; i < models.Length; i++)
            {
                var model = models[i];
                model.SetActive(true);
                if (alpha == 1)
                {
                    var meshs = model.GetComponentsInChildren<Renderer>();
                    foreach (var mesh in meshs)
                    {
                        mesh.material = materials[i];
                    }
                }
                else
                {
                    clearMat.color = new Color(1, 1, 1, alpha);
                    var meshs = model.GetComponentsInChildren<Renderer>();
                    foreach (var mesh in meshs)
                    {
                        mesh.material = clearMat;
                    }
                }
            }
        }
    }
}