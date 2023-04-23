using System.Collections.Generic;
using UnityEngine;

namespace TF.Runtime
{

    public class PointData : MonoBehaviour
    {
        private Dictionary<string, Transform> points;

        // Start is called before the first frame update
        void Awake()
        {
            points = new Dictionary<string, Transform>();
            var objects = transform.GetComponentsInChildren<PointObject>(true);
            foreach (var obj in objects)
            {
                points.Add(obj.name, obj.transform);
            }
        }

        public Transform GetPoint(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                Debug.LogError("point key is nil");
                return transform;
            }

            if (points.TryGetValue(key, out Transform point))
            {
                return point;
            }

            Debug.LogWarning("找不到point:" + key);
            return transform;
        }

    }
}