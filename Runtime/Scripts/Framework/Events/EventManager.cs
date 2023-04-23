using UnityEngine;
using System;
using System.Collections.Generic;

namespace TF.Runtime
{
    public class EventManager
    {
        private static Dictionary<EventID, Delegate> mEvents = new Dictionary<EventID, Delegate>();

        private static void OnListenerAdding(EventID e, Delegate d)
        {
            if (!mEvents.ContainsKey(e))
            {
                mEvents.Add(e, null);
            }

            Delegate ed = mEvents[e];
            if (ed != null && d.GetType() != ed.GetType())
            {
                Debug.LogError(string.Format("添加事件监听错误，EventID:{0}，添加的事件{1},已存在的事件{2}", e.ToString(), d.GetType().Name,
                    ed.GetType().Name));
            }
        }

        private static void OnListenerRemoved(EventID e)
        {
            if (mEvents[e] == null)
            {
                mEvents.Remove(e);
            }
        }

        public static void AddHandler(EventID e, Callback handler)
        {
            OnListenerAdding(e, handler);
            mEvents[e] = (Callback)mEvents[e] + handler;
        }

        public static void AddHandler<T>(EventID e, Callback<T> handler)
        {
            OnListenerAdding(e, handler);
            mEvents[e] = (Callback<T>)mEvents[e] + handler;
        }

        public static void AddHandler<T, U>(EventID e, Callback<T, U> handler)
        {
            OnListenerAdding(e, handler);
            mEvents[e] = (Callback<T, U>)mEvents[e] + handler;
        }

        public static void AddHandler<T, U, V>(EventID e, Callback<T, U, V> handler)
        {
            OnListenerAdding(e, handler);
            mEvents[e] = (Callback<T, U, V>)mEvents[e] + handler;
        }

        public static void AddHandler<T, U, V, X>(EventID e, Callback<T, U, V, X> handler)
        {
            OnListenerAdding(e, handler);
            mEvents[e] = (Callback<T, U, V, X>)mEvents[e] + handler;
        }

        public static void RemoveHandler(EventID e, Callback handler)
        {
            OnListenerAdding(e, handler);
            mEvents[e] = (Callback)mEvents[e] - handler;
            OnListenerRemoved(e);
        }

        public static void RemoveHandler<T>(EventID e, Callback<T> handler)
        {
            mEvents[e] = (Callback<T>)mEvents[e] - handler;
            OnListenerRemoved(e);
        }

        public static void RemoveHandler<T, U>(EventID e, Callback<T, U> handler)
        {
            mEvents[e] = (Callback<T, U>)mEvents[e] - handler;
            OnListenerRemoved(e);
        }

        public static void RemoveHandler<T, U, V>(EventID e, Callback<T, U, V> handler)
        {
            mEvents[e] = (Callback<T, U, V>)mEvents[e] - handler;
            OnListenerRemoved(e);
        }

        public static void RemoveHandler<T, U, V, X>(EventID e, Callback<T, U, V, X> handler)
        {
            mEvents[e] = (Callback<T, U, V, X>)mEvents[e] - handler;
            OnListenerRemoved(e);
        }

        public static void FireEvent(EventID e)
        {
            Delegate d;
            if (mEvents.TryGetValue(e, out d))
            {
                Callback callback = d as Callback;

                if (callback != null)
                {
                    callback();
                }
            }
        }

        public static void FireEvent<T>(EventID e, T arg1)
        {
            Delegate d;
            if (mEvents.TryGetValue(e, out d))
            {
                Callback<T> callback = d as Callback<T>;

                if (callback != null)
                {
                    callback(arg1);
                }
            }
        }

        public static void FireEvent<T, U>(EventID e, T arg1, U arg2)
        {
            Delegate d;
            if (mEvents.TryGetValue(e, out d))
            {
                Callback<T, U> callback = d as Callback<T, U>;

                if (callback != null)
                {
                    callback(arg1, arg2);
                }
            }
        }

        public static void FireEvent<T, U, V>(EventID e, T arg1, U arg2, V arg3)
        {
            Delegate d;
            if (mEvents.TryGetValue(e, out d))
            {
                Callback<T, U, V> callback = d as Callback<T, U, V>;

                if (callback != null)
                {
                    callback(arg1, arg2, arg3);
                }
            }
        }

        public static void FireEvent<T, U, V, X>(EventID e, T arg1, U arg2, V arg3, X arg4)
        {
            Delegate d;
            if (mEvents.TryGetValue(e, out d))
            {
                Callback<T, U, V, X> callback = d as Callback<T, U, V, X>;

                if (callback != null)
                {
                    callback(arg1, arg2, arg3, arg4);
                }
            }
        }
    }
}