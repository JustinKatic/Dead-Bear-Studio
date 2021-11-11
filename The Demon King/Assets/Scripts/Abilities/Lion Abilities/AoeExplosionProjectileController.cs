using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class AoeExplosionProjectileController : MonoBehaviourPun
{
    public PlayerControllerRuntimeSet playerControllerRuntimeSet;

    private int damage = 1;
    private int aoeDamage = 1;

    private int attackerId;
    private float aoeRadius;
    [SerializeField] LayerMask damageableObjects;


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
        FMODUnity.RuntimeManager.PlayOneShotAttached(OnTriggerSound, gameObject);
    }

    // Called when the bullet is spawned by the player who spawned it
    public void Initialize(int damage, int aoeDamage, int attackerId, float aoeRadius)
    {
        this.damage = damage;
        this.aoeDamage = aoeDamage;
        this.attackerId = attackerId;
        this.aoeRadius = aoeRadius;

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
            DealDamageToPlayersAndMinions(other, damage, false);

            Collider[] colliders = Physics.OverlapSphere(transform.position, aoeRadius, damageableObjects);
            foreach (Collider col in colliders)
            {
                DealDamageToPlayersAndMinions(col, aoeDamage, true);
            }
            GameObject impactFX = PhotonNetwork.Instantiate("LionImpactFX", transform.position, Quaternion.identity);
            impactFX.GetComponent<LionExplosionVFX>().Init(aoeRadius);

            PhotonNetwork.Destroy(gameObject);
        }
    }

    void DealDamageToPlayersAndMinions(Collider other, int damage, bool aoeDmg)
    {
        //stores refrence to tag collided with
        string objTag = other.transform.tag;

        if (objTag.Equals("Player") || objTag.Equals("PlayerParent"))
        {
            //tell the player who was hit to take damage
            PlayerHealthManager playerHealth = other.GetComponentInParent<PlayerHealthManager>();
            if (playerHealth.PlayerId != attackerId)
            {
                playerHealth.TakeDamage(damage, attackerId);
                if (!aoeDmg)
                    playerControllerRuntimeSet.GetPlayer(attackerId).PlayRectAnim();

                playerControllerRuntimeSet.GetPlayer(attackerId).IncreaseLionDamage(damage);
            }
        }
        //If tag is Minion
        else if (objTag.Equals("Minion"))
        {
            //tell the minion who was hit to take damage
            MinionHealthManager minionHealth = other.GetComponentInParent<MinionHealthManager>();
            minionHealth.TakeDamage(damage, attackerId);
            if (!aoeDmg)
                playerControllerRuntimeSet.GetPlayer(attackerId).PlayRectAnim();

            playerControllerRuntimeSet.GetPlayer(attackerId).IncreaseLionDamage(damage);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, aoeRadius);
    }
}