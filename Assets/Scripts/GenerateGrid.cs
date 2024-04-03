using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class NewBehaviourScript : MonoBehaviour
{
    public GameObject gridItem;
    public float noiseHeight = 10f;

    public float waterProbability = 0.5f;

    public int worldSizeX = 30;
    public int worldSizeZ = 30;
    float gridOffset = 1f;

    // Start is called before the first frame update
    void Start()
    {
        for(int x = 0; x < worldSizeX; x++) {
            for (int z = 0; z < worldSizeZ; z++) {
                Vector3 pos = new Vector3(x * gridOffset, generateNoise(x, z, 12) * noiseHeight, z * gridOffset);
                GameObject block = Instantiate(gridItem, pos, Quaternion.identity) as GameObject;
                
                block.GetComponent<MeshRenderer>().material.color = generateColor(x, z);
                block.transform.SetParent(this.transform);
            }
        }
    }

    private float generateNoise(int x, int z, float detailScale) {
        float xNoise = (x + this.transform.position.x) / detailScale;
        float yNoise = (z + this.transform.position.z) / detailScale;

        return 0;
        // return Mathf.PerlinNoise(xNoise, yNoise);
    }

    public float focus = 0.1f;
    public float shift = 1f;
    private Color generateColor(int x, int z) {
        // if (Mathf.PerlinNoise(x*0.01f, z*0.01f) > waterProbability) {
        //     return Color.blue;
        // }
        // return Color.green;
        float r = Mathf.PerlinNoise((x + shift)*focus, (z + shift)*focus);
        if (r > waterProbability) {
            return Color.blue;
        }
        return Color.green;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
