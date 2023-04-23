using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TF.Runtime
{
    public class MenuPanel : BaseTalkPanel
    {
        private TextMeshProUGUI titleTxt;
        private TextMeshProUGUI contentTxt;
        private RectTransform root;
        private RectTransform content;
        private GameObject menuObject;

        private List<Button> menus = new List<Button>();
        private TextMeshProUGUI[] buttonTxts;

        public event Action<int> Callback;

        public MenuPanel OnCallback(Action<int> callback)
        {
            Callback += callback;
            return this;
        }

        protected override void InitWidget()
        {
            base.InitWidget();
            root = transform as RectTransform;
            titleTxt = this.transform.Find("title").GetComponent<TextMeshProUGUI>();
            contentTxt = this.transform.Find("content").GetComponent<TextMeshProUGUI>();
            content = this.transform.Find("bottom") as RectTransform;
            menuObject = this.transform.Find("bottom/menu").gameObject;
            menuObject.SetActive(false);
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

        public override BaseWindow SetCorrect(params string[] args)
        {
            base.SetCorrect(args);
            for (int i = 0; i < menus.Count; i++)
            {
                if (corrects.Contains(i))
                {
                    menus[i].GetComponent<Image>().color = Color.green;
                }
            }

            return this;
        }

        protected override void Show(string[] talks)
        {
            this.title_str = talks[2];
            Show(title_str);
        }

        //单选题
        public MenuPanel Show(string title)
        {
            titleTxt.text = title;
            this.Speak(taskData.id, title, 0, null);

            LayoutRebuilder.ForceRebuildLayoutImmediate(root);
            LayoutRebuilder.ForceRebuildLayoutImmediate(content);

            return this;
        }

        private void OnMenuClick(int index)
        {
            AudioManager.Instance.PlayEffect("1002");
            Callback?.Invoke(index);
            if (corrects.IsNullOrEmpty())
            {
                UIManager.Instance.CloseWindow(this);
                EventManager.FireEvent(EventID.MakeChoice, index);
                return;
            }
            
            if ( corrects.Contains(index))
            {
                UIManager.Instance.CloseWindow(this);
                SpeakerManager.PlaySelectSuccess(() => { EventManager.FireEvent(EventID.MakeChoice, index);});
            }
            else
            {
                SpeakerManager.PlaySelectError();
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