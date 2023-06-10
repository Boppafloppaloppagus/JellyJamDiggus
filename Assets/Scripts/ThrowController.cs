using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ThrowController : MonoBehaviour
{
    [Header("References")]
    public Transform cam;
    public Transform attackPoint;
    public GameObject objectThrow;

    [Header("Settings")]
    public float throwCooldown;

    [Header("Throwing")]
    bool isInteracting;
    bool isReleasing;
    //public KeyCode throwKey = KeyCode.Mouse0;
    //public KeyCode dropKey = KeyCode.Mouse1;
    public float throwForce;
    public float throwUpwardForce;

    bool readyToThrow;


    //Boppa Zone------------------------------------------------------
    //I want to add the players rb velocity to the throw
    public Rigidbody playerRb;
    //I only want to be able to pick things up if they're close enough
    public float pickupRange;
    //I want to know what I can interact with
    public LayerMask interactableObjectMask;
    public LayerMask jellyLayerMask;
    private RaycastHit interactableInfo;
    private GameObject interactableObjectInView;
    public GameObject interactableObject;
    Renderer objectRenderer;
    //making sure I can do things without other things freaking out
    bool seenAnIneractable;
    public bool holdingSomething;
    bool setNewHeldObject;
    //this is being used by the nav agent to start the fetching mechanic
    public bool holdingSomethingFetchable;
    //hand UI stuff
    public Image[] crosshair;
    //----------------------------------------------------------------
    public bool holdingJelly;
    public bool calledJelly;
    public bool petTimeNow;

    //----------------------------------------------------------------
    public AudioSource whistle;
    public AudioClip[] whistleClips;
    int audioIndex;

    private void Start()
    {
        //readyToThrow = true;
        audioIndex = whistleClips.Length - 1;
    }

    private void Update()
    {
        if(!holdingSomething)
            FindPickup();
        else
            HoldObject();


        if (holdingSomething && isInteracting) //&& readyToThrow)
        {
            Throw();
            isInteracting = false;  
        }
        else if (isInteracting) //&& readyToThrow)
        {
            Pickup();
            isInteracting = false;
        }
        if (holdingSomething && isReleasing)
        {
            holdingSomething = false;
            holdingJelly = false;
            isReleasing = false;
        }
        else if (isReleasing)
        {

            if (Physics.Raycast(cam.position, cam.forward, out interactableInfo, pickupRange, jellyLayerMask) && interactableInfo.collider.gameObject.tag == "Jelly")
            {
                PetTheGoodBoi();
            }
            else if(isReleasing)
            {
                if (audioIndex > whistleClips.Length - 1)
                {
                    audioIndex = 0;
                }
                else
                {
                    whistle.clip = whistleClips[audioIndex];
                    whistle.Play(0);
                    audioIndex++;
                    CallTheGoodBoi();
                }
                isReleasing = false;
            }
        }
        else
        {
            petTimeNow = false;
            calledJelly = false;
            isReleasing = false;
        }


    }
    /* && interactableInfo.collider.gameObject.tag == "Jelly"
     * 
     * 
     * 
     *
     * Start by checking the value of the forcedirection property at different camera angles
     * 
     * 
     * 
     * 
     */
    #region Input System
    public void Interact(InputAction.CallbackContext context)
    {
       if (context.started)
        {
            isInteracting = true;
        }
        if (context.canceled)
        {
            isInteracting = false;
        }
    }
    public void Release(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            isReleasing = true;
        }
        if (context.canceled)
        {
            isReleasing = false;
        }
    }
    #endregion
    private void Throw()
    {
        //readyToThrow = false;
        //-----------------------------------------------------
        holdingSomething = false;
        holdingSomethingFetchable = false;
        holdingJelly = false;
        changeCrosshair(0);
        //--------------------------------------------------

        GameObject projectile = interactableObject;//Instantiate(objectThrow, attackPoint.position, cam.rotation);

        Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();

        Vector3 forceDirection = cam.transform.forward;


        /*
        RaycastHit hit;
        
        if (Physics.Raycast(cam.position, cam.forward, out hit, 500f))
        {
            forceDirection = (hit.point - attackPoint.position).normalized;
        }
        */
        projectileRb.velocity = Vector3.zero ;
        Vector3 forceToAdd = forceDirection * throwForce + transform.up * throwUpwardForce + playerRb.velocity;
        projectileRb.AddForce(forceToAdd, ForceMode.Impulse);

        //Invoke(nameof(ResetThrows), throwCooldown);

    }
    /*
    private void ResetThrows()
    {
        readyToThrow = true;
    }
    */
    //----------------------------
    //This is for finding interactable objects
    private void FindPickup()
    {
        if (Physics.Raycast(cam.position, cam.forward, out interactableInfo, pickupRange, interactableObjectMask) && !seenAnIneractable)
        {

            seenAnIneractable = true;
            changeCrosshair(1);
            //Now I know about what it is
            interactableObjectInView = interactableInfo.transform.gameObject;


            interactableObject = interactableObjectInView;


        }
        //Stop Highlighting
        else if (!Physics.Raycast(cam.position, cam.forward, out interactableInfo, pickupRange, interactableObjectMask) && seenAnIneractable)
        {
            changeCrosshair(0);
            seenAnIneractable = false;
            interactableObjectInView = null;
        }
    }
    void PetTheGoodBoi()
    {
        petTimeNow = true;
    }
    void CallTheGoodBoi()
    {
        calledJelly = true;
    }
    //This is to signal whether or not the interactable objects transform should have its transform tied to the player
    private void Pickup()
    {
        if (Physics.Raycast(cam.position, cam.forward, out interactableInfo, pickupRange, interactableObjectMask))
            holdingSomething = true;
    }

    private void HoldObject()
    {
        if (interactableObject.activeSelf == false)
        {
            holdingSomething = false;
        }
        else if (interactableObjectInView = null)
        {
            interactableObject = null;
        }
        else
        {
            changeCrosshair(2);
            interactableObject.transform.position = attackPoint.transform.position;
            interactableObject.transform.rotation = attackPoint.transform.rotation;
            interactableObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            if (interactableObject.tag == "Fetchable")
                holdingSomethingFetchable = true;
            else if (interactableObject.tag == "Jelly")
            {
                holdingJelly = true;
            }
        }
    }

    void changeCrosshair(int n)
    {
        for (int i = 0; i < crosshair.Length; i++)
        {
            crosshair[i].gameObject.SetActive(false);
        }
        crosshair[n].gameObject.SetActive(true);
    }
    //----------------------------
}
