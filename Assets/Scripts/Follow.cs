using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Follow : MonoBehaviour
{
    public Transform target;
    public float speed;
    public float wall;
    Vector3 offset;
    bool nearGarage;
    public UnityEvent onGarageEnter;
    public UnityEvent onGarageExit;

    void OnEnable()
    {
        offset = transform.position - target.position;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, target.position + offset, Time.deltaTime * speed);
        if (transform.position.z < Road.instance.firstTileZPos - Road.instance.tileSize / 2 + wall)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, Road.instance.firstTileZPos - Road.instance.tileSize / 2 + wall);
            if (nearGarage == false)
                onGarageEnter?.Invoke();
            nearGarage = true;
        }
        else
        {
            if (nearGarage == true)
                onGarageExit?.Invoke();
            nearGarage = false;
        }
    }
}
