using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeFog : MonoBehaviour
{
    private float fogDefaultVal;
    private Color32 fogDefaultCol;

    [SerializeField] private Transform groundFog;

    [SerializeField] private Color32 colorBelowFog;
    [SerializeField] private float fogDensityBelowFog;

    bool belowFog = false;


    private void Start()
    {
        fogDefaultVal = RenderSettings.fogDensity;
        fogDefaultCol = RenderSettings.fogColor;
    }

    private void Update()
    {
        if (!belowFog && transform.position.y <= groundFog.position.y)
        {
            belowFog = true;
            RenderSettings.fogDensity = fogDensityBelowFog;
            RenderSettings.fogColor = colorBelowFog;
        }
        else if (belowFog && transform.position.y > groundFog.position.y)
        {
            belowFog = false;
            RenderSettings.fogDensity = fogDefaultVal;
            RenderSettings.fogColor = fogDefaultCol;
        }
    }

}

