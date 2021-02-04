using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scanner : MonoBehaviour
{
    Rigidbody myRigidbody;
    public static Scanner instance;
    public GrabObject grabObject;
    List<Collider> inTrigger = new List<Collider>();
    private void Start()
    {
        instance = this;
        myRigidbody = GetComponent<Rigidbody>();
    }
    public void TryGrab()
    {
        if (inTrigger.Count > 0)
        {
            grabObject = inTrigger[inTrigger.Count - 1].gameObject.GetComponentInParent<GrabObject>();
            if (grabObject != null)
                grabObject.Grab(myRigidbody);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        inTrigger.Add(other);
    }
    private void OnTriggerExit(Collider other)
    {
        inTrigger.Remove(other);
    }
}
