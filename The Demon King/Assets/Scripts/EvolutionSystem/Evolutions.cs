using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class Evolutions : MonoBehaviour
{
    public MinionType MyType;
    public int MaxHealth;

    private List<SkinnedMeshRenderer> skinMeshes = new List<SkinnedMeshRenderer>();
    
    [HideInInspector] public Animator animator;
    [HideInInspector] public Transform ShootPoint;
    

    private void Awake()
    {
        skinMeshes = GetComponentsInChildren<SkinnedMeshRenderer>().ToList();
        animator = GetComponent<Animator>();
        ShootPoint = GameObject.FindGameObjectWithTag("shootPoint").transform;
    }
}
