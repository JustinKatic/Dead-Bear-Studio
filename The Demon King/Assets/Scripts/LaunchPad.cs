using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchPad : MonoBehaviour
{
    [SerializeField] private Vector3 launchDirection;

    [SerializeField] private bool negX;
    [SerializeField] private bool negY;
    [SerializeField] private bool negZ;


    private void OnTriggerEnter(Collider other)
    {        
        if (other.gameObject.CompareTag("PlayerParent"))
        {
            other.gameObject.GetComponent<PlayerController>().LaunchPad(launchDirection, negX, negY, negZ);
        }
    }
}
