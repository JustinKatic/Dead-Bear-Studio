using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class HeavyProjectileController : MonoBehaviour
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

    // called when the bullet is spawned
    public void Initialize(int damage, int attackerId, bool isMine)
    {
        this.damage = damage;
        this.attackerId = attackerId;
        this.isMine = isMine;

        // set a lifetime so it can't go on forever
        Destroy(gameObject, 5.0f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isMine)
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, 4, damageable);
            foreach (Collider col in hitColliders)
            {
                string objTag = col.transform.tag;
                Debug.Log(objTag);

                if (objTag.Equals("Player"))
                {
                    PlayerController player = GameManager.instance.GetPlayer(col.gameObject);
                    if (player.id != attackerId)
                        player.photonView.RPC("TakeDamage", player.photonPlayer, attackerId, damage);
                }
                else if (objTag.Equals("Minion"))
                {
                    PhotonView hitView = col.gameObject.GetComponent<PhotonView>();
                    hitView.RPC("TakeDamage", RpcTarget.All, damage);
                }
            }
            Instantiate(impactFX, gameObject.transform.position, Quaternion.identity);
            Destroy(gameObject);
        }

    }
}
