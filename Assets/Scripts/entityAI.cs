using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class entityAI : MonoBehaviour
{
    // Start is called before the first frame update
    private BoxCollider myBoxCollider;
    private Transform myTransform;
    private Rigidbody myRigidbody;
    private entitySense senses;
    public interactionRange reach;

    // public GameObject Model;

    public bool pause;
    public GameObject LookAtMe;

    public float hunger = 0f;
    public float thirst = 0f;
    public float reproductiveUrge = 0f;
    public float age = 0f;
    public float maxAge = 120f;

    public float box = 0f;
    private string priority;
    private List<string> priorities;
    public Collider priorityCollider = null;
    private bool canJump = false;

    public List<string> predators;
    Quaternion newRotation;

    private string myTag;
    public float desirability; // max amount is 100
    public float speed = 10f;
    float currentSpeed;
    public float sight = 20f;
    float currentSight;
    public float gestationTime = 10f;
    public float herdTendency = 40f;
    public float currentHerdTendency;

    // if hunger or thirst exceeds this, then they die
    const float hungerThreshold = 150f;
    const float thirstThreshold = 50f;
    public float reproductiveThreshold = 30f;
    public List<AudioClip> audioClips;
    AudioSource audioSource;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        // Debug.Log(mutateAverage(10f, 10f, 0f, 20f));
        myBoxCollider = GetComponent<BoxCollider>();
        myTransform = GetComponent<Transform>();
        myRigidbody = GetComponent<Rigidbody>();
        senses = GetComponentInChildren<entitySense>();

        reach = GetComponentInChildren<interactionRange>();
        // LookAtMe = GameObject.Find("LookAtMe");
        pause = false;
        myTag = gameObject.tag;
        if(myTag.Contains("Rabbit")) {
            fullGrowthAge = 15f;
        }

        desirabilityThreshold = 2*desirability/3;

        // get model, adjust color to desirability
        // Model = myTransform.Find("Model").gameObject;   
        // if(myTag.Contains("Fox")) {
        //     Model.GetComponent<MeshRenderer>().material.color = new Color(0.1f+(desirability/100f)*0.6f, 0.1f+(desirability/100f)*0.05f, 0.1f-(desirability/100f)*0.1f, 1.0f);
        // } else if(myTag.Contains("Rabbit")) {
        //     Model.GetComponent<MeshRenderer>().material.color = new Color(0.2f + (desirability/100f)*0.2f, 0.2f-(desirability/100f)*0.15f, 0.2f-(desirability/100f)*0.2f, 1.0f);
        // }
        age = initialAge;
        senses.setSenseSize(sight);
        currentHerdTendency = 100f;
    }

    // Update is called once per frame
    Vector2 tempAngle;
    
    // used for females only
    public bool courted = false;
    Vector3 courterPos;
    bool gestating = false;
    
    float gestationAmount = 0f;
    // once gestation amount passes this threshold, the current rabbit will give birth
    float gestationThreshold = 2f; 
    float fullGrowthAge = 20f;

    float initialAge = 5f;

    float timeSinceLastJump = 0f;

    
    string tempStr;
    void FixedUpdate()
    {   
        float ratio = 0.5f*(age/fullGrowthAge);
        timeSinceLastJump += Time.fixedDeltaTime;

        // force entity to jump if it hasn't jumped in a long time.
        if(timeSinceLastJump > 3f) {
            canJump = true;
            timeSinceLastJump = 0f;
        }

        if(age < fullGrowthAge) {
            // myBoxCollider.size = new Vector3(1f, ratio/50f, 1f);
            gameObject.transform.localScale = new Vector3(ratio, ratio, ratio);
            currentSpeed = (float)((0.3f + 0.7f*(age/fullGrowthAge))*speed);
            currentHerdTendency = 100 - ((100-herdTendency) * (age/fullGrowthAge));
        }

        if(hunger < hungerThreshold && thirst < thirstThreshold && age < maxAge && myTransform.position.y > -200f) {
            if(gestating) {
                // if gestation complete and we can jump (we are on the ground), then birth babies
                if(gestationAmount > gestationThreshold && canJump) {
                    StartCoroutine(reproduce());
                    gestating = false;
                }
                gestationAmount += 0.01f;
            }

            if(!pause) {
                if(courted) {
                    newRotation = Quaternion.Euler(0, LookAt(courterPos, myTransform.position).y, 0);
                } else {
                    if(senses.findAnyOfTags(predators, 15f)) {
                        if(canJump) {
                            // newRotation = Quaternion.Euler(0, 180f+LookAt(senses.getAveragePositionOfTags(predators), myTransform.position).y, 0);
                            playSound("jump");
                            StartCoroutine(JumpTowardsPositionVelocity(senses.getAveragePositionOfTags(predators, 15f), 0.3f, true, true, true));
                            timeSinceLastJump = 0f;
                        }
                    } else {
                        // tempAngle = LookAt(LookAtMe.transform.position, myTransform.position);
                        // myTransform.rotation = Quaternion.Euler(0, tempAngle.y, 0);

                        // determine what desire we want to make our priority
                        assesPriority2();

                        // as long as we are not a female rabbit looking for a mate, search for priority
                        priorityCollider = senses.findPriority2(priorities);

                        // if we are a male and we are looking for and have found a female mate, request to mate
                        if(( (myTag == "MaleRabbit" && prioritiesContain("Rabbit")) || (myTag == "MaleFox" && prioritiesContain("Fox"))) && priorityCollider) {
                            try {
                                tempStr = priorityCollider.GetComponent<entityAI>().requestMate(myTransform.position, desirability);
                            
                                // female accepts
                                if(tempStr == "y") {
                                    // heck yes, keep hopping towards mate
                                } else {
                                    // aww man, keep wandering

                                    // full rejection == "f"
                                    // occurs if desirability is not up to standards

                                    // half rejection == "h". 
                                    // typically because they are too hungry or thirsty, or are in danger.
                                    
                                    addRejection(priorityCollider, tempStr);
                                    priorityCollider = null;
                                }
                            } catch {
                                // bunny might be dead
                                priorityCollider = null;
                            }
                        }

                        // determine if touching priority
                        if(priorityCollider && reach && reach.touchingGameObjectWithName(priorityCollider.name) && !courted) {
                            StartCoroutine(addressPriority(priorityCollider.tag));
                        }
                        // if not, jump towards priority if we can jump
                        else { 
                            if(canJump) {
                                // if we have no target, we can choose to follow our nearest neighbor
                                if(priorityCollider == null && hunger < hungerThreshold/3 && thirst < thirstThreshold/3) {
                                    if(Random.value*100 < herdTendency) {
                                        priorityCollider = senses.findAdult(myTag, age);
                                        // if(priorityCollider) {
                                        //     // Debug.Log("Following Closest Elder "+priorityCollider.tag);
                                        // } else {
                                        //     // Debug.Log(myTag);
                                        // }
                                    } else {
                                        // Debug.Log("Not following closest elder");
                                    }
                                    priorities = null;
                                } else {
                                    // Debug.Log(myTag+" "+priorityCollider.tag);
                                }
                                playSound("jump");
                                StartCoroutine(JumpTowardsDestinationVelocity(priorityCollider, 0.3f));
                                timeSinceLastJump = 0f;
                            }
                        }

                        if(priorityCollider) {
                            Vector3 look = LookAt(priorityCollider.transform.position, transform.position);
                            newRotation = Quaternion.Euler(0, look.y, 0);
                        }
                    }
                }
                // rotates towards target rotation
            }
            hunger += 0.01f;
            
            thirst += 0.01f;
            if (!gestating) {
                reproductiveUrge += 0.01f;
            }
            myTransform.rotation = Quaternion.RotateTowards(myTransform.rotation, newRotation, 10f);
            if(!pause) {
                courted = false;
            }
        } else {
            killed();
        }
        age += 0.01f;
    }

    void OnCollisionEnter(Collision collision) {
        // Debug.Log(collision.collider.tag);
        if(collision.collider.tag == "Grass") {
            canJump = true;
        }
    }

    // void assesPriority() {
    //     string maxPriority = "";
    //     float maxPriorityValue = 0f;

    //     if(myTag == "MaleRabbit" || myTag == "FemaleRabbit") {
    //         if(hunger > hungerThreshold/8 && hunger > maxPriorityValue) {
    //             maxPriority = "Food";
    //             maxPriorityValue = hunger;
    //         }

    //         if (thirst > thirstThreshold/8 && thirst > maxPriorityValue) {
    //             maxPriority = "Water";
    //             maxPriorityValue = thirst;
    //         }
            
    //         // if we are male, our reproductive urge is great, and we arent super thirsty or hungry, we should mate. 
    //         if (age > fullGrowthAge && (myTag == "MaleRabbit" && reproductiveUrge > reproductiveThreshold) && (reproductiveUrge > maxPriorityValue && (thirst <= thirstThreshold/2 && hunger <= hungerThreshold/2))) {
    //             maxPriority = "FemaleRabbit";
    //             maxPriorityValue = reproductiveUrge;
    //         }
    //     }
    //     else if(myTag == "MaleFox" || myTag == "FemaleFox") {
    //         if(hunger > hungerThreshold/8 && hunger > maxPriorityValue) {
    //             maxPriority = "Rabbit";
    //             maxPriorityValue = hunger;
    //         }

    //         if (thirst > thirstThreshold/8 && thirst > maxPriorityValue) {
    //             maxPriority = "Water";
    //             maxPriorityValue = thirst;
    //         }
            
    //         // if we are male, our reproductive urge is great, and we arent super thirsty or hungry, we should mate. 
    //         if (age > fullGrowthAge && (myTag == "MaleFox" && reproductiveUrge > reproductiveThreshold) && (reproductiveUrge > maxPriorityValue && (thirst <= thirstThreshold/2 && hunger <= hungerThreshold/2))) {
    //             maxPriority = "FemaleFox";
    //             maxPriorityValue = reproductiveUrge;
    //         }
    //     }
    //     priority = maxPriority;
    // }

    void assesPriority2() {
        List<string> maxPriorities = new List<string>();
        float maxPriorityValue = 0f;

        if(myTag == "MaleRabbit" || myTag == "FemaleRabbit") {
            if(hunger > hungerThreshold/8) {
                maxPriorities.Add("Food");
                maxPriorities.Add("Berry");
                maxPriorityValue = hunger;
            }

            if (thirst > thirstThreshold/8 && thirst > maxPriorityValue) {
                maxPriorities.Clear();
                maxPriorities.Add("Water");
                maxPriorityValue = thirst;
            }
            
            // if we are male, our reproductive urge is great, and we arent super thirsty or hungry, we should mate. 
            if (age > fullGrowthAge && (myTag == "MaleRabbit" && reproductiveUrge > reproductiveThreshold) && (reproductiveUrge > maxPriorityValue && (thirst <= thirstThreshold/3 && hunger <= hungerThreshold/3))) {
                maxPriorities.Clear();
                maxPriorities.Add("FemaleRabbit");
                maxPriorityValue = reproductiveUrge;
            }
        }
        else if(myTag == "MaleFox" || myTag == "FemaleFox") {
            if(hunger > hungerThreshold/4) {
                maxPriorities.Add("MaleRabbit");
                maxPriorities.Add("FemaleRabbit");
                maxPriorities.Add("Berry");
                maxPriorityValue = hunger;
            }

            if (thirst > thirstThreshold/8 && thirst > maxPriorityValue) {
                maxPriorities.Clear();
                maxPriorities.Add("Water");
                maxPriorityValue = thirst;
            }
            
            // if we are male, our reproductive urge is great, and we arent super thirsty or hungry, we should mate. 
            if (age > fullGrowthAge && (myTag == "MaleFox" && reproductiveUrge > reproductiveThreshold) && (reproductiveUrge > maxPriorityValue && (thirst <= thirstThreshold/2 && hunger <= hungerThreshold/2))) {
                maxPriorities.Clear();
                maxPriorities.Add("FemaleFox");
                maxPriorityValue = reproductiveUrge;
            }

            // too many rabbits, must purge
            if(StaticScript.getNumRabbits() > 20 && thirst < thirstThreshold/2 && hunger < hungerThreshold/2 && reproductiveUrge < reproductiveThreshold) {
                // Debug.Log("Too many Rabbits");
                maxPriorities.Clear();
                maxPriorities.Add("MaleRabbit");
                maxPriorities.Add("FemaleRabbit");
            }
        }
        priorities = maxPriorities;
    }

    float addressingTime = 1f;
    IEnumerator addressPriority(string priority) {
        // Debug.Log("INTERACTING FOO");
        // if(priorityCollider.name == "Plane") {
        //     Debug.Log("Trying to Interact with Plane");
        // }

        pause = true;
        priority = priority.ToLower();
        if(myTag == "MaleRabbit" || myTag == "FemaleRabbit") {
            if(priority.Contains("water")) {
                playSound("drink");
                thirst = 0f;
            }
            else if (priority.Contains("food") || priority.Contains("carrot") || priority.Contains("berry")) {
                try {
                    playSound("bunnyEat");
                    priorityCollider.GetComponent<foodBehavior>().Eaten();
                    hunger = 0f;
                } catch {

                }
                priorityCollider = null;
                // Debug.Log("EATING");
            } else if (priority.Contains("rabbit")) {
                entityAI femaleScript = null;
                try {
                    femaleScript = priorityCollider.GetComponent<entityAI>();
                } catch {

                }
                if(femaleScript) {
                    // pregnatitizes female rabbit
                    yield return new WaitForSeconds(addressingTime*2);
                    femaleScript.reproductiveUrge = 0f;
                    femaleScript.startGestating(this);
                    femaleScript.courted = false;
                    reproductiveUrge = 0f;
                }
            }
        }
        else if (myTag == "MaleFox" || myTag == "FemaleFox") {
            if(priority.Contains("water")) {
                playSound("drink");
                thirst = 0f;
            }
            else if (priority.Contains("rabbit")) {
                try {
                    playSound("foxEatRabbit");
                    priorityCollider.GetComponent<entityAI>().killed();
                    hunger = 0f;
                } catch {

                }
                priorityCollider = null;
            } else if (priority.Contains("berry")) {
                try {
                    playSound("bunnyEat");
                    priorityCollider.GetComponent<foodBehavior>().Eaten();
                    hunger -= 15f;
                } catch {

                }
                priorityCollider = null;
            } 
            else if (priority.Contains("fox")) {
                entityAI femaleScript = null;
                try {
                    femaleScript = priorityCollider.GetComponent<entityAI>();
                } catch {

                }
                if(femaleScript) {
                    // pregnatitizes female rabbit
                    yield return new WaitForSeconds(addressingTime*2);
                    femaleScript.reproductiveUrge = 0f;
                    femaleScript.startGestating(this);
                    femaleScript.courted = false;
                    reproductiveUrge = 0f;
                }
            }
        }
        yield return new WaitForSeconds(addressingTime);
        pause = false;
    }


    // IEnumerator JumpTowardsDestination(Collider destination, float time) {
    //     canJump = false;
    //     Vector3 direction;
    //     float reqForce = -1f;
    //     yield return new WaitForSeconds(time*0.1f);
    //     if(destination == null) {
    //         direction = randomDirection();
    //     } else {
    //         Vector2 tempAngle = LookAt(destination.transform.position, myTransform.position);
    //         // float angle = Mathf.MoveTowardsAngle(transform.eulerAngles.y, destination.y, 1000);
    //         direction = new Vector3(0, tempAngle.y, 0);
    //         reqForce = requiredForceForDistance(distance(destination.ClosestPoint(myTransform.position), myTransform.position), myRigidbody.mass, Physics.gravity.y, 45, 2.11f*Time.fixedDeltaTime);
    //     }
    //     newRotation = Quaternion.Euler(direction.x, direction.y, direction.z);
    //     yield return new WaitForSeconds(time);
    //     // Debug.Log("Max:                  "+projectileMotionDistance(currentSpeed, myRigidbody.mass, Physics.gravity.y, 45, Time.fixedDeltaTime));
    //     // Debug.Log("Dis:                  "+distance(destination, myTransform.position));
    //     // Debug.Log(reqForce);
    //     float appliedForce = currentSpeed;
    //     if(reqForce != -1 && reqForce*1.1 < currentSpeed) {
    //         appliedForce = reqForce;
    //     }
    //     // Debug.Log("Req force:            "+reqForce);    
    //     // Debug.Log("Max adjusted for req: "+projectileMotionDistance(currentSpeed, myRigidbody.mass, Physics.gravity.y, 45, Time.fixedDeltaTime));
    //     myRigidbody.AddForce((myTransform.forward*0.5f+myTransform.up*0.5f)*appliedForce/Mathf.Sqrt(2));
    // }
    // IEnumerator JumpTowardsDirectionVelocity(float angle, float time) {
    //     yield return new WaitForSeconds(time*0.1f);
    //     newRotation = Quaternion.Euler(0, angle, 0);
    //     yield return new WaitForSeconds(time);
    //     myRigidbody.velocity = (myTransform.forward+myTransform.up)*currentSpeed/Mathf.Sqrt(2);
    // }
    const int defaultObstacleLayer = 8;
    const float noiseRange = 50f;
    IEnumerator JumpTowardsPositionVelocity(Vector3 target, float time, bool flee = false, bool avoidObstacle=false, bool randomNoise = false) {
        canJump = false;
        float noiseAmount = 0f;
        if(randomNoise) {
            noiseAmount = Random.value*noiseRange - noiseRange/2;
        }
        yield return new WaitForSeconds(time*0.1f);
        if(Physics.Raycast(transform.position, myTransform.forward, 6f, 1 << defaultObstacleLayer)) {
            float randAng;
            if(!Physics.Raycast(transform.position, myTransform.right, 3f, 1 << defaultObstacleLayer)) {
                randAng = 45f;
            } else if (!Physics.Raycast(transform.position, -myTransform.right, 3f, 1 << defaultObstacleLayer)) {
                randAng = -45f;
            } else {
                if(Random.value > 0.5) {
                randAng = 110f;
                } else {
                    randAng = -110f;
                }
            }
            newRotation = Quaternion.Euler(0, myTransform.rotation.eulerAngles.y + randAng, 0);
        } else {
            if(flee) {
                newRotation = Quaternion.Euler(0, noiseAmount+180+LookAt(target, myTransform.position).y, 0);
            } else {
                newRotation = Quaternion.Euler(0, LookAt(target, myTransform.position).y, 0);
            }
        }
        yield return new WaitForSeconds(time);
        myRigidbody.velocity = (myTransform.forward+myTransform.up)*currentSpeed/Mathf.Sqrt(2);
    }
    IEnumerator JumpTowardsDestinationVelocity(Collider destination, float time) {
        canJump = false;
        Vector3 direction;
        float reqVel = -1f;
        yield return new WaitForSeconds(time*0.1f);
        if(destination == null) {
            direction = randomDirection();
        } else {
            Vector2 tempAngle = LookAt(destination.transform.position, myTransform.position);
            // float angle = Mathf.MoveTowardsAngle(transform.eulerAngles.y, destination.y, 1000);
            direction = new Vector3(0, tempAngle.y, 0);
            reqVel = requiredVelocityForDistance(distance(destination.ClosestPoint(myTransform.position), myTransform.position), Physics.gravity.y, 45);
        }
        newRotation = Quaternion.Euler(direction.x, direction.y, direction.z);
        yield return new WaitForSeconds(time);
        // Debug.Log("Max:                  "+projectileMotionDistance(currentSpeed, myRigidbody.mass, Physics.gravity.y, 45, Time.fixedDeltaTime));
        // Debug.Log("Dis:                  "+distance(destination, myTransform.position));
        // Debug.Log(reqVel);
        float appliedVel = currentSpeed;
        if(reqVel != -1 && reqVel < currentSpeed) {
            appliedVel = reqVel;
            try {
                if(destination.tag.Contains("Rabbit") && myTag.Contains("Fox")) {
                    if(Random.value < 0.2f) {
                        appliedVel = Mathf.Min(currentSpeed, appliedVel + 5f);
                        // Debug.Log("In Here");
                    }
                }
            } catch {}
            
        }
        // Debug.Log("Req force:            "+reqVel);    
        // Debug.Log("Max adjusted for req: "+projectileMotionDistance(currentSpeed, myRigidbody.mass, Physics.gravity.y, 45, Time.fixedDeltaTime));
        myRigidbody.velocity = (myTransform.forward+myTransform.up)*appliedVel/Mathf.Sqrt(2);
    }

    private Vector3 randomDirection() {
        if(Physics.Raycast(transform.position, myTransform.forward, 8f, 1 << defaultObstacleLayer)) {
            float randAng;
            if(Random.value > 0.5) {
                randAng = 110f;
            } else {
                randAng = -110f;
            }
            return new Vector3(0, myTransform.rotation.eulerAngles.y + randAng, 0);
        }
        Vector3 currentRotation = myTransform.rotation.eulerAngles;
        Vector3 newRot = new Vector3(0, currentRotation.y + Random.value*200 - 100, 0);
        return newRot;
    }

    private Vector2 LookAt(Vector3 target, Vector3 self) {
        float xDiff = target.x - self.x;
        float yDiff = target.y - self.y;
        float zDiff = target.z - self.z;
        if(xDiff == 0 && yDiff == 0 && zDiff == 0) {
            return new Vector2(0f, 0f);
        }
        float xAngle = (Mathf.Atan2(yDiff, zDiff)/Mathf.PI)*180;
        float yAngle = (Mathf.Atan2(xDiff, zDiff)/Mathf.PI)*180;
        return new Vector2(xAngle, yAngle);
    }

    private float distance(Vector3 from, Vector3 to) {
        float xDiff = from.x - to.x;
        float yDiff = from.y - to.y;
        float zDiff = from.z - to.z;
        return Mathf.Sqrt(Mathf.Pow(Mathf.Sqrt(Mathf.Pow(xDiff, 2) + Mathf.Pow(yDiff, 2)), 2) + Mathf.Pow(zDiff, 2));
    }

    public float requiredVelocityForDistance(float distance, float gravity, float angle) {
        float numerator = Mathf.Abs(gravity*distance);
        float denominator = 2f*Mathf.Cos(angle)*Mathf.Sin(angle);
        return Mathf.Sqrt(numerator/denominator);
    }
    // private float requiredForceForDistance(float distance, float mass, float gravity, float angle, float timeForceApplied) {
    //     float numerator = distance*gravity*Mathf.Pow(mass, 2);
    //     float denominator = Mathf.Cos(angle)*Mathf.Sin(angle)*Mathf.Pow(timeForceApplied, 2)/(-2);
    //     // Debug.Log("numer: "+numerator);
    //     // Debug.Log("denom: "+denominator);
    //     return Mathf.Abs(Mathf.Sqrt(numerator/denominator));
    // }

    // private float projectileMotionDistance(float force, float mass, float gravity, float angle, float timeForceApplied) {
    //     float velocity = (force/mass)*timeForceApplied;
    //     float timeOffGround = 2*(Mathf.Sin(angle)*velocity/(gravity));
    //     float distanceTravelled = velocity*Mathf.Cos(angle)*timeOffGround;
    //     return Mathf.Abs(distanceTravelled);
    // }

    float desirabilityThreshold;
    public string requestMate(Vector3 matePosition, float desirability) {
        if(age < fullGrowthAge || hunger > hungerThreshold/2 || thirst > thirstThreshold/2 || courted) {
            return "h";
        }
        else if(desirability < desirabilityThreshold) {
            courted = false;
            desirabilityThreshold /= 2;
            return "n";
        }
        courted = true;
        courterPos = matePosition;
        return "y";
    }

    public List<Collider> rejectionList = new List<Collider>();
    float tempTime = 0f;
    void addRejection(Collider rejector, string rejectionType) {
        if(rejectionType == "n") {
            tempTime = 15f;
        } else if (rejectionType == "h") {
            tempTime = 5f;
        }
        StartCoroutine(processRejection(rejector, tempTime));
    }

    IEnumerator processRejection(Collider rejector, float rejectionTime) {
        rejectionList.Add(rejector);
        yield return new WaitForSeconds(rejectionTime);
        try {
            rejectionList.Remove(rejector);
        } catch {
            // can't remove, probably died.
        }
    }

    float fatherDesire;
    float fatherGesThresh;
    float fatherRepThresh;
    float fatherInitialAge;
    float fatherSight;
    float fatherSpeed;
    float fatherMaxAge;
    public void startGestating(entityAI fatherScript) {
        entityAI currentFatherScript = fatherScript;
        fatherDesire = fatherScript.desirability;
        fatherGesThresh = fatherScript.gestationThreshold;
        fatherRepThresh = fatherScript.reproductiveThreshold;
        fatherInitialAge = fatherScript.initialAge;
        fatherSight = fatherScript.sight;
        fatherSpeed = fatherScript.speed;
        fatherMaxAge = fatherScript.maxAge;
        gestating = true;
    }
    float spitOutBabiesTime = 0.7f;
    int maxRabbitBabies = 6;
    int maxFoxBabies = 4;
    IEnumerator reproduce() {
        int numBabies = 1;
        if (myTag == "FemaleRabbit") {
            numBabies = (int)Mathf.Floor((Random.value*maxRabbitBabies)+1);
        } else if (myTag == "FemaleFox") {
            numBabies = (int)Mathf.Floor((Random.value*maxFoxBabies)+1);
        }
        pause = true;
        for(int x = 0; x<numBabies; x++) {
            yield return new WaitForSeconds(spitOutBabiesTime);
            birthBaby2();
        }
        gestating = false;
        gestationAmount = 0f;

        yield return new WaitForSeconds(1f);
        pause = false;
    }

    public GameObject offspring;
    void birthBaby2() {
        float childDesire = 80f;
        float childGesThresh = mutateRange(gestationThreshold, fatherGesThresh, 0f, 10f);
        float childRepThresh = mutateRange(reproductiveThreshold, fatherRepThresh, 5f, 50f);
        float childInitialAge = childGesThresh;
        float childSight = mutateRange(sight, fatherSight, 3f, 50f);
        float childSpeed = mutateRange(speed, fatherSpeed, 3f, 20f);
        float childMaxAge = mutateRange(maxAge, fatherMaxAge, 3f, 150f);


        if(Random.value > 0.5) {
            if(myTag == "FemaleRabbit") { 
                offspring.tag = "MaleRabbit";
            } else {
                offspring.tag = "MaleFox";
            }
        } else {
            if(myTag == "FemaleRabbit") { 
                offspring.tag = "FemaleRabbit";
            } else {
                offspring.tag = "FemaleFox";
            }
        }
        try {
            entityAI currentChildScript = offspring.GetComponent<entityAI>();

            currentChildScript.desirability = childDesire;
            currentChildScript.gestationThreshold = childGesThresh;
            currentChildScript.reproductiveThreshold = childRepThresh;
            currentChildScript.initialAge = childInitialAge;
            currentChildScript.sight = childSight;
            currentChildScript.speed = childSpeed;
            currentChildScript.maxAge = childMaxAge;
            if(myTag.Contains("Fox")) {
                playSound("foxBorn");
            } else if (myTag.Contains("Rabbit")) {
                playSound("rabbitBorn");
            }
            GameObject.Instantiate(offspring, myTransform.position, Quaternion.identity);
        } catch {

        }
    }

    void birthBaby() {
        float childDesire = mutateAverage(desirability, fatherDesire, 0f, 100f);
        float childGesThresh = mutateAverage(gestationThreshold, fatherGesThresh, 0f, 10f);
        float childRepThresh = mutateAverage(reproductiveThreshold, fatherRepThresh, 5f, 50f);
        float childInitialAge = childGesThresh;
        float childSight = mutateAverage(sight, fatherSight, 3f, 50f);
        float childSpeed = mutateAverage(speed, fatherSpeed, 3f, 20f);
        float childMaxAge = mutateAverage(maxAge, fatherMaxAge, 3f, 150f);

        if(Random.value > 0.5) {
            if(myTag == "FemaleRabbit") { 
                offspring.tag = "MaleRabbit";
            } else {
                offspring.tag = "MaleFox";
            }
        } else {
            if(myTag == "FemaleRabbit") { 
                offspring.tag = "FemaleRabbit";
            } else {
                offspring.tag = "FemaleFox";
            }
        }
        try {
            entityAI currentChildScript = offspring.GetComponent<entityAI>();

            currentChildScript.desirability = childDesire;
            currentChildScript.gestationThreshold = childGesThresh;
            currentChildScript.reproductiveThreshold = childRepThresh;
            currentChildScript.initialAge = childInitialAge;
            currentChildScript.sight = childSight;
            currentChildScript.speed = childSpeed;
            currentChildScript.maxAge = childMaxAge;
            if(myTag.Contains("Fox")) {
                playSound("foxBorn");
            } else if (myTag.Contains("Rabbit")) {
                playSound("rabbitBorn");
            }
            GameObject.Instantiate(offspring, myTransform.position, Quaternion.identity);
        } catch {

        }
    }

    const float mutationMin = -5f;
    const float mutationRange = 20f;
    float mutateAverage(float parent1, float parent2, float min, float max) {
        float range = Mathf.Abs(parent1 - parent2);
        float minVal = Mathf.Min(parent1, parent2);
        float minMaxRange = max - min;
        float val = (float)((minVal)+(Random.value*range)+(Random.value-0.5)*minMaxRange*0.1);
        if(val > max) {
            return max;
        }
        return val;
    }

    float mutateRandom(float val1, float val2) {
        return Mathf.Min(val1, val2)+Random.value*(Mathf.Abs(val1-val2));
    }
    float mutateRange(float val1, float val2, float min, float max, float additionalRange=10f) {
        float res = Mathf.Min(val1, val2)+Random.value*(Mathf.Abs(val1-val2)+additionalRange)-additionalRange/2;
        if(res > max) {
            return max;
        } else if (res < min) {
            return min;
        }
        return res;
    }

    bool prioritiesContain(string tag) {
        foreach(string priority in priorities) {
            if(priority.Contains(tag)) {
                return true;
            }
        }
        return false;
    }

    void playSound(string name) {
        try {
            foreach (AudioClip audioClip in audioClips) {
                if(audioClip.name == name) {
                    audioSource.clip = audioClip;
                    audioSource.pitch = 1f + (Random.value*0.3f-0.15f);
                    audioSource.Play();
                    break;
                }
            }
        } catch {}
    }

    public void killed() {
        if(myTag.Contains("Rabbit")) {
            playSound("bunnyDeath");
        } else if (myTag.Contains("Fox")) {
            playSound("foxDeath");
        }
        StartCoroutine(removeFromWorld());
    }
    
    IEnumerator removeFromWorld() {
        myTransform.position = new Vector3(myTransform.position.x, -5f, myTransform.position.z);
        yield return new WaitForSeconds(0.8f);
        myTransform.position += new Vector3(0f, -100f, 0f);
        yield return new WaitForSeconds(0.02f);
        Destroy(gameObject);
        this.enabled = false;
    }
}
