namespace TF.Runtime
{
    /// <summary>
    /// 计数器
    /// </summary>
    public class Counter
    {
        public string id;
        public int count;
        public int finished;

        public static Counter[] CreateArray(string[] items)
        {
            var counters = new Counter[items.Length];
            for (int i = 0; i < items.Length; i++)
            {
                counters[i] = new Counter(items[i]);
            }

            return counters;
        }

        public Counter(string _id)
        {
            id = _id;
            count = 1;
            finished = 0;
        }

        public bool Count(string _id, int num)
        {
            if (this.id == _id)
            {
                finished += num;
                return true;
            }

            return false;
        }

        public bool Count(string _id, bool value)
        {
            if (this.id == _id)
            {
                finished = value ? 1 : 0;
                return true;
            }

            return false;
        }

        public bool IsFinish()
        {
            return finished >= count;
        }
    }
}