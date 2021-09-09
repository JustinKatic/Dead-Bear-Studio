using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Devour : MonoBehaviourPun
{
    [SerializeField] private float devourRange;
    [SerializeField] private Transform devourPoint;
    [SerializeField] private LayerMask LayersCanDevour;

    private bool IsDevouring;
    private bool isTargetPlayer = false;

    //Components
    private Camera cam;
    private PlayerController playerController;
    private HealthManager healthManager;
    private ExperienceManager experienceManager;
    [HideInInspector] public HealthManager targetBeingDevouredHealthManager = null;
    private PlayerTimers debuffTimer;

    DemonKingEvolution demonKingEvolution;
    private LeaderboardManager leaderboardManager;


    #region Start Up
    private void Awake()
    {
        experienceManager = GetComponent<ExperienceManager>();
        //Run following if local player
        if (photonView.IsMine)
        {
            //Getting components
            playerController = GetComponent<PlayerController>();
            cam = Camera.main;
            healthManager = GetComponent<HealthManager>();
            debuffTimer = GetComponentInChildren<PlayerTimers>();

            demonKingEvolution = GetComponent<DemonKingEvolution>();
            leaderboardManager = GetComponentInChildren<LeaderboardManager>();
            //Interact callback
            playerController.CharacterInputs.Player.Interact.performed += OnInteract;
        }
    }
    #endregion

    #region Update loops
    private void Update()
    {
        if (photonView.IsMine)
        {
            if (IsDevouring && healthManager.isStunned)
            {
                //Tell the hitTarget to call CancelDevour RPC (inside of targets health manager)
                targetBeingDevouredHealthManager.InterruptDevourOnSelf();
                PhotonNetwork.SendAllOutgoingCommands();
                IsDevouring = false;
                targetBeingDevouredHealthManager = null;
                PlayerSoundManager.Instance.StopDevourSound();
                debuffTimer.StopDevourTimer();
            }
            if (IsDevouring && targetBeingDevouredHealthManager.CurAttackerId != playerController.id)
            {
                DevouringHasCompleted(true);
            }
        }
    }
    #endregion

    #region Devour
    //Called when the interact key is pressed
    private void OnInteract(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        //Run following if local player
        if (photonView.IsMine)
        {
            //If already devoouring return
            if (IsDevouring)
                return;

            //check to see if can devour target
            CheckForDevourTarget();
        }
    }

    private void CheckForDevourTarget()
    {
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        //Shoots ray from center of screen
        if (Physics.SphereCast(ray, 3, out hit, 10, LayersCanDevour))
        {
            Debug.Log("1.hit a target in layer");
            //If raycast hits player or minion
            if (hit.transform.CompareTag("PlayerParent") || hit.transform.CompareTag("Minion") || hit.transform.CompareTag("DemonKingCrown"))
            {
                Debug.Log("2.devouring target hit a tag");
                if (Vector3.Distance(devourPoint.position, hit.point) > devourRange)
                    return;

                Debug.Log("3.devouring in range");

                //Get the healthManager of hit target
                targetBeingDevouredHealthManager = hit.transform.gameObject.GetComponent<HealthManager>();

                //check if the target can be devoured
                if (targetBeingDevouredHealthManager.canBeDevoured)
                {
                    Debug.Log("4.target can be devoured");
                    //Disable and Enable the player devourings movement for duration
                    StartCoroutine(DevourCorutine());
                    IEnumerator DevourCorutine()
                    {
                        PlayerSoundManager.Instance.PlayDevourSound();
                        debuffTimer.StartDevourTimer(targetBeingDevouredHealthManager.TimeTakenToBeDevoured);

                        CallDevourOnTarget();

                        yield return new WaitForSeconds(targetBeingDevouredHealthManager.TimeTakenToBeDevoured);

                        if (!healthManager.isStunned)
                        {
                            DevouringHasCompleted(false);
                        }
                    }
                }
            }
        }
    }


    void CallDevourOnTarget()
    {
        //Tell the hitTarget to call OnDevour RPC (inside of targets health manager)
        if (targetBeingDevouredHealthManager.gameObject.tag == "PlayerParent")
        {
            isTargetPlayer = true;
            targetBeingDevouredHealthManager.OnDevour(playerController.id);
        }
        else
        {
            isTargetPlayer = false;
            targetBeingDevouredHealthManager.OnDevour(playerController.id);
        }

        IsDevouring = true;
        playerController.currentAnim.SetBool("Devouring", true);
        playerController.DisableMovement();

    }

    void DevouringHasCompleted(bool interupted)
    {
        //Reset my controller and animator
        playerController.currentAnim.SetBool("Devouring", false);
        IsDevouring = false;
        playerController.EnableMovement();
        debuffTimer.StopDevourTimer();

        if (!interupted)
        {
            // If the target is a player
            if (isTargetPlayer && targetBeingDevouredHealthManager.GetComponent<DemonKingEvolution>().AmITheDemonKing)
            {
                demonKingEvolution.ChangeToTheDemonKing();
            }
            else if (targetBeingDevouredHealthManager.gameObject.transform.CompareTag("DemonKingCrown"))
            {
                demonKingEvolution.ChangeToTheDemonKing();
            }
            else
            {
                experienceManager.AddExpereince(targetBeingDevouredHealthManager.MyMinionType, targetBeingDevouredHealthManager.MyExperienceWorth);
            }

            if (demonKingEvolution.AmITheDemonKing)
            {
                leaderboardManager.UpdateDemonKingScore(targetBeingDevouredHealthManager.myScoreWorth);
            }

            healthManager.healthRegenTimer = healthManager.timeForHealthRegenToActivate;
        }
        //reset the target to null
        targetBeingDevouredHealthManager = null;
        targetBeingDevouredHealthManager = null;

        PlayerSoundManager.Instance.StopDevourSound();
    }
    #endregion
}
