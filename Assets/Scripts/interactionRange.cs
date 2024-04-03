using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class interactionRange : MonoBehaviour
{
    private List<Collider> touching = new List<Collider>();
    void Start()
    {
        // parentTransform = GetComponentInParent<Transform>();
    }

    void FixedUpdate() {}
    void OnTriggerEnter(Collider collider) {
        touching.Add(collider);
    }

    void OnTriggerExit(Collider collider) {
        touching.Remove(collider);
    }

    public bool touchingGameObjectWithName(string gameObjectName) {
        for(int index = 0; index < touching.Count; index++) {
            try {
                if(touching[index].name == gameObjectName) {
                // Debug.Log("Found it");
                return true;
                }
                else {
                    // Debug.Log("Nah!!!!");
                    // Debug.Log(touching[index].tag);
                }
            } catch {}
            
        }
        return false;
    }
}
