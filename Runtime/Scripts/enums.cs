namespace TF.Runtime
{

    /// <summary>
    /// 任务目标类型
    /// </summary>
    public enum EGoalType
    {
        None = 0,
        Panel = 1, //对话
        Touch = 3, //触摸
        Hilight = 4, //高亮
        Teleport = 5, //传送点
        Audio = 6, //声音
        Object = 8, //物体组件函数调用
    }

    /// <summary>
    /// 任务状态
    /// </summary>
    public enum ETaskState
    {
        None,
        Running,
        Finish,
    }

    /// <summary>
    /// pickpanel的选中类型
    /// </summary>
    public enum EPickType
    {
        None, //无状态
        Check, //有选中状态，需要手动确定完成
    }

    /// <summary>
    /// 任务目标：完成类型（触摸，点击）
    /// </summary>
    public enum EGoalFinishType
    {
        Or,
        And,
        Sync,
    }

    /// <summary>
    /// 传送类型
    /// </summary>
    public enum ETeleportType
    {
        Trigger, //触发，不设置位置
        Locate, //传送点位置
        Target, //移动到目标位置
        Scene, //切换场景
        Step //脚步位置
    }


    public enum EPermission
    {
        Read, //只读
        ReadWrite //读写
    }

    /// <summary>
    /// 声音类型
    /// </summary>
    public enum EAudioType
    {
        Effect, //音效，可叠加
        Audio, //音效，不可叠加
        Music, //背景音乐
        Man, //自定义男音
        Woman, //自定义女音
    }

    /// <summary>
    /// ui的类型
    /// </summary>
    public enum ECloseType
    {
        Auto, //任务完成，自动关闭
        Manul, //需手动关闭
    }

    /// <summary>
    /// 电表状态
    /// </summary>
    public enum EMeterState
    {
        None, //正常
        Crash, //死机
        PowerOn, //开机
        PowerOff, //断电
        LeakageOn, //漏电分闸
        LeakageOff, //漏电合闸
    }

    /// <summary>
    /// 电表仪器的按钮
    /// </summary>
    public enum EButtonType
    {
        None,
        Confirm,
        Cancel,
        Up,
        Down,
        Left,
        Right,
        Setting,
    }

//开关
    public enum ESwitchState
    {
        Close, //关
        Open, //开
    }

    /// <summary>
    /// 万用表模式
    /// </summary>
    public enum EMultiMeterMode
    {
        Off,
        ACVoltage, //交流电压
        DCVoltage, //直流电压
        Resistance, //20MΩ电阻档
        OnOffGear, //通断档
        ElecCurrent, //电流档
        AC750V, //特定AC档
        AC200V
    }

    public enum ENpc
    {
        System, //系统音
        Player, //VR玩家
        Monitor, //班长
        XiaoLi, //女秘书-小李
        WangWu, //监护人-王五
        Customer //客户
    }

    public enum EPanel
    {
        Alert,
        NpcTalk,
        SysTalk,
        Menu,
        Book,
        Exam,
        SelectTool,
        SelectBoard,
        Mode,
        Main,
        End,
        Finish,
        Lable,
        Arrow,
        Failure,
    }

    public enum EColor
    {
        white = 0,
        red = 1,
        green = 2,
        blue = 3,
        black = 4,
        yellow = 5,
        cyan = 6,
        magenta = 7,
        gray = 8,
        grey = 8,
    }
}