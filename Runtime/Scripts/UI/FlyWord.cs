using TMPro;
using UnityEngine;

namespace TF.Runtime
{
    /// <summary>
    /// 飘字
    /// </summary>
    public class FlyWord : MonoBehaviour
    {
        public TextMeshProUGUI content;

        void Awake()
        {
            content = transform.Find("content").GetComponent<TextMeshProUGUI>();
        }

        public void ShowText(string str, float delay = 0.5f)
        {
            content.text = str;
            SpeakerManager.Instance.Speak(str,0, () =>
            {
                if (gameObject != null)
                {
                    Destroy(gameObject, delay);
                }
            });
        }
    }
}