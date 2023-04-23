using System;
using TF.Runtime;

namespace TF.Editor
{
    public struct UICmd
    {
        public ECommandUI name;
        public string args;


        public UICmd(string value)
        {
            var arr = value.Split(':');
            Enum.TryParse(arr.TryGetValue(0), true, out name);
            this.args = arr.TryGetValue(1);
        }

        public override string ToString()
        {
            return TaskTools.ToString(name) + ":" + args;
        }
    }
}