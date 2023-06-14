using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VectorFieldGrabber : MonoBehaviour
{
    public LayerMask interactableLayerMask;
    SphereCollider theField;
    Rigidbody objectInField;
    // Start is called before the first frame update
    void Start()
    {
        theField = this.gameObject.GetComponent<SphereCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerStay(Collider collider)
    {
        objectInField = collider.gameObject.GetComponent<Rigidbody>();
        Debug.Log(this.gameObject.transform.position - collider.gameObject.transform.position);   
        objectInField.AddForce((this.gameObject.transform.position - collider.gameObject.transform.position), ForceMode.Impulse    );
    }
}
