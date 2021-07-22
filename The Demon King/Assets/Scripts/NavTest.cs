using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavTest : MonoBehaviour
{
    [SerializeField] private float walkRadius;
    [SerializeField] private GameObject _type;
    private NavMeshSurface t;
    private NavMeshData n;
    
    
    // Start is called before the first frame update
    void Start()
    {
        t = GetComponent<NavMeshSurface>(); 
        n = t.navMeshData;

        
        InvokeRepeating("RandomPointOnNav", 0, 2);
    }

    void RandomPointOnNav()
    {
        Vector3 randomPoint =  n.sourceBounds.center + Random.insideUnitSphere * walkRadius;
        while (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, 1, NavMesh.AllAreas))
        {
            Instantiate(_type, hit.position, Quaternion.identity);

        }

    }
   
    
}
