using System;
using UnityEngine;
using UnityEngine.UI;

public class Meter : MonoBehaviour
{
    Text text;
    static Meter instance;
    void Awake()
    {
        text = GetComponent<Text>();
        instance = this;
    }
    public static void SetValue(int value)
    {
        if (value % 10 != 0)
            throw new Exception("Meter value not a multiple of 10");
        instance.text.text = (value / 10 + 1).ToString();
    }
}
