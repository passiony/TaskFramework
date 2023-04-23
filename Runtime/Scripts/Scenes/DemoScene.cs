using System;
using UniRx;

namespace TF.Runtime
{
    /// <summary>
    /// 示例场景
    /// </summary>
[Scene(0)]
    public class Demo : BaseScene
    {
        public override void OnEnter()
        {
            base.OnEnter();
            ScoreManager.Instance.Reset();

            Observable.Timer(TimeSpan.FromMilliseconds(100)).Subscribe(_ =>
            {
                var p2 = ObjectManager.Instance.GetPoint2("p2");
                ObjectManager.Instance.GetMainRole().SetPosition(p2.Position, p2.Rotation);

                TaskModel.Instance.Resume();
            });
        }

        public override void OnLeave()
        {

        }
    }
}