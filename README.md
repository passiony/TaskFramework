## TaskFramework 
一套用于流程化，剧情化的任务执行框架。可用于通用的3D，VR等项目

version:  v1.0.0

## 一.编辑器面板
任务系统的编辑器面板TaskWindow，是基于任务Excel数据在Editor下的扩展功能。开发时可以不操作Excel，直接通过编辑器修改数据，即可快速，高效的完成项目任务的配置。
注意：面板开发基于一个第三方插件Odin，需要购买才能使用。

功能介绍
* 1.加载路径：TaskConfig+projectId+taskId，如TaskConfig3-3，代表工程3-任务3
		projectId:一个工程内的不同项目，如：一级漏保，二级漏保，三级漏保
		taskId:单个项目的不同场景，使用不同的taskConfig

* 2.ExcelExport：Excel导出器，用于单独导出Excel到本地json

* 3.打开文件夹：打开excel所在目录文件夹

* 4.保存修改：保存编辑器修改的数据到Excel中，并实时导出json到工程。

* 5.加载配置：重新加载excel到编辑器面板

* 6.刷新ID：把任务数据列表的id按照自增方式刷新一下

* 7.生成音频：批量生成所有Talk对话的语音文件

## 二.任务参数

|  id  |  desc  | talk  |  submit  | delay   | condition  | goals   |
| ------------ | ------------ | ------------ | ------------ | ------------ | ------------ | ------------ |
| 任务id  | 描述   | 对话   |  自动完成  | 自动完成延迟 | 条件分支  |  目标列表  |

### id
任务id，无特殊要求可以直接自增，

### Desc
任务描述，最好简介明了的说清楚当前任务内容。

### Submit自动完成
Submit：是否自动完成
Delay：延迟完成任务
常用于设置位置，播放动画之类的无操作指令

### Talk对话
用于Talk目标的对话内容
Npc：显示对话的Npc
Title：对话的标题，为空则为Npc名字
Sentence：对话的内容，会朗读出来
生成音频：通过微软在线TTS生成对应的朗读内容，不生成则使用本地朗读

### condition条件分支
相当于if-else语句，根据用户的选择，往不同的任务分支跳转，只能与Talk，Touch搭配使用。
Permission：只读：每次都做所有条件判断，读写：剔除之前的选择，再进行条件判断
TaskIds：可跳转的任务id数组，与Talk，Touch的Option选项长度相同。
如：
```
选择A|B|C|D，
点击物体A|B|C|D
对应taskId列表2001|3001|4001|5001
```

### Goals目标
一条任务的目标数组，可以配置单个或多个目标，按照顺序执行，知道所有的Goals目标全部处于完成状态，当前Task才会完成，并执行下一个任务。

|  type  |  auto  | other  |
| ------------ | --------- | ------------ |
| 目标类型 | 自动完成 | 其他参数  |

参数{类型,自动完成,其他参数}#代表可选
#### 示例：
```c
//p2点展示对话
{Talk,0,NpcTalk,p2,,}

//高亮提示点击lock1，lock2任意对象，播放转动动画
{Touch,0,Or,True,lock1|lock2,Animate:rotate}

//高亮MeterDoor,粗细10，颜色Green
{Hilight,1,MeterDoor,10,Green}

//展示传送点tele1，触发后进入ToolsScene场景
{Teleport,0,tele1,Scene,库房,,ToolsScene}

//播放特效声音2004(滴滴声)，2s自动结束
{Audio,0,Effect,2004,2}

//在p4点展示ArrowPanel界面
{UI,0,Arrow,p4}

//对象Player移动到p2点
{Object,1,Player,MoveTo:p2}

```
#### 类型描述
|  Key | 类型 |  参数列表  | 描述  |
| ------------ | --------- | ------------ |
| Talk | 对话 | {uiname,point,#cmds}| 对话类，界面类 等UI展示  |
| Touch | 点击 | {type(or,and,sync),items[],#cmds} | 拾取，触摸，点击，射线 等类型  |
| Hilight | 高亮 | {enable,items[],width,color} | 3D物体的外轮廓高亮  |
| Teleport | 传送点 | {type[l,t,s],Id} | VR传送点，位置传送，出发，场景切换  |
| Audio | 声音 | {type[e,a,m],id,loop} | 播放背景音乐，音效等，或者指定人物说话  |
| OpenUI | 界面UI | {ui,point,type,args} | 除对话类界面外，其他UI展示  |
| Object | 对象函数 | {objects[],#cmds} | 场景Object执行特定函数命令  |


## 三.场景对象
### SceneObject：所有场景对象的基类
### SceneItem：物品类基类,父类：SceneObject
### BaseEquip：工具类物品的基类，用于触碰操作，
### Role：角色类基类
```
Npc：Npc类
VRPlayer：VR主角
PCPlayer：PC主角
```

## 四.配置表管理
配置表统一放在工程根目录的Config目录下，可以根据自己需要添加删除
当前配置说明
AudioConfig：声音配置
NpcConfig：游戏NPC配置
TaskConfig：任务流程配置
ExamConfig：考试试题配置（可选）
Game Config：游戏通用配置（可选）

扩展任务配置命名：TaskConfig+ProjectId+TaskId

## 六.场景管理
SceneManager统一管理场景跳转，每个场景必须有一个对应的Scene类，继承BaseScene
且添加[Scene(taskId)]标签，后续任务执行过程中，可以直接通过场景名字进行场景跳转。
生命周期
```
public abstract class BaseScene
{
    public int TaskId;//当前场景对应的TaskId,通过[Scene(taskId)]标签配置
    public virtual void OnEnter()//进入场景（创建物体，打开页面）
    public virtual void OnLeave()//离开场景（销毁物体，关闭页面）
```
任务配置示例：
```
//通过传送点tele1，跳转到场景LeakageScene
Teleport,0,tele1,Scene,现场,LeakageScene
```

## 七.Talk对话类
NpcTalkPanel：Npc对话面板
SysTalkPanel：系统提示面板
AlertPanel：警告面板
MenuPanel：菜单选择面板
ExamPanel：试题考试面板
SelectBoardPanel：警示牌选择
SelectToolPanel：工具选择

## 八.Command命令
项目的所有操作均使用命令模式触发，
好处：
    1.方便事件函数的统一模块化，
    2.方便任务指令的回滚，
    3.方便配置
常用指令介绍
```
public enum ECommand : byte
{
	// 通用指令，无目标
    PlayAudio,          //播放声音
    LoadScene,          //场景加载
    TaskJump,           //任务跳转
    CreateObject,       //创建对象
    ShowFlyWord,        //显示Text
    ProjectId,          //设置ProjectId

	// 场景物体指令
    PlayAnim,           //播放动画
    PlayChildAnim,      //播放动画
    SetActive,          //激活状态
    SetColor,           //设置颜色
    SetAlpha,           //设置透明度
    SetPosition,        //设置位置
    SetScale,           //设置位置
    SetState,           //设置状态
    SetCollider,        //设置碰撞
    SetChildActive,     //设置子节点激活
    Move,               //移动
    MoveTo,             //移动到
    ShowLable,          //显示文本信息
    ShowLableMark,      //显示文本+对号信息
    Hilight,            //高亮
    IsState,             //检测游戏对象状态
    LoadItem,           //加载道具
    UnLoadItem,         //卸载道具

	// 角色指令
    LoadEquip,          //手穿戴装备
    UnLoadEquip,        //卸载装备
    Talk,               //交谈
    Dress,              //穿戴上
    UnDress,            //脱掉
}
```

## 九.任务快进和回退
快进：TaskManager.ForwardTask()
回退：TaskManager.BaseTask()
框架支持快进和回退功能，但是如果要扩展命令，记得把do和undo逻辑写完。
示例：
```
	//移动命令
    private ECommandReply CheckMoveTo(MoveToCommand cmd, bool undo)
    {
        cmd.Owner.DOKill(true);
        if (undo)
        {
            cmd.Owner.transform.DOMove(cmd.Origin.Position, cmd.Duration);
            cmd.Owner.transform.DORotate(cmd.Origin.Rotation, cmd.Duration);
        }
        else
        {
            cmd.Owner.transform.DOMove(cmd.Target.Position, cmd.Duration);
            cmd.Owner.transform.DORotate(cmd.Target.Rotation, cmd.Duration);
        }

        return ECommandReply.Y;
    }
```

## 十一.UI框架
项目内所有的UI界面均继承自BaseWindow类，根据需要自行重写生命周期，然后就可以直接通过UIManager.OpenWindow的方式打开UI，注意UI的预制体和脚本名保持一致。
UI界面中所有文本显示均使用TextMeshProUGUI，保持字体和显示的一致性。
UI界面统一再UIScene下编辑，统一挂载Canvas组件，且使用Word模式，在场景中创建成功后直接设置位置即可进行正常显示
UI界面在场景中的缩放统一使用0.001f倍缩放。方便所有UI的大小统一控制
