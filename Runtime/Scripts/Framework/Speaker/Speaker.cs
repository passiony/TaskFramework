using System;
using SpeechLib;

namespace TF.Runtime
{
    public class Speaker
    {
        private SpVoice manVoice;

        private bool speaking;
        private Action complete;

        private Action Complete
        {
            get => complete;
            set { complete = value; }
        }

        public Speaker(string vName)
        {
            manVoice = new SpVoice();
            manVoice.Rate = 3;
            manVoice.Volume = 100;
            InitSex(manVoice, vName);
        }

        void InitSex(SpVoice voice, string name)
        {
            if (voice != null)
            {
                for (int i = 0; i < manVoice.GetVoices().Count; i++)
                {
                    var item = voice.GetVoices().Item(i);
                    var desc = item.GetDescription();
                    // Debug.Log(desc);
                    if (desc.Contains(name))
                    {
                        voice.Voice = item;
                        break;
                    }
                }
            }
        }

        public void Speak(string content, Action callback)
        {
            Stop(true);
            manVoice.Speak(content, SpeechVoiceSpeakFlags.SVSFlagsAsync);
            speaking = true;
            Complete = callback;
        }

        public void Update()
        {
            if (speaking)
            {
                if (manVoice.Status.RunningState == SpeechRunState.SRSEDone)
                {
                    speaking = false;
                    Trigger();
                }
            }
        }

        public void Stop(bool auto = false)
        {
            if (speaking)
            {
                if (!auto)
                    Trigger();
                speaking = false;
                manVoice.Speak(string.Empty, SpeechVoiceSpeakFlags.SVSFPurgeBeforeSpeak);
            }
        }

        public void Trigger()
        {
            Complete?.Invoke();
            Complete = null;
        }
    }
}