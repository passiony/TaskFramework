using System;

using UnityEngine;

namespace TF.Runtime
{
    public class ObjectManager : MonoSingleton<ObjectManager>
    {
        [NonSerialized] public Role[] roles;
        [NonSerialized] public MainRole mainRole;

        private SceneData curSceneData;
        private PointData curPointData;

        public void ResetSceneData()
        {
            mainRole = FindObjectOfType<MainRole>();
            if (mainRole == null)
            {
                Debug.LogError("mainRole == null");
            }

            var equips = mainRole.gameObject.GetComponentsInChildren<SceneItem>(true);
            roles = GameObject.FindObjectsOfType<Role>();

            if (curSceneData) Destroy(curSceneData);
            curSceneData = GameObject.FindObjectOfType<SceneData>();
            curSceneData.AddItem(mainRole);
            curSceneData.AddItems(equips);
            curSceneData.Init();

            if (curPointData)
                Destroy(curPointData);
            curPointData = FindObjectOfType<PointData>();
        }

        public Vector3 GetNpcPosition(int target_id)
        {
            return Vector3.zero;
        }

        public MainRole GetMainRole()
        {
            return mainRole;
        }

        public Role GetNPC(string target_id)
        {
            for (int i = 0; i < roles.Length; i++)
            {
                if (string.Equals(roles[i].id, target_id, StringComparison.OrdinalIgnoreCase))
                {
                    return roles[i];
                }
            }

            Debug.LogError("找不到npc：" + target_id);
            return null;
        }

        public void AddSceneItem(SceneObject item)
        {
            curSceneData.AddItem(item);
        }

        public SceneObject GetSceneItem(string itemId)
        {
            return curSceneData.GetItem(itemId);
        }

        public T GetSceneItem<T>(string itemId) where T : SceneObject
        {
            return curSceneData.GetItem(itemId) as T;
        }

        public SceneData GetSceneData()
        {
            return curSceneData;
        }

        public Transform GetPoint(string key)
        {
            return curPointData.GetPoint(key);
        }

        public Point GetPoint2(string key)
        {
            var point = Point.Parse(curPointData.GetPoint(key));
            return point;
        }

        public override void Dispose()
        {
        }
    }
}