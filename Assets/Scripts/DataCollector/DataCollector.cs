using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataCollector : MonoBehaviour
{
    // Start is called before the first frame update
    float collectionDelay=10f;
    bool collecting = false;
    // public UILineRenderer graphScript;
    public void startCollecting() {
        collecting = true;
        collect();
    }

    public void stopCollecting() {
        collecting = false;
    }

    void collect() {
        if(collecting && !currentlyCollecting) {
            StartCoroutine(collectData());
        }
    }

    // float num, float speed, float sight, float love, float longitivity, float beauty
    bool currentlyCollecting = false;
    IEnumerator collectData() {
        currentlyCollecting = true;
        collectDataType("Fox");
        collectDataType("Rabbit");

        // graphScript.updateGraph(StaticScript.foxSimData[0]);

        // will try to collect again if it is allowed to collect
        yield return new WaitForSeconds(collectionDelay);
        currentlyCollecting = false;
        // Debug.Log("COLELELE");
        collect();
    }

    void collectDataType(string type) {
        float[] currentData = getCurrentData(type);
        if(type == "Rabbit") {
            StaticScript.addToBunnySimData(currentData[0], currentData[1], currentData[2], currentData[3], currentData[4], currentData[5]);
        } else if (type == "Fox") {
            StaticScript.addToFoxSimData(currentData[0], currentData[1], currentData[2], currentData[3], currentData[4], currentData[5]);
        }
        // Debug.Log("Average "+type+" Currently: "+num+", "+speed+" "+sight+" "+love+" "+longitivity+" "+beauty);
    }

    public float[] getCurrentData(string type) {
        GameObject[] maleData = GameObject.FindGameObjectsWithTag("Male"+type);
        GameObject[] femaleData = GameObject.FindGameObjectsWithTag("Female"+type);
        float num = maleData.Length + femaleData.Length;
        float speed = 0f, sight = 0f, love = 0f, longitivity = 0f, beauty = 0f;
        entityAI currentScript;
        
        foreach (GameObject current in maleData) {
            currentScript = current.GetComponent<entityAI>();
            speed += currentScript.speed;
            sight += currentScript.sight;
            love += currentScript.reproductiveThreshold;
            longitivity += currentScript.maxAge;
            beauty += currentScript.desirability;
        } foreach (GameObject current in femaleData) {
            currentScript = current.GetComponent<entityAI>();
            speed += currentScript.speed;
            sight += currentScript.sight;
            love += currentScript.reproductiveThreshold;
            longitivity += currentScript.maxAge;
            beauty += currentScript.desirability;
        }

        speed/=num;
        sight/=num;
        love = 50f-(love/num);
        longitivity/=num;
        beauty/=num;
        float[] dat = { num, speed, sight, love, longitivity, beauty };
        return dat;
    }
}
