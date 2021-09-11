using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddGameObjectRuntimeSet : MonoBehaviour
{

    public GameObjectRuntimeSet gameObjectRuntimeSet;
    private void OnEnable()
    {
        gameObjectRuntimeSet.AddToList(gameObject);
    }
    private void OnDisable()
    {
        gameObjectRuntimeSet.RemoveFromList(gameObject);

    }
}
