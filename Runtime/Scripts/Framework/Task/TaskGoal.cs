using System;
using System.Collections.Generic;

namespace TF.Runtime
{
    public class TaskGoal
    {
        public EGoalType type;
        public string[] array;
        public db_goal_data data;
        public ETaskState state;
        public Counter[] counters;

        public virtual void Init(string[] array)
        {
            this.array = array;
            Enum.TryParse(array.TryGetValue(0), true, out type);
            data = db_goal_data.Create(type, array);
            state = ETaskState.None;

            Refresh();
        }

        public void Refresh()
        {
            //touch有收集功能
            if (type == EGoalType.Touch)
            {
                var t = data as touch_goal_data;
                counters = Counter.CreateArray(t.items);
            }

            state = ETaskState.None;
        }

        public void CheckFinish()
        {
            var finishType = EGoalFinishType.Or;
            if (type == EGoalType.Touch)
            {
                finishType = ((touch_goal_data)data).touch_type;
            }

            bool finish = true;
            if (finishType == EGoalFinishType.Or)
            {
                finish = false;
                for (int i = 0; i < counters.Length; i++)
                {
                    if (counters[i].IsFinish())
                    {
                        finish = true;
                        break;
                    }
                }
            }
            else if (finishType == EGoalFinishType.And || finishType == EGoalFinishType.Sync)
            {
                finish = true;
                for (int i = 0; i < counters.Length; i++)
                {
                    if (!counters[i].IsFinish())
                    {
                        finish = false;
                        break;
                    }
                }
            }

            if (finish)
            {
                state = ETaskState.Finish;
            }
        }
    }
}