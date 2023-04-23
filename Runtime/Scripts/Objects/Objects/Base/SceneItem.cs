using System;

namespace TF.Runtime
{
    /// <summary>
    /// 世界物体基类
    /// </summary>
    public class SceneItem : SceneObject
    {
        [NonSerialized]
        public bool touching;
        [NonSerialized]
        public bool touched;

        /// <summary>
        /// 触摸进入物体
        /// </summary>
        /// <param name="sourceId">触摸源</param>
        public virtual void EnterTouch(string sourceId)
        {
            touched = true;
            touching = true;
            EventManager.FireEvent(EventID.TouchObject, id, true);
        }

        /// <summary>
        /// 触摸离开物体
        /// </summary>
        /// <param name="sourceId">触摸源</param>
        public virtual void ExitTouch(string sourceId)
        {
            touching = false;
            EventManager.FireEvent(EventID.TouchObject, id, false);
        }

        public void Hide()
        {
            this.gameObject.SetActive(false);
        }
    }
}