using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerStairsAnim : MonoBehaviour
{
    public Animator anim;
    bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && triggered == false)
        {
            anim.Play("RisingStairs");
            triggered = true;
        }
    }
}
