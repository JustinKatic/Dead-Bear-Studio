using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    public Transform _cam;

    private void Start()
    {
        _cam = Camera.main.transform;
        
    }

    void LateUpdate()
    {
        transform.LookAt(transform.position + _cam.forward);
    }
}
