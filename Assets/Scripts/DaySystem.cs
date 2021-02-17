using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DaySystem : MonoBehaviour
{
    //Midday = 43200
    //MidNight = 0
    public Color dayColor;
    public Color nightColor;
    public Light mainLight;
    static int currentTime;
    void Awake()
    {
        //midRotation = mainLight.transform.rotation;
        var date = DateTime.Now;
        currentTime = date.Hour * 3600 + date.Minute * 60 + date.Second ;

        currentTime = 40000;

        currentTime = Mathf.Clamp(currentTime, 0, 43200 * 2);

        mainLight.transform.Rotate(Vector3.up * Mathf.Lerp(-50, 50, ((float)((currentTime + 21600) % 43200)) / 43200), Space.Self);

        mainLight.color = Color.Lerp(dayColor, nightColor, Mathf.Abs(currentTime - 43200) / 43200.0f);
        RenderSettings.ambientIntensity = Mathf.Lerp(1, 0.5f, Mathf.Abs(currentTime - 43200) / 43200.0f);
    }
    public static bool IsDay()
    {
        return currentTime >= 21600 && currentTime < 64800;
    }
}
