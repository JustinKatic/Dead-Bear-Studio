using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnTriggerEvent : MonoBehaviour
{
    public UnityEvent OnEnter;
    public UnityEvent OnExit;


    private void OnTriggerEnter(Collider other)
    {
        OnEnter.Invoke();
    }
    private void OnTriggerExit(Collider other)
    {
        OnExit.Invoke();
    }
}
