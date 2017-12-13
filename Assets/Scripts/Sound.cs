using UnityEngine.Audio;
using UnityEngine;



[System.Serializable] //allows custom class to be viewed in inspector
public class Sound
{
    
    public string name;
    public bool mute;
    public bool loop;
    public int type; 
    public AudioClip clip;
    public AudioMixerGroup output;

    [Range(0f, 5f)] //adds slider range for volume
    public float volume;
    



    [Range(.1f, 3f)] //adds slider range for pitch
    public float pitch;

    


    [HideInInspector]
    public AudioSource source;
}