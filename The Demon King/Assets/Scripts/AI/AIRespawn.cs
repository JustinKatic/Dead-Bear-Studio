using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class AIRespawn : MonoBehaviourPun
{
    public float respawnTimer;

    public GameObject model;
    private Collider col;
    private Canvas hudCanvas;
    private HealthManager healthManager;

    void Awake()
    {

        healthManager = GetComponent<HealthManager>();
    }



}
