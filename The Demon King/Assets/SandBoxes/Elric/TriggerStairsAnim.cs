using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerStairsAnim : MonoBehaviour
{
    public Animator anim;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            anim.Play("RisingStairs");
        }
    }
}
