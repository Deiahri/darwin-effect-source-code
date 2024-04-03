using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControls : MonoBehaviour
{
    public float w = 0f, s = 0f, d = 0f, a = 0f, scroll = 0f, boost = 0f;
    public float boostMultiplier = 2f;
    public float acceleration = 0.1f;
    public float scrollSensitivity = 5f;
    public float scrollSpeed = 2f;
    public float speed = 10f;

    Vector3 horizontal = new Vector3(1f, 0f, 1f);
    Vector3 vertical = new Vector3(-1f, 0f, 1f);
    Vector3 depth = new Vector3(-1f, -1f, 1f);

    public float horizontalBound = 60f;
    public float verticalBound = 60f;
    public float heightMin = 5f;
    public float heightMax = 30f;
    // Start is called before the first frame update
    void Start()
    {
    }

    float change = 0f;
    // Update is called once per frame
    void FixedUpdate()
    {
        change = acceleration*Time.deltaTime;
        if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) {
            w = add(w, change);
        } else {
            w = normalize(w, change);
        }

        if(Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) {
            s = add(s, change);
        } else {
            s = normalize(s, change);
        }

        if(Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) {
            d = add(d, change);
        } else {
            d = normalize(d, change);
        }

        if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) {
            a = add(a, change);
        } else {
            a = normalize(a, change);
        }

        if(Mathf.Abs(Input.mouseScrollDelta.y) > 0.05f) {
            scroll = add(scroll, Input.mouseScrollDelta.y*change*scrollSensitivity);
        } else {
            scroll = normalize(scroll, change*scrollSensitivity);
        }
        
        if(Input.GetKey(KeyCode.LeftShift)) {
            boost = add(boost, change, 2f, 1f);
        } else {
            boost = add(boost, -change, 2f, 1f);
        }

        applyVelocities();
    }

    void applyVelocities() {
        if((scroll > 0 && transform.position.y-0.01f <= heightMin) || (scroll < 0 && transform.position.y+0.01f >= heightMax)) {
            scroll = 0f;
        }
        Vector3 newPos = transform.position + (boost * boostMultiplier)*(speed/10)*(vertical * (w-s) + horizontal * (d-a) + depth * scroll * scrollSpeed);

        if(newPos.x > horizontalBound) {
            newPos.x = horizontalBound;
        } else if (newPos.x < -horizontalBound) {
            newPos.x = -horizontalBound;
        }

        if(newPos.z > verticalBound) {
            newPos.z = verticalBound;
        } else if (newPos.z < -verticalBound) {
            newPos.z = -verticalBound;
        }

        if(newPos.y > heightMax) {
            newPos.y = heightMax;
        } else if (newPos.y < heightMin) {
            newPos.y = heightMin;
        }

        transform.position = newPos;
    }

    float add(float var, float amount, float max = 1f, float min = -1f, bool snapToZero = false) {
        var += amount;

        if(snapToZero) {
            if(Mathf.Abs(var) > 0 && Mathf.Abs(var) - Mathf.Abs(amount) < 0) {
                return 0f;
            }
        }
        if(var > max) {
            var = max;
        } else if (var < min) {
            var = min;
        }
        return var;
    }

    float normalize(float var, float amount) {
        if(var == 0) {

        }
        else if(var > 0) {
            var = add(var, -amount, 1f, -1f, true);
        } else {
            var = add(var, amount, 1f, -1f, true);
        }
        return var;
    }
    // void normalizeAll() {
    //     float dif = acceleration*Time.deltaTime/2;
    //     if(forward > 0) {
    //         forward = add(forward, -dif, 1f, -1f, true);
    //     } else {
    //         forward = add(forward, dif, 1f, -1f, true);
    //     }

    //     if(sideways > 0) {
    //         sideways = add(sideways, -dif, 1f, -1f, true);
    //     } else {
    //         sideways = add(sideways, dif, 1f, -1f, true);
    //     }
    // }
}
