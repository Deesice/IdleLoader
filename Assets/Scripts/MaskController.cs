using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaskController : MonoBehaviour
{
    Camera cam;
    public Transform target;
    Vector2 shift;
    RectTransform rect;
    public Vector2 resolution;
    public float scale;
    public float speed;
    void Start()
    {
        cam = Camera.main;
        rect = GetComponent<RectTransform>();
        rect.localScale = Vector3.one * 0.001f;
    }

    // Update is called once per frame
    void Update()
    {
        shift.x = -Vector3.SignedAngle(Vector3.ProjectOnPlane((target.position - cam.transform.position), cam.transform.up), cam.transform.forward, cam.transform.up) / cam.aspect;
        shift.y = Vector3.SignedAngle(Vector3.ProjectOnPlane((target.position - cam.transform.position), cam.transform.right), cam.transform.forward, cam.transform.right);
        shift /= cam.fieldOfView;
        rect.anchoredPosition = shift * resolution;
        rect.localScale = Vector3.Lerp(rect.localScale, Vector3.one * scale * 80, Time.deltaTime * speed);
        if (rect.localScale == Vector3.one * 80)
            enabled = false;
    }
    public void SetScale(float scale)
    {
        this.scale = Mathf.Clamp(scale, 0.0001f, 1);
    }
    public void SetSpeed(float speed)
    {
        this.speed = speed;
    }
}
