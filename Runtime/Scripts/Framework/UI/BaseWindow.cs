using System;
using UniRx;
using UnityEngine;

namespace TF.Runtime
{
    public class BaseWindow : MonoBehaviour
    {
        protected bool mVisable = false;
        protected bool mResident = false;

        protected string mResPath = string.Empty;
        public WindowType winType = WindowType.WINDOW;

        public bool HasParentWindow { get; private set; }

        protected virtual void InitWidget()
        {
        }

        protected virtual void ReleaseWidget()
        {
        }

        protected virtual void OnAddButtonListener()
        {
        }

        protected virtual void OnAddHandler()
        {
        }

        protected virtual void OnRemoveHandler()
        {
        }

        protected virtual void OnShow()
        {
        }

        protected virtual void OnHide()
        {
        }

        public bool IsVisable()
        {
            return mVisable;
        }

        public bool IsResident()
        {
            return mResident;
        }

        public void Init()
        {
            InitWidget();
            OnAddButtonListener();
        }


        public virtual BaseWindow SetOptions(params string[] args)
        {
            return this;
        }

        public virtual BaseWindow SetCorrect(params string[] args)
        {
            return this;
        }

        public virtual BaseWindow SetLife(params string[] args)
        {
            return this;
        }

        public virtual BaseWindow SetArgs(params string[] args)
        {
            return this;
        }

        public virtual BaseWindow Open(params object[] args)
        {
            return this;
        }

        public void Active()
        {
            if (transform)
            {
                transform.gameObject.SetActive(true);
                OnAddHandler();
                OnShow();
            }

            mVisable = true;
        }

        public void RefreshVRCanvas()
        {
#if VRTK_VERSION_3_3_0_OR_NEWER
        if (!gameObject.GetComponent<Canvas>())
        {
            return;
        }

        var vrtk_canvas = gameObject.GetOrAddComponent<VRTK.VRTK_UICanvas>();
        vrtk_canvas.enabled = false;

        Observable.Timer(TimeSpan.FromMilliseconds(500))
            .Subscribe(_ => { vrtk_canvas.enabled = true; }).AddTo(this);
#endif
        }

        public void Hide()
        {
            UIManager.Instance.CloseWindow(this);
        }

        public void Close()
        {
            if (transform != null)
            {
                mVisable = false;
                OnRemoveHandler();
                OnHide();
                transform.SetParent(UIManager.Instance.UIRoot, false);
                transform.gameObject.SetActive(false);
            }

            HasParentWindow = false;
        }

        public void Destroy()
        {
            ReleaseWidget();
            if (transform)
            {
                Destroy(gameObject);
            }

            mVisable = false;
        }

        public void SetParent(BaseWindow parent)
        {
            if (transform == null)
                return;
            if (parent == null || parent.transform == null)
                return;
            transform.parent = parent.transform;
            HasParentWindow = true;
        }
        
        protected void Speak(string id, string content, int sex, Action callback)
        {
            string path = string.Format("Task{0}-{1}/{2}", ConfigManager.Instance.ProjectId,
                ConfigManager.Instance.TaskId, id);
            bool playSuccess = AudioManager.Instance.PlayAudioByPath(path, callback);
            if (!playSuccess)
            {
                Debug.Log($"音频不存在：{path}");
                SpeakerManager.Instance.Speak(content, sex, callback);
            }
        }
    }
}