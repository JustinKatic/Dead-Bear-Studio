using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetEmoteInactive : MonoBehaviour
{
    private Animation floatingAnimation;
    // Start is called before the first frame update
    void Start()
    {
        floatingAnimation = GetComponent<Animation>();
    }

    private void Update()
    {
        if (!floatingAnimation.isPlaying)
        {
            gameObject.SetActive(false);
        }
    }
}
