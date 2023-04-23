namespace TF.Runtime
{
    public struct cmd_data
    {
        public string name;
        public string[] args;

        public cmd_data(string value)
        {
            var arr = value.Split(':');
            name = arr.TryGetValue(0);
            args = arr.TryGetSplit(1);
        }
    }
}