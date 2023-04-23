using System;
using TF.Runtime;

namespace TF.Editor
{
    [Serializable]
    public class TaskStruct
    {
        public int id;
        public string desc;
        public bool submit;
        public float delay;
        public string[] talk;
        public string[] condition;
        public string[][] goals;

        public TaskData ToData()
        {
            var data = new TaskData();
            data.id = this.id;
            data.desc = this.desc;
            data.submit = this.submit;
            data.delay = this.delay;

            if (talk != null && talk.Length >= 3)
            {
                data.talk.enabled = true;
                Enum.TryParse(talk.TryGetValue(0), true, out data.talk.npc);
                data.talk.title = talk.TryGetValue(1);
                data.talk.sentence = talk.TryGetValue(2);
            }

            if (condition != null && condition.Length > 1)
            {
                data.condition.enabled = true;
                Enum.TryParse(condition.TryGetValue(0), true, out data.condition.permission);
                data.condition.taskIds = condition.TryGetValue(1);
            }

            foreach (var goal_arr in this.goals)
            {
                EGoalType gtype;
                var tstr = goal_arr.TryGetValue(0);
                if (int.TryParse(tstr, out int type))
                {
                    gtype = (EGoalType)type;
                }
                else
                {
                    Enum.TryParse(goal_arr.TryGetValue(0), true, out gtype);
                }

                var goal = db_goal_data.CreateGoalObject(gtype, goal_arr);
                if (goal != null)
                {
                    data.goals.Add(goal);
                }
                else
                {
                    throw new ArgumentNullException($"task {id} goal type {tstr} not found");
                }
            }

            return data;
        }
    }
}
