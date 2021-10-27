using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrownVFXBillboard : MonoBehaviour
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
            transform.LookAt(transform.position + _cam.forward);
            Vector3 eulerAngles = transform.eulerAngles;
            eulerAngles.x = 0;
            eulerAngles.z = 0;

            transform.eulerAngles = eulerAngles;
        }
    }
}
