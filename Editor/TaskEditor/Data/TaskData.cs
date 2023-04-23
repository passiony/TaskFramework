using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TF.Runtime;
using UnityEngine;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace TF.Editor
{
#if ODIN_INSPECTOR
    [InlineProperty(LabelWidth = 30)]
#endif
    [Serializable]
    public class TaskData
    {
#if ODIN_INSPECTOR
        [HorizontalGroup("Data", 55), HideLabel] [GUIColor("GetButtonColor")]
#endif
        public int id;

#if ODIN_INSPECTOR
        [FoldoutGroup("Data/$Name", false), LabelWidth(100)]
#endif
        public string desc;
#if ODIN_INSPECTOR
        [FoldoutGroup("Data/$Name", false), LabelWidth(100)]
#endif
        public bool submit;
#if ODIN_INSPECTOR
        [FoldoutGroup("Data/$Name", false), ShowIf("submit"), LabelWidth(100)]
#endif
        public float delay;
#if ODIN_INSPECTOR
        [FoldoutGroup("Data/$Name", false), Toggle("enabled")]
#endif
        public Dialogue talk = new Dialogue();
#if ODIN_INSPECTOR
        [FoldoutGroup("Data/$Name", false), Toggle("enabled")]
#endif
        public Condition condition = new Condition();
#if ODIN_INSPECTOR
        [FoldoutGroup("Data/$Name", false), OnInspectorGUI] [TypeFilter("GetFilteredTypeList")]
#endif
        public List<db_goal_data> goals = new List<db_goal_data>();

        public TaskData()
        {
            talk.OnGenAudio += () =>
            {
                var configId = TaskEditorWindow.Instance.GetConfigId();
                if (string.IsNullOrEmpty(talk.sentence))
                {
                    Debug.LogError(this.id + " 对话为空");
                }
                else
                {
                    var path = Application.dataPath + "/Resources/Task" + configId + "/" + this.id;
                    TTSWindow.ToAudio(path, talk.sentence, Npc2Type(talk.npc));
                }
            };
        }

        public IEnumerable<Type> GetFilteredTypeList()
        {
            var q = typeof(db_goal_data).Assembly.GetTypes()
                .Where(x => !x.IsAbstract)
                .Where(x => !x.IsGenericTypeDefinition)
                .Where(x => typeof(db_goal_data).IsAssignableFrom(x));
            return q;
        }

        public string GetError()
        {
            var sb = new StringBuilder();
            foreach (var goal in goals)
            {
                var error = goal.GetError(this);
                if (!string.IsNullOrEmpty(error))
                {
                    sb.Append(error);
                }
            }

            return sb.ToString();
        }

        public string Name
        {
            get { return this.desc; }
        }

        public TaskStruct ToStruct()
        {
            var task = new TaskStruct();
            task.id = this.id;
            task.desc = this.desc;
            task.submit = this.submit;
            task.delay = this.delay;

            if (this.talk.enabled)
            {
                task.talk = new[] { talk.npc.ToString(), talk.title, talk.sentence };
            }
            else
            {
                task.talk = Array.Empty<string>();
            }

            if (this.condition.enabled)
            {
                task.condition = new[] { condition.permission.ToString(), condition.taskIds };
            }
            else
            {
                task.condition = Array.Empty<string>();
            }

            var array = new List<string[]>();
            foreach (var goal in this.goals)
            {
                array.Add(goal.ToStringArray());
            }

            task.goals = array.ToArray();

            return task;
        }


        private Color GetButtonColor()
        {
            var error = GetError();
            if (string.IsNullOrEmpty(error))
            {
                return Color.white;
            }

            return Color.red;
        }

        private TTSWindow.VoiceType Npc2Type(ENpc npc)
        {
            switch (npc)
            {
                case ENpc.System:
                    return TTSWindow.VoiceType.Xiaoxiao;
                case ENpc.XiaoLi:
                    return TTSWindow.VoiceType.Xiaochen;
                case ENpc.Player:
                    return TTSWindow.VoiceType.Yunyang;
                case ENpc.Monitor:
                    return TTSWindow.VoiceType.Yunyang;
                case ENpc.WangWu:
                    return TTSWindow.VoiceType.Yunfeng;
                case ENpc.Customer:
                    return TTSWindow.VoiceType.Yunhao;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }


    [Serializable]
    public class Dialogue
    {
#if ODIN_INSPECTOR
        [LabelWidth(100)]
#endif
        public bool enabled;
#if ODIN_INSPECTOR
        [LabelWidth(100)]
#endif
        public ENpc npc;
#if ODIN_INSPECTOR
        [LabelWidth(100)]
#endif
        public string title;
#if ODIN_INSPECTOR
        [TextArea, LabelWidth(100)]
#endif
        public string sentence;

        public event Action OnGenAudio;
#if ODIN_INSPECTOR
        [Button(Name = "生成音频")]
#endif

        public void GenAudio()
        {
            OnGenAudio?.Invoke();
        }
    }

    [Serializable]
    public class Condition
    {
#if ODIN_INSPECTOR
        [LabelWidth(100)]
#endif
        public bool enabled;
#if ODIN_INSPECTOR
        [LabelWidth(100)]
#endif
        public EPermission permission;
#if ODIN_INSPECTOR
        [LabelWidth(100)]
#endif
        public string taskIds;
    }
}