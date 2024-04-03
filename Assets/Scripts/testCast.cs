using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testCast : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Debug.Log(Physics.Raycast(transform.position, Vector3.down, 5, 1 << 6));
    }
}
