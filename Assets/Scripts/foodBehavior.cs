using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class foodBehavior : MonoBehaviour
{
    // Start is called before the first frame update
    private Vector3 travelAmount = new Vector3(0f, 400f, 0f);
    void Start()
    {}

    // Update is called once per frame
    public void Eaten() {
        StartCoroutine(Eat());
    }

    IEnumerator Eat() {
        transform.position -= travelAmount;
        yield return new WaitForSeconds(0.05f);
        Destroy(gameObject);
    }
}
