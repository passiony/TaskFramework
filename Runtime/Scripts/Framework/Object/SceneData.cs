using System.Collections.Generic;
using UnityEngine;

namespace TF.Runtime
{

    /// <summary>
    /// 物品管理器
    /// </summary>
    public class SceneData : MonoBehaviour
    {
        public int taskId;
        public GameObject[] Groups;

        //所有物品组
        public Dictionary<string, SceneObject> sceneObjects = new Dictionary<string, SceneObject>();

        public void Init()
        {
            InitData();
        }

        void InitData()
        {
            //初始化场景道具
            var childs = gameObject.GetComponentsInChildren<SceneObject>(true);
            AddItems(childs);
            foreach (var group in Groups)
            {
                var objs = group.GetComponentsInChildren<SceneObject>(true);
                AddItems(objs);
            }
        }

        public void AddItem(SceneObject obj)
        {
            var id = obj.id;
            if (sceneObjects.ContainsKey(id))
            {
                Debug.LogError("重复的Sceneobject.id:" + id);
            }

            sceneObjects.Add(id, obj);
            obj.InitCmd();
        }

        public void AddItems(SceneObject[] objs)
        {
            foreach (var obj in objs)
            {
                AddItem(obj);
            }
        }

        public SceneObject RemoveItem(string itemId)
        {
            var id = itemId;
            if (sceneObjects.TryGetValue(id, out SceneObject obj))
            {
                sceneObjects.Remove(id);
                return obj;
            }

            return null;
        }

        public SceneObject GetItem(string itemId)
        {
            string id = itemId;
            if (sceneObjects.TryGetValue(id, out SceneObject item))
            {
                return item;
            }

            Debug.LogError($"找不到item：{id}");
            return null;
        }

        public SceneObject TryGetItem(string itemId)
        {
            string id = itemId;
            if (sceneObjects.TryGetValue(id, out SceneObject item))
            {
                return item;
            }

            return null;
        }
    }
}