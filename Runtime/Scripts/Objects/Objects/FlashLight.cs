using System;
using UniRx;
using UnityEngine;

namespace TF.Runtime
{
    /// <summary>
    /// 闪烁灯
    /// </summary>
    public class FlashLight : MonoBehaviour
    {
        public Color[] colors;
        private float interval;
        private MeshRenderer mesh;
        private Material mat;
        private IDisposable timer;
        private int index;

        private void Awake()
        {
            mesh = this.GetComponent<MeshRenderer>();
            mat = mesh.material;
        }

        void OnEnable()
        {
            timer?.Dispose();
            interval = 1 + UnityEngine.Random.Range(-0.2f, 0.2f);
            timer = Observable.Interval(TimeSpan.FromSeconds(interval)).Subscribe((x) => { SetLightColor(); }).AddTo(this);
        }

        private void OnDisable()
        {
            timer?.Dispose();
        }

        void SetLightColor()
        {
            index++;
            var id = (int)Mathf.Repeat(index, colors.Length);
            Color color = colors[id];
            mat.color = color;
        }
    }
}