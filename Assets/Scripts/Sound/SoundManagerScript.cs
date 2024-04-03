using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManagerScript : MonoBehaviour
{
    // Start is called before the first frame update
    public List<AudioClip> audioClips;
    List<AudioSource> audioSources = new List<AudioSource>();
    void Start()
    {
        AudioSource current;
        for(int index=0; index < audioClips.Count; index++) {
            current = gameObject.AddComponent<AudioSource>();
            current.clip = audioClips[index];
            audioSources.Add(current);
        }
    }


    public void Play(string name) {
        if(name != null) {
            foreach(AudioSource audioSource in audioSources) {
                if(audioSource.clip.name == name) {
                    audioSource.Play();
                    break;
                }
            }
        }
    }
}
