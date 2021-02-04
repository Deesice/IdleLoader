using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabObject : MonoBehaviour
{
    Rigidbody myRigidbody;
    public float massMultiplier = 0.001f;
    FixedJoint joint;
    void Awake()
    {
        myRigidbody = GetComponent<Rigidbody>();
    }    
    private void Update()
    {
        if (transform.position.y < Ground.instance.verticalOffset)
        {
            PoolManager.Destroy(gameObject);
            if (Mathf.Abs(transform.position.x) > Road.instance.tileSize / 2)
                PlayerController.instance.score++;
        }
    }
    private void FixedUpdate()
    {
        if (Mathf.Abs(transform.position.x) > Road.instance.tileSize / 2)
        {
            if (joint != null)
                PlayerController.instance.HandsDown(true);
            var dir = transform.position;
            dir.y = 0;
            dir.z = 0;
            myRigidbody.AddForce(dir.normalized / 10, ForceMode.VelocityChange);
        }
    }
    public void DestroyJoint()
    {
        if (joint != null)
        {
            Destroy(joint);
            myRigidbody.mass /= massMultiplier;
        }
    }
    public void Grab(Rigidbody hand)
    {
        joint = gameObject.AddComponent<FixedJoint>();
        myRigidbody.mass *= massMultiplier;
        //joint.breakForce = myRigidbody.mass * 10;
        //joint.breakTorque = myRigidbody.mass * 10;
        joint.connectedBody = hand;
    }
}
