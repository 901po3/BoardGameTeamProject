using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine;
using System;


public class AudioManager : MonoBehaviour
{
    
    public Sound[] sounds;
    Sound sound = new Sound();
    public AudioMixer MainMixer;
        
    //use this for initialization
    void Awake()
    {
        foreach (Sound s in sounds)
        {

            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.mute = s.mute;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            s.source.outputAudioMixerGroup = s.output;   
        }

        
    }

    private void Start ()
    {
       Play("MAIN");
    }
   

  
    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
        s.source.Play();
    }

    public void Stop(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        s.source.Stop();
    }

    public void SetMusicVol (float Volume)
    {
        MainMixer.SetFloat("MusicVol", Volume);

    }

    public void SetSFXVol(float Volume)
    {
        MainMixer.SetFloat("SFXVol", Volume);

    }
}

