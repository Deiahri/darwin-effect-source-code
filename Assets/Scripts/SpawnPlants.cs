using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPlants : MonoBehaviour
{
    public List<GameObject> spawnItems;
    public int maxSpawns = 50;
    public int totalSpawns = 0;
    public float spawnHeight;
    // Start is called before the first frame update
    private MeshCollider myCollider;

    void Start()
    {
        myCollider = GetComponent<MeshCollider>();
        InstantPolulate();
        SpawnAfterDelay();
    }

    // Update is called once per frame
    GameObject temp;
    Vector3 position;
    Vector3 direction;
    GameObject spawnItem;
    void InstantPolulate() {
        while(totalSpawns < maxSpawns) {
            // Debug.Log("Trying");
            position = new Vector3((2*Random.value-1)*myCollider.bounds.extents.x, 10f, (2*Random.value-1)*myCollider.bounds.extents.z);
            if(Physics.Raycast(position, Vector3.down, 15f, 1 << 6) && !Physics.Raycast(position, Vector3.down, 15f, 1 << 4)) {
                spawnItem = spawnItems[(int)Mathf.Floor(Random.value*spawnItems.Count+1)-1];
                totalSpawns++;
                position.y = spawnHeight;
                temp = GameObject.Instantiate(spawnItem, position, Quaternion.Euler(-90, 0, 0));
                // temp.tag = "Food";
                temp.transform.SetParent(transform);
            }
        }
    }

    void SpawnAfterDelay() {
        StartCoroutine(spawnOne());
    }

    IEnumerator spawnOne() {
        // Debug.Log("Spawning");
        bool spawned = false;
        while(!spawned) {
            position = new Vector3((2*Random.value-1)*myCollider.bounds.extents.x, 10f, (2*Random.value-1)*myCollider.bounds.extents.z);
            if(Physics.Raycast(position, Vector3.down, 15f, 1 << 6) && !Physics.Raycast(position, Vector3.down, 15f, 1 << 4)) {
                spawnItem = spawnItems[(int)Mathf.Floor(Random.value*spawnItems.Count+1)-1];
                spawned = true;
                position.y = spawnHeight;
                temp = GameObject.Instantiate(spawnItem, position, Quaternion.Euler(-90, 0, 0));
                // temp.tag = "Food";
                temp.transform.SetParent(transform);
            }
        }
        yield return new WaitForSeconds(3f);
        SpawnAfterDelay();
    }

    private Vector2 LookAt(Vector3 target, Vector3 self) {
        float xDiff = target.x - self.x;
        float yDiff = target.y - self.y;
        float zDiff = target.z - self.z;
        float xAngle = (Mathf.Atan2(yDiff, zDiff)/Mathf.PI)*180;
        float yAngle = (Mathf.Atan2(xDiff, zDiff)/Mathf.PI)*180;
        return new Vector2(xAngle, yAngle);
    }
}
