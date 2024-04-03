using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jukebox : MonoBehaviour
{
    // Start is called before the first frame update
    public List<AudioClip> StartScreenSongs;
    public List<AudioClip> SmallSongs;
    AudioSource myAudioSource;
    string currentScene;
    public List<AudioClip> sceneSongs;
    void Start() {
        myAudioSource = GetComponent<AudioSource>();
        currentScene = GameObject.Find("GameManager").GetComponent<GameManager>().GetActiveSceneName();
        PlayToScene();
        StartCoroutine(checkForUpdate());
    }

    void PlayToScene() {
        switch (currentScene) {
            case "StartScreen":
                sceneSongs = StartScreenSongs;
                break;
            case "small":
            case "medium":
            case "large":
                sceneSongs = SmallSongs;
                break;
            default:
                Debug.Log("Could not play songs for "+currentScene+" scene");
                break;
        }
        PlayRandom(sceneSongs);
        QueueNext();
    }

    void PlayRandom(List<AudioClip> audioClips) {
        if(audioClips.Count == 0) {
            return;
        }

        int randIndex = (int)Mathf.Floor(Random.value*audioClips.Count);
        myAudioSource.clip = audioClips[randIndex];
        myAudioSource.Play();
    }
    void QueueNext() {
        StartCoroutine(TryQueueNext());
    }

    IEnumerator TryQueueNext() {
        if(!myAudioSource.isPlaying) {
            PlayRandom(sceneSongs);
        }
        yield return new WaitForSeconds(1f);
        QueueNext();
    }

    IEnumerator checkForUpdate() {
        while(true) {
            musicToggle(StaticScript.musicOn);
            yield return new WaitForSeconds(1f);
        }
    }

    void musicToggle(bool toggleValue) {
        float val = 1f;
        if(!toggleValue) {
            val = 0f;
        }
        myAudioSource.volume = val;
    }
}
