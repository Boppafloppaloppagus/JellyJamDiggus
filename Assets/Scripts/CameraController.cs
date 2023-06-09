    using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    public float sensX;
    public float sensY;
    
    public Transform orientation;

    Vector2 movePos;
    float xRotation;
    float yRotation;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    public void CameraInput(InputAction.CallbackContext context)
    {
        movePos = context.ReadValue<Vector2>();
    }
    void Update()
    {
        float mouseX = movePos.x * Time.deltaTime * sensX;
        float mouseY = movePos.y * Time.deltaTime * sensY;
        yRotation += mouseX;
        xRotation -= mouseY;

        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);
    }
}
