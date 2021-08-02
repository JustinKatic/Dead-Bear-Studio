using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AiBehaviorManager : MonoBehaviour
{
    private List<Behaviour> behaviours = new List<Behaviour>();

    private Behaviour activeBehaviour;

    // Start is called before the first frame update
    void Start()
    {
        behaviours = GetComponentsInChildren<Behaviour>().ToList();

        foreach (var behaviour in behaviours)
        {
            if (behaviour.enabled == true)
            {
                activeBehaviour = behaviour;
                return;
            }
        }
    }

    private void Update()
    {
        activeBehaviour.RunBehaviour();
    }

    void ChangeBehaviour(Behaviour newBehaviour)
    {
        activeBehaviour.enabled = false;
        activeBehaviour = newBehaviour;
        activeBehaviour.enabled = true;
    }
    
    
}
