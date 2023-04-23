using System;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace TF.Runtime
{
    public class PickItem : MonoBehaviour
    {
        private TextMeshProUGUI title;
        private Button bgBtn;
        private GameObject yes;

        private bool toogle;

        private string itemId;
        private SceneObject sceneItem;
        private Action trigger;
        private bool selected;
        private bool finished;

        void Awake()
        {
            bgBtn = transform.Find("bg").GetComponent<Button>();
            title = transform.Find("title").GetComponent<TextMeshProUGUI>();
            bgBtn.onClick.AddListener(OnClick);
            yes = transform.Find("yes").gameObject;

#if VRTK_VERSION_3_3_0_OR_NEWER
        Observable.Timer(TimeSpan.FromMilliseconds(100)).Subscribe(_ =>
            {
                gameObject.AddComponent<VRTK.VRTK_UICanvas>();
            })
            .AddTo(this);
#endif

        }

        public void Init(bool toogle, SceneObject item)
        {
            this.itemId = item.id;
            this.toogle = toogle;
            this.sceneItem = item;
            title.text = sceneItem.desc;

            UpdateView();
        }


        private void OnClick()
        {
            this.trigger?.Invoke();
            AudioManager.Instance.PlayEffect("1002");
        
            selected = !selected;
            if (selected)
            {
                EventManager.FireEvent(EventID.TouchObject, this.itemId, true);
            }
            else
            {
                EventManager.FireEvent(EventID.TouchObject, this.itemId, false);
            }

            if (!toogle)
            {
                var item = ObjectManager.Instance.GetSceneItem(this.itemId);
                item.gameObject.SetActive(false);

                //尝试穿戴
                var role = ObjectManager.Instance.GetMainRole();
                var cmd = new DressCommand(role, itemId);
                Link.SendCommand(cmd);

                Destroy(gameObject);
            }
            else
            {
                UpdateView();
            }
        }

        void UpdateView()
        {
            yes.SetActive(selected);
        }

        //设置完成状态
        public void SetState(bool state)
        {
            finished = state;
        }

        public void SetTrigger(Action callback = null)
        {
            this.trigger = callback;
        }
    }
}