using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DeathGround : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            other.gameObject.GetComponent<PlayerHealthManager>().Respawn(false);
            Debug.Log("DIED BY LAVA");
        }
    }
}

