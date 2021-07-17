using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AISpawner : MonoBehaviour
{
    [SerializeField] private GameObject agent = null;
    //[SerializeField] private int spawnLimit = 5;
    [SerializeField] private List<Transform> spawnPositions = new List<Transform>();

    private List<Vector3> spawnLocations = null;
    private int numberOfLocations = 0;



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
        Instantiate(agent, RandomSpawnLocation(), Quaternion.identity);

    }

    private int AgentCount()
    {
        int count = 0;
        
        return count;
    }
}
