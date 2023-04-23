namespace TF.Runtime
{
    public enum EventID : ushort
    {
        TeleportTrigger = 9,
        SwitchToScene = 10,
        PickSuccess = 12, //选择成功
        FinishTalkGoal = 13,
        MakeChoice = 14,
        FinishTask = 15,
        TouchObject = 16,

        LoadPencil = 100, //加载万用表表笔
        RedTouchBlack = 101, //万用表红黑笔相碰
        TaskJump = 102, //任务跳转
        ShowText = 103, //显示文字
    }
}