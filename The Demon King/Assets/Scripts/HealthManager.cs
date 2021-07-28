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
    public Canvas overheadHealthBar;


    // [HideInInspector] public Slider statusBar = null;

     public int CurrentHealth = 0;
    [HideInInspector] public bool beingDevoured = false;
    [HideInInspector] public bool canBeDevoured = false;
    [HideInInspector] public bool isStunned = false;

    protected IEnumerator myDevourCo;
    
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
    
    protected void Heal(int amountToHeal)
    {
        //Only running on local player
        CurrentHealth = Mathf.Clamp(CurrentHealth + amountToHeal, 0, MaxHealth);
        //Updates this charcters health bars on all players in network
        photonView.RPC("UpdateHealthBar", RpcTarget.All, CurrentHealth);
        HealthRegenTimer = TimeBeforeHealthRegen;
    }

    //Overrides for inherited classes
    protected virtual void OnStunStart()
    {

    }

    protected virtual void OnStunEnd()
    {

    }

    [PunRPC]
    protected void InterruptDevourOnSelf()
    {
        beingDevoured = false;
        StopCoroutine(myDevourCo);
    }
}
