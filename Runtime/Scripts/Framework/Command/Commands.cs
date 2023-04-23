using System;
using System.Linq;

namespace TF.Runtime
{
    /// 无Owner 命令集合

    /// <summary>
    /// 播放声音 命令
    /// PlayAudio
    /// </summary>
    public class AudioCommand : ICommand
    {
        public EAudioType AudioType;
        public string AudioId;
        public float LoopTime;

        public AudioCommand(SceneObject obj, params string[] args)
        {
            this.Command = ECommand.PlayAudio;
            Enum.TryParse(args.TryGetValue(0), out AudioType);
            this.AudioId = args.TryGetValue(1);
            this.LoopTime = args.TryGetFloatValue(2);
        }

        public AudioCommand(EAudioType type, string id, float loop)
        {
            this.Command = ECommand.PlayAudio;
            this.AudioType = type;
            this.AudioId = id;
            this.LoopTime = loop;
        }
    }

    /// <summary>
    /// 显示飘字文本命令
    /// ShowFlyWord
    /// </summary>
    public class FlyWordCommand : ICommand
    {
        public string Content;
        public Point pointData;
        public int DelayTime;

        public FlyWordCommand(SceneObject obj, params string[] args)
        {
            this.Command = ECommand.ShowFlyWord;
            this.Content = args.TryGetValue(0);
            var ts = ObjectManager.Instance.GetPoint(args.TryGetValue(1));
            pointData = Point.Parse(ts);
            this.DelayTime = args.TryGetIntValue(2);
        }
    }


    /// <summary>
    /// 创建游戏对象 命令
    /// CreateObject
    /// </summary>
    public class CreateObjCommand : ICommand
    {
        public int ItemId;
        public Point pointData;

        public CreateObjCommand(SceneObject obj, params string[] args)
        {
            this.Command = ECommand.CreateObject;
            this.ItemId = args.TryGetIntValue(0);
            var ts = ObjectManager.Instance.GetPoint(args.TryGetValue(1));
            pointData = Point.Parse(ts);
        }
    }

    /// <summary>
    /// 场景加载
    /// ChangeScene
    /// </summary>
    public class LoadSceneCommand : ICommand
    {
        public string SceneName;

        public LoadSceneCommand(SceneObject obj, params string[] args)
        {
            this.Command = ECommand.LoadScene;
            this.SceneName = args.TryGetValue(0);
        }

        public LoadSceneCommand(string name)
        {
            this.SceneName = name;
            this.Command = ECommand.LoadScene;
        }
    }

    /// <summary>
    /// 任务跳转
    /// TaskJump
    /// </summary>
    public class TaskJumpCommand : ICommand
    {
        public string OriginId;
        public string TargetId;

        public TaskJumpCommand(SceneObject obj, params string[] args)
        {
            this.Command = ECommand.TaskJump;
            this.OriginId = TaskModel.Instance.GetCurrentId();
            this.TargetId = args.TryGetValue(0);
        }
    }

    /// <summary>
    /// 设置ProjectID
    /// </summary>
    public class ProjectIdCommand : ICommand
    {
        public int Id;

        public ProjectIdCommand(SceneObject obj, params string[] args)
        {
            this.Command = ECommand.ProjectId;
            this.Id = args.TryGetIntValue(0);
        }
    }
}