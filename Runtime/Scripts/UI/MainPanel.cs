using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TF.Runtime
{
    public class MainPanel : BaseTalkPanel
    {
        private TextMeshProUGUI titleTxt;
        private RectTransform content;
        private GameObject menuObject;
        private Button quitBtn;

        private List<Button> menus = new List<Button>();
        private TextMeshProUGUI[] buttonTxts;

        protected override void InitWidget()
        {
            base.InitWidget();
            titleTxt = this.transform.Find("title").GetComponent<TextMeshProUGUI>();
            content = this.transform.Find("menus") as RectTransform;
            menuObject = this.transform.Find("menus/menu").gameObject;
            menuObject.SetActive(false);
            quitBtn = transform.Find("quitBtn").GetComponent<Button>();
            quitBtn.onClick.AddListener(OnQuitClick);
        }

        private void OnQuitClick()
        {
            Application.Quit();
        }

        public override BaseWindow SetOptions(params string[] args)
        {
            base.SetOptions(args);
            for (int i = 0; i < args.Length; i++)
            {
                var index = i;
                var go = Instantiate(menuObject, content);
                go.SetActive(true);
                var text = go.GetComponentInChildren<TextMeshProUGUI>();
                text.text = args[i];
                menus.Add(go.GetComponent<Button>());
                menus[i].onClick.AddListener(() => { this.OnMenuClick(index); });
            }

            return this;
        }

        protected override void UpdateView()
        {
            titleTxt.text = content_str;
            this.Speak(taskData.id, content_str, 0, null);
            
            LayoutRebuilder.ForceRebuildLayoutImmediate(content);
        }

        private void OnMenuClick(int index)
        {
            AudioManager.Instance.PlayEffect("1002");

            if (corrects.IsNullOrEmpty() || corrects.Contains(index))
            {
                UIManager.Instance.CloseWindow(this);
                EventManager.FireEvent(EventID.MakeChoice, index);
            }
        }

        protected override void OnHide()
        {
            base.OnHide();
            Clear();
        }

        public void Clear()
        {
            foreach (var menu in menus)
            {
                menu.onClick.RemoveAllListeners();
                Destroy(menu.gameObject);
            }

            menus.Clear();
        }
    }
}