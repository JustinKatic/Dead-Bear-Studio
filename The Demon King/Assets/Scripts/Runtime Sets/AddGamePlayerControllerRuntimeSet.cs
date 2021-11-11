using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddGamePlayerControllerRuntimeSet : MonoBehaviour
{

    public PlayerControllerRuntimeSet playerControllerRuntimeSet;
    private void OnEnable()
    {
        playerControllerRuntimeSet.AddToList(gameObject.GetComponent<PlayerController>());
    }
    private void OnDisable()
    {
        playerControllerRuntimeSet.RemoveFromList(gameObject.GetComponent<PlayerController>());
    }
}
