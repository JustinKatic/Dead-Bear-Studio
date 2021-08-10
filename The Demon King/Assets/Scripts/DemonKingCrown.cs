using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemonKingCrown : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<ExperienceManager>();
        }
    }
}
