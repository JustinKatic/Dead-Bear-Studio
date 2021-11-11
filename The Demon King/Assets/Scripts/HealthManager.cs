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
    [SerializeField] protected float TimeBeforeHealthRegen = 3f;
    [SerializeField] protected float healthRegenTickrate = .5f;
    [SerializeField] protected float RespawnTime = 6;

    [SerializeField] private float stunnedDuration = 3;

    public int healthRegenAmount = 1;
    public float TimeTakenToBeDevoured;
    public float TimeTakenToBeDesinegrated;

    public float StunnedDuration { get { return stunnedDuration; } private set { stunnedDuration = value; } }

    [Header("Evolution Stats")]
    [HideInInspector] public float MyExperienceWorth;
    [HideInInspector] public int myScoreWorth;
    [HideInInspector] public int myDemonKingScoreWorth;


    [HideInInspector] public MinionType MyMinionType;

    [Header("healthBar Health UI IMG")]
    [SerializeField] protected Image healthBarPrefab;

    [Header("Overhead healthBar Hud")]
    public GameObject overheadHealthBar;
    public GameObject DevourTargetIcon;

    [Header("VFX Effects")]
    [SerializeField] protected GameObject StunVFX;
    [SerializeField] protected GameObject poisonedStatusVfx;




    public float healthRegenTimer = 0f;
    public float timeForHealthRegenToActivate = 8f;


    protected float healthRegenTickrateTimer = 0f;


    protected float stunnedTimer;


    /// <summary>
    /// MAY BE ABLE TO REMOVE
    /// </summary>
    public int CurAttackerId;

    public int CurrentHealth = 0;
    [HideInInspector] public bool beingDevoured = false;
    public bool canBeDevoured = false;
    [HideInInspector] public bool isStunned = false;

    protected float currentHealthOffset = 0;
    [SerializeField] protected float healthOffsetTime = 5;

    protected IEnumerator myDevourCo;

    protected bool gasEffect;
    protected int gasDamage;
    protected float gasTickRate;
    protected float gasTimer;
    protected float gasFrequencyTimer;
    protected float gasDurationOnPlayer;

    private bool coRunning = false;
    public PlayerControllerRuntimeSet playerControllerRuntimeSet;

    private void Start()
    {
        UpdateDevourImgText();
    }

    #region Update Loops
    virtual protected void Update()
    {
        //Run following if local player
        if (photonView.IsMine)
        {
            if (isStunned)
            {
                stunnedTimer += Time.deltaTime;
                if (stunnedTimer >= StunnedDuration)
                {
                    if (!beingDevoured)
                    {
                        OnBeingStunnedEnd();
                        stunnedTimer = 0;
                    }
                }
            }
        }
    }
    #endregion


    #region Devour

    public void OnDevour(int attackerID)
    {
        photonView.RPC("OnDevour_RPC", RpcTarget.All, attackerID);
    }

    [PunRPC]
    public void OnDevour_RPC(int attackerID)
    {
        canBeDevoured = false;

        if (photonView.IsMine)
        {
            if (!coRunning)
            {
                coRunning = true;
                myDevourCo = DevourCorutine();
                StartCoroutine(myDevourCo);
            }
            else
            {
                playerControllerRuntimeSet.GetPlayer(attackerID).photonView.RPC("InteruptDevourOnPersonDevouring_RPC", RpcTarget.All);
            }
        }


        IEnumerator DevourCorutine()
        {
            OnBeingDevourStart();

            yield return new WaitForSeconds(TimeTakenToBeDevoured);

            coRunning = false;
            CurAttackerId = 0;
            OnBeingDevourEnd(attackerID);
        }
    }

    public void UpdateDevourImgText()
    {
        string path = InputManager.inputActions.Player.Interact.bindings[0].path;
        string key = path.Split('/').Last();
        DevourTargetIcon.GetComponentInChildren<TextMeshProUGUI>().text = key;
    }

    //Overrides for inherited classes
    protected virtual void OnBeingDevourStart()
    {

    }

    protected virtual void OnBeingDevourEnd(int attackerID)
    {

    }

    public void InteruptDevourOnPersonDevouring()
    {
        photonView.RPC("InteruptDevourOnPersonDevouring_RPC", RpcTarget.All);
    }

    [PunRPC]
    protected virtual void InteruptDevourOnPersonDevouring_RPC()
    {

    }


    public void InterruptDevourOnSelf()
    {
        photonView.RPC("InterruptDevourOnSelf_RPC", RpcTarget.All);
    }

    [PunRPC]
    protected virtual void InterruptDevourOnSelf_RPC()
    {
        beingDevoured = false;
        CurAttackerId = 0;
        coRunning = false;

        if (photonView.IsMine)
        {
            StopCoroutine(myDevourCo);
            OnBeingStunnedEnd();
            stunnedTimer = 0;
        }
    }

    #endregion

    #region Stun

    //Overrides for inherited classes
    protected virtual void OnBeingStunnedStart()
    {

    }

    protected virtual void OnBeingStunnedEnd()
    {

    }

    #endregion

    #region HealthFunctions
    virtual public void Heal(int amountToHeal)
    {

    }
    #endregion
}
