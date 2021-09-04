using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class RicochetProjectileController : MonoBehaviourPun
{
    private int damage = 1;
    private int attackerId;
    private int numberOfBouncesBeforeDestroys;
    private int currentNumberOfBounces;
    private int damageOverTimeDamage;
    private LayerMask layersGasCanDamage;



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
    public void Initialize(int damage, int attackerId, int numberOfBouncesBeforeDestroys, int damageOverTimeDamage, LayerMask layersGasCanDamage)
    {
        this.damage = damage;
        this.attackerId = attackerId;
        this.numberOfBouncesBeforeDestroys = numberOfBouncesBeforeDestroys;
        this.damageOverTimeDamage = damageOverTimeDamage;
        this.layersGasCanDamage = layersGasCanDamage;

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
            DealDamageToPlayersAndMinions(other);

            PhotonNetwork.Instantiate("DragonImpactFX", transform.position, Quaternion.identity);

            GameObject CreatedGasEffect = PhotonNetwork.Instantiate("DragonGas", transform.position, Quaternion.identity);
            CreatedGasEffect.GetComponent<GasEffect>().Initialize(attackerId, damageOverTimeDamage, layersGasCanDamage);

            FMODUnity.RuntimeManager.PlayOneShotAttached(OnTriggerSound, gameObject);

            if (currentNumberOfBounces >= numberOfBouncesBeforeDestroys)
                PhotonNetwork.Destroy(gameObject);

            currentNumberOfBounces++;
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
                playerHealth.TakeDamage(damage, attackerId);
        }
        //If tag is Minion
        else if (objTag.Equals("Minion"))
        {
            //tell the minion who was hit to take damage
            MinionHealthManager minionHealth = other.GetComponentInParent<MinionHealthManager>();
            minionHealth.TakeDamage(damage, attackerId);
        }
    }
}
