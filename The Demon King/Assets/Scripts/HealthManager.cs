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

    [HideInInspector] public Slider statusBar = null;

    [HideInInspector] public int CurrentHealth = 0;
    [HideInInspector] public bool beingDevoured = false;
    [HideInInspector] public bool canBeDevoured = false;
    [HideInInspector] public bool isStunned = false;


    protected virtual void Awake()
    {
        //Get value of everyones status bar and set there sliders max length and values so accurate when adjust later in rpc calls
        statusBar = GetComponentInChildren<Slider>();
        CurrentHealth = MaxHealth;
        statusBar.maxValue = MaxHealth;
        statusBar.value = CurrentHealth;
    }

    private void Update()
    {
        //Run following if local player
        if (photonView.IsMine)
        {
            //Heal every X seconds if not at max health
            if (CurrentHealth < MaxHealth)
            {
                HealthRegenTimer -= Time.deltaTime;
                if (HealthRegenTimer <= 0)
                    Heal(1);
            }
        }
    }
    //Set overhead start values
    public void SetStartValues()
    {
        CurrentHealth = MaxHealth;
        statusBar.maxValue = MaxHealth;
        statusBar.value = CurrentHealth;
    }

    //Updates the players status to everyone on the server
    [PunRPC]
    public virtual void UpdateStatusBar(int value)
    {
        statusBar.value = value;
    }



   protected virtual void Heal(int amountToHeal)
    {

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



   


    //Overrides for inherited classes
    protected virtual void OnStunStart()
    {

    }

    [PunRPC]
    protected virtual void InterruptDevourOnSelf()
    {

    }

    protected virtual void OnStunEnd()
    {

    }

}
