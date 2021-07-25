using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class PrimaryProjectileController : MonoBehaviour
{
    private int damage = 1;
    private int attackerId;
    private bool isMine;


    public Rigidbody rb;

    public GameObject impactFX;

    private void OnEnable()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Called when the bullet is spawned by the player who spawned it
    public void Initialize(int damage, int attackerId, bool isMine)
    {
        this.damage = damage;
        this.attackerId = attackerId;
        this.isMine = isMine;

        // set a lifetime of bullet
        Destroy(gameObject, 5.0f);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (isMine)
        {
            //stores refrence to tag collided with
            string objTag = other.transform.tag;

            //If tag is Player
            if (objTag.Equals("Player"))
            {
                //tell the player who was hit to take damage
                PlayerController player = GameManager.instance.GetPlayer(other.gameObject);
                if (player.id != attackerId)
                    player.photonView.RPC("TakeDamage", player.photonPlayer, attackerId, damage);
            }
            //If tag is Minion
            else if (objTag.Equals("Minion"))
            {
                //tell the minion who was hit to take damage
                PhotonView hitView = other.gameObject.GetComponent<PhotonView>();
                hitView.RPC("TakeDamage", RpcTarget.All, damage);
            }
        }
        //Spawn explision vfx
        Instantiate(impactFX, transform.position, Quaternion.identity);
        //Detroy this bullet
        Destroy(gameObject);
    }
}


