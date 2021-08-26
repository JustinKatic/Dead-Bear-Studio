using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestorySelfAfterXNonPhoton : MonoBehaviour
{
    public float DestroySelfTime = .5f;
    private void OnEnable()
    {
        Invoke("DestroySelf", DestroySelfTime);
    }

    void DestroySelf()
    {
        Destroy(gameObject);
    }
}
