using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class AISpawner : MonoBehaviour
{
    [SerializeField] private GameObject agent = null;
    [SerializeField] public List<SpawnLocation> spawnsList = new List<SpawnLocation>();
    [SerializeField] private float timeToRespawn;

    private List<GameObject> minions = new List<GameObject>();
    private int numberOfLocations = 0;
    private LayerMask obstruction;
    
    // Start is called before the first frame update
    void Start()
    {
        //Spawn AI at each Location on the given Navmesh
        foreach (var spawn in spawnsList)
        {
           GameObject minion = Instantiate(agent, spawn.spawnPosition, Quaternion.identity);
           
           minions.Add(minion);
        }
        //Get the number of Spawns
        numberOfLocations = spawnsList.Count;

    }

    private void Update()
    {
        foreach (var minion in minions)
        {
            if (!minion.activeSelf)
            {
                RespawnMinion(minion);
            }
        }
    }

    //Get a random position that is available and return that position for minion
    private Vector3 RandomSpawnLocation()
    {
        int position = 0;
        
        position = Random.Range(0, numberOfLocations - 1);

        Vector3 randomLocation = Vector3.zero;
        
        //If a spawn location is blocked or not available
        //keep checking until one is available
        while (spawnsList[position].spawnInUse)
        {
            position = Random.Range(0, numberOfLocations - 1);
            spawnsList[position].spawnInUse = CheckSpawnLocationIsFree(spawnsList[position].spawnPosition);
        }
        
        // Set the random position to this Spawn Position
        randomLocation = spawnsList[position].spawnPosition;
        
        return randomLocation;
    }

    //Check if any objects are blocking the spawn position
    bool CheckSpawnLocationIsFree(Vector3 spawnLocation)
    {
        if (Physics.SphereCast(spawnLocation,5,Vector3.zero, out RaycastHit hitInfo,4, obstruction))
        {
            return false;
        }
        return true;
    }

    public void RespawnMinion(GameObject minion)
    {
        StartCoroutine(SpawnTimer(minion));
    }
    
    IEnumerator SpawnTimer(GameObject minion)
    {
        minion.transform.position = RandomSpawnLocation();
        yield return new WaitForSeconds(timeToRespawn);
        minion.SetActive(true);
    }

}
