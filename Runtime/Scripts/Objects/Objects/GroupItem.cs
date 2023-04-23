


namespace TF.Runtime
{
    /// <summary>
    /// 物品组
    /// </summary>
    public class GroupItem : SceneItem
    {
        protected override void Awake()
        {
            base.Awake();
            UIRoot = transform.Find("UIRoot");
        }

        protected override ECommandReply CheckSetLable(LableCommand command, bool undo)
        {
            ShowChildLable(!undo, false, command.FontSize);
            return ECommandReply.Y;
        }

        void ShowChildLable(bool show, bool hasToogle, int fontSize)
        {
            //标题
            var window = UIManager.Instance.OpenWindow<LablePanel>()
                .AddToPoint(UIRoot)
                .SetArgs(fontSize.ToString());
            window.Open(TaskModel.Instance.cur_task);

            //子items
            var childs = GetComponentsInChildren<SceneObject>(true);
            foreach (var child in childs)
            {
                if (child != this)
                {
                    child.gameObject.SetActive(true);
                    child.ShowLable(show, hasToogle);
                }
            }
        }
    }
}