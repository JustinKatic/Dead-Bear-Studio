using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class DragonProjectileController : MonoBehaviourPun
{
    private int attackerId;

    public GameObject EnemyGasEffect;
    public GameObject FriendlyGasEffect;

    private int damage;
    private float damageFrequency;
    private float gasDuration;
    private float gasSize;


    [FMODUnity.EventRef]
    [SerializeField] string OnTriggerSound;

    [FMODUnity.EventRef]
    [SerializeField] string AbilityTravelSound;

    FMOD.Studio.EventInstance abilityTravellingEvent;

    public Rigidbody rb;

    private void OnEnable()
    {
        rb = GetComponent<Rigidbody>();
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
    }

    // Called when the bullet is spawned by the player who spawned it
    public void Initialize(int attackerId, int damage, float damageFrequency, float gasDuration, float gasSize)
    {
        this.attackerId = attackerId;
        this.damage = damage;
        this.damageFrequency = damageFrequency;
        this.gasDuration = gasDuration;
        this.gasSize = gasSize;


        // set a lifetime of bullet
        if (photonView.IsMine)
            Invoke("DestroyBullet", 5f);
    }

    void DestroyBullet()
    {
        PhotonNetwork.Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (photonView.IsMine)
        {
            PhotonNetwork.Instantiate("DragonImpactFX", transform.position, Quaternion.identity);

            SpawnGasEffect();

            FMODUnity.RuntimeManager.PlayOneShotAttached(OnTriggerSound, gameObject);

            PhotonNetwork.Destroy(gameObject);
        }
    }

    void SpawnGasEffect()
    {
        photonView.RPC("SpawnGasEffect_RPC", RpcTarget.All, transform.position.x, transform.position.y, transform.position.z, attackerId, damage, damageFrequency, gasDuration, gasSize);
    }

    [PunRPC]
    void SpawnGasEffect_RPC(float x, float y, float z, int attackerID, int damage, float damageFrequency, float gasDuration, float gasSize)
    {
        if (attackerId == PhotonNetwork.LocalPlayer.ActorNumber)
        {
            GameObject CreatedGasEffect = Instantiate(FriendlyGasEffect, new Vector3(x, y, z), Quaternion.identity);
            CreatedGasEffect.GetComponent<DragonGasEffect>().Initialize(attackerID, damage, damageFrequency, gasDuration, gasSize);
        }
        else
        {
            GameObject CreatedGasEffect = Instantiate(EnemyGasEffect, new Vector3(x, y, z), Quaternion.identity);
            CreatedGasEffect.GetComponent<DragonGasEffect>().Initialize(attackerID, damage, damageFrequency, gasDuration, gasSize);
        }
    }
}
