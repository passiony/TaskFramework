using System;

namespace TF.Runtime
{
    public class SceneAttribute : Attribute
    {
        public int taskId;

        public SceneAttribute() {}
    
        public SceneAttribute(int taskId)
        {
            this.taskId = taskId;
        }
    }
}
