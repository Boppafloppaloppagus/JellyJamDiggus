using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//this script has been poorly named and is now unofficially the SlimeEverythingController.



public class SlimeNavAgent : MonoBehaviour
{
    #region VRMode
    [Header("VR MODE")]
    [SerializeField] bool useVRMode;
    #endregion
    #region Player Values
    private ThrowController throwController;
    private VRThrowController vrThrowController;
    GameObject interactableObject;
    bool holdingSomethingFetchable;
    bool holdingJelly;
    bool petTimeNow;
    bool calledJelly;
    bool holdingSomething;
    #endregion
    
   
    public GameObject playerStuff;
    public GameObject ballStuff;
    public GameObject foodStuff;
    public GameObject home;
    public SphereCollider jellyInteractRadiusContainer;
    public SphereCollider playerInteractRadiusContainer;
    public SphereCollider theZone;

    public Renderer jellySkin;
    [Header("Animation")]
    public Animator jellyAnimator;
    public string jellyWalkAnimName;
    public string jellyIdleAnimName;
    public string jellyEatAnimName;
    public string jellyPetAnimName;
    #region Sound

    [Header("Sound")]
    public AudioSource jellyAudioSource;
    public AudioClip eatAudio;
    public AudioClip[] petAudios;
    public AudioClip walkAudio;


    #endregion
    Rigidbody rb;

    public Transform[] palmTrees;
    private Vector3 withoutThatStupidY;
    private Vector3 randomPoint;
    private Vector3 goTo;
    private Vector3 startPoint;
    private Vector3 dangerZone;

    //other scripts
    private FoodChecker foodChecker;
    public GameController gameController;

    NavMeshAgent agent;

    //public bool takeAnL;
    bool fetchStart;
    bool carryingSomething;
    [HideInInspector]
    public bool foodStart;
    bool playerInZone;
    bool petStart;
    bool haveIBeenBeaned;
    bool haveIBeenCalled;
    bool pointSet;

    bool isEating;


    public float force;
    float waitASec;
    float wait;
    float timer;

    enum State
    {
        MuckAbout,
        Fetch,
        Food,
        Pet,
        Called,
        CommitDie
    }

    State slimeState;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        //just gettin some components and stuff
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (useVRMode)
        {
            vrThrowController = playerStuff.GetComponent<VRThrowController>();
        }
        else
        {
            throwController = playerStuff.GetComponent<ThrowController>();
        }
        foodChecker = jellyInteractRadiusContainer.GetComponent<FoodChecker>();

        //Set Initial destination for jelly, and intialize components for MuckAbout state.
        wait = Random.Range(5f, 15f);
        randomPoint = Random.insideUnitSphere * 3 + home.transform.position;
        randomPoint = new Vector3(randomPoint.x, this.transform.position.y, randomPoint.z);
        agent.destination = randomPoint;

        //this is used later to prevent the jelly from just 
        //grabbing the ball out of the air when its tossed.
        waitASec = 1;

        startPoint = this.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (!foodStart)
        {
            jellyAnimator.SetBool("Eating", false);
        }
        if (slimeState != State.Pet)
        {
            agent.isStopped = false;
        }
        //VRMODE
        SetupVRMode(useVRMode);
        //Debug.Log(slimeState);
        ApplyAudioWhenWalking();
        CheckForFetch();
        CheckForFood();
        CheckForPetTime();
        CheckForCall();
        IsPlayerInZone();
        SetState();

        switch (slimeState)
        {
            case State.MuckAbout:
                MuckAbout();
                LookAtPlayerInRange(6,3);
                //jellySkin.material.SetColor("_Color", new Color(1, 1, 1, 1));
                break;
            case State.Fetch:
                Fetch();
                break;
            case State.Food:
                Eat();
                break;
            case State.Pet:
                LookAtPlayerInRange(6, 3);
                GetPetNerd();
                break;
            case State.Called:
                Called();
                break;
            case State.CommitDie:
                //GoCommitDie();
                break;
        }

    }
    //bad naming
    void ApplyAudioWhenWalking()
    {
        //worked! good catch from boppa!
        if (agent.velocity.magnitude > 1f)
        {
            if (!jellyAudioSource.isPlaying)
            {

                jellyAudioSource.clip = walkAudio;
                jellyAudioSource.loop = true;
                jellyAudioSource.Play();
            }
            if (isEating)
            {
                SetWalkingTree(true, 1);
            }
            else
            {
                SetWalkingTree(true, 0);
            }
        }
        else if (jellyAudioSource.isPlaying)
        {
            jellyAudioSource.loop = false;
            SetWalkingTree(false,0);
        }

    }
    void SetWalkingTree(bool value, float blendTreeValue = 0)
    {
        jellyAnimator.SetBool("IsWalking", value);
        jellyAnimator.SetFloat("Trigger", blendTreeValue);
    }
    //methods associated with states
    void MuckAbout()
    {

        if (Vector3.Distance(this.transform.position, agent.destination) <= 2)
        {
            if (wait > 0)
            {
                wait -= Time.deltaTime;
            }
            else
            {
                //Debug.Log("destination set");
                randomPoint = Random.insideUnitSphere * 5 + home.transform.position;
                randomPoint = new Vector3(randomPoint.x, this.transform.position.y, randomPoint.z);
                agent.destination = randomPoint;
                wait = Random.Range(3f, 10f);
            }
        }
        //Debug.Log(agent.destination);
    }
    void LookAtPlayerInRange(float distance,float speed)
    {
        if (Vector3.Distance(playerStuff.transform.position,transform.position) < distance&&!jellyAnimator.GetCurrentAnimatorStateInfo(0).IsName(jellyWalkAnimName))
        {
            Quaternion lookRotation = Quaternion.LookRotation(playerStuff.transform.position - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, speed * Time.deltaTime);
        }
    }
    void Eat()
    {
        
        /*
        if (throwController.holdingSomething)
        {
            // ObjA is looking mostly towards ObjB
            agent.destination = playerInteractRadiusContainer.ClosestPoint(this.transform.position);
            wait = 1.5f;

        }
        else if (wait > 0)
        {
            wait -= Time.deltaTime;

        }
        */
        if (Vector3.Distance(this.transform.position, foodStuff.transform.position) > 1)
        {
            agent.destination = foodStuff.transform.position;
            isEating = true;

        }
        else if (Vector3.Distance(this.transform.position, foodStuff.transform.position) > 3)
        {
            foodStart = false;
        }
        else
        {
            if (!holdingSomething) {
                jellyAnimator.SetBool("Eating", true);
                isEating = false;
            }
            }
    }
    void Fetch()
    {
        ballStuff = interactableObject;
        if (!theZone.bounds.Contains(ballStuff.transform.position))
        {
            waitASec = 1;
            carryingSomething = false;
            fetchStart = false;
        }

        if (waitASec > 0 && holdingSomethingFetchable)
        {
            agent.destination = playerInteractRadiusContainer.ClosestPoint(this.transform.position);
        }
        else
        {
            waitASec -= Time.deltaTime;
        }
        if (!carryingSomething && waitASec <= 0 && Vector3.Distance(this.transform.position, ballStuff.transform.position) > 2)
        {
            agent.destination = ballStuff.transform.position;
        }
        else if (waitASec <= 0)
        {
            carryingSomething = true;
            ballStuff.transform.localPosition = this.transform.position + new Vector3(0, 1, 0);
        }
        if (carryingSomething && Vector3.Distance(this.transform.position, playerStuff.transform.position) > 5)
        {
            agent.destination = playerInteractRadiusContainer.ClosestPoint(this.transform.position);
        }
        else if (carryingSomething || holdingJelly)
        {
            //Debug.Log(Vector3.Angle(transform.forward, playerStuff.transform.position - transform.position));
            float angle = Vector3.Angle(transform.forward, playerStuff.transform.position - transform.position);
            if (angle > 15)
            {
                Quaternion lookRotation = Quaternion.LookRotation(playerStuff.transform.position - transform.position);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 3 * Time.deltaTime);
            }
            else
            {
                waitASec = 1;
                carryingSomething = false;
                fetchStart = false;
                Vector3 forceToAdd = this.transform.forward * 15;
                ballStuff.GetComponent<Rigidbody>().AddForce(forceToAdd, ForceMode.Impulse);
            }
        }
        //If the player is still holding the ball I want to follow him, otherwise if he's released the ball and I don't have it I want to go grab it

    }

    void GetPetNerd()
    {
        agent.isStopped = true;
        petStart = false;
        if (!jellyAudioSource.isPlaying&&canPerform)
        {
            AudioClip clip = petAudios[Random.Range(0, petAudios.Length)];
           while (jellyAudioSource.clip == clip)
            {
                clip = petAudios[Random.Range(0, petAudios.Length)];
            }
            jellyAudioSource.clip = clip;
            jellyAudioSource.loop = false;
            jellyAudioSource.Play();
            StartCoroutine(WaitJellySound(2));
        }
        if (jellyAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
        {
            //Change Here to Animation Name
            jellyAnimator.Play(jellyPetAnimName);
        }

    }
    bool canPerform = true;
    IEnumerator WaitJellySound(float seconds)
    {
        canPerform = false;
        yield return new WaitForSeconds(seconds);
        canPerform = true;
    }

    void Called()
    {
        if (!playerInteractRadiusContainer.bounds.Contains(this.transform.position))
        {
            agent.destination = playerInteractRadiusContainer.ClosestPoint(this.transform.position);
            wait = 1.5f;
        }
        else if (wait > 0)
        {
            wait -= Time.deltaTime;
        }
        else
        {
            haveIBeenCalled = false;
        }

    }

    //----------------------------------------------------------------------------------------Past this line thar be checking for things
    //I need this for when the player throws the ball and the fetch state isn't actually over
    void CheckForFetch()
    {
        if (holdingSomethingFetchable && theZone.bounds.Contains(this.transform.position) && theZone.bounds.Contains(playerStuff.transform.position))
        {
            fetchStart = true;
        }
    }

    void CheckForFood()
    {
        if (foodChecker.isFood == true)
        {
            foodStuff = foodChecker.maybeFood;
            foodStart = true;
        }
    }

    void CheckForPetTime()
    {
        if (petTimeNow)
        {
            petStart = true;
        }
    }

    void CheckForCall()
    {
        if (calledJelly)
            haveIBeenCalled = true;

    }

    void IsPlayerInZone()
    {
        if (!theZone.bounds.Contains(playerStuff.transform.position))
        {
            playerInZone = false;
            waitASec = 1;
            carryingSomething = false;
            fetchStart = false;
        }
        else
            playerInZone = true;

    }


    //-----------------------------------------------------------------------------------------------checking for things ends here


    void SetState()
    {
        if (haveIBeenCalled)
            slimeState = State.Called;
        else if (foodStart && playerInZone && theZone.bounds.Contains(this.transform.position))
            slimeState = State.Food;
        else if (fetchStart && playerInZone && theZone.bounds.Contains(this.transform.position))
            slimeState = State.Fetch;
        else if (petStart)
            slimeState = State.Pet;
        /*else if (!playerInZone || pointSet == true && !theZone.bounds.Contains(this.transform.position))
            slimeState = State.CommitDie; //change this later - changed*/
        else if (playerInZone && theZone.bounds.Contains(this.transform.position))
            slimeState = State.MuckAbout;

    }


    void SetupVRMode(bool value)
    {
        if (value)
        {
            interactableObject = vrThrowController.interactableObject;
            holdingSomethingFetchable = vrThrowController.isHoldingSomethingFetchable;
            holdingJelly = vrThrowController.isHoldingJelly;
            petTimeNow = vrThrowController.petTimeNow;
            calledJelly = vrThrowController.calledJelly;
            holdingSomething = vrThrowController.isGrabbing;
        }
        else
        {
            interactableObject = throwController.interactableObject;
            holdingSomethingFetchable = throwController.holdingSomethingFetchable;
            holdingJelly = throwController.holdingJelly;
            petTimeNow = throwController.petTimeNow;
            calledJelly = throwController.calledJelly;
            holdingSomething = throwController.holdingSomething;
        }
    }

    //past ere be old stuff me wanted saved.
    /*
    void GoCommitDie()
    {
        if (throwController.holdingJelly)
        {
            
            dangerZone = palmTrees[0].position;
            Debug.Log(palmTrees.Length);
            for (int i = 1; i < palmTrees.Length; i++)
            {
                if (Vector3.Distance(palmTrees[i].position, this.transform.position) < Vector3.Distance(dangerZone, this.transform.position))
                    dangerZone = palmTrees[i].position;
                Debug.Log(dangerZone);
            }
            agent.destination = dangerZone;
            pointSet = true;
        }
        else if (!pointSet && !throwController.holdingJelly)
        {
            int index = Random.Range(0, 3);
            dangerZone = new Vector3(palmTrees[index].position.x, 0, palmTrees[index].position.z);
            agent.destination = dangerZone;
            pointSet = true;
        }
        else if (Vector3.Distance(this.transform.position, agent.destination) <= 2 && !throwController.holdingJelly)
        {
            jellySkin.material.SetColor("_Color", new Color(1, 0, 0, 1));
            agent.enabled = false;
            pointSet = false;
            takeAnL = true;
        }
    }
    */

}
