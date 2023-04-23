using UnityEngine;

namespace TF.Runtime
{

    /// <summary>
    /// 工具笔类物品的基类，用于触碰等操作
    /// </summary>
    public class BaseEquip : SceneItem
    {
        //加载动画
        public string loadAnim;

        //加载声音
        public string loadAudio;

        protected virtual void OnTriggerEnter(Collider other)
        {
            var item = other.GetComponent<SceneItem>();
            if (item == null)
            {
                item = other.transform.parent.GetComponent<SceneItem>();
                if (item == null)
                {
                    return;
                }
            }

            Debug.Log(id + "触碰了 物体：" + item.id);
            item.EnterTouch(this.id);
        }

        protected virtual void OnTriggerExit(Collider other)
        {
            var item = other.GetComponent<SceneItem>();
            if (item == null)
            {
                item = other.transform.parent.GetComponent<SceneItem>();
                if (item == null)
                {
                    return;
                }
            }

            item.ExitTouch(this.id);
        }
    }
}