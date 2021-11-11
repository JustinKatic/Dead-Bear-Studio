using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmoteBillboard : MonoBehaviour
{
    private Transform _cam;

    private void Start()
    {
        FindMainCam();
    }

    void FindMainCam()
    {
        _cam = Camera.main.transform;
    }

    void LateUpdate()
    {
        _cam = Camera.main.transform;
        transform.LookAt(transform.position + _cam.forward);
    }
}
