using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public Sound[] sounds;
    public static SoundManager Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        Init();
    }

    private void Init()
    {
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            s.source.playOnAwake = s.playOnAwake;
            if(s.playOnAwake)
            {
                s.source.Play();
            }
        }
    }
    public void MusicOn()
    {
        foreach(Sound s in sounds)
        {
            if(s.source.loop)
            {
                s.source.volume = 1;
            }
        }
    }   
    public void MusicOff()
    {
        foreach(Sound s in sounds)
        {
            if(s.source.loop)
            {
                s.source.volume = 0;
            }
        }
    }
    public void SoundOn()
    {
        foreach (Sound s in sounds)
        {
            if (!s.source.loop)
            {
                s.source.volume = 1;
            }
        }
    }   
    public void SoundOff()
    {
        foreach (Sound s in sounds)
        {
            if (!s.source.loop)
            {
                s.source.volume = 0;
            }
        }
    }
    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s != null)
        {
            if (s.source.isPlaying)
            {
                s.source.Stop();
            }
            s.source.Play();
        }
        else
        {
            Debug.LogWarning($"This '{name}' Audio clip is not correct.");
        }
    }   
    public void Stop(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s != null)
        {
            if (s.source.isPlaying) s.source.Stop();
        }
        else
        {
            Debug.LogWarning($"This '{name}' Audio clip is not correct.");
        }
    }
}

[Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
    [Range(0f, 1f)]
    public float volume;
    [Range(0.1f, 3f)]
    public float pitch;
    public bool loop;
    public bool playOnAwake;
    [HideInInspector]
    public AudioSource source;
}