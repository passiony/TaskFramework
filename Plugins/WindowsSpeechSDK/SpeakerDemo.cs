using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpeechLib;
using UnityEngine.UI;

public class SpeakerDemo : MonoBehaviour
{
    private SpVoice voice;
    public SpeechRunState voidState;
    public string content;

    void Awake()
    {
        voice = new SpVoice();
        voice.Rate = 2;
        voice.Volume = 100;
        for (int i = 0; i < voice.GetVoices().Count; i++)
        {
            Debug.Log(voice.GetVoices().Item(i).GetDescription());
        }

        voice.Voice = voice.GetVoices().Item(0);
    }

    void Start()
    {
        Speak(content);
    }

    private void Update()
    {
        voidState = voice.Status.RunningState;
    }

    public void Speak(string str)
    {
        this.content = str;
        voice.Speak(str, SpeechVoiceSpeakFlags.SVSFlagsAsync);
    }

    public string GetSpeakedStr()
    {
        return content.Substring(0, Mathf.Min(voice.Status.InputWordPosition + 2, content.Length));
    }
}