using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class HeavyProjectileController : MonoBehaviourPun
{
    private int damage = 1;
    private int attackerId;
    private bool isMine;
    public LayerMask damageable;

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

        // set a lifetime so it can't go on forever
        Destroy(gameObject, 5.0f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (photonView.IsMine)
        {
            //Get list of all colliders who have layer damageable layer
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, 4, damageable);
            Debug.Log("Exploded");

            foreach (Collider col in hitColliders)
            {
                //store tag of collider
                string objTag = col.transform.tag;

                //If hit another player
                if (objTag.Equals("Player"))
                {
                    //tell the player who was hit to take damage
                    PlayerController player = GameManager.instance.GetPlayer(col.gameObject);
                    if (player.id != attackerId)
                        player.photonView.RPC("TakeDamage", player.photonPlayer, attackerId, damage);
                }
                //tell the minion who was hit to take damage
                else if (objTag.Equals("Minion"))
                {
                    //call take damage on the minion that was hit
                    PhotonView hitView = col.gameObject.GetComponent<PhotonView>();
                    hitView.RPC("TakeDamage", RpcTarget.All, damage);
                }
            }
            PhotonNetwork.Instantiate("FireballExplosionFX", transform.position, Quaternion.identity);
            PhotonNetwork.Destroy(gameObject);
        }
    }
}


