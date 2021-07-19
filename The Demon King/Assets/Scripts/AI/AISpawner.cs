using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AISpawner : MonoBehaviour
{
    [SerializeField] private GameObject agent = null;
    [SerializeField] private int spawnLimit = 5;
    [SerializeField] public List<SpawnLocation> spawnsList = new List<SpawnLocation>();
    [SerializeField] private NavMeshSurface area;
    
    private int numberOfLocations = 0;
    private LayerMask obstruction;
    
    // Start is called before the first frame update
    void Start()
    {
        area = GetComponent<NavMeshSurface>();
        
        //Spawn AI at each Location on the given Navmesh
        foreach (var spawn in spawnsList)
        {
            Instantiate(agent, spawn.spawnPosition, Quaternion.identity);
        }

    }

    private Vector3 RandomSpawnLocation()
    {
        int position = Random.Range(0, numberOfLocations - 1);
   

        Vector3 randomLocation = spawnsList[position].spawnPosition;

        return randomLocation;
    }

    private void SpawnNewAI()
    {
        //If a spawn location is blocked or not available
        //keep checking until one is available
        Vector3 spawnLocation = RandomSpawnLocation();
            
            if (CheckSpawnLocationIsFree(spawnLocation))
            {
                Instantiate(agent, spawnLocation, Quaternion.identity);
            }


    }
    bool CheckSpawnLocationIsFree(Vector3 spawnLocation)
    {
        if (Physics.SphereCast(spawnLocation,2,Vector3.zero, out RaycastHit hitInfo,1, obstruction))
        {
            return false;
        }
        return true;
    }
}
