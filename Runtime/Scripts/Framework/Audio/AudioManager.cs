using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace TF.Runtime
{
    public class AudioManager : MonoSingleton<AudioManager>
    {
        private AudioSource soundSource;
        private AudioSource audioSource;
        private Queue<AudioSource> effectSources;
        private int EffectCount = 10;
        private Dictionary<string, AudioSource> loopAudio;

        private ICommandReceiver _receiver;

        protected ICommandReceiver Receiver
        {
            get
            {
                if (_receiver == null)
                {
                    _receiver = new ICommandReceiver();
                }

                return _receiver;
            }
        }

        public ECommandReply Command<T>(T cmd, bool undo = false) where T : ICommand
        {
            return Receiver.Command(cmd, undo);
        }

        void Awake()
        {
            var sound = new GameObject("sound");
            sound.transform.SetParent(transform);
            soundSource = sound.AddComponent<AudioSource>();

            var audio = new GameObject("audio");
            audio.transform.SetParent(transform);
            audioSource = audio.AddComponent<AudioSource>();

            var effect = new GameObject("effect");
            effect.transform.SetParent(transform);
            effectSources = new Queue<AudioSource>();
            for (int i = 0; i < EffectCount; i++)
            {
                effectSources.Enqueue(audio.AddComponent<AudioSource>());
            }

            loopAudio = new Dictionary<string, AudioSource>();
            AddCommands();
        }

        protected void AddCommands()
        {
            Receiver.AddCommand<AudioCommand>(ECommand.PlayAudio, CheckPlayAudio);
        }

        private ECommandReply CheckPlayAudio(AudioCommand cmd, bool undo)
        {
            if (undo)
            {
                switch (cmd.AudioType)
                {
                    case EAudioType.Effect:
                        StopEffect(cmd.AudioId);
                        break;
                    case EAudioType.Audio:
                        StopAudio();
                        break;
                    case EAudioType.Music:
                        StopMusic();
                        break;
                    case EAudioType.Man:
                    case EAudioType.Woman:
                        break;
                }
            }
            else
            {
                TryPlay(cmd);
            }

            return ECommandReply.Y;
        }

        public void PlayMusic(string id, bool loop = true)
        {
            string path = GetPathById(id);

            AudioClip clip = Resources.Load<AudioClip>(path);
            soundSource.clip = clip;
            soundSource.loop = loop;
            soundSource.Play();
        }

        public void PlayAudio(string id, bool loop = false)
        {
            string path = GetPathById(id);

            AudioClip clip = Resources.Load<AudioClip>(path);
            audioSource.clip = clip;
            soundSource.loop = loop;
            audioSource.Play();
        }

        public void PlayEffect(string id, bool loop = false)
        {
            string path = GetPathById(id);
            var source = effectSources.Dequeue();
            AudioClip clip = Resources.Load<AudioClip>(path);
            source.clip = clip;
            source.loop = loop;
            source.Play();

            if (loop)
            {
                loopAudio.Add(id, source);
            }
            else
            {
                effectSources.Enqueue(source);
            }
        }

        public void StopMusic()
        {
            soundSource.Stop();
        }

        public void StopAudio()
        {
            audioDispose?.Dispose();
            audioSource.Stop();
        }

        public void StopEffect(string id)
        {
            if (loopAudio.TryGetValue(id, out AudioSource au))
            {
                au.Stop();
                loopAudio.Remove(id);
                effectSources.Enqueue(au);
            }
        }

        string GetPathById(string id)
        {
            var config = ConfigManager.Instance.GetTable<db_AudioConfig>(id);
            var path = "Audio/" + config.name;
            path = path.Remove(path.IndexOf('.'));
            return path;
        }

        private IDisposable audioDispose;

        public bool PlayAudioByPath(string path, Action onFinish)
        {
            StopAudio();
            AudioClip clip = Resources.Load<AudioClip>(path);
            if (clip == null)
            {
                return false;
            }

            audioSource.clip = clip;
            audioSource.loop = false;
            audioSource.Play();
            if (onFinish != null)
            {
                audioDispose = Observable.Timer(TimeSpan.FromSeconds(audioSource.clip.length)).Subscribe((_) =>
                {
                    onFinish?.Invoke();
                });
            }

            return true;
        }

        public void TryPlay(AudioCommand cmd)
        {
            switch (cmd.AudioType)
            {
                case EAudioType.Effect:
                    PlayEffect(cmd.AudioId, cmd.LoopTime > 0);
                    if (cmd.LoopTime > 0)
                    {
                        Observable.Timer(TimeSpan.FromSeconds(cmd.LoopTime)).Subscribe((_) =>
                        {
                            StopEffect(cmd.AudioId);
                        });
                    }

                    break;
                case EAudioType.Audio:
                    PlayAudio(cmd.AudioId, cmd.LoopTime > 0);
                    if (cmd.LoopTime > 0)
                    {
                        Observable.Timer(TimeSpan.FromSeconds(cmd.LoopTime))
                            .Subscribe((_) => { StopEffect(cmd.AudioId); });
                    }

                    break;
                case EAudioType.Music:
                    PlayMusic(cmd.AudioId, true);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}