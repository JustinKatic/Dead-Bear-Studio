using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class DragonProjectileController : MonoBehaviourPun
{
    private int attackerId;

    public GameObject EnemyGasEffect;
    public GameObject FriendlyGasEffect;

    private int projectileHitDmg;
    private int damage;
    private float damageFrequency;
    private float frequencyToReapplyGas;
    private float gasDuration;
    private float gasDurationOnPlayer;
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
    public void Initialize(int attackerId, int damage, float damageFrequency, float frequencyToReapplyGas, float gasDuration, float gasDurationOnPlayer, float gasSize, int projectileHitDmg)
    {
        this.projectileHitDmg = projectileHitDmg;
        this.attackerId = attackerId;
        this.damage = damage;
        this.damageFrequency = damageFrequency;
        this.frequencyToReapplyGas = frequencyToReapplyGas;
        this.gasDuration = gasDuration;
        this.gasDurationOnPlayer = gasDurationOnPlayer;
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

            DealDamageToPlayersAndMinions(other);

            SpawnGasEffect();

            FMODUnity.RuntimeManager.PlayOneShotAttached(OnTriggerSound, gameObject);

            PhotonNetwork.Destroy(gameObject);
        }
    }


    void SpawnGasEffect()
    {
        photonView.RPC("SpawnGasEffect_RPC", RpcTarget.All, transform.position.x, transform.position.y, transform.position.z, attackerId, damage, damageFrequency, frequencyToReapplyGas, gasDuration, gasDurationOnPlayer, gasSize);
    }

    [PunRPC]
    void SpawnGasEffect_RPC(float x, float y, float z, int attackerID, int damage, float damageFrequency, float frequencyToReapplyGas, float gasDuration, float gasDurationOnPlayer, float gasSize)
    {
        if (attackerId == GameManager.instance.myIdIndex)
        {
            GameObject CreatedGasEffect = Instantiate(FriendlyGasEffect, new Vector3(x, y, z), Quaternion.identity);
            CreatedGasEffect.GetComponent<DragonGasEffect>().Initialize(attackerID, damage, damageFrequency, frequencyToReapplyGas, gasDuration, gasDurationOnPlayer, gasSize);
        }
        else
        {
            GameObject CreatedGasEffect = Instantiate(EnemyGasEffect, new Vector3(x, y, z), Quaternion.identity);
            CreatedGasEffect.GetComponent<DragonGasEffect>().Initialize(attackerID, damage, damageFrequency, frequencyToReapplyGas, gasDuration, gasDurationOnPlayer, gasSize);
        }
    }

    void DealDamageToPlayersAndMinions(Collider other)
    {
        //stores refrence to tag collided with
        string objTag = other.transform.tag;

        if (objTag.Equals("Player"))
        {
            //tell the player who was hit to take damage
            PlayerHealthManager playerHealth = other.GetComponentInParent<PlayerHealthManager>();
            if (playerHealth.PlayerId != attackerId)
            {
                playerHealth.TakeDamage(projectileHitDmg, attackerId);
                GameManager.instance.GetPlayer(attackerId).PlayRectAnim();
            }
        }
        //If tag is Minion
        else if (objTag.Equals("Minion"))
        {
            //tell the minion who was hit to take damage
            MinionHealthManager minionHealth = other.GetComponentInParent<MinionHealthManager>();
            minionHealth.TakeDamage(projectileHitDmg, attackerId);
            GameManager.instance.GetPlayer(attackerId).PlayRectAnim();
        }
    }
}
