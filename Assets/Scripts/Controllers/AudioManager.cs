using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public Sound[] musicSounds, sfxSounds;
    public AudioSource masterSource, musicSource, sfxSource;

    private void Awake()
    {
        if(instance == null)
        {
        instance = this; 
        DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    
    }

    private void Start()
    {
        PlayMusic("");
    }

    public void PlayMusic(string name)
    {
        Sound s = Array.Find(musicSounds, x => x.name == name);

        if (s == null)
        {
            Debug.Log("Sound Not Found");
        }

        else
        {
            musicSource.clip = s.clip;
            musicSource.volume = musicSource.volume * masterSource.volume;
            musicSource.Play();
        }
    }

    public void PlaySFX(string name)
    {
        Sound s = Array.Find(sfxSounds, x => x.name == name);

        if (s == null)
        {
            Debug.Log("Sound Not Found");
        }

        else
        {
            sfxSource.PlayOneShot(s.clip, sfxSource.volume * masterSource.volume);
        }
    }

    public void ToggleMusic()
    {
        musicSource.mute = !musicSource.mute;
    }

    public void ToggleSFX()
    {
        sfxSource.mute= !sfxSource.mute;
    }

    public void MusicVolume(float volume)
    {
        musicSource.volume = volume * masterSource.volume;
    }

    public void SFXVolume(float volume)
    {
        sfxSource.volume = volume * masterSource.volume;
    }

    public void MasterVolume(float volume)
    {
        masterSource.volume = volume;
        musicSource.volume = musicSource.volume * masterSource.volume;
        sfxSource.volume = sfxSource.volume * masterSource.volume;
    }

    public void ToggleMaster()
    {
        masterSource.mute = !masterSource.mute;
        musicSource.mute = masterSource.mute;
        sfxSource.mute = masterSource.mute;
    }
}
