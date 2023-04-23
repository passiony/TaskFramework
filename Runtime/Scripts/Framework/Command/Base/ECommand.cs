namespace TF.Runtime
{
    public enum ECommand : byte
    {
        None = 0,

// common
        PlayAudio, //播放声音
        LoadScene, //场景加载
        TaskJump, //任务跳转
        CreateObject, //创建对象
        ShowFlyWord, //显示Text
        ProjectId, //设置ProjectId

// items
        PlayAnim, //播放动画
        PlayChildAnim, //播放动画
        SetActive, //激活状态
        SetColor, //设置颜色
        SetAlpha, //设置透明度
        SetPosition, //设置位置
        SetScale, //设置位置
        SetState, //设置状态
        SetCollider, //设置碰撞
        SetChildActive, //设置子节点激活
        Move, //移动
        MoveTo, //移动到
        ShowLable, //显示文本信息
        ShowLableMark, //显示文本+对号信息
        Hilight, //高亮
        IsState, //检测游戏对象状态
        LoadItem, //加载道具
        UnLoadItem, //卸载道具
        SendEvent, //发送事件

// meter
        SetDisplay, //设置显示内容
        MeterButton, //仪器按钮点击
        SetID, //设置ID

// Role
        LoadEquip, //手穿戴装备
        UnLoadEquip, //卸载装备
        Talk, //交谈
        Dress, //穿戴上
        UnDress, //脱掉
    }


    public enum ECommandUI : byte
    {
        None = 0,
        SetOptions,
        SetCorrect,
        SetArgs,
        SetLife,
    }
}