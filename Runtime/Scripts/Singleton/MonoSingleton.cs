using UnityEngine;

namespace TF.Runtime
{
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
    {
        private static T mInstance = null;

        public static T Instance
        {
            get
            {
                if (mInstance == null)
                {
                    mInstance = GameObject.FindObjectOfType(typeof(T)) as T;
                    if (mInstance == null)
                    {
                        GameObject go = new GameObject(typeof(T).Name);
                        mInstance = go.AddComponent<T>();
                        GameObject parent = GameObject.Find("Boot");
                        if (parent == null)
                        {
                            parent = new GameObject("Boot");
                            GameObject.DontDestroyOnLoad(parent);
                        }

                        if (parent != null)
                        {
                            go.transform.parent = parent.transform;
                        }
                    }
                }

                return mInstance;
            }
        }

        /*
         * 没有任何实现的函数，用于保证MonoSingleton在使用前已创建
         */
        public void StartUp()
        {

        }

        private void Awake()
        {
            if (mInstance == null)
            {
                mInstance = this as T;
                DontDestroyOnLoad(gameObject);
                Init();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        protected virtual void Init()
        {

        }

        public void DestroySelf()
        {
            Destroy(mInstance);
            mInstance = null;
        }

        public virtual void Dispose()
        {

        }
    }
}