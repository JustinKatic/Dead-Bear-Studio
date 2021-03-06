using System.Collections;
using UnityEngine;
using TMPro;
using Photon.Pun;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class PlayerHealthManager : HealthManager
{
    private const byte DisplayPlayerKilledSomeoneMessage = 4;
    private const byte DisplayPlayerKilledSelfMessage = 5;

    [Header("Player Hud UI")]
    [SerializeField] protected Canvas MyHUDCanvas;
    [SerializeField] protected Transform HealthBarContainer;
    [SerializeField] private GameObject ExperienceBarContainer;
    [SerializeField] GameObject playerHealVfx;
    [SerializeField] float invulenerableTimeAfterStun = 2f;
    [HideInInspector] public int AmountOfHealthAddedAfterStunned = 3;
    [HideInInspector] public int MaxHealth;

    [SerializeField] private GameObject rayEndVFX;


    [Header("Kill Cam UI")]
    [SerializeField] GameObject KilledByUIPanel;
    [SerializeField] TextMeshProUGUI KilledByText;


    private PlayerController player;
    private ExperienceManager experienceManager;
    private PlayerController PlayerWhoDevouredMeController;
    private PlayerHealthManager playerWhoLastShotMeHealthManager;
    private IEnumerator SetPlayerWhoLastShotMeLastNullCo;

    private DemonKingEvolution demonKingEvolution;
    private EvolutionManager evolutionManager;
    private CrownHealthManager demonKingCrownHealthManager;
    private Devour devour;

    [HideInInspector] public bool invulnerable = false;
    [HideInInspector] public bool isRespawning = false;
    private PlayerTimers debuffTimer;

    public int PlayerId;

    public Image overheadHudHealthbarImg;
    private Material OverheadHealthBarMat;

    public Image PlayerHudHealthBarImg;
    private Material playerHudHealthBarMat;
    private int playerDeaths = 0;

    [SerializeField] private TextMeshProUGUI healthBarTxt;
    [SerializeField] private GameObject namebarTxt;


    [SerializeField] private Slider healthRegenTimerSlider;

    private IEnumerator damageEffectCo;
    [SerializeField] float TimeToLerpOutOfDmgEffect = .5f;

    private IEnumerator beingDevourEffectCo;
    [SerializeField] private SpawnPointRuntimeSet spawnPoints;

    [SerializeField] private float TimeToTrackLastPlayerWhoShotMe = 10;



    #region Startup
    void Awake()
    {
        player = GetComponent<PlayerController>();

        OverheadHealthBarMat = Instantiate(overheadHudHealthbarImg.material);
        overheadHudHealthbarImg.material = OverheadHealthBarMat;

        playerHudHealthBarMat = Instantiate(PlayerHudHealthBarImg.material);
        PlayerHudHealthBarImg.material = playerHudHealthBarMat;

        evolutionManager = GetComponent<EvolutionManager>();
        //Run following if not local player
        if (!photonView.IsMine)
        {
            Destroy(MyHUDCanvas.gameObject);
        }
        //Run following if local player
        else
        {
            debuffTimer = GetComponentInChildren<PlayerTimers>();
            overheadHealthBar.gameObject.SetActive(false);
            CurrentHealth = MaxHealth;
            experienceManager = GetComponent<ExperienceManager>();
            demonKingEvolution = GetComponent<DemonKingEvolution>();
            demonKingCrownHealthManager = FindObjectOfType<CrownHealthManager>();
            devour = GetComponent<Devour>();
            //SetHealth(MaxHealth);
            healthRegenTimerSlider.maxValue = timeForHealthRegenToActivate;

            Hashtable PlayerDeaths = new Hashtable();
            PlayerDeaths.Add("PlayerDeaths", playerDeaths);
            PhotonNetwork.LocalPlayer.SetCustomProperties(PlayerDeaths);
        }
    }

    #endregion

    protected override void Update()
    {
        base.Update();
        if (photonView.IsMine)
        {
            if (currentHealthOffset > 0)
            {
                currentHealthOffset -= healthOffsetTime * Time.deltaTime;
                playerHudHealthBarMat.SetFloat("_OffsetHealth", currentHealthOffset);
            }

            if (gasEffect)
            {
                gasTimer += Time.deltaTime;
                gasFrequencyTimer += Time.deltaTime;

                if (gasTimer >= gasDurationOnPlayer)
                {
                    PlayPoisionVFX(false);
                    gasEffect = false;
                }

                if (gasFrequencyTimer >= gasTickRate)
                {
                    gasFrequencyTimer = 0;
                    TakeDamage(gasDamage, CurAttackerId);
                    playerControllerRuntimeSet.GetPlayer(CurAttackerId).IncreaseDragonDamage(gasDamage);
                }
                if (isStunned)
                {
                    gasEffect = false;
                    PlayPoisionVFX(false);
                }
            }

            if (beingDevoured || isStunned)
            {
                playerHealVfx.SetActive(false);
                return;
            }
            //Heal every X seconds if not at max health
            if (healthRegenTimer < timeForHealthRegenToActivate)
                healthRegenTimer += Time.deltaTime;


            if (CurrentHealth < MaxHealth && healthRegenTimer >= timeForHealthRegenToActivate)
            {
                healthRegenTickrateTimer += Time.deltaTime;
                if (healthRegenTickrateTimer >= healthRegenTickrate)
                {
                    playerHealVfx.SetActive(true);
                    Heal(healthRegenAmount);
                    healthRegenTickrateTimer = 0;
                }
            }
            else
                playerHealVfx.SetActive(false);
        }
        else
        {
            if (currentHealthOffset > 0)
            {
                currentHealthOffset -= healthOffsetTime * Time.deltaTime;
                OverheadHealthBarMat.SetFloat("_OffsetHealth", currentHealthOffset);
            }
        }
        healthRegenTimerSlider.value = healthRegenTimer;
    }




    public void ApplyGasEffect(int damageOverTimeDamage, int attackerId, float gasFrequency, float gasDurationOnPlayer)
    {
        photonView.RPC("ApplyGasEffect_RPC", RpcTarget.All, damageOverTimeDamage, attackerId, gasFrequency, gasDurationOnPlayer);
    }

    [PunRPC]
    public void ApplyGasEffect_RPC(int damageOverTimeDamage, int attackerId, float gasFrequency, float gasDurationOnPlayer)
    {
        if (!photonView.IsMine || isStunned || invulnerable)
            return;

        if (!gasEffect)
        {
            CurAttackerId = attackerId;
            TakeDamage(damageOverTimeDamage, CurAttackerId);
            PlayPoisionVFX(true);
        }
        gasTimer = 0;
        gasEffect = true;
        gasDamage = damageOverTimeDamage;
        gasTickRate = gasFrequency;
        this.gasDurationOnPlayer = gasDurationOnPlayer;
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


    #region Devour
    protected override void OnBeingDevourStart()
    {
        debuffTimer.StopStunTimer();
        debuffTimer.StartBeingDevouredTimer(TimeTakenToBeDevoured);
        beingDevoured = true;
        PlayDevourEffect();
    }

    protected override void OnBeingDevourEnd(int attackerID)
    {
        debuffTimer.StopBeingDevouredTimer();

        Respawn(true, attackerID);
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
            evolutionManager.activeEvolution.myMatInstance.SetFloat("_BeingDevouredEffectTime", valToBeLerped);
            yield return null;
        }

        if (beingDevourEffectCo != null)
            StopCoroutine(beingDevourEffectCo);
        beingDevourEffectCo = ToggleDisinegrateShader();
        StartCoroutine(beingDevourEffectCo);
    }

    IEnumerator ToggleDisinegrateShader()
    {
        float lerpTime = 0;

        while (lerpTime < TimeTakenToBeDesinegrated)
        {
            float valToBeLerped = Mathf.Lerp(0, 1, (lerpTime / TimeTakenToBeDesinegrated));
            lerpTime += Time.deltaTime;
            evolutionManager.activeEvolution.myMatInstance.SetFloat("_DisintegrateEffectTime", valToBeLerped);
            yield return null;
        }
        beingDevourEffectCo = null;
    }



    [PunRPC]
    protected override void InteruptDevourOnPersonDevouring_RPC()
    {
        if (photonView.IsMine)
            devour.DevouringHasCompleted(true);
    }


    [PunRPC]
    protected override void InterruptDevourOnSelf_RPC()
    {
        base.InterruptDevourOnSelf_RPC();
        if (photonView.IsMine)
        {
            debuffTimer.StopBeingDevouredTimer();
            StopDevourEffect();
        }
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
        evolutionManager.activeEvolution.myMatInstance.SetFloat("_BeingDevouredEffectTime", 0);
        evolutionManager.activeEvolution.myMatInstance.SetFloat("_DisintegrateEffectTime", 0);

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
        if (CurrentHealth <= 0 || beingDevoured)
            return;
        PlayDmgShader();
        if (invulnerable)
            return;

        if (CurrentHealth - damage <= 0)
            currentHealthOffset = damage - (CurrentHealth - damage) * -1;
        else
            currentHealthOffset = damage;

        //Remove health
        CurrentHealth = Mathf.Clamp(CurrentHealth - damage, 0, MaxHealth);

        if (SetPlayerWhoLastShotMeLastNullCo != null)
            StopCoroutine(SetPlayerWhoLastShotMeLastNullCo);

        SetPlayerWhoLastShotMeLastNullCo = SetPlayerWhoShotMeLastNull();
        StartCoroutine(SetPlayerWhoLastShotMeLastNullCo);
        playerWhoLastShotMeHealthManager = playerControllerRuntimeSet.GetPlayer(attackerID).GetComponent<PlayerHealthManager>();


        UpdateHealthBar(CurrentHealth, currentHealthOffset);

        //Reset health regen timer
        healthRegenTimer = 0;

        //call Stunned() on all player on network if no health left
        if (CurrentHealth <= 0)
            OnBeingStunnedStart();
    }

    IEnumerator SetPlayerWhoShotMeLastNull()
    {
        yield return new WaitForSeconds(TimeToTrackLastPlayerWhoShotMe);
        playerWhoLastShotMeHealthManager = null;
    }

    void PlayDmgShader()
    {
        photonView.RPC("PlayDmgShader_RPC", RpcTarget.All);
    }

    [PunRPC]
    void PlayDmgShader_RPC()
    {
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
            evolutionManager.activeEvolution.myMatInstance.SetFloat("_DamageEffectTime", valToBeLerped);
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

    #region PlayerRespawn

    public void Respawn(bool DidIDieFromPlayer, int attackerID)
    {
        if (photonView.IsMine)
            photonView.RPC("Respawn_RPC", RpcTarget.All, DidIDieFromPlayer, attackerID);
    }

    [PunRPC]
    public void Respawn_RPC(bool DidIDieFromPlayer, int attackerID)
    {
        StartCoroutine(ResetPlayer());

        IEnumerator ResetPlayer()
        {
            if (photonView.IsMine)
            {
                player.onLaunchPad = false;
                player.knockback = false;
                debuffTimer.StopStunTimer();
                debuffTimer.StopBeingDevouredTimer();
                HealthBarContainer.gameObject.SetActive(false);
                ExperienceBarContainer.SetActive(false);

                playerDeaths++;

                Hashtable PlayerDeaths = new Hashtable();
                PlayerDeaths.Add("PlayerDeaths", playerDeaths);
                PhotonNetwork.LocalPlayer.SetCustomProperties(PlayerDeaths);

                PlayerSoundManager.Instance.StopStunnedSound();

                DisablePlayerOnRespawn();


            }
            else
            {
                overheadHealthBar.SetActive(false);
                namebarTxt.SetActive(false);
            }

            yield return new WaitForSeconds(TimeTakenToBeDesinegrated);


            if (photonView.IsMine)
            {
                debuffTimer.StartRespawnTimer(RespawnTime);

                if (attackerID != -1)
                {
                    PlayerWhoDevouredMeController = playerControllerRuntimeSet.GetPlayer(attackerID).gameObject.GetComponent<PlayerController>();
                    PlayerWhoDevouredMeController.vCam.m_Priority = 12;
                    KilledByText.text = "Killed By: " + PlayerWhoDevouredMeController.photonPlayer.NickName;

                    KilledByUIPanel.SetActive(true);
                }

                //Teleport player to spawn pos and set correct look at direction
                player.cc.enabled = false;
                transform.position = spawnPoints.GetItemIndex(spawnPoints.CurrentSpawnIndex).transform.position;
                player._cinemachineTargetYaw = spawnPoints.GetItemIndex(spawnPoints.CurrentSpawnIndex).transform.eulerAngles.y;
                player._cinemachineTargetPitch = spawnPoints.GetItemIndex(spawnPoints.CurrentSpawnIndex).transform.eulerAngles.z;
                player.cc.enabled = true;

                CheckIfIWasTheDemonKing(DidIDieFromPlayer);
                player.currentAnim.SetBool("Stunned", false);

                if (PlayerWhoDevouredMeController != null)
                {
                    RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
                    SendOptions sendOptions = new SendOptions { Reliability = true };
                    object[] data = new object[] { PlayerWhoDevouredMeController.photonPlayer.NickName, PhotonNetwork.LocalPlayer.NickName };
                    PhotonNetwork.RaiseEvent(DisplayPlayerKilledSomeoneMessage, data, raiseEventOptions, sendOptions);
                }
                else if (!DidIDieFromPlayer && playerWhoLastShotMeHealthManager != null)
                //killed self but a player shot me recently
                {
                    RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
                    SendOptions sendOptions = new SendOptions { Reliability = true };
                    object[] data = new object[] { PlayerWhoDevouredMeController.photonPlayer.NickName, PhotonNetwork.LocalPlayer.NickName };
                    PhotonNetwork.RaiseEvent(DisplayPlayerKilledSomeoneMessage, data, raiseEventOptions, sendOptions);

                    //Give the last player who hit exp
                    AwardLastPlayerWhoShotMe(photonView.ViewID);
                    playerWhoLastShotMeHealthManager = null;
                }
                //Commited suicide no one shot me recently
                else
                {
                    RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
                    SendOptions sendOptions = new SendOptions { Reliability = true };
                    object[] data = new object[] { PhotonNetwork.LocalPlayer.NickName };
                    PhotonNetwork.RaiseEvent(DisplayPlayerKilledSelfMessage, data, raiseEventOptions, sendOptions);
                }

            }

            Evolutions currentActiveEvolution = gameObject.GetComponentInChildren<Evolutions>();
            currentActiveEvolution?.gameObject.SetActive(false);
            canBeDevoured = false;
            beingDevoured = false;
            isStunned = false;
            isRespawning = true;
            StopDevourEffect();



            yield return new WaitForSeconds(RespawnTime);

            if (photonView.IsMine)
            {
                EnablePlayerOnRespawn();
            }
            else
            {
                overheadHealthBar.SetActive(true);
                namebarTxt.SetActive(true);
            }
            if (gameObject.GetComponentInChildren<Evolutions>() == null)
                currentActiveEvolution?.gameObject.SetActive(true);
            isRespawning = false;
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
        IncrementSpawnPos();
        player.DisableMovement();
    }

    public void IncrementSpawnPos()
    {
        photonView.RPC("IncrementSpawnPos_RPC", RpcTarget.All);
    }


    [PunRPC]
    public void IncrementSpawnPos_RPC()
    {
        spawnPoints.CurrentSpawnIndex++;

        if (spawnPoints.CurrentSpawnIndex >= spawnPoints.Length())
            spawnPoints.CurrentSpawnIndex = 0;
    }


    void EnablePlayerOnRespawn()
    {
        if (PlayerWhoDevouredMeController != null)
        {
            PlayerWhoDevouredMeController.vCam.Priority = 10;
            KilledByUIPanel.SetActive(false);
            PlayerWhoDevouredMeController = null;
        }
        debuffTimer.StopRespawnTimer();
        experienceManager.CheckIfNeedToDevolve();
        player.EnableMovement();
        HealthBarContainer.gameObject.SetActive(true);
        ExperienceBarContainer.SetActive(true);
        CurrentHealth = MaxHealth;
        UpdateHealthBar(CurrentHealth, 0);
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
    public void SetPlayerValuesOnEvolve(int MaxHealthValue, float expWorth, int scoreWorth, float timeTakenToBeDevoured, int healthRegenAmount, int demonKingScoreWorth, float timeTakenToBeDesinegrated)
    {
        photonView.RPC("SetPlayerValuesOnEvolve_RPC", RpcTarget.All, MaxHealthValue, expWorth, scoreWorth, timeTakenToBeDevoured, healthRegenAmount, demonKingScoreWorth, timeTakenToBeDesinegrated);
    }

    [PunRPC]
    protected void SetPlayerValuesOnEvolve_RPC(int MaxHealthValue, float expWorth, int scoreWorth, float timeTakenToBeDevoured, int healthRegenAmount, int demonKingScoreWorth, float timeTakenToBeDesinegrated)
    {
        //Run following on everyone
        MaxHealth = MaxHealthValue;

        MyExperienceWorth = expWorth;
        myScoreWorth = scoreWorth;
        myDemonKingScoreWorth = demonKingScoreWorth;
        this.healthRegenAmount = healthRegenAmount;
        TimeTakenToBeDevoured = timeTakenToBeDevoured;
        TimeTakenToBeDesinegrated = timeTakenToBeDesinegrated;


        //Run following if not local player
        if (!photonView.IsMine)
        {
            //Update overhead healthbar values
            OverheadHealthBarMat.SetFloat("_MaxHealth", MaxHealth);
        }
        //Run following if local player
        else
        {
            playerHudHealthBarMat.SetFloat("_MaxHealth", MaxHealth);
            UpdateHealthBar(CurrentHealth, 0);
        }
    }

    public void UpdateHealthBar(int CurrentHealth, float healthOffset)
    {
        photonView.RPC("UpdateHealthBar_RPC", RpcTarget.All, CurrentHealth, healthOffset);
    }

    [PunRPC]
    public void UpdateHealthBar_RPC(int CurrentHealth, float healthOffset)
    {
        //Run following if local player
        if (photonView.IsMine)
        {
            //Update our healthbar values
            playerHudHealthBarMat.SetFloat("_CurrentHealth", CurrentHealth);
            currentHealthOffset = healthOffset;
            healthBarTxt.text = CurrentHealth.ToString();
        }
        else
        {
            //Update overhead healthbar
            OverheadHealthBarMat.SetFloat("_CurrentHealth", CurrentHealth);
            currentHealthOffset = healthOffset;
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
            isStunned = true;
        }
        else
        {
            StunVFX.SetActive(false);
            canBeDevoured = false;
            isStunned = false;
            StartCoroutine(BecomeInvulenerable());
        }
    }

    protected override void OnBeingStunnedStart()
    {
        //Things that only affect local
        if (photonView.IsMine)
        {
            Stun(true);
            debuffTimer.StartStunTimer(StunnedDuration);
            player.currentAnim.SetBool("Devouring", false);
            player.currentAnim.SetBool("Stunned", true);
            player.DisableMovement();
            PlayerSoundManager.Instance.PlayStunnedSound();
        }
    }

    protected override void OnBeingStunnedEnd()
    {
        if (photonView.IsMine)
        {
            if (!beingDevoured)
            {
                Stun(false);
                debuffTimer.StopStunTimer();
                player.EnableMovement();
                Heal(AmountOfHealthAddedAfterStunned);
                player.currentAnim.SetBool("Stunned", false);
                PlayerSoundManager.Instance.StopStunnedSound();
            }
        }
    }
    #endregion
    IEnumerator BecomeInvulenerable()
    {
        invulnerable = true;
        evolutionManager.activeEvolution.myMatInstance.SetFloat("_IsDamageImmune", 1);
        yield return new WaitForSeconds(invulenerableTimeAfterStun);
        evolutionManager.activeEvolution.myMatInstance.SetFloat("_IsDamageImmune", 0);
        invulnerable = false;
    }
}



