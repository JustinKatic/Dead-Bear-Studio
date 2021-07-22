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
    [SerializeField] private string agent = null;
    [SerializeField] public List<SpawnLocation> spawnsList = new List<SpawnLocation>();
    [SerializeField] private float timeToRespawn;

    private List<Minion> minions = new List<Minion>();
    private int numberOfLocations = 0;
    public LayerMask obstruction;

    [SerializeField] private float sphereCheck;
    
    
    // Start is called before the first frame update
    void Start()
    {
        //Spawn AI at each Location on the given Navmesh
        foreach (var spawn in spawnsList)
        { 
            Vector3 pos = spawn.transform.position;
            GameObject minion = PhotonNetwork.Instantiate(agent, pos , Quaternion.identity);
            minions.Add(minion.GetComponent<Minion>());
        }
        //Get the number of Spawns
        numberOfLocations = spawnsList.Count;

    }

    private void Update()
    {
        foreach (var minion in minions)
        {
            if (!minion.gameObject.activeSelf)
            {
                RespawnMinion(minion);
            }
        }
    }

    //Get a random position that is available and return that position for minion
    private Vector3 RandomSpawnLocation()
    {
        int position = 0;
        
        position = Random.Range(0, numberOfLocations);

        Vector3 randomLocation = Vector3.zero;
        
        //If a spawn location is blocked or not available
        //keep checking until one is available
        while (spawnsList[position].spawnInUse)
        {
            position = Random.Range(0, numberOfLocations);
            spawnsList[position].spawnInUse = CheckSpawnLocationIsFree(spawnsList[position].spawnPosition);
        }
        
        // Set the random position to this Spawn Position
        randomLocation = spawnsList[position].spawnPosition;
        
        return randomLocation;
    }

    //Check if any objects are blocking the spawn position
    bool CheckSpawnLocationIsFree(Vector3 spawnLocation)
    {
        if (Physics.SphereCast(spawnLocation,sphereCheck,Vector3.zero, out RaycastHit hitInfo,4, obstruction))
        {
            return false;
        }
        return true;
    }
    
    public void RespawnMinion(Minion minion)
    {
        StartCoroutine(SpawnTimer(minion));
    }
    
    IEnumerator SpawnTimer(Minion minion)
    {
        Vector3 spawnPos = RandomSpawnLocation();
        yield return new WaitForSeconds(timeToRespawn);
        minion.transform.position = spawnPos;
        minion.gameObject.SetActive(true);
        minion.photonView.RPC( "RespawnThisMinion",RpcTarget.All);
    }

    private void OnDrawGizmos()
    {
        foreach (var spawn in spawnsList)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(spawn.transform.position, sphereCheck);
        }
    }
}
