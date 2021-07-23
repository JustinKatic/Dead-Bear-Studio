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
    public int MaxHealth = 3;
    public int CurrentHealth = 0;
    public float HealthRegenTimer = 3f;
    protected float TimeBeforeHealthRegen = 3f;

    public float DevourTime = 3f;


    [Header("StunStats")]
    public float stunnedDuration;

    public bool Dead = false;
    protected int curAttackerId;

    public TMP_Text OverheadText = null;
    public Slider statusBar =  null;

    public bool beingDevoured = false;
    public bool canBeDevoured = false;

    public bool isStunned = false;


    public void SetStatusStartValues()
    {
        statusBar.maxValue = MaxHealth;
        statusBar.value = CurrentHealth;
    }

    //Updates the players text to everyone on the server
    [PunRPC]
    public void UpdateStatusBar(int value)
    {
        statusBar.value = value;
    }

    //Updates the players text to everyone on the server
    [PunRPC]
    public void UpdateOverheadText(string textToDisplay)
    {
        //OverheadText.text = textToDisplay;
    }

    [PunRPC]
    public void Heal(int amountToHeal)
    {
        CurrentHealth = Mathf.Clamp(CurrentHealth + amountToHeal, 0, MaxHealth);
        photonView.RPC("UpdateStatusBar", RpcTarget.All, CurrentHealth);
        statusBar.value = CurrentHealth;
        HealthRegenTimer = TimeBeforeHealthRegen;
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

    [PunRPC]
    public void TakeDamage(int attackerId, int damage)
    {
        if (beingDevoured)
            return;

        CurrentHealth -= damage;
        curAttackerId = attackerId;

        HealthRegenTimer = TimeBeforeHealthRegen;

        photonView.RPC("UpdateStatusBar", RpcTarget.All, CurrentHealth);

        //die if no health left
        if (CurrentHealth <= 0)
            photonView.RPC("Stunned", RpcTarget.All);
    }

    [PunRPC]
    public void TakeDamage(int damage)
    {
        if (beingDevoured)
            return;

        CurrentHealth -= damage;

        HealthRegenTimer = TimeBeforeHealthRegen;

        photonView.RPC("UpdateStatusBar", RpcTarget.All, CurrentHealth);

        //die if no health left
        if (CurrentHealth <= 0)
            photonView.RPC("Stunned", RpcTarget.All);
    }

    [PunRPC]
    protected void ChangeStatusBarStun()
    {
        var fill = statusBar.GetComponentsInChildren<Image>().FirstOrDefault(t => t.name == "Fill");
        
        if (fill != null)
        {
            fill.color = Color.yellow;
        }
        statusBar.maxValue = stunnedDuration;
        statusBar.value = stunnedDuration;
    }
    [PunRPC]
    protected void ChangeStatusBarHealth()
    {
        var fill = statusBar.GetComponentsInChildren<Image>().FirstOrDefault(t => t.name == "Fill");
        
        if (fill != null)
        {
            fill.color = Color.red;
        }

        statusBar.maxValue = MaxHealth;
        statusBar.value = CurrentHealth;
    }

    protected virtual void OnStunStart()
    {
        
    }

    protected virtual void InterruptedDevour()
    {

    }

    protected virtual void OnStunEnd()
    {

    }
    
}
