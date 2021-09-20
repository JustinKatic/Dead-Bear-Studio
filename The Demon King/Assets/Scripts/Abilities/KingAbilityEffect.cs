using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class KingAbilityEffect : MonoBehaviourPun
{
    private int attackerId;
    private float damageFrequency;
    private float radius;
    private int damage;
    [SerializeField] private LayerMask layersCanDamage;


    public void Initialize(int attackerId, float damageFrequency, float abilityDuration, int damage)
    {
        if (photonView.IsMine)
        {
            this.attackerId = attackerId;
            this.damageFrequency = damageFrequency;
            this.damage = damage;

            radius = transform.localScale.x / 2;
            Invoke("DestroySelf", abilityDuration);
        }
    }


    private void Start()
    {
        if (photonView.IsMine)
            InvokeRepeating("CallEffectOnMinionsAndPlayers", 0, damageFrequency);
    }
    void DestroySelf()
    {
        PhotonNetwork.Destroy(gameObject);
    }

    void CallEffectOnMinionsAndPlayers()
    {
        Collider[] cols = Physics.OverlapSphere(transform.position, radius, layersCanDamage);
        //stores refrence to tag collided with

        foreach (var col in cols)
        {
            string objTag = col.transform.tag;

            if (objTag.Equals("PlayerParent"))
            {
                //tell the player who was hit to take damage
                PlayerHealthManager playerHealth = col.GetComponentInParent<PlayerHealthManager>();
                if (playerHealth.PlayerId != attackerId)
                {
                    playerHealth.TakeDamage(damage, attackerId);
                }
            }
            //If tag is Minion
            if (objTag.Equals("Minion"))
            {
                //tell the minion who was hit to take damage
                MinionHealthManager minionHealth = col.GetComponentInParent<MinionHealthManager>();
                //call gas effect
                minionHealth.TakeDamage(damage, attackerId);
            }
        }
    }
}
