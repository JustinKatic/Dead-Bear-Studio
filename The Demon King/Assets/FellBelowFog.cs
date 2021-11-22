using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class FellBelowFog : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {

        if (other.tag == "PlayerParent")
        {
            PlayerHealthManager playerHealth = other.GetComponent<PlayerHealthManager>();
            playerHealth.SetFellInLavaCam();
        }
    }
}
