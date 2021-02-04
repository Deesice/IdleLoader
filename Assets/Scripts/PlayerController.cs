using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    public float speed;
    public float angularSpeed;
    public Transform[] wheels;
    public bool mobileControl;
    
    Rigidbody myRigidbody;
    Vector3 input;    
    Joystick joystick;
    ConfigurableJoint joint;
    public float angle;
    Vector3 euler;
    IEnumerator coroutine;
    bool readyToDoubleTap;
    bool handsUp;

    public static PlayerController instance;
    public float score;
    public bool forcedMovement { get; set; }
    public GameObject trailPrefab;

    void Awake()
    {
        forcedMovement = true;
        instance = this;
        myRigidbody = GetComponent<Rigidbody>();
        joint = GetComponent<ConfigurableJoint>();
        joystick = FindObjectOfType<Joystick>(true);
        //StartCoroutine(Trailing());
        var collider = GetComponentInChildren<Collider>();
        var mat = new PhysicMaterial();
        mat.bounciness = 0;
        mat.dynamicFriction = 0;
        mat.staticFriction = 0;
        collider.material = mat;
        Invoke(nameof(ChangeFriction), 1);
    }
    void ChangeFriction()
    {
        var collider = GetComponentInChildren<Collider>();
        collider.material.frictionCombine = PhysicMaterialCombine.Minimum;
    }
    void Update()
    {
#if UNITY_EDITOR
        input.z = Input.GetAxis("Vertical");
        input.x = Input.GetAxis("Horizontal");
        if (Input.GetKeyDown(KeyCode.Space))
            SwitchHands();
#else
        input.z = joystick.Vertical;
        input.x = joystick.Horizontal;
        if (Input.touchCount == 0)
            readyToDoubleTap = true;
        if (DoubleTap())
            SwitchHands();
#endif
        if (forcedMovement)
            input = Vector3.forward;
        else
            input = Vector3.ClampMagnitude(input, 1);
    }
    void SwitchHands()
    {
        if (handsUp)
            HandsDown();
        else
            HandsUp();
    }
    bool DoubleTap()
    {
        if (!readyToDoubleTap)
            return false;

        foreach (var t in Input.touches)
            if (t.tapCount > 1)
            {
                readyToDoubleTap = false;
                return true;
            }
        return false;
    }
    public void HandsUp()
    {
        if (handsUp || coroutine != null)
            return;
        handsUp = true;
        Scanner.instance.TryGrab();

        coroutine = Hands(true, 0.5f);
        StartCoroutine(coroutine);
    }
    public void HandsDown(bool forced = false)
    {
        if (!handsUp || (!forced && coroutine != null))
            return;
        handsUp = false;

        if (Scanner.instance.grabObject != null)
        {
            Scanner.instance.grabObject.DestroyJoint();
            Scanner.instance.grabObject = null;
        }

        coroutine = Hands(false, 0.5f);
        StartCoroutine(coroutine);
    }
    IEnumerator Hands(bool isUp, float time)
    {
        var startPos = joint.targetPosition;
        float i = 0;
        while (i < 1)
        {
            if (isUp)
                joint.targetPosition = Vector3.Lerp(startPos, Vector3.right * joint.linearLimit.limit, i);
            else
                joint.targetPosition = Vector3.Lerp(startPos, Vector3.left * joint.linearLimit.limit, i);
            yield return null;
            i += Time.deltaTime / time;
        }
        joint.targetPosition = isUp ? Vector3.right * joint.linearLimit.limit : Vector3.left * joint.linearLimit.limit;
        coroutine = null;
    }
    IEnumerator Trailing()
    {
        while(true)
        {
            yield return null;
            PoolManager.Instantiate(trailPrefab, transform.position, transform.rotation);
        }
    }
    private void FixedUpdate()
    {
        angle = Vector3.SignedAngle(input, transform.forward, Vector3.up);
        foreach (var i in wheels)
        {
            euler = i.transform.localRotation.eulerAngles;
            euler.y = Mathf.Clamp(angle, -30, 30);
            i.transform.localRotation = Quaternion.Euler(euler);
        }
        myRigidbody.angularVelocity = -Vector3.up * angle * angularSpeed * input.magnitude;
        myRigidbody.velocity = transform.forward * input.magnitude * speed * Mathf.Cos(angle * Mathf.Deg2Rad)
            + myRigidbody.velocity.y * Vector3.up;
    }
    private void OnCollisionEnter(Collision collision)
    {
        myRigidbody.constraints |= RigidbodyConstraints.FreezePositionY;
    }
}
