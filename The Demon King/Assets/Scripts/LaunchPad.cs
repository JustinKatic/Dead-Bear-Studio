using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchPad : MonoBehaviour
{
    [SerializeField] private Vector3 launchVelocity;

    private void OnTriggerEnter(Collider other)
    {        
        if (other.gameObject.CompareTag("PlayerParent"))
        {
            other.gameObject.GetComponent<PlayerController>().LaunchPad(launchVelocity);
        }
    }
}
