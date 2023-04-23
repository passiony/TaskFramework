using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using System.Threading;
using Cysharp.Threading.Tasks;


namespace TF.Runtime
{
    public class TaskModel : Singleton<TaskModel>
    {
        public List<TaskData> tasks = new List<TaskData>();
        private IDisposable taskTimer;
        private IDisposable nextTimer;
        public TaskData cur_task;
        public int cur_task_index = -1;

        private bool pause = true;

        protected CancellationTokenSource cancellationTokenSource;

        public void Pause()
        {
            pause = true;
        }

        public void Resume(bool current = false)
        {
            pause = false;
            if (current)
            {
                DoCurrentTask();
            }
            else
            {
                DoNextTask();
            }
        }

        public void AddTast(TaskData task)
        {
            tasks.Add(task);
        }

        public void DeleteTask(TaskData data)
        {
            // tasks.Remove(data);
        }

        public void FinishCurTask()
        {
            if (pause)
            {
                return;
            }

            ObjectManager.Instance.GetMainRole().KillTween();
            SpeakerManager.Instance.StopAll();
            AudioManager.Instance.StopAudio();
            EventManager.FireEvent(EventID.FinishTask, cur_task.id);
            this.FinishTaskCalback(cur_task);
            this.DeleteTask(cur_task);
            if (!pause)
            {
                nextTimer?.Dispose();
                nextTimer = Observable.Timer(TimeSpan.FromMilliseconds(100)).Subscribe(_ => { this.DoNextTask(); });
            }
        }

        void FinishTaskCalback(TaskData task)
        {
            foreach (var goal in task.goals)
            {
                if (goal.type == EGoalType.Touch)
                {
                    var data = goal.data as touch_goal_data;
                    foreach (var item in data.items)
                    {
                        var obj = ObjectManager.Instance.GetSceneItem(item);
                        obj.SetHilight(false);
                        if (data.touch_type > EGoalFinishType.Or)
                        {
                            foreach (var cmd in data.cmds)
                            {
                                Link.SendCommand(obj, cmd.name, cmd.args);
                            }
                        }
                    }
                }

                if (goal.type == EGoalType.Hilight)
                {
                    var data = goal.data as hilight_goal_data;
                    foreach (var item in data.items)
                    {
                        var obj = ObjectManager.Instance.GetSceneItem(item);
                        obj.SetHilight(false);
                    }
                }
            }
        }

        public TaskData FindTask(string task_id)
        {
            for (int i = 0; i < tasks.Count; i++)
            {
                if (tasks[i].id == task_id)
                {
                    return tasks[i];
                }
            }

            return null;
        }

        public bool DoLastTask()
        {
            cur_task_index--;
            if (cur_task_index < 0)
            {
                cur_task_index = 0;
                Debug.LogWarning("人物已全部回退");
                return false;
            }

            cur_task = tasks[cur_task_index];
            DoTask(cur_task);

            return true;
        }

        public bool DoCurrentTask()
        {
            if (cur_task_index < 0)
            {
                DoNextTask();
                return false;
            }

            cur_task = tasks[cur_task_index];
            DoTask(cur_task);

            return true;
        }

        public bool DoNextTask()
        {
            cur_task_index++;
            if (cur_task_index >= tasks.Count)
            {
                cur_task_index = tasks.Count;
                Debug.LogWarning("任务已全部完成");
                return false;
            }

            cur_task = tasks[cur_task_index];
            DoTask(cur_task);

            return true;
        }

        //任务目标
        public void DoTaskGoal(TaskGoal cur_goal)
        {
            cur_goal.state = ETaskState.Running;

            //界面
            if (cur_goal.type == EGoalType.Panel)
            {
                string npcName = "System";
                if (cur_task.config.talk.Length > 0)
                {
                    npcName = cur_task.config.talk[0];
                }

                Role npc = ObjectManager.Instance.GetNPC(npcName);
                var cmd = new TalkCommand(npc, cur_task, cur_goal);
                Link.SendCommand(cmd);
            } //触摸3
            else if (cur_goal.type == EGoalType.Touch)
            {
                var data = cur_goal.data as touch_goal_data;
                data.items = cur_task.FilterItem(cur_goal.type, data.items);
                if (data.hilight)
                {
                    foreach (var item_id in data.items)
                    {
                        var obj = ObjectManager.Instance.GetSceneItem(item_id);
                        var cmd = new HilightCommand(obj, true, 5, Color.green);
                        Link.SendCommand(cmd);
                    }
                }

                cur_goal.Refresh();
            } //高亮4
            else if (cur_goal.type == EGoalType.Hilight)
            {
                var data = cur_goal.data as hilight_goal_data;

                data.items = cur_task.FilterItem(cur_goal.type, data.items);
                foreach (var item_id in data.items)
                {
                    var obj = ObjectManager.Instance.GetSceneItem(item_id);
                    var cmd = new HilightCommand(obj, data.enable, data.hilight_width, data.color);
                    Link.SendCommand(cmd);
                }
            } //传送点5
            else if (cur_goal.type == EGoalType.Teleport)
            {
                var data = cur_goal.data as teleport_goal_data;
                var item = ObjectManager.Instance.GetSceneItem<TeleportItem>(data.id);
                item.InitData(data);

                var cmd = new ActiveCommand(item, true);
                Link.SendCommand(cmd);
            } //声音6
            else if (cur_goal.type == EGoalType.Audio)
            {
                var data = cur_goal.data as audio_goal_data;
                if (data.audio_type == EAudioType.Man || data.audio_type == EAudioType.Woman)
                {
                    string path = string.Format("Task{0}-{1}/{2}", ConfigManager.Instance.ProjectId,
                        ConfigManager.Instance.TaskId, cur_task.id);
                    if (cur_task.config.talk.Length < 3) Debug.LogError("找不到语音的内容");

                    string content = cur_task.config.talk[2];
                    int sex = data.audio_type == EAudioType.Man ? 1 : 0;

                    Action complete = () =>
                    {
                        cur_goal.state = ETaskState.Finish;
                        TaskManager.Instance.CheckTaskFinish();
                    };
                    bool playSuccess = AudioManager.Instance.PlayAudioByPath(path, complete);
                    if (!playSuccess)
                    {
                        Debug.Log($"音频不存在：{path}");
                        SpeakerManager.Instance.Speak(content, sex, complete);
                    }
                }
                else
                {
                    var cmd = new AudioCommand(data.audio_type, data.audioId, data.loop);
                    AudioManager.Instance.Command(cmd);
                }
            } //ObjectFunc函数9
            else if (cur_goal.type == EGoalType.Object)
            {
                var data = cur_goal.data as object_goal_data;

                foreach (var cmd in data.cmds)
                {
                    if (cmd.name == "IsState")
                    {
                        ObjectCheckState(cur_goal, cmd.args);
                    }
                    else
                    {
                        if (data.items.Length == 0)
                        {
                            Link.SendCommand(null, cmd.name, cmd.args);
                        }

                        foreach (var objId in data.items)
                        {
                            var obj = ObjectManager.Instance.GetSceneItem(objId);
                            Link.SendCommand(obj, cmd.name, cmd.args);
                        }
                    }
                }
            }

            //自动完成goal
            if (!cur_task.config.submit && cur_goal.data.auto_finish > 0)
            {
                cur_goal.state = ETaskState.Finish;
            }

            TaskManager.Instance.CheckTaskFinish();
        }

        public void DoTask(TaskData data)
        {
            if (pause) return;

            UIManager.Instance.CloseAllWindowByType(WindowType.WINDOW);
            taskTimer?.Dispose();
            nextTimer?.Dispose();

            string task_id = data.id;
            Debug.Log($"开始任务：{task_id}");
            var goals = data.goals;

            //分支完成，跳过本次任务
            if (CheckCondition())
            {
                return;
            }
            foreach (var goal in goals)
            {
                DoTaskGoal(goal);
            }

            //自动完成
            if (cur_task.config.submit)
            {
                taskTimer = Observable.Timer(TimeSpan.FromMilliseconds(cur_task.config.delay * 1000 + 100)).Subscribe(
                    _ =>
                    {
                        Debug.Log("自动完成");
                        FinishCurTask();
                    });
            }
        }

        bool CheckCondition()
        {
            if (cur_task.HasCondition() && cur_task.condition.ReadWrite() && cur_task.condition.IsFinished())
            {
                FinishTaskCalback(cur_task);
                string result = cur_task.condition.GetDefault();
                JumpToTask(result);
                return true;
            }

            return false;
        }
        
        //完成任务
        public void SelectConditionGoal(TaskGoal goal, string id)
        {
            //写入选择项
            cur_task.condition?.SetSelect(id);
        }

        //条件节点，完成一个goal，即可跳过任务
        public void FinishConditionGoal(TaskGoal goal)
        {
            //后置条件
            if (cur_task.HasCondition())
            {
                FinishTaskCalback(cur_task);
                string result = cur_task.condition.GetSelect();
                JumpToTask(result);
            }
        }

        public string GetCurrentId()
        {
            return cur_task?.id;
        }

        public void JumpToTask(string taskId)
        {
            for (int i = 0; i < tasks.Count; i++)
            {
                tasks[i].Reset();
            }

            for (int i = 0; i < tasks.Count; i++)
            {
                if (tasks[i].id == taskId)
                {
                    cur_task_index = i;
                    cur_task = tasks[i];
                    DoTask(cur_task);
                    break;
                }
            }
        }


        async void ObjectCheckState(TaskGoal cur_goal, string[] args)
        {
            try
            {
                var data = cur_goal.data as object_goal_data;
                cancellationTokenSource = new CancellationTokenSource();
                await UniTask.WaitUntil(() =>
                {
                    bool finish = true;
                    foreach (var objId in data.items)
                    {
                        var obj = ObjectManager.Instance.GetSceneItem(objId);
                        var ret = Link.SendCommand(obj, "IsState", args);
                        if (ret == ECommandReply.N)
                        {
                            finish = false;
                        }
                    }

                    if (finish)
                    {
                        cur_goal.state = ETaskState.Finish;
                        TaskManager.Instance.CheckTaskFinish();
                    }

                    return finish;
                }, PlayerLoopTiming.Update, cancellationTokenSource.Token);
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }

        public override void Dispose()
        {
            taskTimer?.Dispose();
            nextTimer?.Dispose();
            cancellationTokenSource?.Cancel();

            tasks.Clear();
            cur_task_index = -1;
            cur_task = null;
        }
    }

}