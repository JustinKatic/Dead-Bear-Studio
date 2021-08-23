using System.Collections;
using UnityEngine;
using TMPro;
using Photon.Pun;


public class PlayerHealthManager : HealthManager
{
    [Header("HealthBar Hud")]
    [SerializeField] protected Transform HealthBarContainer;

    [Header("Player Hud UI")]
    [SerializeField] protected Canvas MyHUDCanvas;

    [Header("Kill Cam UI")]
    [SerializeField] GameObject KilledByUIPanel;
    [SerializeField] TextMeshProUGUI KilledByText;


    private PlayerController player;
    private ExperienceManager experienceManager;
    private PlayerController PlayerWhoDevouredMeController;
    private PlayerHealthManager playerWhoLastShotMeHealthManager;
    private DemonKingEvolution demonKingEvolution;
    private CrownHealthManager demonKingCrownHealthManager;
    [HideInInspector] public bool invulnerable = false;
    private PlayerTimers debuffTimer;

    [HideInInspector] public int PlayerId;


    #region Startup
    void Awake()
    {
        player = GetComponent<PlayerController>();
        //Run following if not local player
        if (!photonView.IsMine)
        {
            Destroy(MyHUDCanvas.gameObject);
        }
        //Run following if local player
        else
        {
            debuffTimer = GetComponentInChildren<PlayerTimers>();
            Destroy(overheadHealthBar.gameObject);
            CurrentHealth = MaxHealth;
            experienceManager = GetComponent<ExperienceManager>();
            demonKingEvolution = GetComponent<DemonKingEvolution>();
            demonKingCrownHealthManager = FindObjectOfType<CrownHealthManager>();
            PlayerId = player.id;
            SetHealth(MaxHealth);
        }
    }
    #endregion

    #region Devour
    protected override void OnBeingDevourStart()
    {
        canBeDevoured = false;

        if (photonView.IsMine)
        {
            debuffTimer.StopStunTimer();
            debuffTimer.StartBeingDevouredTimer(DevourTime);
            beingDevoured = true;
        }
    }

    protected override void OnBeingDevourEnd(int attackerID)
    {
        if (photonView.IsMine)
        {
            debuffTimer.StopBeingDevouredTimer();
            PlayerWhoDevouredMeController = GameManager.instance.GetPlayer(attackerID).gameObject.GetComponent<PlayerController>();
            PlayerWhoDevouredMeController.vCam.m_Priority = 12;
            KilledByText.text = "Killed By: " + PlayerWhoDevouredMeController.photonPlayer.NickName;
            KilledByUIPanel.SetActive(true);
            Respawn(true);
        }
    }


    [PunRPC]
    protected override void InterruptDevourOnSelf_RPC()
    {
        base.InterruptDevourOnSelf_RPC();
        if (photonView.IsMine)
            debuffTimer.StopBeingDevouredTimer();
    }
    #endregion

    #region Take Damage/ Heal Damage
    public void TakeDamage(int damage, int attackerID)
    {
        photonView.RPC("TakeDamage_RPC", player.photonPlayer, damage, attackerID);
    }

    [PunRPC]
    public void TakeDamage_RPC(int damage, int attackerID)
    {
        if (invulnerable || CurrentHealth <= 0 || beingDevoured)
            return;

        //Remove health
        CurrentHealth -= damage;

        playerWhoLastShotMeHealthManager = GameManager.instance.GetPlayer(attackerID).gameObject.GetComponent<PlayerHealthManager>();

        UpdateHealthBar(CurrentHealth);

        //Reset health regen timer
        healthRegenTimer = TimeBeforeHealthRegen;

        //call Stunned() on all player on network if no health left
        if (CurrentHealth <= 0)
            OnBeingStunnedStart();
    }

    protected override void Heal(int amountToHeal)
    {
        //Only running on local player
        CurrentHealth = Mathf.Clamp(CurrentHealth + amountToHeal, 0, MaxHealth);
        //Updates this charcters health bars on all players in network
        UpdateHealthBar(CurrentHealth);
        healthRegenTimer = TimeBeforeHealthRegen;
    }

    #endregion

    #region PlayerRespawn

    public void Respawn(bool DidIDieFromPlayer)
    {
        if (photonView.IsMine)
            photonView.RPC("Respawn_RPC", RpcTarget.All, DidIDieFromPlayer);
    }

    [PunRPC]
    public void Respawn_RPC(bool DidIDieFromPlayer)
    {
        StartCoroutine(ResetPlayer());

        IEnumerator ResetPlayer()
        {
            Evolutions currentActiveEvolution = gameObject.GetComponentInChildren<Evolutions>();
            currentActiveEvolution?.gameObject.SetActive(false);

            canBeDevoured = false;
            beingDevoured = false;
            isStunned = false;

            if (photonView.IsMine)
            {
                debuffTimer.StopStunTimer();
                debuffTimer.StopBeingDevouredTimer();
                stunnedTimer = 0;

                CheckIfIWasTheDemonKing(DidIDieFromPlayer);
                PlayerSoundManager.Instance.StopStunnedSound();
                DisablePlayerOnRespawn();

                //Check if the player died via no player death
                if (!DidIDieFromPlayer && playerWhoLastShotMeHealthManager != null)
                {
                    //Give the last player who hit exp
                    AwardLastPlayerWhoShotMe(photonView.ViewID);
                    playerWhoLastShotMeHealthManager = null;
                }
            }
            else
            {
                overheadHealthBar.enabled = false;
            }

            yield return new WaitForSeconds(RespawnTime);

            if (photonView.IsMine)
            {
                EnablePlayerOnRespawn();
            }
            else
            {
                overheadHealthBar.enabled = true;
            }
            if (gameObject.GetComponentInChildren<Evolutions>() == null)
                currentActiveEvolution?.gameObject.SetActive(true);
        }
    }

    void CheckIfIWasTheDemonKing(bool DidIDieFromPlayer)
    {
        //If the player died off the side as the demon king respawn back at the crown spawn

        if (demonKingEvolution.AmITheDemonKing)
        {
            experienceManager.DecreaseExperince(experienceManager.DemonKingExpLossDeath);
            demonKingEvolution.KilledAsDemonKing();

            if (!DidIDieFromPlayer)
            {
                demonKingCrownHealthManager.CrownRespawn(true);
            }
        }
        else
        {
            experienceManager.DecreaseExperince(experienceManager.PercentOfExpToLoseOnDeath);
        }
    }

    void DisablePlayerOnRespawn()
    {
        Stun(false);
        stunnedTimer = 0;
        GameManager.instance.IncrementSpawnPos();
        player.DisableMovement();
        player.cc.enabled = false;
        transform.position = GameManager.instance.spawnPoints[GameManager.instance.spawnIndex].position;
        player.cc.enabled = true;
        player.currentAnim.SetBool("Stunned", false);
    }

    void EnablePlayerOnRespawn()
    {
        if (PlayerWhoDevouredMeController != null)
        {
            PlayerWhoDevouredMeController.vCam.Priority = 10;
            KilledByUIPanel.SetActive(false);
        }
        experienceManager.CheckIfNeedToDevolve();
        player.EnableMovement();
        CurrentHealth = MaxHealth;
        UpdateHealthBar(CurrentHealth);
    }

    void AwardLastPlayerWhoShotMe(int playerWhoKilledSelfID)
    {
        playerWhoLastShotMeHealthManager.photonView.RPC("AwardLastPlayerWhoShotMe_RPC", playerWhoLastShotMeHealthManager.player.photonPlayer, photonView.ViewID);
    }

    [PunRPC]
    void AwardLastPlayerWhoShotMe_RPC(int playerWhoKilledSelfID)
    {
        PlayerHealthManager playerWhoKilledSelfHealthManager = PhotonView.Find(playerWhoKilledSelfID).GetComponent<PlayerHealthManager>();
        experienceManager.AddExpereince(playerWhoKilledSelfHealthManager.MyMinionType, playerWhoKilledSelfHealthManager.MyExperienceWorth);
    }
    #endregion

    #region HealthBar
    public void SetHealth(int MaxHealthValue)
    {
        photonView.RPC("SetHealth_RPC", RpcTarget.All, MaxHealthValue);
    }

    [PunRPC]
    protected void SetHealth_RPC(int MaxHealthValue)
    {
        //Run following on everyone
        MaxHealth = MaxHealthValue;

        //Run following if not local player
        if (!photonView.IsMine)
        {
            AddImagesToHealthBar(healthBarsOverhead, HealthBarContainerOverhead, MaxHealthValue);
        }
        //Run following if local player
        else
        {
            AddImagesToHealthBar(healthBars, HealthBarContainer, MaxHealthValue);

            if (CurrentHealth > MaxHealth)
                CurrentHealth = MaxHealth;

            UpdateHealthBar(CurrentHealth);
        }
    }

    void UpdateHealthBar(int CurrentHealth)
    {
        photonView.RPC("UpdateHealthBar_RPC", RpcTarget.All, CurrentHealth);
    }

    [PunRPC]
    public void UpdateHealthBar_RPC(int CurrentHealth)
    {
        //Run following if local player
        if (photonView.IsMine)
        {
            //Function in Health Manager
            FillBarsOfHealth(CurrentHealth, healthBars);
        }
        else
        {
            //Function in Health Manager
            FillBarsOfHealth(CurrentHealth, healthBarsOverhead);
        }
    }
    #endregion

    #region Stun
    void Stun(bool IsStartOfStun)
    {
        photonView.RPC("StunRPC", RpcTarget.All, IsStartOfStun);
    }

    [PunRPC]
    void StunRPC(bool IsStartOfStun)
    {
        if (IsStartOfStun)
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
            debuffTimer.StartStunTimer(StunnedDuration);
            isStunned = true;
            player.currentAnim.SetBool("Devouring", false);
            player.currentAnim.SetBool("Stunned", true);
            player.DisableMovement();
            PlayerSoundManager.Instance.PlayStunnedSound();
        }
    }

    protected override void OnBeingStunnedEnd()
    {
        Stun(false);
        debuffTimer.StopStunTimer();
        isStunned = false;
        player.EnableMovement();
        Heal(AmountOfHealthAddedAfterStunned);
        player.currentAnim.SetBool("Stunned", false);
        PlayerSoundManager.Instance.StopStunnedSound();
    }
}
#endregion

