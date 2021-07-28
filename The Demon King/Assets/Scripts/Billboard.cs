using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Transform _cam;

    private void Start()
    {
            _cam = Camera.main.transform;

    }

    void LateUpdate()
    {
        _cam = Camera.main.transform;
        
        transform.LookAt(transform.position + _cam.forward);
    }
}
