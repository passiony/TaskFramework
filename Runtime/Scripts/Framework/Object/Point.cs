using System;
using UnityEngine;

namespace TF.Runtime
{
    /// <summary>
    /// 自定义“点”信息
    /// </summary>
    public class Point
    {
        public Vector3 Position;
        public Vector3 Rotation;
        public Vector3 Scale;

        public static Point Parse(Transform point)
        {
            var p = new Point();
            p.Position = point.position;
            p.Rotation = point.eulerAngles;
            p.Scale = point.localScale;
            return p;
        }
    }
}