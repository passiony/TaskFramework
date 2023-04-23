using System;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace TF.Runtime
{
    public class ControlPanel : BaseWindow
    {
        private TextMeshProUGUI titleTxt;
        private Button pauseBtn;
        private Button backBtn;
        private Button nextBtn;
        private Button closeBtn;
        private Button homeBtn;
        private Button quitBtn;

        private GameObject pausePanel;
        
        private ReactiveProperty<bool> _sharedGate = new ReactiveProperty<bool>(true);
    
        protected override void InitWidget()
        {
            base.InitWidget();
            transform.localScale = Vector3.one;
            this.mResident = true;
        
            titleTxt = transform.Find("titleTxt").GetComponent<TextMeshProUGUI>();
            pauseBtn = transform.Find("pauseBtn").GetComponent<Button>();
            backBtn = transform.Find("frontBtn").GetComponent<Button>();
            nextBtn = transform.Find("nextBtn").GetComponent<Button>();

            pausePanel = transform.Find("pausePanel").gameObject;
            pausePanel.SetActive(false);
            
            closeBtn = transform.Find("pausePanel/closeBtn").GetComponent<Button>();
            homeBtn = transform.Find("pausePanel/homeBtn").GetComponent<Button>();
            quitBtn = transform.Find("pausePanel/quitBtn").GetComponent<Button>();
            
            backBtn.BindToOnClick(_sharedGate, _ =>
            {
                OnFrontClick();
                return Observable.Timer(TimeSpan.FromSeconds(0.2f)).AsUnitObservable();
            });
            nextBtn.BindToOnClick(_sharedGate, _ =>
            {
                OnNextClick();
                return Observable.Timer(TimeSpan.FromSeconds(0.2f)).AsUnitObservable();
            });
            pauseBtn.onClick.AddListener(() =>
            {
                pausePanel.SetActive(true);
            });
            closeBtn.onClick.AddListener(() =>
            {
                pausePanel.SetActive(false);
            });
            
            homeBtn.onClick.AddListener(() =>
            {
                SceneManager.Instance.EnterScene("MainScene");
            });
            quitBtn.onClick.AddListener(() =>
            {
                Application.Quit();
            });
        }

        public override BaseWindow Open(params object[] args)
        {
            titleTxt.text = args.TryGetValue(0);
            return this;
        }

        private void OnFrontClick()
        {
            TaskManager.Instance.BackTask();
        }
        private void OnNextClick()
        {
            TaskManager.Instance.ForwardTask();
        }
    }
}
