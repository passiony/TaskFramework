using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TF.Runtime
{
    public class SelectToolPanel : BaseTalkPanel
    {
        private TextMeshProUGUI titleTxt;
        private TextMeshProUGUI contentTxt;
        private RectTransform root;
        private ScrollRect scrollView;
        private RectTransform content;
        private GameObject menuObject;

        private List<Button> menus = new List<Button>();
        private List<int> select = new List<int>();
        public event Action<int> Callback;

        public SelectToolPanel OnCallback(Action<int> callback)
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
            scrollView = this.transform.Find("ScrollView").GetComponent<ScrollRect>();
            content = this.scrollView.content;
            menuObject = this.content.Find("menu").gameObject;
            menuObject.SetActive(false);
        }

        public override BaseWindow SetOptions(params string[] args)
        {
            base.SetOptions(args);
            for (int i = 0; i < args.Length; i++)
            {
                var index = i;
                var itemId = args[i];

                var go = Instantiate(menuObject, content);
                go.SetActive(true);

                var config = ConfigManager.Instance.GetTable<db_ItemConfig>(itemId);
                var img = go.GetComponent<Image>();
                var text = go.GetComponentInChildren<TextMeshProUGUI>();
                text.text = config.name;
                img.sprite = Resources.Load<Sprite>("Sprite/Tools/" + itemId);

                menus.Add(go.GetComponent<Button>());
                menus[i].onClick.AddListener(() => { this.OnMenuClick(index); });
            }

            return this;
        }

        protected override void UpdateView()
        {
            titleTxt.text = title_str;
            contentTxt.text = content_str;

            LayoutRebuilder.ForceRebuildLayoutImmediate(root);
            LayoutRebuilder.ForceRebuildLayoutImmediate(content);

            this.Speak(taskData.id, content_str, 0, null);
        }

        private void OnMenuClick(int index)
        {
            AudioManager.Instance.PlayEffect("1002");
            Callback?.Invoke(index);

            //大于0代表有意义，否则选什么都可以
            if (!corrects.IsNullOrEmpty())
            {
                if (corrects.Contains(index))
                {
                    if (!select.Contains(index))
                    {
                        select.Add(index);
                        menus[index].transform.Find("check").gameObject.SetActive(true);
                    }

                    //全部包含
                    if (ContainsAll(select, corrects))
                    {
                        UIManager.Instance.CloseWindow(this);
                        SpeakerManager.PlaySelectSuccess(() => { EventManager.FireEvent(EventID.MakeChoice, index); });
                    }
                }
                else
                {
                    SpeakerManager.PlaySelectError();
                }
            }
        }

        private bool ContainsAll(List<int> source, int[] targets)
        {
            foreach (var id in targets)
            {
                if (!source.Contains(id))
                {
                    return false;
                }
            }

            return true;
        }

        protected override void OnHide()
        {
            base.OnHide();
            ClearMenus();
        }

        public void ClearMenus()
        {
            foreach (var menu in menus)
            {
                menu.onClick.RemoveAllListeners();
                menu.transform.Find("check").gameObject.SetActive(false);
                Destroy(menu.gameObject);
            }

            menus.Clear();
        }
    }
}