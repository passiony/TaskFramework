using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TF.Runtime
{
    public enum TaskMode
    {
        Teach, //教学
        Practice, //练习
        Examination //考核
    }

    public class TaskConmand
    {
        public string taskId;
        public ICommand command;

        public TaskConmand(string id, ICommand cmd)
        {
            taskId = id;
            command = cmd;
        }
    }

    public class TaskManager : MonoSingleton<TaskManager>
    {
        public int InitId = 0;
        public TaskMode mode { get; private set; }

        TaskModel model;

        public Stack<TaskConmand> taskStack = new Stack<TaskConmand>();

        protected override void Init()
        {
            this.model = TaskModel.Instance;

            EventManager.AddHandler<int>(EventID.MakeChoice, OnMakeChoice);
            EventManager.AddHandler<string, bool>(EventID.TouchObject, OnTouchObject);
            EventManager.AddHandler<string>(EventID.TeleportTrigger, OnTeleportPos);
            EventManager.AddHandler<string>(EventID.SwitchToScene, OnSwitchScene);
            EventManager.AddHandler(EventID.FinishTalkGoal, OnFinishTalkGoal);
        }

        public void SetTaskMode(TaskMode mode)
        {
            this.mode = mode;
            Debug.Log("选择了模式：" + model);
        }

        public void AcceptTask(TaskData data)
        {
            this.model.AddTast(data);
        }

        public bool CheckTaskFinish()
        {
            if (model.cur_task.CheckFinish())
            {
                SubmitTask();

                return true;
            }

            return false;
        }

        public void SubmitTask()
        {
            this.model.FinishCurTask();
        }

        public void AutoInit()
        {
            ResetTask(InitId);
        }

        /// <summary>
        /// 重置到任务
        /// </summary>
        /// <param name="taskId"></param>
        public void ResetTask(int taskId = 0)
        {
            Debug.Log($"重置任务到{taskId}");
            var i = 0;
            var id = "";
            try
            {
                var configs = ConfigManager.Instance.GetTables<db_TaskConfig>();
                var match = taskId == 0;

                foreach (var db_task in configs)
                {
                    id = db_task.id;
                    if (!match) match = int.Parse(db_task.id) == taskId;
                    if (match)
                    {
                        var task = new TaskData();
                        task.Init(db_task);
                        AcceptTask(task);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Init TaskData:{i} taskId:{id} error: {e}");
            }
        }


        private void OnMakeChoice(int choice)
        {
            if (model.cur_task == null || model.cur_task.goals == null)
            {
                return;
            }

            for (int i = 0; i < model.cur_task.goals.Length; i++)
            {
                var goal = model.cur_task.goals[i];
                if (goal.type == EGoalType.Panel)
                {
                    var data = (panel_goal_data)goal.data;
                    string[] options = null;
                    int[] corrects = null;

                    foreach (var cmd_data in data.cmds)
                    {
                        if (cmd_data.name == ECommandUI.SetOptions.ToString())
                        {
                            options = cmd_data.args;
                        }
                        else if (cmd_data.name == ECommandUI.SetCorrect.ToString())
                        {
                            corrects = cmd_data.args.ToIntArr();
                        }
                    }

                    if (corrects != null && corrects.Length >= 1)
                    {
                        if (corrects.Contains(choice))
                        {
                            goal.state = ETaskState.Finish;
                            CheckTaskFinish();
                        }
                    }
                    else if (options != null && options.Length >= 1)
                    {
                        goal.state = ETaskState.Finish;
                        model.SelectConditionGoal(goal, options[choice]);
                        if (goal.state == ETaskState.Finish)
                        {
                            model.FinishConditionGoal(goal);
                        }

                        CheckTaskFinish();
                    }
                }
            }
        }

        private void OnSwitchScene(string sceneName)
        {
            if (model == null || model.cur_task == null || model.cur_task.goals == null)
            {
                return;
            }

            for (int i = 0; i < model.cur_task.goals.Length; i++)
            {
                var goal = model.cur_task.goals[i];
                if (goal.type == EGoalType.Teleport && ((teleport_goal_data)goal.data).sceneName == sceneName)
                {
                    goal.state = ETaskState.Finish;
                    CheckTaskFinish();
                }
            }
        }

        public void OnTeleportPos(string teleportId)
        {
            for (int i = 0; i < model.cur_task.goals.Length; i++)
            {
                var goal = model.cur_task.goals[i];
                if (goal.type == EGoalType.Teleport && ((teleport_goal_data)goal.data).id == teleportId)
                {
                    goal.state = ETaskState.Finish;
                    CheckTaskFinish();
                }
            }
        }

        private void OnTouchObject(string objId, bool touching)
        {
            for (int i = 0; i < model.cur_task.goals.Length; i++)
            {
                var goal = model.cur_task.goals[i];
                var data = goal.data as touch_goal_data;
                if (goal.type == EGoalType.Touch)
                {
                    for (int j = 0; j < goal.counters.Length; j++)
                    {
                        var counter = goal.counters[j];
                        if (counter.id == objId)
                        {
                            //非sync模式，只检测touching进入事件
                            if (data.touch_type == EGoalFinishType.Sync || touching)
                            {
                                if (counter.Count(objId, touching))
                                {
                                    //sync模式需同时完成才回调
                                    if (data.touch_type != EGoalFinishType.Sync)
                                    {
                                        var item = ObjectManager.Instance.GetSceneItem(objId);
                                        foreach (var cmd in data.cmds)
                                        {
                                            Link.SendCommand(item, cmd.name, cmd.args, !touching);
                                        }

                                        item.SetHilight(false);
                                        model.SelectConditionGoal(goal, item.id);
                                    }
                                }
                            }
                        }
                    }

                    goal.CheckFinish();
                    if (goal.state == ETaskState.Finish)
                    {
                        model.FinishConditionGoal(goal);
                    }

                    if (data.touch_type == EGoalFinishType.Sync && goal.state == ETaskState.Finish)
                    {
                        for (int j = 0; j < goal.counters.Length; j++)
                        {
                            var counter = goal.counters[j];
                            var item = ObjectManager.Instance.GetSceneItem(counter.id);
                            foreach (var cmd in data.cmds)
                            {
                                Link.SendCommand(item, cmd.name, cmd.args, !touching);
                            }

                            item.SetHilight(false);
                        }
                    }

                    CheckTaskFinish();
                }
            }
        }

        public void OnFinishTalkGoal()
        {
            if (model.cur_task == null)
            {
                return;
            }

            for (int i = 0; i < model.cur_task.goals.Length; i++)
            {
                var goal = model.cur_task.goals[i];
                if (goal.type == EGoalType.Panel)
                {
                    goal.state = ETaskState.Finish;
                    CheckTaskFinish();
                }
            }
        }

        public void PushStack(ICommand cmd)
        {
            taskStack.Push(new TaskConmand(TaskModel.Instance.cur_task.id, cmd));
        }

        public void BackTask()
        {
            TaskModel.Instance.Pause();
            SpeakerManager.Instance.StopAll();

            var cur_task = TaskModel.Instance.cur_task;
            cur_task.Reset();
            while (taskStack.Count > 0)
            {
                var tcmd = taskStack.Peek();
                if (tcmd.taskId != cur_task.id)
                {
                    break;
                }

                taskStack.Pop();
                Link.UndoCommand(tcmd);
            }

            int count = 2;
            while (taskStack.Count > 0)
            {
                TaskModel.Instance.DoLastTask();
                cur_task = TaskModel.Instance.cur_task;
                cur_task.Reset();
                while (taskStack.Count > 0)
                {
                    var tcmd = taskStack.Peek();
                    if (tcmd.taskId != cur_task.id)
                    {
                        break;
                    }

                    taskStack.Pop();
                    Link.UndoCommand(tcmd);
                }

                count--;
                if (!cur_task.config.submit && count < 0)
                {
                    break;
                }
            }

            TaskModel.Instance.Resume(true);
        }

        public void ForwardTask()
        {
            SpeakerManager.Instance.StopAll();
            TaskModel.Instance.FinishCurTask();
        }

        public override void Dispose()
        {
            model.Dispose();
            taskStack.Clear();
        }

#if UNITY_EDITOR

        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.T))
            {
                ForwardTask();
            }
        }
#endif
    }
}