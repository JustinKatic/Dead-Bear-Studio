using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class KingAbilityEffect : MonoBehaviourPun
{
    private int attackerId;
    private float damageFrequency;
    private int damage;
    [SerializeField] private LayerMask layersCanDamage;
    private Animator anim;


    public void Initialize(int attackerId, float damageFrequency, float abilityDuration, int damage)
    {
        if (photonView.IsMine)
        {
            this.attackerId = attackerId;
            this.damageFrequency = damageFrequency;
            this.damage = damage;

            Invoke("EndAnimation", abilityDuration - 0.4f);
            Invoke("DestroySelf", abilityDuration);
            anim = GetComponent<Animator>();
        }
    }


    private void Start()
    {
        if (photonView.IsMine)
            InvokeRepeating("CallEffectOnMinionsAndPlayers", 0, damageFrequency);
    }
    void DestroySelf()
    {
        PhotonNetwork.Destroy(gameObject.transform.parent.gameObject);
    }

    void EndAnimation()
    {
        anim.Play("DKsecondFXEnd");
    }

    void CallEffectOnMinionsAndPlayers()
    {
        float radius = transform.localScale.x / 2;

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
