using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    UIManager UIManagerScript;
    SoundManagerScript soundManager;
    public string island;
    bool transitioning = false;
    public float functionStartDelay = 0f;
    void Start()
    {
        soundManager = GameObject.Find("SoundManager").GetComponent<SoundManagerScript>();
        UIManagerScript = GameObject.Find("StartScreenManager").GetComponent<UIManager>();
    }

    public void runFunction(string functionName, string playSoundName) {
        if(!transitioning) {
            StartCoroutine(startFunction(functionName, playSoundName));
        }
    }

    IEnumerator startFunction(string functionName, string playSoundName) {
        soundManager.Play(playSoundName);
        transitioning = true;
        switch(functionName) {
            case "small_island":
                island = "small";
                break;
            case "medium_island":
                island = "medium";
                break;
            case "large_island":
                island = "large";
                break;
            case "read_bunny":
                read_bunny();
                break;
            case "start_sim":
                read_fox();
                start_sim();
                break;
            case "end_sim":
                end_sim();
                break;
            case "main_menu":
                main_menu();
                break;
        }
        yield return new WaitForSeconds(functionStartDelay);
        transitioning = false;
    }

    void read_bunny() {
        Transform sliderContainer = GameObject.Find("BunnySlidersContainer").transform;
        List<float> sliderValues = new List<float>();
        for(int childIndex = 0; childIndex < sliderContainer.childCount; childIndex ++) {
            sliderValues.Add(sliderContainer.GetChild(childIndex).gameObject.GetComponentInChildren<Slider>().value);
        }
        StaticScript.bunnyData = sliderValues;
    }
    void read_fox() {
        Transform sliderContainer = GameObject.Find("FoxSlidersContainer").transform;
        List<float> sliderValues = new List<float>();
        for(int childIndex = 0; childIndex < sliderContainer.childCount; childIndex ++) {
            sliderValues.Add(sliderContainer.GetChild(childIndex).gameObject.GetComponentInChildren<Slider>().value);
        }
        StaticScript.foxData = sliderValues;
        // Debug.Log(sliderValues);
    }

    void start_sim() {
        SceneManager.LoadScene(island);
    }

    void end_sim() {
        GameObject.Find("End Button Container").SetActive(false);
        GameObject.Find("DataCollector").GetComponent<DataCollector>().stopCollecting();
        KillAll("Rabbit");
        KillAll("Fox");
        StartCoroutine(ShowStatsScreen());
    }

    public float deleteDelay = 0.01f;
    void KillAll(string type) {
        GameObject[] maleData = GameObject.FindGameObjectsWithTag("Male"+type);
        GameObject[] femaleData = GameObject.FindGameObjectsWithTag("Female"+type);

        entityAI currentScript;
        foreach (GameObject current in maleData) {
            try {
                currentScript = current.GetComponent<entityAI>();
                currentScript.killed();
            } catch {
                continue;
            }
        } foreach (GameObject current in femaleData) {
            try {
                currentScript = current.GetComponent<entityAI>();
                currentScript.killed();
            } catch {
                continue;
            }
        }
    }

    public GameObject postNum;
    public GameObject postButtons;
    IEnumerator ShowStatsScreen() {
        yield return new WaitForSeconds(3f);
        GameObject.Find("SimEndedScreen").SetActive(false);
        postNum.SetActive(true);
        postButtons.SetActive(true);
    }

    void main_menu() {
        StaticScript.clearSimArrays();
        SceneManager.LoadScene("StartScreen");
    }

    public string GetActiveSceneName() {
        return SceneManager.GetActiveScene().name;
    }
}
