namespace TF.Runtime
{
    /// <summary>
    /// 穿戴装备 命令
    /// LoadEquip
    /// </summary>
    public class LoadEquipCommand : ICommand
    {
        public string Item;
        public int HandIndex;

        public LoadEquipCommand(SceneObject obj, params string[] args)
        {
            this.Command = ECommand.LoadEquip;
            this.Owner = obj;
            this.Item = args.TryGetValue(0);
            this.HandIndex = args.TryGetIntValue(1);
        }

    }

    /// <summary>
    /// 卸载装备 命令
    /// UnLoadEquip
    /// </summary>
    public class UnLoadEquipCommand : ICommand
    {
        public string ItemId;

        public UnLoadEquipCommand(SceneObject obj, params string[] args)
        {
            this.Command = ECommand.UnLoadEquip;
            this.Owner = obj;
            this.ItemId = args.TryGetValue(0);
        }
    }


    /// <summary>
    /// 对话 命令
    /// Talk
    /// </summary>
    public class TalkCommand : ICommand
    {
        public TaskData Task;
        public TaskGoal Goal;

        public TalkCommand(Role owner, TaskData task, TaskGoal goal)
        {
            this.Owner = owner;
            this.Command = ECommand.Talk;
            this.Task = task;
            this.Goal = goal;
        }
    }

    /// <summary>
    /// 物体穿戴 命令
    /// Dress
    /// </summary>
    public class DressCommand : ICommand
    {
        public string ItemId;

        public DressCommand(SceneObject owner, params string[] args)
        {
            this.Command = ECommand.Dress;
            this.Owner = owner;
            this.ItemId = args.TryGetValue(0);
        }

        public DressCommand(SceneObject owner, string itemId)
        {
            this.Command = ECommand.Dress;
            this.Owner = owner;
            this.ItemId = itemId;
        }
    }

    /// <summary>
    /// 物体脱下 命令
    /// Dress
    /// </summary>
    public class UndressCommand : ICommand
    {
        public string ItemId;

        public UndressCommand(Role owner, params string[] args)
        {
            this.Command = ECommand.UnDress;
            this.Owner = owner;
            this.ItemId = args.TryGetValue(0);
        }

        public UndressCommand(Role owner, string itemId)
        {
            this.Command = ECommand.UnDress;
            this.Owner = owner;
            this.ItemId = itemId;
        }
    }
}