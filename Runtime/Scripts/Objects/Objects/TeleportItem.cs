
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TF.Runtime
{
    /// <summary>
    /// 传送点
    /// </summary>
    public class TeleportItem : SceneItem
    {
        public teleport_goal_data data;
        public ETeleportType teleType;
        public bool locked;
        public string title;
        public Transform target;
        public string sceneName;

        private GameObject scene_icon;
        private GameObject location_icon;
        private GameObject locked_icon;
        private GameObject step_icon;
        private TextMeshProUGUI marker_text;
        private Button trigger_btn;

        // Start is called before the first frame update
        protected override void Awake()
        {
            trigger_btn = transform.Find("teleport_marker_lookat_joint/teleport_marker_canvas/triggerBtn")
                .GetComponent<Button>();
            marker_text = transform.Find("teleport_marker_lookat_joint/teleport_marker_canvas/markerTxt")
                .GetComponent<TextMeshProUGUI>();
            scene_icon = transform.Find("teleport_marker_lookat_joint/teleport_marker_icons/switch_scenes_icon")
                .gameObject;
            location_icon = transform.Find("teleport_marker_lookat_joint/teleport_marker_icons/move_location_icon")
                .gameObject;
            locked_icon = transform.Find("teleport_marker_lookat_joint/teleport_marker_icons/locked_icon")
                .gameObject;
            step_icon = transform.Find("step_icon")
                .gameObject;
        
            trigger_btn.onClick.AddListener(OnTriggerClick);
        }

        private void OnEnable()
        {
            UpdateView();
        }

        public void InitData(teleport_goal_data data)
        {
            this.data = data;
            teleType = data.teleport_type;
            if (!string.IsNullOrEmpty(data.target))
            {
                this.target = ObjectManager.Instance.GetPoint(data.target);
            }

            if (!string.IsNullOrEmpty(data.name))
            {
                this.title = data.name;
            }
            this.sceneName = data.sceneName;
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        void UpdateView()
        {
            marker_text.text = title;
            if (locked)
            {
                locked_icon.SetActive(true);
                scene_icon.SetActive(false);
                location_icon.SetActive(false);
                return;
            }

            switch (teleType)
            {
                case ETeleportType.Locate:
                case ETeleportType.Target:
                    scene_icon.SetActive(false);
                    location_icon.SetActive(true);
                    locked_icon.SetActive(false);
                    step_icon.SetActive(false);
                    break;
                case ETeleportType.Scene:
                    scene_icon.SetActive(true);
                    location_icon.SetActive(false);
                    locked_icon.SetActive(false);
                    step_icon.SetActive(false);
                    break;
                case ETeleportType.Step:
                    scene_icon.SetActive(false);
                    location_icon.SetActive(false);
                    locked_icon.SetActive(false);
                    step_icon.SetActive(true);
                    break;
            }
        }


        protected override void AddCommands()
        {
            base.AddCommands();
            this.Receiver.AddCommand<LoadSceneCommand>(ECommand.LoadScene, CheckChangeScene);
        }

        private ECommandReply CheckChangeScene(LoadSceneCommand cmd, bool undo)
        {
            SceneManager.Instance.EnterScene(cmd.SceneName);
            EventManager.FireEvent(EventID.SwitchToScene, sceneName);
            return ECommandReply.Y;
        }
    
        private void OnTriggerClick()
        {
            if (locked)
            {
                return;
            }

            gameObject.SetActive(false);
            EventManager.FireEvent(EventID.TeleportTrigger, id);

            switch (teleType)
            {
                case ETeleportType.Trigger:
                    break;
                case ETeleportType.Locate:
                {
                    var cmd = new PositionCommand(ObjectManager.Instance.GetMainRole(), transform, false);
                    Link.SendCommand(cmd);
                    break;
                }
                case ETeleportType.Target:
                {
                    var cmd = new PositionCommand(ObjectManager.Instance.GetMainRole(), target, false);
                    Link.SendCommand(cmd);
                    break;
                }
                case ETeleportType.Scene:
                {
                    var cmd = new LoadSceneCommand(sceneName);
                    Link.SendCommand(cmd);
                    break;
                }
                case ETeleportType.Step:
                {
                    var cmd = new PositionCommand(ObjectManager.Instance.GetMainRole(), transform, false);
                    Link.SendCommand(cmd);
                    if (target != null)
                    {
                        target.gameObject.SetActive(true);
                    }
                    break;
                }
            }
        }

        public void Trigger()
        {
            OnTriggerClick();
        }
    
        public override void EnterTouch(string sourceId)
        {
            base.EnterTouch(sourceId);
            Trigger();
        }
    }
}