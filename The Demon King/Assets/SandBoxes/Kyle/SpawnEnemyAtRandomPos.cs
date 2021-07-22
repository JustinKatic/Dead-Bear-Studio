using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class SpawnEnemyAtRandomPos : MonoBehaviour
{
    public float RadiusCheck = 10.0f;

    Vector3 RandomPoint(Vector3 center, float range)
    {
        Vector3 result = Vector3.zero;
        Vector3 randomPoint = center + Random.insideUnitSphere * range;
        NavMeshHit hit;
        while (!NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
        {
            result = hit.position;
            return result;
        }
        return Vector3.zero;
    }

    void Update()
    {
        Debug.DrawRay(RandomPoint(transform.position, RadiusCheck), Vector3.up, Color.blue, 1.0f);
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position, RadiusCheck);
    }
}

