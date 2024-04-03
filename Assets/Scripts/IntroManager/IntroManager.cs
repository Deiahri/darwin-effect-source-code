using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroManager : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject rabbit;
    public GameObject fox;
    public GameObject mainCamera;
    Vector3 FoxSpawn, RabbitSpawn;
    Vector3 FoxSpawnCamera, RabbitSpawnCamera, cameraStartPos;
    public float zoomOutAmount = 3f;
    public float cameraSnapThreshold = 0.005f;

    public GameObject activateOnComplete;

    int initialRabbits=4; int initialFoxes=2;
    void Start()
    {
        if(StaticScript.foxData != null) {
            compileGameObject(fox, StaticScript.foxData);
        }
        if(StaticScript.bunnyData != null) {
            compileGameObject(rabbit, StaticScript.bunnyData);
        }
        mainCamera = GameObject.FindWithTag("MainCamera");
        mainCamera.GetComponent<CameraControls>().enabled = false;

        FoxSpawn = GameObject.Find("FoxSpawnPoint").transform.position;
        FoxSpawnCamera = FoxSpawn - mainCamera.transform.forward*zoomOutAmount;
        RabbitSpawn = GameObject.Find("RabbitSpawnPoint").transform.position;
        RabbitSpawnCamera = RabbitSpawn - mainCamera.transform.forward*zoomOutAmount;
        try {
            initialRabbits = (int)StaticScript.bunnyData[4];
            initialFoxes = (int)StaticScript.foxData[4];
        } catch {}
        cameraStartPos = RabbitSpawnCamera - mainCamera.transform.forward*zoomOutAmount*1.5f;
    }

    void compileGameObject(GameObject r, List<float> data) {
        try {
            entityAI g = r.GetComponent<entityAI>();
            // g.enabled = false;
            g.speed = data[0]*17f + 3f;
            g.sight = data[1]*47f + 3f;
            g.reproductiveThreshold = (1-data[2])*45f + 5f;
            g.maxAge = data[3]*147f + 3f;
            g.desirability = 90f;
            // g.thirstThreshold = data[5]*70f+10f;
            g.desirability = 50f;

            // disabled movement for right now
        } catch {
            Debug.Log("Problem while fixing "+r.tag);
        }
    }
    int stage = 0;
    Vector3 diff;
    Vector3 currentCamPos;
    void FixedUpdate() {
        currentCamPos = mainCamera.transform.position;
        if(stage == 0) {
            if(distanceBetweenPoints(currentCamPos, FoxSpawnCamera) > cameraSnapThreshold) {
                diff = FoxSpawnCamera - currentCamPos;
                mainCamera.transform.position += diff/10;
            } else {
                mainCamera.transform.position = FoxSpawnCamera;
                stage = 1;
            }
        } else if (stage == 1) {
            if(!spawning) {
                spawning = true;
                StartCoroutine(spawn(fox, FoxSpawn, initialFoxes));
            }
        } else if (stage == 2) {
            if(distanceBetweenPoints(currentCamPos, RabbitSpawnCamera) > cameraSnapThreshold) {
                diff = RabbitSpawnCamera - currentCamPos;
                mainCamera.transform.position += diff/10;
            } else {
                mainCamera.transform.position = RabbitSpawnCamera;
                stage = 3;
            }
        } else if (stage == 3) {
            if(!spawning) {
                spawning = true;
                StartCoroutine(spawn(rabbit, RabbitSpawn, initialRabbits));
            }
        } else if (stage == 4) {
            if(distanceBetweenPoints(currentCamPos, cameraStartPos) > cameraSnapThreshold) {
                diff = cameraStartPos - currentCamPos;
                mainCamera.transform.position += diff/10;
            } else {
                mainCamera.transform.position = cameraStartPos;
                stage = 5;
            }
        } else {
            try {
                foreach (entityAI current in spawns) {
                    current.enabled = true;
                }
            } catch {}
            
            mainCamera.GetComponent<CameraControls>().enabled = true;
            GameObject.Find("DataCollector").GetComponent<DataCollector>().startCollecting();
            activateOnComplete.SetActive(true);
            this.enabled = false;
        }
    }

    public float spawnDelay = 0.5f;
    bool spawning = false;
    List<entityAI> spawns = new List<entityAI>();
    public float randomPositionRange = 5f;
    IEnumerator spawn(GameObject g, Vector3 pos, int quantity) {
        entityAI current;

        for(int index = 0; index < quantity; index++) {
            current = GameObject.Instantiate(assignRandomSex(g), pos + new Vector3(3f*Random.value-1.5f, 0f, 3f*Random.value-1.5f), Quaternion.Euler(0f, Random.value*360f, 0f)).GetComponent<entityAI>();
            spawns.Add(current);
            yield return new WaitForSeconds(spawnDelay);
            current.enabled = false;
        }
        yield return new WaitForSeconds(spawnDelay*3);
        spawning = false;
        stage += 1;
    }

    float distanceBetweenPoints(Vector3 pos1, Vector3 pos2) {
        return Mathf.Sqrt(Mathf.Pow(pos1.x - pos2.x, 2) + Mathf.Pow(pos1.y - pos2.y, 2) + Mathf.Pow(pos1.z - pos2.z, 2));
    }

    bool lastMale = false;
    GameObject assignRandomSex(GameObject g) {
        entityAI s = g.GetComponent<entityAI>();
        if(lastMale) {
            if(s.tag.Contains("Fox")) {
                g.tag = "FemaleFox";
            } else {
                g.tag = "FemaleRabbit";
            }
        } else {
            if(s.tag.Contains("Fox")) {
                g.tag = "MaleFox";
            } else {
                g.tag = "MaleRabbit";
            }
        }
        lastMale = !lastMale;
        return g;
    }
}
