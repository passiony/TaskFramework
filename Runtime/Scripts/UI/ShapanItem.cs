using System;
using System.Collections;
using System.Collections.Generic;

using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace TF.Runtime
{

    public class ShapanItem : SceneItem, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        private SceneObject item;
        private GameObject prefab;
        private GameObject scaleText;

        public Action<string> OnClick;

        protected override void Awake()
        {
            base.Awake();
            item = ObjectManager.Instance.GetSceneItem(id);
            prefab = Resources.Load<GameObject>(UIManager.UIPath + "ScaleText");
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            item.SetHilight(true, 10, Color.green);

            if (scaleText == null)
            {
                scaleText = Instantiate(prefab, transform);
            }

            scaleText.SetActive(true);
            scaleText.transform.Find("Frame/Text").GetComponent<TextMeshProUGUI>().text = item.desc;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            item.SetHilight(false);
            scaleText?.SetActive(false);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            OnClick?.Invoke(id);
        }
    }
}