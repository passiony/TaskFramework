
using UnityEngine;

namespace TF.Runtime
{
    /// <summary>
    /// 播放动画 命令
    /// PlayAnim
    /// </summary>
    public class AnimateCommand : ICommand
    {
        public string OriginAnimName;
        public string TargetAnimName;
        public bool rootMotion;

        public AnimateCommand(SceneObject obj, params string[] args)
        {
            this.Command = ECommand.PlayAnim;
            this.Owner = obj;
            this.OriginAnimName = obj.GetAnimName();
            this.TargetAnimName = args.TryGetValue(0);
            this.rootMotion = args.TryGetBoolValue(1);
        }
    }

    /// <summary>
    /// 激活命令
    /// SetActive
    /// </summary>
    public class ActiveCommand : ICommand
    {
        public bool Enable;

        public ActiveCommand(SceneObject obj, params string[] args)
        {
            this.Command = ECommand.SetActive;
            this.Owner = obj;
            this.Enable = bool.Parse(args[0]);
        }

        public ActiveCommand(SceneObject obj, bool active)
        {
            this.Command = ECommand.SetActive;
            this.Owner = obj;
            this.Enable = active;
        }
    }

    /// <summary>
    /// 激活子节点命令
    /// SetChildActive
    /// </summary>
    public class ActiveChildCommand : ICommand
    {
        public string ChildName;
        public bool Enable;

        public ActiveChildCommand(SceneItem obj, params string[] args)
        {
            this.Command = ECommand.SetChildActive;
            this.Owner = obj;
            this.ChildName = args.TryGetValue(0);
            this.Enable = args.TryGetBoolValue(1);
        }
    }


    /// <summary>
    /// 播放手动画 命令
    /// PlayHandAnim
    /// </summary>
    public class ChildAnimCommand : ICommand
    {
        public string ChildId;
        public string OriginAnimName;
        public string TargetAnimName;

        public ChildAnimCommand(SceneObject obj, params string[] args)
        {
            this.Command = ECommand.PlayChildAnim;
            this.Owner = obj;
            this.ChildId = args[0];
            this.OriginAnimName = obj.GetChildAnim(ChildId);
            this.TargetAnimName = args[1];
        }
    }

    /// <summary>
    /// 设置状态
    /// SetState
    /// </summary>
    public class StateCommand : ICommand
    {
        public string OriginState;
        public string TargetState;

        public StateCommand(SceneObject obj, params string[] args)
        {
            this.Command = ECommand.SetState;
            this.Owner = obj;
            this.OriginState = obj.GetState();
            this.TargetState = args[0];
        }
    }


    /// <summary>
    /// 高亮 命令
    /// Hilight
    /// </summary>
    public class HilightCommand : ICommand
    {
        public bool Enable;
        public int Width;
        public Color Color;

        public HilightCommand(SceneObject obj, params string[] args)
        {
            this.Command = ECommand.Hilight;
            this.Owner = obj;
            this.Enable = bool.Parse(args[0]);
            this.Width = args.TryGetIntValue(1);
            this.Color = args.TryGetValue(2).ToColor();
        }

        public HilightCommand(SceneObject obj, bool enable, int width, Color color)
        {
            this.Command = ECommand.Hilight;
            this.Owner = obj;
            this.Enable = enable;
            this.Width = width;
            this.Color = color;
        }
    }


    /// <summary>
    /// 设置位置 命令
    /// SetPosition
    /// </summary>
    public class PositionCommand : ICommand
    {
        public Point Origin;
        public Point Target;
        public bool Rotate;

        public PositionCommand(SceneObject obj, params string[] args)
        {
            this.Command = ECommand.SetPosition;
            this.Owner = obj;
            this.Origin = Point.Parse(obj.transform);
            var point = ObjectManager.Instance.GetPoint(args.TryGetValue(0));
            this.Target = Point.Parse(point);
            this.Rotate = args.TryGetBoolValue(1);
        }

        public PositionCommand(SceneObject obj, Transform point, bool rotate = false)
        {
            this.Command = ECommand.SetPosition;
            this.Owner = obj;
            this.Origin = Point.Parse(obj.transform);
            this.Target = Point.Parse(point);
            this.Rotate = rotate;
        }
    }

    /// <summary>
    /// 设置缩放 命令
    /// SetScale
    /// </summary>
    public class ScaleCommand : ICommand
    {
        public Vector3 Origin;
        public Vector3 Target;

        public ScaleCommand(SceneObject obj, params string[] args)
        {
            this.Command = ECommand.SetScale;
            this.Owner = obj;
            this.Origin = obj.transform.localScale;
            this.Target = args.TryGetValue(0).SplitToVector3('/');
        }
    }

    /// <summary>
    /// 运动命令
    /// </summary>
    public class MoveCommand : ICommand
    {
        public Vector3 Origin;
        public float Duration;
        public Vector3 Velocity;

        public MoveCommand(SceneObject obj, params string[] args)
        {
            this.Command = ECommand.Move;
            this.Owner = obj;
            this.Origin = obj.transform.localScale;
            this.Duration = args.TryGetFloatValue(0);
            this.Velocity = args.TryGetValue(1).SplitToVector3('/');
        }
    }

    /// <summary>
    /// 检测Obj的状态
    /// IsState
    /// </summary>
    public class IsStateCommand : ICommand
    {
        public int State;

        public IsStateCommand(SceneObject owner, params string[] args)
        {
            this.Command = ECommand.IsState;
            this.Owner = owner;
            this.State = args.TryGetIntValue(0);
        }
    }

    /// <summary>
    /// 设置透明度
    /// SetColor
    /// </summary>
    public class AlphaCommand : ICommand
    {
        public float OriginAlpha;
        public float TargetAlpha;

        public AlphaCommand(SceneObject owner, params string[] args)
        {
            this.Command = ECommand.SetAlpha;
            this.Owner = owner;
            this.OriginAlpha = owner.GetAlpha();
            this.TargetAlpha = args.TryGetFloatValue(0);
        }
    }

    /// <summary>
    /// 设置颜色
    /// SetColor
    /// </summary>
    public class ColorCommand : ICommand
    {
        public Color OriginColor;
        public Color TargetColor;

        public ColorCommand(SceneObject owner, params string[] args)
        {
            this.Command = ECommand.SetColor;
            this.Owner = owner;
            this.OriginColor = owner.GetColor();
            this.TargetColor = new Color()
            {
                r = args.TryGetIntValue(0),
                g = args.TryGetIntValue(1),
                b = args.TryGetIntValue(2),
                a = args.TryGetIntValue(3),
            };
        }
    }

    /// <summary>
    /// 设置碰撞命令
    /// </summary>
    public class ColliderCommand : ICommand
    {
        public bool OriginEnable;
        public bool TargetEnable;

        public ColliderCommand(SceneObject obj, params string[] args)
        {
            this.Command = ECommand.SetCollider;
            this.Owner = obj;
            this.OriginEnable = obj.gameObject.GetOrAddComponent<Collider>();
            this.TargetEnable = args.TryGetBoolValue(0);
        }
    }


    /// <summary>
    /// 加载道具 命令
    /// LoadItem
    /// </summary>
    public class LoadItemCommand : ICommand
    {
        public string[] Items;

        public LoadItemCommand(SceneObject obj, params string[] args)
        {
            this.Command = ECommand.LoadItem;
            this.Owner = obj;
            this.Items = args;
        }
    }

    /// <summary>
    /// 卸载道具 命令
    /// UnLoadItem
    /// </summary>
    public class UnLoadItemCommand : ICommand
    {
        public string[] Items;

        public UnLoadItemCommand(SceneObject obj, params string[] args)
        {
            this.Command = ECommand.UnLoadItem;
            this.Owner = obj;
            this.Items = args;
        }
    }


    /// <summary>
    /// 发送事件
    /// SendEvent
    /// </summary>
    public class EventCommand : ICommand
    {
        public string EventId;
        public string Value;

        public EventCommand(SceneItem owner, params string[] args)
        {
            this.Command = ECommand.SendEvent;
            this.Owner = owner;
            this.EventId = args.TryGetValue(0);
            this.Value = args.TryGetValue(1);
        }
    }

    /// <summary>
    /// 显示文本命令
    /// ShowLable
    /// </summary>
    public class LableCommand : ICommand
    {
        public bool OriginActive;
        public int FontSize;

        public LableCommand(SceneItem owner, params string[] args)
        {
            this.Command = ECommand.ShowLable;
            this.Owner = owner;
            this.OriginActive = owner.gameObject.activeSelf;
            this.FontSize = args.TryGetIntValue(0);
        }
    }

    /// <summary>
    /// 显示文本+对号命令
    /// ShowLable2
    /// </summary>
    public class LableMarkCommand : ICommand
    {
        public bool OriginActive;
        public int FontSize;

        public LableMarkCommand(SceneItem owner, params string[] args)
        {
            this.Command = ECommand.ShowLableMark;
            this.Owner = owner;
            this.OriginActive = owner.gameObject.activeSelf;
            this.FontSize = args.TryGetIntValue(0);
        }
    }

    /// <summary>
    /// 移动到
    /// </summary>
    public class MoveToCommand : ICommand
    {
        public Point Origin;
        public Point Target;
        public float Duration;

        public MoveToCommand(SceneObject obj, params string[] args)
        {
            this.Command = ECommand.MoveTo;
            this.Owner = obj;
            this.Origin = Point.Parse(obj.transform);
            this.Target = Point.Parse(ObjectManager.Instance.GetPoint(args.TryGetValue(0)));
            this.Duration = args.TryGetFloatValue(1);
            if (this.Duration == 0) //不配置默认给个1
            {
                this.Duration = 1;
            }
        }
    }
}
