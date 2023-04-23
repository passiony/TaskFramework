
namespace TF.Runtime
{

    public enum ECommandReply : byte
    {
        Y,
        N,
    }

    public class ICommand
    {
        public ECommand Command { get; set; }
        public SceneObject Owner { get; set; }
    }
}