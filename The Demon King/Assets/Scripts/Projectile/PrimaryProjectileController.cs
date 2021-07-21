using System.Collections;
using System.Collections.Generic;
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

    // called when the bullet is spawned
    public void Initialize(int damage, int attackerId, bool isMine)
    {
        this.damage = damage;
        this.attackerId = attackerId;
        this.isMine = isMine;

        // set a lifetime so it can't go on forever
        Destroy(gameObject, 5.0f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isMine && collision.gameObject.CompareTag("Player"))
        {
            PlayerController player = GameManager.instance.GetPlayer(collision.gameObject);
            if (player.id != attackerId)
                player.photonView.RPC("TakeDamage", player.photonPlayer, attackerId, damage);
        }
        Instantiate(impactFX, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}

