using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Timer : MonoBehaviour, IPool
{
    public bool notPrefab;
    public float delay;
    public UnityEvent OnTimeLeft;
    public UnityEvent OnReset;

    void Start()
    {
        if (notPrefab)
            OnTake();
    }

    public void OnPush()
    {
        CancelInvoke();
    }

    public void OnTake()
    {
        OnReset?.Invoke();
        Invoke(nameof(Action), delay);
    }
    void Action()
    {
        OnTimeLeft?.Invoke();
    }
}
