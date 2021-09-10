using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LionExplosionVFX : MonoBehaviour
{
    public ParticleSystem PsToChangeSizeOf;

    public void Init(float radius)
    {
        var main = PsToChangeSizeOf.main;
        main.startSize = radius;
    }
}
