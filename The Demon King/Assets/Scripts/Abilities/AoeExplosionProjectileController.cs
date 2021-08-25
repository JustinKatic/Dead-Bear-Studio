using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class AoeExplosionProjectileController : MonoBehaviourPun
{
    private int damage = 1;
    private int attackerId;
    [SerializeField] float aoeRadius;
    [SerializeField] LayerMask damageableObjects;


    public Rigidbody rb;
    public GameObject impactFX;

    private void OnEnable()
    {
        rb = GetComponent<Rigidbody>();
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

            Collider[] colliders = Physics.OverlapSphere(transform.position, aoeRadius, damageableObjects);
            foreach (Collider col in colliders)
            {
                DealDamageToPlayersAndMinions(col);
            }
            PhotonNetwork.Instantiate("FireballExplosionFX", transform.position, Quaternion.identity);
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
            PlayerHealthManager playerHealth = other.GetComponent<PlayerHealthManager>();
            if (playerHealth.PlayerId != attackerId)
                playerHealth.TakeDamage(damage, attackerId);
        }
        //If tag is Minion
        else if (objTag.Equals("Minion"))
        {
            //tell the minion who was hit to take damage
            MinionHealthManager minionHealth = other.gameObject.GetComponent<MinionHealthManager>();
            minionHealth.TakeDamage(damage, attackerId);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, aoeRadius);
    }
}