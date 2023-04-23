using System;
using UnityEngine;
using UnityEngine.UI;

namespace TF.Runtime
{
    public class BookPanel : BaseTalkPanel
    {
        private Image image;

        private Button lastBtn;
        private Button nextBtn;
        private Button confirmBtn;

        private int index;
        private Action complete;

        protected override void InitWidget()
        {
            base.InitWidget();
            image = transform.Find("Image").GetComponent<Image>();
            lastBtn = transform.Find("lastBtn").GetComponent<Button>();
            nextBtn = transform.Find("nextBtn").GetComponent<Button>();
            confirmBtn = transform.Find("confirmBtn").GetComponent<Button>();
        }

        protected override void OnAddButtonListener()
        {
            lastBtn.onClick.AddListener(OnLastBtnClick);
            nextBtn.onClick.AddListener(OnNextBtnClick);
            confirmBtn.onClick.AddListener(OnConfirmBtnClick);
        }

        protected override void Show(string[] talks)
        {
            base.Show(talks);

            index = 0;
            complete = () => { TaskModel.Instance.FinishCurTask(); };

            UpdateView();
        }

        protected override void UpdateView()
        {
            var imgPath = "Sprite/Book/" + options[index];
            var sp = Resources.Load<Sprite>(imgPath);
            this.image.sprite = sp;

            lastBtn.gameObject.SetActive(index > 0);
            nextBtn.gameObject.SetActive(index < options.Length - 1);

            this.Speak(taskData.id, content_str, 0, null);
        }

        private void OnLastBtnClick()
        {
            index -= 1;
            index = Mathf.Clamp(index, 0, options.Length - 1);
            UpdateView();
        }

        private void OnNextBtnClick()
        {
            index += 1;
            index = Mathf.Clamp(index, 0, options.Length - 1);
            UpdateView();
        }

        private void OnConfirmBtnClick()
        {
            UIManager.Instance.CloseWindow<BookPanel>();
            complete?.Invoke();
        }
    }
}