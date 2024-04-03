using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class ToggleCommunication : MonoBehaviour, IPointerDownHandler
{
    // Start is called before the first frame update
    public Toggle toggleScript;
    public string valueName;
    void Start()
    {
        toggleScript = GetComponent<Toggle>();
    }

    // Update is called once per frame
    public void OnPointerDown(PointerEventData data)
    {
        StaticScript.toggleValue(valueName, !toggleScript.isOn);
        Debug.Log("Toggling: "+!toggleScript.isOn);
    }
}
