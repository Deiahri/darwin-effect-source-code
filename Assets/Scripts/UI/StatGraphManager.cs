using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StatGraphManager : MonoBehaviour
{
    // Start is called before the first frame update
    public RectTransform[] bars;
    public TextMeshProUGUI quantityText;
    public float maxHeight = 450f;
    public string type = "";
    DataCollector dC;
    void Start()
    {
        dC = GameObject.Find("DataCollector").GetComponent<DataCollector>();
        bars = new RectTransform[4];
        quantityText = gameObject.transform.GetChild(4).GetChild(0).GetComponent<TextMeshProUGUI>();
        for(int index = 0; index < 4; index++) {
            bars[index] = gameObject.transform.GetChild(index).GetChild(0).GetComponent<RectTransform>();
        }
        // StaticScript.addToBunnySimData(0, 0, 0, 0, 0, 0);
    }

    void OnEnable() {
        updateBars();
    }

    float getVal(int index, string type) {
        if(type == "fox") {
            return StaticScript.foxSimData[index][StaticScript.foxDataCount-1];
        } else if(type == "rabbit") {
            return StaticScript.bunnySimData[index][StaticScript.bunnyDataCount-1];
        }
        return 0f;
    }

    float timeout = 1f;
    void updateBars() {
        StartCoroutine(updateBarsWithTimeout());
    }

    IEnumerator updateBarsWithTimeout() {
        yield return new WaitForSeconds(timeout);
        float[] currentData = dC.getCurrentData(type);
        quantityText.text = ""+((int)currentData[0]);
        bars[0].SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, maxHeight*(currentData[1]/20));
        bars[1].SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, maxHeight*currentData[2]/50);
        bars[2].SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, (maxHeight*((currentData[3]-5)/50)));
        bars[3].SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, maxHeight*currentData[4]/150);
        yield return new WaitForSeconds(timeout);
        updateBars();
    }
}
