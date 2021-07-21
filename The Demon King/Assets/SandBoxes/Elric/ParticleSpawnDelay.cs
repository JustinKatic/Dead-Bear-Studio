using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSpawnDelay : MonoBehaviour
{
    public GameObject objToActivate;
    public float playAfterX;
    private void Awake()
    {
        Invoke("PlayFX", playAfterX);
    }

    void PlayFX()
    {
        objToActivate.SetActive(true);
    }
}
