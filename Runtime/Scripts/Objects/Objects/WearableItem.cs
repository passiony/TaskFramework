

namespace TF.Runtime
{
    /// <summary>
    /// 可穿戴设备，如:手套
    /// </summary>
    public class WearableItem : SceneItem
    {
        protected override void AddCommands()
        {
            base.AddCommands();
            this.Receiver.AddCommand<DressCommand>(ECommand.Dress, CheckDress);
        }

        private ECommandReply CheckDress(DressCommand command, bool undo)
        {
            var role = ObjectManager.Instance.GetMainRole();
            command.Owner = role;
            command.ItemId = id;
            Link.SendCommand(command, undo);

            return ECommandReply.Y;
        }

        /// <summary>
        /// 触摸离开物体
        /// </summary>
        /// <param name="sourceId">触摸源</param>
        public override void EnterTouch(string sourceId)
        {
            base.EnterTouch(sourceId);

            //尝试穿戴
            var role = ObjectManager.Instance.GetMainRole();
            var cmd = new DressCommand(role, id);
            Link.SendCommand(cmd);
        }
    }
}