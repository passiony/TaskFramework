using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TF.Runtime
{
    public class TaskData
    {
        public string id;
        public ETaskState state;
        public int talk_index;
        public db_TaskConfig config;
        public TaskCondition condition;
        public TaskGoal[] goals;

        public void Init(db_TaskConfig _config)
        {
            config = _config;
            id = config.id;
            state = ETaskState.None;
            talk_index = 0;

            if (config.goals == null || config.goals.Length == 0)
            {
                Debug.LogError($"{id}Task配置表错误，没有找到goals");
            }

            goals = new TaskGoal[config.goals.Length];
            for (int i = 0; i < goals.Length; i++)
            {
                TaskGoal goal = new TaskGoal();
                goal.Init(config.goals[i]);
                goals[i] = goal;
            }

            if (config.condition?.Length > 0 && !string.IsNullOrEmpty(config.condition[0]))
            {
                condition = new TaskCondition(config.condition, GetConditionGoals());
            }
        }

        public bool CheckFinish()
        {
            if (HasCondition())
            {
                return false;
            }

            foreach (var goal in this.goals)
            {
                if (goal.state != ETaskState.Finish)
                {
                    return false;
                }
            }

            return true;
        }

        public bool HasCondition()
        {
            return condition != null;
        }

        public void Reset()
        {
            for (int i = 0; i < goals.Length; i++)
            {
                goals[i].Refresh();
            }

            if (condition != null && condition.permission == EPermission.Read)
            {
                condition.Reset();
            }
        }

        public string[] FilterItem(EGoalType type, string[] items)
        {
            if (condition != null && condition.type == type && condition.HasRead())
            {
                var temp = new List<string>(items);
                for (int i = temp.Count - 1; i >= 0; i--)
                {
                    int value = condition.GetValue(temp[i]);
                    if (value > 0)
                        temp.RemoveAt(i);
                }

                items = temp.ToArray();
            }

            return items;
        }

        TaskGoal[] GetConditionGoals()
        {
            var matchs = goals.Where((goal) =>
            {
                if (goal.type == EGoalType.Panel)
                {
                    var data = goal.data as panel_goal_data;
                    foreach (var cmd_data in data.cmds)
                    {
                        if (cmd_data.name == ECommandUI.SetOptions.ToString())
                        {
                            return true;
                        }
                    }
                }
                else if (goal.type == EGoalType.Touch)
                {
                    var data = goal.data as touch_goal_data;
                    return data.items.Length >= 1;
                }

                return false;
            });

            return matchs.ToArray();
        }
    }

}