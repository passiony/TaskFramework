using System;

namespace TF.Editor
{
    public enum ECommand : byte
    {
        None = 0,
// common
        Audio, //播放声音
        LoadScene, //场景加载
        TaskJump, //任务跳转
        CreateObj, //创建对象
        FlyWord, //显示Text
        ProjectId, //设置ProjectId

// items
        Animate, //播放动画
        Active, //激活状态
        ActiveChild, //设置子节点激活
        Color, //设置颜色
        Alpha, //设置透明度
        Position, //设置位置
        Scale, //设置位置
        State, //设置状态
        Collider, //设置碰撞
        ChildAnim, //播放动画
        Move, //移动
        MoveTo, //移动到
        Lable, //显示文本信息
        LoadItem, //加载道具
        UnLoadItem, //卸载道具
        Event, //发送事件
        IsState, //检测游戏对象状态

// meter
        Display, //设置显示内容
        Button, //仪器按钮点击
        ID, //设置ID

// Role
        LoadEquip, //手穿戴装备
        UnLoadEquip, //卸载装备
        Dress, //穿戴上
        UnDress, //脱掉
    }
    
    public struct ObjectCmd
    {
        public ECommand name;
        public string args;


        public ObjectCmd(string value)
        {
            var arr = value.Split(':');
            Enum.TryParse(arr.TryGetValue(0), true, out name);
            this.args = arr.TryGetValue(1);
        }

        public override string ToString()
        {
            return TaskTools.ToString(name) + ":" + args;
        }
    }
}