using System;
using UniRx;

namespace TF.Runtime
{
    public static class ObservableUtil
    {
        public static void Delay(float seconds, Action callback)
        {
            Observable.Timer(TimeSpan.FromSeconds(seconds)).Subscribe(_ =>
            {
                callback?.Invoke();
            });
        }
    }
}