using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultProjectileController : MonoBehaviourPun
{
    private int damage = 1;
    private int attackerId;


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
    public void Initialize(int damage, int attackerId)
    {
        this.damage = damage;
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
            DealDamageToPlayersAndMinions(other);

            PhotonNetwork.Instantiate("DefaultImpactFX", transform.position, Quaternion.identity);
            PhotonNetwork.Destroy(gameObject);
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
                playerHealth.TakeDamage(damage, attackerId);
                GameManager.instance.GetPlayer(attackerId).PlayRectAnim();
            }
        }
        //If tag is Minion
        else if (objTag.Equals("Minion"))
        {
            //tell the minion who was hit to take damage
            MinionHealthManager minionHealth = other.GetComponentInParent<MinionHealthManager>();
            minionHealth.TakeDamage(damage, attackerId);
            GameManager.instance.GetPlayer(attackerId).PlayRectAnim();
        }
    }
}
