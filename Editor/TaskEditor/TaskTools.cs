using System;
using System.Collections.Generic;
using TF.Runtime;
using UnityEngine;

namespace TF.Editor
{
    public static class TaskTools
    {
        private static Dictionary<string, SceneObject> items = new Dictionary<string, SceneObject>();
        private static Dictionary<string, PointObject> points = new Dictionary<string, PointObject>();
        public static ECommand[] IgnoreItems =
        {
            ECommand.Audio,
            ECommand.FlyWord,
            ECommand.CreateObj,
            ECommand.TaskJump,
            ECommand.LoadScene,
            ECommand.ProjectId
        };
        
        public static void Refresh()
        {
            InitItems();
            InitPoints();
        }

        public static bool IsItemNil()
        {
            foreach (var pair in items)
            {
                if (pair.Value == null)
                {
                    return true;
                }
            }

            return false;
        }

        static void InitItems()
        {
            items.Clear();

            var root = GameObject.FindObjectOfType<SceneData>();
            if (root == null)
            {
                Debug.Log("找不到Scene Data");
                return;
            }

            var mainRole = GameObject.FindObjectOfType<MainRole>();
            if (mainRole == null)
            {
                Debug.LogError("找不到MainRole");
                return;
            }

            var equips = mainRole.GetComponentsInChildren<SceneItem>(true);
            AddItem(mainRole);
            AddItems(equips);
            var sceneData = root.GetComponent<SceneData>();
            AddItems(sceneData.gameObject.GetComponentsInChildren<SceneObject>(true));
            foreach (var group in sceneData.Groups)
            {
                var array = group.GetComponentsInChildren<SceneObject>(true);
                AddItems(array);
            }
        }

        static void InitPoints()
        {
            points.Clear();
            var root = GameObject.FindObjectOfType<PointData>();
            if (root == null)
            {
                Debug.LogError("找不到PointData");
                return;
            }

            var childs = root.GetComponentsInChildren<PointObject>();
            foreach (var point in childs)
            {
                points.Add(point.name, point);
            }
        }

        static void AddItems(SceneObject[] objs)
        {
            foreach (var obj in objs)
            {
                if (string.IsNullOrEmpty(obj.id))
                    throw new ArgumentException($"item {obj.name}'s id is null");
                if (items.ContainsKey(obj.id))
                    throw new ArgumentException($"item {obj.id} with the same key has already been added");
                AddItem(obj);
            }
        }

        static void AddItem(SceneObject obj)
        {
            items.Add(obj.id, obj);
        }

        public static SceneObject GetItem(string id)
        {
            if (items.TryGetValue(id, out SceneObject obj))
            {
                return obj;
            }

            return null;
        }

        public static PointObject GetPoint(string name)
        {
            if (points.TryGetValue(name, out PointObject p))
            {
                return p;
            }

            return null;
        }

        public static string GetItemsId(List<SceneObject> objects, string[] itemIds)
        {
            var str = "";
            if (objects != null)
            {
                for (int i = 0; i < objects.Count; i++)
                {
                    var obj = objects[i];
                    if (obj != null)
                    {
                        str += obj.id;
                    }
                    else
                    {
                        str += itemIds[i];
                    }

                    str += "|";
                }

                if (str.Length > 1)
                {
                    str = str.TrimEnd('|');
                }
            }

            return str;
        }

        public static string ToString(ECommand command)
        {
            if (command == ECommand.None)
            {
                return string.Empty;
            }

            return command.ToString();
        }
        public static string ToString(ECommandUI command)
        {
            if (command == ECommandUI.None)
            {
                return string.Empty;
            }

            return command.ToString();
        }

        public static bool CanNil(ECommand command)
        {
            foreach (var eCmd in IgnoreItems)
            {
                if (eCmd == command)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool CanNil(List<ObjectCmd> cmds)
        {
            foreach (var cmd in cmds)
            {
                if (!CanNil(cmd.name))
                {
                    return false;
                }
            }

            return true;
        }

        public static bool CanNil(EPanel panel)
        {
            if (panel == EPanel.NpcTalk || panel == EPanel.Main)
            {
                return true;
            }

            return false;
        }
    }
}