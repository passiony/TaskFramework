using TMPro;
using UnityEngine.Events;
using UnityEngine.UI;

namespace TF.Runtime
{
    public class TutorialPanel : BaseWindow
    {
        private TextMeshProUGUI titleTxt;
        private TextMeshProUGUI contentTxt;
        private Button btn1;
        private Button btn2;
        private TextMeshProUGUI btn1Txt;
        private TextMeshProUGUI btn2Txt;

        protected override void InitWidget()
        {
            winType = WindowType.SYSTEM;
        
            base.InitWidget();
            titleTxt = transform.Find("title").GetComponent<TextMeshProUGUI>();
            contentTxt = transform.Find("content/Text").GetComponent<TextMeshProUGUI>();
            btn1 = transform.Find("bottom/btn1").GetComponent<Button>();
            btn2 = transform.Find("bottom/btn2").GetComponent<Button>();
            btn1Txt = btn1.GetComponentInChildren<TextMeshProUGUI>();
            btn2Txt = btn2.GetComponentInChildren<TextMeshProUGUI>();
        }

        public void Show(string title, string content, string txt1,string txt2,UnityAction btn1Handler,UnityAction btn2Handler)
        {
            this.titleTxt.text = title;
            this.contentTxt.text = content;
            this.btn1Txt.text = txt1;
            this.btn2Txt.text = txt2;
            btn1.onClick.AddListener(btn1Handler);
            btn2.onClick.AddListener(btn2Handler);
        }

    }
}
