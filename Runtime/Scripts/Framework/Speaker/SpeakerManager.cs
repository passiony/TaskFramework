using System;
using System.Text.RegularExpressions;
using UnityEngine;

namespace TF.Runtime
{
    public class SpeakerManager : MonoSingleton<SpeakerManager>
    {
        private Speaker[] speakers;

        void Awake()
        {
            speakers = new[]
            {
                new Speaker("Yaoyao"), //女0
                new Speaker("Kangkang"), //男1
            };
        }

        public void Speak(string content, int sex, Action callback)
        {
            Debug.Log("Speak:" + content);
            content = ReplaceKeyword(content);

            if (sex >= speakers.Length)
            {
                Debug.LogError("sex 超出了speakers 索引");
            }

            speakers[sex].Speak(content, callback);
        }

        public static string ReplaceKeyword(string origin)
        {
            foreach (var rp in SpeakFilter.ignores)
            {
                origin = Regex.Replace(origin, rp.origin, rp.replace, RegexOptions.IgnoreCase);
            }

            foreach (var rp in SpeakFilter.replaces)
            {
                origin = Regex.Replace(origin, rp.origin, rp.replace, RegexOptions.IgnoreCase);
            }

            foreach (var rp in SpeakFilter.ignores)
            {
                origin = Regex.Replace(origin, rp.origin, rp.origin, RegexOptions.IgnoreCase);
            }

            return origin;
        }

        void Update()
        {
            speakers[0].Update();
            speakers[1].Update();
        }

        public void StopAll()
        {
            speakers[0].Stop(true);
            speakers[1].Stop(true);
        }

        public override void Dispose()
        {
        }


        public static void PlaySelectSuccess(Action callback = null)
        {
            Instance.Speak("选择正确", 0, callback);
        }

        public static void PlaySelectError(Action callback = null)
        {
            Instance.Speak("选择错误，请重新选择", 0, callback);
        }

        public static void PlaySelectError(string correct, Action callback = null)
        {
            Instance.Speak($"选择错误，正确答案是\"{correct}\"。", 0, callback);
        }
    }
}