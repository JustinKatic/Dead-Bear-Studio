using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AISpawner : MonoBehaviour
{
    [SerializeField] private GameObject agent = null;
    [SerializeField] private int spawnLimit = 5;
    [SerializeField] private List<Transform> spawnPositions = new List<Transform>();
    [SerializeField] private NavMeshSurface area;
    

    private List<Vector3> spawnLocations = null;
    private int numberOfLocations = 0;
    private LayerMask obstruction;
    
    // Start is called before the first frame update
    void Start()
    {
        spawnLocations = new List<Vector3>();
        foreach (var position in spawnPositions)
        {
            spawnLocations.Add(position.position);
        }

        numberOfLocations = spawnLocations.Count;
    }

    private Vector3 RandomSpawnLocation()
    {
        int position = Random.Range(0, numberOfLocations - 1);

        Vector3 randomLocation = spawnLocations[position];

        return randomLocation;
    }

    private void SpawnNewAI()
    {
        bool spawnLocationAvailable = false;
        //If a spawn location is blocked or not available
        //keep checking until one is available
        while (!spawnLocationAvailable)
        {
            Vector3 spawnLocation = RandomSpawnLocation();
            
            if (CheckSpawnLocationIsFree(spawnLocation))
            {
                spawnLocationAvailable = true;
                Instantiate(agent, spawnLocation, Quaternion.identity);
            }

        }
        spawnLocationAvailable = false;

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
