using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonGasEffect : MonoBehaviour
{
    private int attackerId;
    [SerializeField] private int damageOverTimeDamage;
    [SerializeField] private float damageFrequency = 1f;
    [SerializeField] private LayerMask layersGasCanDamage;
    private float radius;

    public void Initialize(int attackerId)
    {
        this.attackerId = attackerId;
    }

    private void OnEnable()
    {
        radius = transform.localScale.x / 2;
    }
    private void Start()
    {
        InvokeRepeating("DealDamageToPlayersAndMinions", 0, damageFrequency);
    }

    void DealDamageToPlayersAndMinions()
    {
        Collider[] cols = Physics.OverlapSphere(transform.position, radius, layersGasCanDamage);
        //stores refrence to tag collided with

        foreach (var col in cols)
        {
            string objTag = col.transform.tag;

            if (objTag.Equals("PlayerParent"))
            {
                //tell the player who was hit to take damage
                PlayerHealthManager playerHealth = col.GetComponentInParent<PlayerHealthManager>();
                if (playerHealth.PlayerId != attackerId)
                    playerHealth.TakeDamage(damageOverTimeDamage, attackerId);
            }
            //If tag is Minion
            if (objTag.Equals("Minion"))
            {
                //tell the minion who was hit to take damage
                MinionHealthManager minionHealth = col.GetComponentInParent<MinionHealthManager>();
                minionHealth.TakeDamage(damageOverTimeDamage, attackerId);
            }
        }
    }
}
