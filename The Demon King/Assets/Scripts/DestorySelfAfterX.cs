using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestorySelfAfterX : MonoBehaviour
{
    public float DestroySelfTime = 5f;
    private void OnEnable()
    {
        Destroy(gameObject, DestroySelfTime);
    }
}
