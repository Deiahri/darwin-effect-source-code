using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class entitySense : MonoBehaviour
{
    // Start is called before the first frame update
    
    // memory of what rabbit has seen
    private List<Collider> spotted = new List<Collider>();
    private List<int> indexOfInterest = new List<int>();
    private Vector3 myPosition;
    private SphereCollider sphereCollider;
    private entityAI myentityAI;

    void Start() {
        // GetComponentInParent<entityAI>
        sphereCollider = GetComponent<SphereCollider>();
        myentityAI = GetComponentInParent<entityAI>();
    }

    void FixedUpdate() {}
    void OnTriggerEnter(Collider collider) {
        spotted.Add(collider);
    }

    void OnTriggerExit(Collider collider) {
        spotted.Remove(collider);
    }
    
    float currentDistance = 0f;
    private int bestMatchIndex;
    private float bestMatchDistance;
    private List<Collider> rejectionList;
    public Collider findPriority(string priorityTag) {
        if(priorityTag == "") {
            return null;
        }

        if(priorityTag == "FemaleRabbit") {
            rejectionList = myentityAI.rejectionList;
        }
        myPosition = transform.position;
        indexOfInterest.Clear();
        for(int index = 0; index < spotted.Count; index++) {
            try {
                if(spotted[index].tag == priorityTag) {
                    indexOfInterest.Add(index);
                }
            } catch {}
        }
        
        if(indexOfInterest.Count > 0) {
            bestMatchDistance = 1000000f;
            bestMatchIndex = -1;
            for(int interestIndex = 0; interestIndex < indexOfInterest.Count; interestIndex++) {
                // check rejection list if we are looking for female rabbits
                if(priorityTag.Contains("Female") && rejectionList.Contains(spotted[indexOfInterest[interestIndex]])) {
                    // skip this iteration if this female rabbit has rejected us.
                    continue;
                }
                currentDistance = distanceBetweenPoints(myPosition, spotted[indexOfInterest[interestIndex]].transform.position);
                if(currentDistance < bestMatchDistance) {
                    bestMatchIndex = indexOfInterest[interestIndex];
                    bestMatchDistance = currentDistance;
                }
            }
            // Debug.Log("Returning "+spotted[0].tag+"! | "+bestMatchIndex+" ");
            if(bestMatchIndex != -1) {
                return spotted[bestMatchIndex];
            }
        }
        return null;
    }

    float currentTag;
    public Collider findPriority2(List<string> priorityTags, string descriminant="distance") {
        if(priorityTags.Count == 0) {
            return null;
        }
        
        indexOfInterest.Clear();

        for(int index = 0; index < spotted.Count; index++) {
            foreach(string priorityTag in priorityTags) {
                try {
                    if(spotted[index].tag == priorityTag) {
                        indexOfInterest.Add(index);
                        break;
                    } 
                } catch {}                
            }
        }
        
        if(indexOfInterest.Count > 0) {
            if(descriminant == "distance") {
                return returnSpottedBasedOnDistance(priorityTags, indexOfInterest);
            }
            else if (descriminant == "age") {
                return returnSpottedBasedOnDistance(priorityTags, indexOfInterest, true);
            }
        }
        return null;
    }

    public Collider returnSpottedBasedOnDistance(List<string> priorityTags, List<int> indexOfInterest = null, bool considerAge = false) {
        rejectionList = myentityAI.rejectionList;
        bestMatchDistance = 1000000f;
        bestMatchIndex = -1;
        myPosition = transform.position;
        float age;
        for(int interestIndex = 0; interestIndex < indexOfInterest.Count; interestIndex++) {
            // check rejection list if we are looking for female rabbits
            if(priorityTags[0].Contains("Female") && rejectionList.Contains(spotted[indexOfInterest[interestIndex]])) {
                // skip this iteration if this female rabbit has rejected us.
                continue;
            }
            currentDistance = distanceBetweenPoints(myPosition, spotted[indexOfInterest[interestIndex]].transform.position);
            if(currentDistance < bestMatchDistance) {
                bestMatchIndex = indexOfInterest[interestIndex];
                if(considerAge) {
                    age = spotted[bestMatchIndex].GetComponent<entityAI>().age;
                    bestMatchDistance = currentDistance/age;
                } else {
                    bestMatchDistance = currentDistance;
                }
            }
        }
        // Debug.Log("Returning "+spotted[0].tag+"! | "+bestMatchIndex+" ");
        if(bestMatchIndex != -1) {
            return spotted[bestMatchIndex];
        }
        return null;
    }


    public Collider findAdult(string entityTag, float entityAge = 0) {
        List<string> parentTags;
        if(entityTag.Contains("Rabbit")) {
            parentTags = new List<string>{ "MaleRabbit", "FemaleRabbit" };
        } else if (entityTag.Contains("Fox")) {
            parentTags = new List<string>{ "MaleFox", "FemaleFox" };
        } else {
            return null;
        }
        Collider ret = findPriority2(parentTags, "age");
        if(ret) {
            float targetAge = ret.GetComponent<entityAI>().age;
            if(targetAge >= entityAge || Random.value < 0.5*(entityAge-targetAge)/30) {
                return ret;
            }
        }
        return null;
    }
    public bool findAnyOfTags(List<string> tags, float distanceMax) {
        for(int index = 0; index < spotted.Count; index++) {
            foreach(string tag in tags) {
                try {
                    if(spotted[index].tag == tag && distanceBetweenPoints(transform.position, spotted[index].transform.position) < distanceMax) {
                        return true;
                    }
                }
                catch {}
            }
        }
        return false;
    }
    public bool findAnyOfTag(string tag, float distanceMax) {
        for(int index = 0; index < spotted.Count; index++) {
            try {
                if(spotted[index].tag == tag && distanceBetweenPoints(transform.position, spotted[index].transform.position) < distanceMax) {
                    return true;
                }
            } catch {}
        }
        return false;
    }

    Vector3 nullVector = new Vector3(0f, 0f, 0f);
    public Vector3 getAveragePositionOfTags(List<string> tags, float distanceMax = 1000f) {
        indexOfInterest.Clear();
        for(int index = 0; index < spotted.Count; index++) {
            foreach(string tag in tags) {
                try {
                    if(spotted[index].tag == tag && distanceBetweenPoints(transform.position, spotted[index].transform.position) < distanceMax) {
                        indexOfInterest.Add(index);
                        break;
                    }
                } catch {}
            }
        }
        if(indexOfInterest.Count > 0) {
            Vector3 net = new Vector3(0f, 0f, 0f);
            for(int interestIndex = 0; interestIndex < indexOfInterest.Count; interestIndex++) {
                net += spotted[indexOfInterest[interestIndex]].transform.position;
            }
            return net / indexOfInterest.Count;
        }
        return nullVector;
    }

    public void setSenseSize(float radius) {
        if(!sphereCollider) {
            sphereCollider = GetComponent<SphereCollider>();
        }
        sphereCollider.radius = radius;
    }

    float distanceBetweenPoints(Vector3 pos1, Vector3 pos2) {
        return Mathf.Sqrt(Mathf.Pow(pos1.x - pos2.x, 2) + Mathf.Pow(pos1.y - pos2.y, 2) + Mathf.Pow(pos1.z - pos2.z, 2));
    }

}
