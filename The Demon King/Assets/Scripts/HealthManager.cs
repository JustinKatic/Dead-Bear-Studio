using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviourPun
{

    [Header("HealthStats")]
    [Tooltip("Max health value")]
    public int MaxHealth = 3;
    [Tooltip("Time before health regen, Reset on dmg taken")]
    public float HealthRegenTimer = 3f;

    [Header("Stun/Devour Duration")]
    [Tooltip("Time spent being devoured")]
    public float DevourTime = 3f;
    public float stunnedDuration;


    protected float TimeBeforeHealthRegen = 3f;
    protected int curAttackerId;
    
    public Canvas HealthBar;
    public Transform HealthBarContainer;
    public Image healthBarPrefab;
    protected List<Image> healthBars = new List<Image>();
    protected List<Image> healthBarsOverhead = new List<Image>();
    public Transform HealthBarContainerOverhead;



    // [HideInInspector] public Slider statusBar = null;

     public int CurrentHealth = 0;
    [HideInInspector] public bool beingDevoured = false;
    [HideInInspector] public bool canBeDevoured = false;
    [HideInInspector] public bool isStunned = false;


    private void Update()
    {
        //Run following if local player
        if (photonView.IsMine)
        {
            if (beingDevoured || isStunned)
                return;
            //Heal every X seconds if not at max health
            if (CurrentHealth < MaxHealth)
            {
                HealthRegenTimer -= Time.deltaTime;
                if (HealthRegenTimer <= 0)
                {
                    if (!beingDevoured || !isStunned)
                        Heal(1);
                }
            }
        }
    }
    [PunRPC]
    public void UpdateHealthBar(int CurrentHealth)
    {
        //Run following if local player
        if (photonView.IsMine)
        {
            for (int i = 0; i < MaxHealth; i++)
            {
                //Change health bar red if the bar we are looking at is < currentHealth
                if (i < CurrentHealth)
                    healthBars[i].color = Color.red;
                //Change health bar transparent if the bar we are looking at is > currentHealth
                else
                    healthBars[i].color = new Color(255, 0, 0, 0);
            }
        }
        else
        {
            for (int i = 0; i < MaxHealth; i++)
            {
                //Change health bar red if the bar we are looking at is < currentHealth
                if (i < CurrentHealth)
                    healthBarsOverhead[i].color = Color.red;
                //Change health bar transparent if the bar we are looking at is > currentHealth
                else
                    healthBarsOverhead[i].color = new Color(255, 0, 0, 0);
            }
        }
    }
    [PunRPC]
    protected void SetHealth(int MaxHealthValue, int CurrentHealthValue)
    {
        //Run following on everyone
        MaxHealth = MaxHealthValue;

        if (CurrentHealth > MaxHealth)
            CurrentHealth = MaxHealth;


        //Run following if not local player
        if (!photonView.IsMine)
        {
            if (MaxHealth > MaxHealthValue)
            {
                foreach (Image healthBar in healthBarsOverhead)
                {
                    Destroy(healthBar.gameObject);
                }
                healthBarsOverhead.Clear();
            }
            //Adds additional health bars to playerhealthBarContainer.
            for (int i = healthBarsOverhead.Count; i < MaxHealthValue; i++)
            {
                Image healthBar = Instantiate(healthBarPrefab, HealthBarContainerOverhead);
                healthBarsOverhead.Add(healthBar);
            }
        }
        //Run following if local player
        else
        {
            if (MaxHealth > MaxHealthValue)
            {
                foreach (Image healthBar in healthBars)
                {
                    Destroy(healthBar.gameObject);
                }
                healthBars.Clear();
            }

            //Adds additional health bars to playerhealthBarContainer.
            for (int i = healthBars.Count; i < MaxHealthValue; i++)
            {
                Image healthBar = Instantiate(healthBarPrefab, HealthBarContainer);
                healthBars.Add(healthBar);
            }

            photonView.RPC("UpdateHealthBar", RpcTarget.All, CurrentHealth);
        }
    }

    //This is run when the player has been stunned
    [PunRPC]
    protected void Stunned()
    {
        if (!isStunned)
            StartCoroutine(StunnedCorutine());

        IEnumerator StunnedCorutine()
        {
            OnStunStart();

            yield return new WaitForSeconds(stunnedDuration);

            OnStunEnd();
        }
    }



    protected virtual void Heal(int amountToHeal)
    {

    }

    //Overrides for inherited classes
    protected virtual void OnStunStart()
    {

    }

    protected virtual void OnStunEnd()
    {

    }

    [PunRPC]
    protected virtual void InterruptDevourOnSelf()
    {

    }
}
