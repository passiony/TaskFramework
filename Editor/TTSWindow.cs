using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using TF.Runtime;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
#endif

namespace TF.Editor
{
#if ODIN_INSPECTOR
    public class TTSWindow : OdinEditorWindow
#else
    public class TTSWindow : EditorWindow
#endif
    {
        public enum VoiceType
        {
            Xiaoxiao, //晓晓
            Xiaochen, //晓晨
            Yunhao, //云皓
            Yunyang, //云杨
            Yunfeng, //云风
        }

#if ODIN_INSPECTOR
        [Searchable]
#endif
        public struct VoiceInfo
        {
#if ODIN_INSPECTOR
        [TableColumnWidth(80, false)]
#endif
             public string name;

#if ODIN_INSPECTOR
        [TableColumnWidth(80, false)]
#endif
            public VoiceType voiceType;

            [TextArea] public string content;
        }

        [MenuItem("Tools/TaskFramework/TTS Window")]
        private static void OpenWindow()
        {
            var window = GetWindow<TTSWindow>();

            var voice = new VoiceInfo();
            voice.name = "voice1001";
            voice.voiceType = VoiceType.Xiaoxiao;
            voice.content = "客户你好，我是小明，请问你是谁？";

            window.voiceList.Add(voice);
        }

#if ODIN_INSPECTOR
        [TableList(AlwaysExpanded = true)]
#endif
         public List<VoiceInfo> voiceList = new List<VoiceInfo>();

#if ODIN_INSPECTOR
        [Button(Name = "生成音频")]
#endif
        private void ToAudio()
        {
            foreach (var voiceInfo in voiceList)
            {
                var voicePath = Application.dataPath + "/" + voiceInfo.name;
                var voiceContent = SpeakerManager.ReplaceKeyword(voiceInfo.content);
                string cmdStr =
                    $"/c chcp 437&&aspeak -t \"{voiceContent}\" -v zh-CN-{voiceInfo.voiceType}Neural -S angry -o {voicePath}.mp3 --mp3 -q=3&exit";
                Cmd(cmdStr);
            }
        }

        public static void ToAudio(string voicePath, string voiceContent, VoiceType voiceType)
        {
            FileUtility.CheckDirAndCreateWhenNeeded(Path.GetDirectoryName(voicePath));
            voiceContent = SpeakerManager.ReplaceKeyword(voiceContent);
            string cmdStr =
                $"/c chcp 437&&aspeak -t \"{voiceContent}\" -v zh-CN-{voiceType}Neural -o {voicePath}.mp3 --mp3 -q=3&exit";
            Cmd(cmdStr);
        }

        private static int genTotal = 0;

        public static void GenBatch(List<TF.Editor.TaskData> taskArray)
        {
            genTotal = 0;
            foreach (var task in taskArray)
            {
                if (task.talk.enabled)
                {
                    task.talk.GenAudio();
                    genTotal++;
                }
            }
        }

        private static void Cmd(string cmdStr)
        {
            Debug.Log(cmdStr);
            Process p = new Process();
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.Arguments = cmdStr;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.CreateNoWindow = true;
            p.OutputDataReceived += (sender, e) =>
            {
                if (e.Data == "Active code page: 437")
                {
                    Debug.Log(e.Data);
                    genTotal--;
                    if (genTotal == 0)
                    {
                        Debug.LogWarning("全部生成完毕");
                        AssetDatabase.Refresh();
                    }
                }
            };
            p.ErrorDataReceived += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                {
                    Debug.LogError(e.Data);
                }
            };
            p.Start();
            p.BeginOutputReadLine();
            p.BeginErrorReadLine();
        }
    }
}