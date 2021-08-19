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
    private PlayerController playerWhoLastShotMeController;
    private DemonKingEvolution demonKingEvolution;
    private PhotonView demonKingCrownPV;
    [HideInInspector] public bool invulnerable = false;
    private PlayerTimers debuffTimer;



    void Awake()
    {
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
            player = GetComponent<PlayerController>();
            experienceManager = GetComponent<ExperienceManager>();
            demonKingEvolution = GetComponent<DemonKingEvolution>();
            demonKingCrownPV = FindObjectOfType<CrownHealthManager>().GetComponent<PhotonView>();
            photonView.RPC("SetHealth", RpcTarget.All, MaxHealth);
        }
    }

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
            photonView.RPC("Respawn", RpcTarget.All, true);
        }
    }


    [PunRPC]
    protected override void InterruptDevourOnSelf()
    {
        base.InterruptDevourOnSelf();
        if (photonView.IsMine)
            debuffTimer.StopBeingDevouredTimer();
    }

    [PunRPC]
    void Suicide(int playerWhoKilledSelfID)
    {
        PhotonView playerWhoKilledSelfPV = PhotonView.Find(playerWhoKilledSelfID);
        experienceManager.AddExpereince(playerWhoKilledSelfPV.GetComponent<PlayerHealthManager>().MyMinionType, playerWhoKilledSelfPV.GetComponent<HealthManager>().MyExperienceWorth);
    }

    [PunRPC]
    public void TakeDamage(int damage, int attackerID)
    {
        //Runing following if local player
        if (photonView.IsMine)
        {
            if (invulnerable || CurrentHealth <= 0 || beingDevoured)
                return;

            //Remove health
            CurrentHealth -= damage;

            playerWhoLastShotMeController = GameManager.instance.GetPlayer(attackerID).gameObject.GetComponent<PlayerController>();

            photonView.RPC("UpdateHealthBar", RpcTarget.All, CurrentHealth);

            //Reset health regen timer
            healthRegenTimer = TimeBeforeHealthRegen;


            //call Stunned() on all player on network if no health left
            if (CurrentHealth <= 0)
                OnBeingStunnedStart();
        }
    }

    #region PlayerRespawn
    [PunRPC]
    public void Respawn(bool DidIDieFromPlayer)
    {
        StartCoroutine(ResetPlayer());

        IEnumerator ResetPlayer()
        {
            Evolutions currentActiveEvolution = gameObject.GetComponentInChildren<Evolutions>();
            currentActiveEvolution?.gameObject.SetActive(false);

            if (photonView.IsMine)
            {
                debuffTimer.StopStunTimer();
                debuffTimer.StopBeingDevouredTimer();

                CheckIfIWasTheDemonKing(DidIDieFromPlayer);
                PlayerSoundManager.Instance.StopStunnedSound();
                DisablePlayerOnRespawn();

                //Check if the player died via no player death
                if (!DidIDieFromPlayer && playerWhoLastShotMeController != null)
                {
                    //Give the last player who hit exp
                    playerWhoLastShotMeController.photonView.RPC("Suicide", playerWhoLastShotMeController.photonPlayer, photonView.ViewID);
                    playerWhoLastShotMeController = null;
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

            canBeDevoured = false;
            beingDevoured = false;
            isStunned = false;
        }
    }

    void CheckIfIWasTheDemonKing(bool DidIDieFromPlayer)
    {
        //If the player died off the side as the demon king respawn back at the crown spawn

        if (demonKingEvolution.AmITheDemonKing)
        {
            experienceManager.DecreaseExperince(experienceManager.DemonKingExpLossDeath);
            demonKingEvolution.ChangeFromTheDemonKing();

            if (!DidIDieFromPlayer)
            {
                demonKingCrownPV.RPC("CrownRespawn", RpcTarget.All);
            }
        }
        else
        {
            experienceManager.DecreaseExperince(experienceManager.PercentOfExpToLoseOnDeath);
        }
    }

    void DisablePlayerOnRespawn()
    {
        photonView.RPC("StunRPC", RpcTarget.All, false);
        stunnedTimer = 0;
        GameManager.instance.photonView.RPC("IncrementSpawnPos", RpcTarget.All);
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
        photonView.RPC("UpdateHealthBar", RpcTarget.All, CurrentHealth);
    }



    #endregion

    #region HealthBar

    [PunRPC]
    protected void SetHealth(int MaxHealthValue)
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

            photonView.RPC("UpdateHealthBar", RpcTarget.All, CurrentHealth);
        }
    }

    [PunRPC]
    public void UpdateHealthBar(int CurrentHealth)
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
    [PunRPC]
    void StunRPC(bool start)
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
            photonView.RPC("StunRPC", RpcTarget.All, true);
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
        if (!beingDevoured)
        {
            //Things that only affect local
            if (photonView.IsMine)
            {
                photonView.RPC("StunRPC", RpcTarget.All, false);
                debuffTimer.StopStunTimer();
                isStunned = false;
                player.EnableMovement();
                Heal(AmountOfHealthAddedAfterStunned);
                player.currentAnim.SetBool("Stunned", false);
                PlayerSoundManager.Instance.StopStunnedSound();
            }
        }
    }


    #endregion


}
