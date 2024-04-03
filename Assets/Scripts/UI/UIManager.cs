using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    SoundManagerScript soundManager;
    public float screenChangeDelay = 0.5f;
    public GameObject ScreenParent;
    bool transitioning = false;
    
    void Start() {
        soundManager = GameObject.Find("SoundManager").GetComponent<SoundManagerScript>();
    }
    public void showScreen(string screenName, string audioName = null) {
        if(!transitioning) {
            StartCoroutine(activateScreen(screenName, audioName, screenChangeDelay));
        }
    }

    public void showScreenDelay(string screenName, string audioName = null, float delay = 0f) {
        if(!transitioning) {
            if(delay > 0) {
                StartCoroutine(activateScreen(screenName, audioName, delay));
            } else {
                findAndActivateScreen(screenName);
            }
        }
    }

    IEnumerator activateScreen(string screenName, string audioName, float delay) {
        soundManager.Play(audioName);
        transitioning = true;
        if(delay > 0f) {
            yield return new WaitForSeconds(delay);
        }
        findAndActivateScreen(screenName);
    }

    public void findAndActivateScreen(string screenName, bool activateOthers = false) {
        GameObject currentChild;
        for(int index = 0; index < ScreenParent.transform.childCount; index++) {
            currentChild = ScreenParent.transform.GetChild(index).gameObject;
            if(currentChild.name == screenName) {
                currentChild.SetActive(true);
            } else {
                currentChild.SetActive(activateOthers);
            }
        }
        transitioning = false;
    }
}
