using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour
{
    public Transform target;
    public float speed;
    Vector3 offset;

    void OnEnable()
    {
        offset = transform.position - target.position;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, target.position + offset, Time.deltaTime * speed);
        if (transform.position.z < Road.instance.firstTileZPos - Road.instance.tileSize / 2)
            transform.position = new Vector3(transform.position.x, transform.position.y, Road.instance.firstTileZPos - Road.instance.tileSize / 2);
    }
}
