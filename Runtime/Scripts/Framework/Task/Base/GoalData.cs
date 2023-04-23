using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;

namespace TF.Runtime
{
    /// <summary>
    /// 任务目标基类
    /// </summary>
    public abstract class db_goal_data
    {
        public EGoalType goal_type;
        public int auto_finish;

        public db_goal_data(params string[] args)
        {
            Enum.TryParse(args.TryGetValue(0), true, out goal_type);
            this.auto_finish = int.Parse(args[1]);
        }

        public static db_goal_data Create(EGoalType goalType, params string[] args)
        {
            var dataObj = GameUtil.GetGoalClass(goalType, args);
            return dataObj;
        }
    }

    /// <summary>
    /// 触摸收集目标touch
    /// </summary>
    public class touch_goal_data : db_goal_data
    {
        public EGoalFinishType touch_type;
        public bool hilight;
        public string[] items;
        public cmd_data[] cmds;

        public touch_goal_data(params string[] args) : base(args)
        {
            if (!Enum.TryParse(args.TryGetValue(2), true, out touch_type))
            {
                Debug.LogError("EGoalFinishType枚举类型转换错误");
            }

            this.hilight = args.TryGetBoolValue(3);
            this.items = args.TryGetSplit(4);
            this.cmds = new cmd_data[args.Length - 5];
            for (int i = 5; i < args.Length; i++)
            {
                cmds[i - 5] = new cmd_data(args.TryGetValue(i));
            }
        }
    }

    /// <summary>
    /// 高亮操作
    /// </summary>
    public class hilight_goal_data : db_goal_data
    {
        public bool enable;
        public string[] items;
        public int hilight_width;
        public Color color;

        public hilight_goal_data(params string[] args) : base(args)
        {
            this.auto_finish = 1;
            this.enable = bool.Parse(args[2]);
            this.items = args.TryGetSplit(3);

            var width = args.TryGetIntValue(4);
            if (width == 0)
            {
                width = width > 0 ? width : 10;
            }

            this.hilight_width = width;
            this.color = args.TryGetValue(5).ToColor();
        }
    }

    /// <summary>
    /// 传送操作
    /// </summary>
    public class teleport_goal_data : db_goal_data
    {
        public string id;
        public ETeleportType teleport_type;
        public string name;
        public string target;
        public string sceneName;

        public teleport_goal_data(params string[] args) : base(args)
        {
            this.id = args.TryGetValue(2);
            Enum.TryParse(args.TryGetValue(3), true, out teleport_type);
            this.name = args.TryGetValue(4);
            this.target = args.TryGetValue(5);
            this.sceneName = args.TryGetValue(6);
        }
    }

    /// <summary>
    /// 播放声音
    /// </summary>
    public class audio_goal_data : db_goal_data
    {
        public EAudioType audio_type;
        public string audioId;
        public int loop; //循环次数，-1:无限循环

        public audio_goal_data(params string[] args) : base(args)
        {
            Enum.TryParse(args.TryGetValue(2), true, out audio_type);
            this.audioId = args.TryGetValue(3);
            this.loop = args.TryGetIntValue(4);
        }
    }


    /// <summary>
    /// 事件
    /// </summary>
    public class event_goal_data : db_goal_data
    {
        public int event_id;
        public string[] event_args;

        public event_goal_data(params string[] args) : base(args)
        {
            this.auto_finish = 1;
            this.event_id = int.Parse(args[2]);
            this.event_args = args.TryGetSplit(3);
        }
    }

    /// <summary>
    /// 打开UI页面
    /// </summary>
    public class panel_goal_data : db_goal_data
    {
        public string uiName;
        public string point;
        public cmd_data[] cmds;

        public panel_goal_data(params string[] args) : base(args)
        {
            this.auto_finish = args.TryGetIntValue(1);
            this.uiName = args.TryGetValue(2);
            this.point = args.TryGetValue(3);
            this.cmds = new cmd_data[args.Length - 4];
            for (int i = 4; i < args.Length; i++)
            {
                this.cmds[i - 4] = new cmd_data(args.TryGetValue(i));
            }
        }
    }

    /// <summary>
    /// sceneObject函数调用
    /// </summary>
    public class object_goal_data : db_goal_data
    {
        public string[] items;
        public cmd_data[] cmds;

        public object_goal_data(params string[] args) : base(args)
        {
            this.items = args.TryGetSplit(2);
            this.cmds = new cmd_data[args.Length - 3];
            for (int i = 3; i < args.Length; i++)
            {
                this.cmds[i - 3] = new cmd_data(args.TryGetValue(i));
            }
        }
    }

}