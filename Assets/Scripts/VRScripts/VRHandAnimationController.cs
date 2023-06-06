using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class VRHandAnimationController : MonoBehaviour
{
    public InputActionProperty pinchAnimationProperty;
    public InputActionProperty grabAnimationProperty;
    Animator anim;
    public VRThrowController controller;    

    private void Start()
    {
        anim = GetComponent<Animator>();
    }
    private void Update()
    {
        float pinchValue = pinchAnimationProperty.action.ReadValue<float>();
        float grabValue = grabAnimationProperty.action.ReadValue<float>();

        anim.SetFloat("Trigger",pinchValue);
        anim.SetFloat("Grab", grabValue);
        if (controller.isGrabbing)
        {
            anim.SetBool("wantsGrab", true);
        }else
        {
            anim.SetBool("wantsGrab", false);
        }
    }

}
