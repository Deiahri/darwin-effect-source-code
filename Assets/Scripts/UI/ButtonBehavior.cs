using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonBehavior : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    // Start is called before the first frame update
    Image myImage;
    public float brightnessChange = 0f;
    Color initialColor;
    public bool changeScreens = true;
    public string ScreenName;

    public bool runFunction = false;
    public string functionName;

    public string playSoundName;
    AudioSource audioSource;
    void Start() {
        myImage = GetComponent<Image>();
        initialColor = myImage.color;
    }
    public void OnPointerEnter(PointerEventData eventData) {
        Color c = initialColor;
        c.a += 0.3f;
        myImage.color = c;
    }
    public void OnPointerExit(PointerEventData eventData) {
        myImage.color = initialColor;
    }

    public void OnPointerDown(PointerEventData eventData) {
        if(changeScreens) {
            try {
                UIManager script = GameObject.Find("StartScreenManager").GetComponent<UIManager>();
                script.showScreen(ScreenName, playSoundName);
            } catch {
                Debug.Log("Could not locate screen manager");
            }
        }
        if (runFunction) {
            try {
                GameManager script = GameObject.Find("GameManager").GetComponent<GameManager>();
                script.runFunction(functionName, playSoundName);
            } catch {
                Debug.Log("Could not locate game manager");
            }
        } 
            
    }
}
