using UnityEngine;

namespace TF.Runtime
{
    /// <summary>
    /// 手部
    /// </summary>
    public class HandItem : BaseEquip
    {
        private bool enableCollider = true;
        private GameObject glove;
        private Collider[] fingers;

        protected override void Start()
        {
            base.Start();
            glove = transform.Find("Hand/Glove")?.gameObject;
            this.Animator = transform.Find("Hand")?.GetComponent<Animator>();
            this.fingers = transform.GetComponentsInChildren<Collider>();
            foreach (var col in fingers)
            {
                col.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
            }
            EnableCollider(enableCollider);
        }

        public void ShowGlove(bool value)
        {
            glove.SetActive(value);
        }

        public void SetCollider(string itemId, bool enable)
        {
            if (itemId == ITEM_ID.LeftHand || itemId == ITEM_ID.RightHand)
            {
                EnableCollider(enable);
            }
        }

        public void EnableCollider(bool value)
        {
            foreach (var finger in fingers)
            {
                finger.enabled = value;
            }
        }
    }
}