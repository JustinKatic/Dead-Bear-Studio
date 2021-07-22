using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class AISpawner : MonoBehaviour
{
    public float RadiusCheck = 10.0f;

    public Vector3 RandomPoint(Vector3 center, float range)
    {
        Vector3 result = Vector3.zero;
        bool searching = true;

        while (searching)
        {
            Vector3 randomPoint = center + Random.insideUnitSphere * range;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
            {

                result = hit.position;
                searching = false;
            }
        }
        return result;
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position, RadiusCheck);
    }

}
