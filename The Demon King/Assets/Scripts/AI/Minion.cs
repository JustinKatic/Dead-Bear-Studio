using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Minion : MonoBehaviour
{
    public bool dead = false;
    public void Death()
    {
        dead = true;
        
        gameObject.SetActive(false);
        
    }

    public void Respawn(Vector3 spawnPosition)
    {
        transform.position = spawnPosition;
        gameObject.SetActive(true);
    }
    
}
