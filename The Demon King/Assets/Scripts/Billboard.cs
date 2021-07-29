using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Transform _cam;

    bool startBillboarding = false;

    private void Start()
    {
        Invoke("FindMainCam", 3);

    }
    void FindMainCam()
    {
        _cam = Camera.main.transform;
        startBillboarding = true;
    }

    void LateUpdate()
    {
        if (startBillboarding)
        {
            _cam = Camera.main.transform;
            transform.LookAt(transform.position + _cam.forward);
        }
    }
}
