using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class PlayerHealthManager : MonoBehaviourPun
{
    public int MaxHealth = 3;
    public int CurrentHealth = 0;
    public TMP_Text OverheadText = null;
    public bool Dead = false;
    public bool stunned = false;
    public float stunnedDuration;

    private float TimeBeforeHealthRegen = 3f;
    public float HealthRegenTimer = 3f;

    private int curAttackerId;

    private PlayerController player;

    private void Awake()
    {
        player = GetComponent<PlayerController>();
        CurrentHealth = MaxHealth;
        OverheadText.text = CurrentHealth.ToString();
    }

    private void Update()
    {
        if (CurrentHealth < MaxHealth)
        {
            HealthRegenTimer -= Time.deltaTime;
            if (HealthRegenTimer <= 0 && !stunned)
            {
                player.photonView.RPC("Heal", RpcTarget.All, 1);
            }
        }
    }

    [PunRPC]
    public void TakeDamage(int attackerId, int damage)
    {
        if (Dead)
            return;

        CurrentHealth -= damage;
        curAttackerId = attackerId;

        HealthRegenTimer = TimeBeforeHealthRegen;

        OverheadText.text = CurrentHealth.ToString();

        //die if no health left
        if (CurrentHealth <= 0)
            photonView.RPC("Stunned", RpcTarget.All);
    }


    [PunRPC]
    public void Heal(int amountToHeal)
    {
        CurrentHealth = Mathf.Clamp(CurrentHealth + amountToHeal, 0, MaxHealth);
        OverheadText.text = CurrentHealth.ToString();
        HealthRegenTimer = TimeBeforeHealthRegen;
    }


    [PunRPC]
    void Stunned()
    {
        StartCoroutine(StunnedCorutine());

        IEnumerator StunnedCorutine()
        {
            stunned = true;
            OverheadText.text = "stunned";
            yield return new WaitForSeconds(stunnedDuration);
            stunned = false;
        }
    }
}
