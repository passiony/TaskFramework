using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace TF.Runtime
{
    public class ScoreManager : Singleton<ScoreManager>
    {
        private const int MaxScore = 100;
        private const int MinScore = 0;

        public int Score { get; private set; }
        public List<int> wrongList = new List<int>();

        public override void Init()
        {
            base.Init();
            Reset();
        }

        public void Reset()
        {
            Score = MaxScore;
            wrongList.Clear();
        }

        public void AddWrongQuestion(int id)
        {
            if (TaskManager.Instance.mode != TaskMode.Practice)
            {
                return;
            }
            wrongList.Add(id);
        }

        public string GetWrongTxt()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("错题记录：");
            foreach (var id in wrongList)
            {
                if (id == 0)
                {
                    sb.AppendLine($"触电死亡     -10分");
                }
                else
                {
                    var text = id % 1000;
                    sb.AppendLine($"第{text}题   -2分");
                }
            }

            return sb.ToString();
        }

        public void AddScore(int value)
        {
            if (TaskManager.Instance.mode != TaskMode.Practice)
            {
                return;
            }
            Score += value;
            Debug.Log("分数：" + value);
            Score = Mathf.Clamp(Score, MinScore, MaxScore);
        }

        public int GetScore()
        {
            return Score;
        }

        public override void Dispose()
        {
        }
    }
}