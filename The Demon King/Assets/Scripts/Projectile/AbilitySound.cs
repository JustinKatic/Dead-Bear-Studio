using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilitySound : MonoBehaviour
{
    [FMODUnity.EventRef]
    [SerializeField] string OnDestorySound;

    [FMODUnity.EventRef]
    [SerializeField] string AbilityTravelSound;

    FMOD.Studio.EventInstance abilityTravellingEvent;

    private void OnEnable()
    {
        abilityTravellingEvent = FMODUnity.RuntimeManager.CreateInstance(AbilityTravelSound);
        abilityTravellingEvent.start();
    }

    private void Update()
    {
        abilityTravellingEvent.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
    }


    private void OnDestroy()
    {
        abilityTravellingEvent.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        abilityTravellingEvent.release();
        FMODUnity.RuntimeManager.PlayOneShotAttached(OnDestorySound, gameObject);
    }
}
