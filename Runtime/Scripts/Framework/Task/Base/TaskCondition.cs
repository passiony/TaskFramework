using System;
using System.Collections.Generic;
using System.Linq;

namespace TF.Runtime
{
    public class ObjSave
    {
        public string id;
        public int value;
    }

    public class TaskCondition
    {
        public EGoalType type;
        public EPermission permission { get; private set; }
        public string[] taskIds;
        public List<ObjSave> saves = new List<ObjSave>();
        protected int currentIndex = -1;

        public TaskCondition(string[] args, TaskGoal[] goals)
        {
            this.permission = args.TryGetValue(0).ToEnum<EPermission>();
            this.taskIds = args.TryGetSplit(1);
            this.type = goals[0].type;
            foreach (var goal in goals)
            {
                var options = goal.array.TryGetValue(4)
                    .Replace("SetOptions:", "")
                    .Split('|');
                foreach (var option in options)
                {
                    var os = new ObjSave();
                    os.id = option;
                    os.value = 0;
                    saves.Add(os);
                }
            }
        }

        public void Reset()
        {
            foreach (var save in saves)
            {
                save.value = 0;
            }
        }

        public bool HasRead()
        {
            return permission == EPermission.Read || permission == EPermission.ReadWrite;
        }

        public bool ReadWrite()
        {
            return permission == EPermission.ReadWrite;
        }

        public string GetDefault()
        {
            return taskIds.Last();
        }
        
        public bool IsFinished()
        {
            foreach (var save in saves)
            {
                if (save.value == 0)
                {
                    return false;
                }
            }

            return true;
        }

        public int GetValue(string id)
        {
            foreach (var save in saves)
            {
                if (save.id == id)
                {
                    return save.value;
                }
            }

            return -1;
        }

        public void SetSelect(string id)
        {
            int num = 0;
            currentIndex = -1;
            foreach (var os in saves)
            {
                if (os.value > num)
                {
                    num = os.value;
                }
            }

            for (int i = 0; i < saves.Count; i++)
            {
                if (saves[i].id == id)
                {
                    saves[i].value = ++num;
                    currentIndex = i;
                }
            }
        }

        /// <summary>
        /// 获取选择后跳转id
        /// </summary>
        /// <returns></returns>
        public string GetSelect()
        {
            if (currentIndex < 0)
            {
                return taskIds.Last();
            }

            return taskIds[currentIndex];
        }
    }
}