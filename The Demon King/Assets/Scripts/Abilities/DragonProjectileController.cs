using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class DragonProjectileController : MonoBehaviourPun
{
    private int attackerId;

    public GameObject EnemyGasEffect;
    public GameObject FriendlyGasEffect;



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
    public void Initialize(int attackerId)
    {
        this.attackerId = attackerId;

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

            SpawnGasEffect(transform.position.x, transform.position.y, transform.position.z);

            FMODUnity.RuntimeManager.PlayOneShotAttached(OnTriggerSound, gameObject);

            PhotonNetwork.Destroy(gameObject);
        }
    }

    void SpawnGasEffect(float x, float y, float z)
    {
        photonView.RPC("SpawnGasEffect_RPC", RpcTarget.All, x, y, z, attackerId);
    }

    [PunRPC]
    void SpawnGasEffect_RPC(float x, float y, float z, int attackerID)
    {
        if (attackerId == PhotonNetwork.LocalPlayer.ActorNumber)
        {
            GameObject CreatedGasEffect = Instantiate(FriendlyGasEffect, new Vector3(x, y, z), Quaternion.identity);
            CreatedGasEffect.GetComponent<DragonGasEffect>().Initialize(attackerID);
        }
        else
        {
            GameObject CreatedGasEffect = Instantiate(EnemyGasEffect, new Vector3(x, y, z), Quaternion.identity);
            CreatedGasEffect.GetComponent<DragonGasEffect>().Initialize(attackerID);
        }
    }
}
