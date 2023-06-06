using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public Transform cameraOrientation;
    // The Rigidbody attached to the GameObject.
    private Rigidbody body;
    /// Speed scale for the velocity of the Rigidbody.
    private float rotationSpeed;
    public float speed;
    public float deadZone;
    /// The upwards jump force of the player.
    public float jumpForce;
    // The vertical input from input devices.
    private float vertical;
    // The horizontal input from input devices.
    private float horizontal;
    // Whether or not the player is on the ground.
    private bool isGrounded;
    // Initialization function

    void Start()
    {
        body = GetComponent<Rigidbody>();
        //cameraTransform = this.GetComponentsInChildren<Transform>();
    }
    // Fixed Update is called a fix number of frames per second.
    void FixedUpdate()
    {
        CallMeInUpdate(this.transform, cameraOrientation);
        UpdateMovement();
        //faceToCamera();
        if (Input.GetAxis("Jump") > 0)
        {
            if (isGrounded)
            {
                body.AddForce(transform.up * jumpForce);
            }
        }

    }
    void CallMeInUpdate(Transform capsule, Transform camera)
    {
        Vector3 newCameraForward = capsule.forward;
        Vector3 newCameraUp = camera.up;

        Vector3.OrthoNormalize(ref newCameraUp, ref newCameraForward);
        camera.rotation = Quaternion.LookRotation(newCameraForward, newCameraUp);
    }
    void faceToCamera()
    {
        //Debug.Log();
        this.transform.Rotate(0, Input.GetAxis("Horizontal"),0);
    }

    private void UpdateMovement()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        if (h != 0 || v != 0)
        {
            this.transform.position += (this.transform.forward * v + this.transform.right * h) * this.speed * Time.deltaTime;
        }
    }

    // This function is a callback for when an object with a collider collides with this objects collider.
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == ("Ground"))
        {
            isGrounded = true;
        }
    }

    // This function is a callback for when the collider is no longer in contact with a previously collided object.
    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == ("Ground"))
        {
            isGrounded = false;
        }
    }
}