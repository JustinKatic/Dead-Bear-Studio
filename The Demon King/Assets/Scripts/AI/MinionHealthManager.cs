using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class MinionHealthManager : HealthManager
{
    public int MaxHealth;

    [Header("Model To Disable On Death")]
    [SerializeField] GameObject myModel;
    [SerializeField] protected int AmountOfHealthAddedAfterStunned = 3;

    [HideInInspector] public GameObject PlayerWhoShotMe;
    [HideInInspector] public State state;

    [SerializeField] Transform[] RespawnPositions;
    private Collider col;
    private Canvas hudCanvas;
    private NavMeshAgent agent;

    public Image overheadHudHealthbarImg;
    private Material OverheadHealthBarMat;

    public bool Respawned = false;

    [SerializeField] float minionExpWorth;
    [SerializeField] int minionScoreWorth;

    [SerializeField] Material myMatInstance;

    private IEnumerator damageEffectCo;
    [SerializeField] float TimeToLerpOutOfDmgEffect = 2;

    private IEnumerator beingDevourEffectCo;


    #region StartUp
    void Awake()
    {
        OverheadHealthBarMat = Instantiate(overheadHudHealthbarImg.material);
        overheadHudHealthbarImg.material = OverheadHealthBarMat;

        MyExperienceWorth = minionExpWorth;
        myScoreWorth = minionScoreWorth;

        col = GetComponent<Collider>();
        hudCanvas = GetComponentInChildren<Canvas>();
        CurrentHealth = MaxHealth;
        SetAIHealthValues(MaxHealth);
        agent = GetComponent<NavMeshAgent>();


        Renderer[] children;
        children = myModel.GetComponentsInChildren<Renderer>(true);

        var newMat = Instantiate(myMatInstance);
        myMatInstance = newMat;

        foreach (Renderer rend in children)
        {
            var mats = new Material[rend.materials.Length];
            for (var j = 0; j < rend.materials.Length; j++)
            {
                mats[j] = newMat;
            }
            rend.materials = mats;
        }
    }
    #endregion

    protected override void Update()
    {
        base.Update();

        if (currentHealthOffset > 0)
        {
            currentHealthOffset -= healthOffsetTime * Time.deltaTime;
            OverheadHealthBarMat.SetFloat("_OffsetHealth", currentHealthOffset);
        }
        if (gasEffect)
        {
            gasTimer += Time.deltaTime;
            gasFrequencyTimer += Time.deltaTime;

            if (gasTimer >= gasDurationOnPlayer)
            {
                gasEffect = false;
                PlayPoisionVFX(false);
            }

            if (gasFrequencyTimer >= gasTickRate)
            {
                gasFrequencyTimer = 0;
                TakeDamage(gasDamage, CurAttackerId);
                GameManager.instance.GetPlayer(CurAttackerId).IncreaseDragonDamage(gasDamage);
            }
            if (isStunned)
            {
                gasEffect = false;
                PlayPoisionVFX(false);
            }
        }

        if (beingDevoured || isStunned)
            return;
        //Heal every X seconds if not at max health
        if (healthRegenTimer < timeForHealthRegenToActivate)
        {
            healthRegenTimer += Time.deltaTime;
        }
        if (CurrentHealth < MaxHealth && healthRegenTimer >= timeForHealthRegenToActivate)
        {
            healthRegenTickrateTimer += Time.deltaTime;
            if (healthRegenTickrateTimer >= healthRegenTickrate)
            {
                Heal(healthRegenAmount);
                healthRegenTickrateTimer = 0;
            }
        }
    }

    public void ApplyGasEffect(int damageOverTimeDamage, int attackerId, float gasFrequency, float gasDurationOnPlayer)
    {
        photonView.RPC("ApplyGasEffect_RPC", RpcTarget.All, damageOverTimeDamage, attackerId, gasFrequency, gasDurationOnPlayer);
    }


    [PunRPC]
    public void ApplyGasEffect_RPC(int damageOverTimeDamage, int attackerId, float gasFrequency, float gasDurationOnPlayer)
    {
        if (isStunned || !photonView.IsMine)
            return;

        CurAttackerId = attackerId;
        gasTimer = 0;
        gasDamage = damageOverTimeDamage;
        gasTickRate = gasFrequency;
        this.gasDurationOnPlayer = gasDurationOnPlayer;

        if (!gasEffect)
        {
            TakeDamage(gasDamage, CurAttackerId);
            PlayPoisionVFX(true);
        }

        gasEffect = true;
    }

    protected void PlayPoisionVFX(bool playing)
    {
        photonView.RPC("PlayPoisionVFX_RPC", RpcTarget.All, playing);
    }


    [PunRPC]
    void PlayPoisionVFX_RPC(bool playing)
    {
        if (playing)
            poisonedStatusVfx.SetActive(true);
        else
            poisonedStatusVfx.SetActive(false);
    }

    #region Take Damage/ Heal Damage
    public void TakeDamage(int damage, int attackerID)
    {
        photonView.RPC("TakeDamage_RPC", RpcTarget.All, damage, attackerID);
    }

    [PunRPC]
    public void TakeDamage_RPC(int damage, int attackerID)
    {
        if (photonView.IsMine)
        {
            //Return if already being devoured
            if (beingDevoured || CurrentHealth <= 0)
                return;

            if (CurrentHealth - damage <= 0)
                currentHealthOffset = damage - (CurrentHealth - damage) * -1;
            else
                currentHealthOffset = damage;


            //Remove health
            CurrentHealth = Mathf.Clamp(CurrentHealth - damage, 0, MaxHealth);
            //Reset health regen timer
            healthRegenTimer = 0;

            //Updates this charcters status bar on all players in network
            UpdateHealthBar(CurrentHealth, currentHealthOffset);

            PlayerWhoShotMe = GameManager.instance.GetPlayer(attackerID).gameObject;

            //call Stunned() on all player on network if no health left
            if (CurrentHealth <= 0)
                OnBeingStunnedStart();
        }
        if (damageEffectCo != null)
            StopCoroutine(damageEffectCo);
        damageEffectCo = ToggleDmgShader();
        StartCoroutine(damageEffectCo);
    }

    IEnumerator ToggleDmgShader()
    {
        float lerpTime = 0;


        while (lerpTime < TimeToLerpOutOfDmgEffect)
        {
            float valToBeLerped = Mathf.Lerp(1, 0, (lerpTime / TimeToLerpOutOfDmgEffect));
            lerpTime += Time.deltaTime;
            myMatInstance.SetFloat("_DamageEffectTime", valToBeLerped);
            yield return null;
        }
    }


    public override void Heal(int amountToHeal)
    {
        //Only running on local player
        CurrentHealth = Mathf.Clamp(CurrentHealth + amountToHeal, 0, MaxHealth);
        //Updates this charcters health bars on all players in network
        UpdateHealthBar(CurrentHealth, 0);
    }

    #endregion

    #region Devour
    protected override void OnBeingDevourStart()
    {
        if (photonView.IsMine)
        {
            beingDevoured = true;
            PlayDevourEffect();
        }
    }

    protected override void OnBeingDevourEnd(int attackerID)
    {
        Respawn();
        StopDevourEffect();
    }

    void PlayDevourEffect()
    {
        photonView.RPC("PlayDevourEffect_RPC", RpcTarget.All);
    }

    [PunRPC]
    void PlayDevourEffect_RPC()
    {
        if (beingDevourEffectCo != null)
            StopCoroutine(beingDevourEffectCo);
        beingDevourEffectCo = ToggleDevourShader();
        StartCoroutine(beingDevourEffectCo);
    }

    IEnumerator ToggleDevourShader()
    {
        float lerpTime = 0;

        while (lerpTime < TimeTakenToBeDevoured)
        {
            float valToBeLerped = Mathf.Lerp(0, 1, (lerpTime / TimeTakenToBeDevoured));
            lerpTime += Time.deltaTime;
            myMatInstance.SetFloat("_BeingDevouredEffectTime", valToBeLerped);
            yield return null;
        }
        beingDevourEffectCo = null;
    }

    void StopDevourEffect()
    {
        photonView.RPC("StopDevourEffect_RPC", RpcTarget.All);
    }

    [PunRPC]
    void StopDevourEffect_RPC()
    {
        if (beingDevourEffectCo != null)
            StopCoroutine(beingDevourEffectCo);
        myMatInstance.SetFloat("_BeingDevouredEffectTime", 0);
    }

    [PunRPC]
    protected override void InterruptDevourOnSelf_RPC()
    {
        base.InterruptDevourOnSelf_RPC();
        if (photonView.IsMine)
            StopDevourEffect();

    }

    #endregion

    #region Respawn

    void Respawn()
    {
        photonView.RPC("Respawn_RPC", RpcTarget.All);
    }

    [PunRPC]
    public void Respawn_RPC()
    {
        StartCoroutine(ResetPlayer());

        IEnumerator ResetPlayer()
        {
            myModel.SetActive(false);
            col.enabled = false;
            hudCanvas.enabled = false;
            isStunned = false;
            beingDevoured = false;
            canBeDevoured = false;
            stunnedTimer = 0;

            if (PhotonNetwork.IsMasterClient)
            {
                Stun(false);
                agent.Warp(RespawnPositions[Random.Range(0, RespawnPositions.Length)].position);
                Respawned = true;
            }

            yield return new WaitForSeconds(RespawnTime);

            if (PhotonNetwork.IsMasterClient)
            {
                CurrentHealth = MaxHealth;
                UpdateHealthBar(CurrentHealth, 0);
            }
            myModel.SetActive(true);
            col.enabled = true;
            hudCanvas.enabled = true;
        }
    }
    #endregion

    #region Stun
    void Stun(bool start)
    {
        photonView.RPC("Stun_RPC", RpcTarget.All, start);
    }

    [PunRPC]
    void Stun_RPC(bool start)
    {
        if (start)
        {
            StunVFX.SetActive(true);
            canBeDevoured = true;
        }
        else
        {
            StunVFX.SetActive(false);
            canBeDevoured = false;
        }
    }

    protected override void OnBeingStunnedStart()
    {
        //Things that only affect local
        if (photonView.IsMine)
        {
            Stun(true);
            isStunned = true;
        }
    }

    protected override void OnBeingStunnedEnd()
    {
        if (!beingDevoured)
        {
            //Things that only affect local
            if (photonView.IsMine)
            {
                Stun(false);

                isStunned = false;
                Heal(AmountOfHealthAddedAfterStunned);
            }
        }
    }

    #endregion

    #region HealthBar
    void SetAIHealthValues(int MaxHealthValue)
    {
        photonView.RPC("SetHealth_RPC", RpcTarget.All, MaxHealthValue);
    }

    [PunRPC]
    protected void SetHealth_RPC(int MaxHealthValue)
    {
        //Run following on everyone
        MaxHealth = MaxHealthValue;
        OverheadHealthBarMat.SetFloat("_MaxHealth", MaxHealth);
        UpdateHealthBar(CurrentHealth, 0);
    }

    void UpdateHealthBar(int CurrentHealth, float healthOffset)
    {
        photonView.RPC("UpdateHealthBar_RPC", RpcTarget.All, CurrentHealth, healthOffset);
    }

    [PunRPC]
    public void UpdateHealthBar_RPC(int CurrentHealth, float healthOffset)
    {
        OverheadHealthBarMat.SetFloat("_CurrentHealth", CurrentHealth);
        if (!photonView.IsMine)
            currentHealthOffset = healthOffset;

    }
    #endregion
}
