using System;
using System.Collections.Generic;
using TF.Runtime;
using UnityEngine;
#if ODIN_INSPECTOR
      using Sirenix.OdinInspector;
#endif

namespace TF.Editor
{
    /// <summary>
    /// 任务目标基类
    /// </summary>
    public abstract class db_goal_data
    {
        [NonSerialized] public EGoalType type;
        [NonSerialized] public int autoFinish;

        public abstract string[] ToStringArray();

        public virtual string GetError(TaskData task)
        {
            return string.Empty;
        }

        public static db_goal_data CreateGoalObject(EGoalType goalType, params string[] args)
        {
            switch (goalType)
            {
                case EGoalType.Panel:
                    return new panel_goal_data(args);
                case EGoalType.Touch:
                    return new touch_goal_data(args);
                case EGoalType.Hilight:
                    return new hilight_goal_data(args);
                case EGoalType.Teleport:
                    return new teleport_goal_data(args);
                case EGoalType.Audio:
                    return new audio_goal_data(args);
                case EGoalType.Object:
                    return new object_goal_data(args);
                default:
                    return null;
            }
        }
    }


    /// <summary>
    /// 打开UI页面
    /// </summary>
    [Serializable]
    public class panel_goal_data : db_goal_data
    {
        public new int autoFinish;
        public EPanel uiName;
        public PointObject point;
#if ODIN_INSPECTOR
        [TableList(AlwaysExpanded = true)]
#endif
        public List<UICmd> cmds = new List<UICmd>();

        public panel_goal_data()
        {
            type = EGoalType.Panel;
        }

        public panel_goal_data(string[] args)
        {
            type = EGoalType.Panel;
            autoFinish = args.TryGetIntValue(1);
            Enum.TryParse(args.TryGetValue(2), true, out uiName);
            point = TaskTools.GetPoint(args.TryGetValue(3));
            for (int i = 4; i < args.Length; i++)
            {
                var value = args.TryGetValue(i);
                if (!string.IsNullOrEmpty(value))
                {
                    cmds.Add(new UICmd(value));
                }
            }
        }

        public override string[] ToStringArray()
        {
            var array = new List<string>();
            array.Add(EGoalType.Panel.ToString());
            array.Add(autoFinish.ToString());
            array.Add(uiName.ToString());
            array.Add(point == null ? "" : point.name);
            foreach (var cmd in cmds)
            {
                array.Add(cmd.ToString());
            }

            return array.ToArray();
        }

        public override string GetError(TaskData task)
        {
            if (!TaskTools.CanNil(uiName) && point == null)
            {
                return $"id={task.id} {uiName}'s point is null; ";
            }

            return base.GetError(task);
        }
    }

    /// <summary>
    /// 触摸收集目标touch
    /// </summary>
    [Serializable]
    public class touch_goal_data : db_goal_data
    {
#if ODIN_INSPECTOR
        [LabelWidth(100)]
#endif
        public EGoalFinishType touchType;
        public bool hilight;
        [NonSerialized] public string[] itemIds;

        public string Name
        {
            get { return "Items: " + TaskTools.GetItemsId(items, itemIds); }
        }

#if ODIN_INSPECTOR
        [BoxGroup("$Name"), ListDrawerSettings(Expanded = true, ShowIndexLabels = true)]
#endif
        public List<SceneObject> items = new List<SceneObject>();

#if ODIN_INSPECTOR
         [TableList(AlwaysExpanded = true)]
#endif
        public List<ObjectCmd> cmds = new List<ObjectCmd>();

        public touch_goal_data()
        {
            type = EGoalType.Touch;
        }

        public touch_goal_data(string[] args)
        {
            type = EGoalType.Touch;
            Enum.TryParse(args.TryGetValue(2), true, out touchType);
            hilight = args.TryGetBoolValue(3);
            itemIds = args.TryGetSplit(4);
            for (int i = 5; i < args.Length; i++)
            {
                var value = args.TryGetValue(i);
                if (!string.IsNullOrEmpty(value))
                {
                    cmds.Add(new ObjectCmd(value));
                }
            }

            if (itemIds.Length > 0 && !string.IsNullOrEmpty(itemIds[0]))
            {
                for (int i = 0; i < itemIds.Length; i++)
                {
                    items.Add(TaskTools.GetItem(itemIds[i]));
                }
            }
        }

        public override string[] ToStringArray()
        {
            var array = new List<string>();
            array.Add(EGoalType.Touch.ToString());
            array.Add("0");
            array.Add(touchType.ToString());
            array.Add(hilight.ToString());
            array.Add(TaskTools.GetItemsId(items, itemIds));
            foreach (var cmd in cmds)
            {
                array.Add(cmd.ToString());
            }

            return array.ToArray();
        }

        public override string GetError(TaskData task)
        {
            if (this.items.Count == 0)
            {
                return $"id={task.id} items.Count = null; ";
            }
            else
            {
                foreach (var obj in this.items)
                {
                    if (obj == null)
                    {
                        return $"id={task.id} items.obj 为空; ";
                    }
                }
            }

            return base.GetError(task);
        }
    }

    /// <summary>
    /// 高亮操作
    /// </summary>
    [Serializable]
    public class hilight_goal_data : db_goal_data
    {
#if ODIN_INSPECTOR
        [ToggleLeft]
#endif
        public bool enable;

        public string Name
        {
            get { return "Items: " + TaskTools.GetItemsId(items, itemIds); }
        }

#if ODIN_INSPECTOR
        [BoxGroup("$Name"), ListDrawerSettings(Expanded = true, ShowIndexLabels = true)]
#endif
        public List<SceneObject> items = new List<SceneObject>();

        [NonSerialized] public string[] itemIds;
        public int hilight_width;
        public EColor color;

        public hilight_goal_data()
        {
            type = EGoalType.Hilight;
        }

        public hilight_goal_data(string[] args)
        {
            type = EGoalType.Hilight;
            enable = args.TryGetBoolValue(2);
            itemIds = args.TryGetSplit(3);
            hilight_width = args.TryGetIntValue(4);
            Enum.TryParse(args.TryGetValue(5), true, out color);

            if (itemIds.Length > 0 && !string.IsNullOrEmpty(itemIds[0]))
            {
                for (int i = 0; i < itemIds.Length; i++)
                {
                    items.Add(TaskTools.GetItem(itemIds[i]));
                }
            }
        }

        public override string[] ToStringArray()
        {
            var array = new List<string>();
            array.Add(EGoalType.Hilight.ToString());
            array.Add("1");
            array.Add(enable.ToString());
            array.Add(TaskTools.GetItemsId(items, itemIds));
            array.Add(hilight_width.ToString());
            array.Add("green");
            return array.ToArray();
        }

        public override string GetError(TaskData task)
        {
            if (this.items.Count == 0)
            {
                return $"id={task.id} items.Count = null; ";
            }
            else
            {
                foreach (var obj in this.items)
                {
                    if (obj == null)
                    {
                        return $"id={task.id} items.obj 为空; ";
                    }
                }
            }

            return base.GetError(task);
        }
    }

    /// <summary>
    /// 传送操作
    /// </summary>
    [Serializable]
    public class teleport_goal_data : db_goal_data
    {
        public string id;
        public string name;
        public ETeleportType teleType;

#if ODIN_INSPECTOR
        [ShowIf("@this.teleType==ETeleportType.Target")]
#endif
        public string target;

#if ODIN_INSPECTOR
        [ShowIf("@this.teleType==ETeleportType.Scene")]
#endif
        public string sceneName;

        public teleport_goal_data()
        {
            type = EGoalType.Teleport;
        }

        public teleport_goal_data(string[] args)
        {
            type = EGoalType.Teleport;
            id = args.TryGetValue(2);
            Enum.TryParse(args.TryGetValue(3), true, out teleType);
            this.name = args.TryGetValue(4);
            this.target = args.TryGetValue(5);
            this.sceneName = args.TryGetValue(6);
        }

        public override string[] ToStringArray()
        {
            var array = new List<string>();
            array.Add(EGoalType.Teleport.ToString());
            array.Add("0");
            array.Add(id);
            array.Add(teleType.ToString());
            array.Add(name);
            array.Add(target);
            array.Add(sceneName);
            return array.ToArray();
        }

        public override string GetError(TaskData task)
        {
            if (teleType == ETeleportType.Target && string.IsNullOrEmpty(target))
            {
                return $"id={task.id} Teleport.target = nil";
            }

            if (teleType == ETeleportType.Scene && string.IsNullOrEmpty(sceneName))
            {
                return $"id={task.id} Teleport.sceneName = nil";
            }

            return base.GetError(task);
        }
    }

    /// <summary>
    /// 播放声音
    /// </summary>
    [Serializable]
    public class audio_goal_data : db_goal_data
    {
        public EAudioType audioType;
        public string audioId; //id
        public float loopTime; //循环次数，-1:无限循环

        public audio_goal_data()
        {
            type = EGoalType.Audio;
        }

        public audio_goal_data(string[] args)
        {
            type = EGoalType.Audio;
            Enum.TryParse(args.TryGetValue(2), true, out audioType);
            this.audioId = args.TryGetValue(3);
            this.loopTime = args.TryGetFloatValue(4);
        }

        public override string[] ToStringArray()
        {
            var array = new List<string>();
            array.Add(EGoalType.Audio.ToString());
            array.Add("0");
            array.Add(audioType.ToString());
            array.Add(audioId);
            array.Add(loopTime.ToString());
            return array.ToArray();
        }

        public override string GetError(TaskData task)
        {
            if (this.audioType >= EAudioType.Man && string.IsNullOrEmpty(task.talk.sentence))
            {
                return $"id={task.id} Talk.Sentence is null; ";
            }

            return base.GetError(task);
        }
    }


    /// <summary>
    /// sceneObject函数调用
    /// </summary>
    [Serializable]
    public class object_goal_data : db_goal_data
    {
        public new int autoFinish;

#if ODIN_INSPECTOR
        [BoxGroup("$Name"), ListDrawerSettings(Expanded = true, ShowIndexLabels = true)]
#endif
        public List<SceneObject> items = new List<SceneObject>();

        [NonSerialized] public string[] itemIds;

#if ODIN_INSPECTOR
        [TableList(AlwaysExpanded = true)] [GUIColor("GetCmdColor")]
#endif
        public List<ObjectCmd> cmds = new List<ObjectCmd>();

        public string Name
        {
            get { return "Items: " + TaskTools.GetItemsId(items, itemIds); }
        }

        private Color GetCmdColor()
        {
            var error = HasError();
            if (error)
            {
                return Color.red;
            }

            return Color.white;
        }

        public object_goal_data()
        {
            type = EGoalType.Object;
        }

        public object_goal_data(string[] args)
        {
            type = EGoalType.Object;
            autoFinish = args.TryGetIntValue(1);
            itemIds = args.TryGetSplit(2);
            for (int i = 3; i < args.Length; i++)
            {
                var value = args.TryGetValue(i);
                if (!string.IsNullOrEmpty(value))
                {
                    cmds.Add(new ObjectCmd(value));
                }
            }

            if (itemIds.Length > 0 && !string.IsNullOrEmpty(itemIds[0]))
            {
                for (int i = 0; i < itemIds.Length; i++)
                {
                    items.Add(TaskTools.GetItem(itemIds[i]));
                }
            }
        }

        public override string[] ToStringArray()
        {
            var array = new List<string>();
            array.Add(EGoalType.Object.ToString());
            array.Add(autoFinish.ToString());
            array.Add(TaskTools.GetItemsId(items, itemIds));
            foreach (var cmd in cmds)
            {
                array.Add(cmd.ToString());
            }

            return array.ToArray();
        }

        public override string GetError(TaskData task)
        {
            var canNil = TaskTools.CanNil(this.cmds);
            if (!canNil)
            {
                if (this.items.Count == 0)
                {
                    return ($"id={task.id} items.Count = null; ");
                }
                else
                {
                    foreach (var obj in this.items)
                    {
                        if (obj == null)
                        {
                            return $"id={task.id} items.obj 为空; ";
                        }
                    }
                }
            }

            return base.GetError(task);
        }

        bool HasError()
        {
            var canNil = TaskTools.CanNil(this.cmds);
            if (!canNil)
            {
                if (this.items.Count == 0)
                {
                    return true;
                }

                foreach (var obj in this.items)
                {
                    if (obj == null)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}