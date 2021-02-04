using System;
using UnityEngine;

public class Trigger : MonoBehaviour
{
    public event Action<GameObject> events;
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject);
        events?.Invoke(other.gameObject);
    }
}
