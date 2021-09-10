using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonGasEffect : MonoBehaviour
{
    private int attackerId;
    private int damageOverTimeDamage;
    private float damageFrequency;
    private float frequencyToReapplyGas;
    private float gasDuration;
    private float gasSize;
    private float gasDurationOnPlayer;

    [SerializeField] private LayerMask layersGasCanDamage;
    private float radius;

    public void Initialize(int attackerId, int damageOverTimeDamage, float damageFrequency, float frequencyToReapplyGas, float gasDuration, float gasDurationOnPlayer, float gasSize)
    {
        this.attackerId = attackerId;
        this.damageOverTimeDamage = damageOverTimeDamage;
        this.damageFrequency = damageFrequency;
        this.frequencyToReapplyGas = frequencyToReapplyGas;
        this.gasDuration = gasDuration;
        this.gasDurationOnPlayer = gasDurationOnPlayer;
        this.gasSize = gasSize;
        transform.localScale = new Vector3(gasSize, gasSize, gasSize);
        radius = transform.localScale.x / 2;
        Invoke("DestroySelf", gasDuration);
    }

    void DestroySelf()
    {
        Destroy(gameObject);
    }

    private void Start()
    {
        InvokeRepeating("CallGasEffectOnMinionsAndPlayers", 0, frequencyToReapplyGas);
    }

    void CallGasEffectOnMinionsAndPlayers()
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
                {
                    playerHealth.ApplyGasEffect(damageOverTimeDamage, attackerId, damageFrequency, gasDurationOnPlayer);
                }
            }
            //If tag is Minion
            if (objTag.Equals("Minion"))
            {
                //tell the minion who was hit to take damage
                MinionHealthManager minionHealth = col.GetComponentInParent<MinionHealthManager>();
                //call gas effect
                minionHealth.ApplyGasEffect(damageOverTimeDamage, attackerId, damageFrequency, gasDurationOnPlayer);
            }
        }
    }
}
