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
    [HideInInspector] public HealthManager targetBeingDevouredHealthManager = null;

    //Components
    private Camera cam;
    private PlayerController playerController;
    private HealthManager healthManager;
    private ExperienceManager experienceManager;
    private HealthManager hitHealthManager;
    private PlayerTimers debuffTimer;

    DemonKingEvolution demonKingEvolution;

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
            //If raycast hits player or minion
            if (hit.transform.CompareTag("Player") || hit.transform.CompareTag("Minion") || hit.transform.CompareTag("DemonKingCrown"))
            {
                if (Vector3.Distance(devourPoint.position, hit.point) > devourRange)
                    return;

                //Get the healthManager of hit target
                hitHealthManager = hit.transform.gameObject.GetComponent<HealthManager>();

                //check if the target can be devoured
                if (hitHealthManager.canBeDevoured)
                {
                    //Disable and Enable the player devourings movement for duration
                    StartCoroutine(DevourCorutine());
                    IEnumerator DevourCorutine()
                    {
                        //Get the photon view of hit target
                        targetBeingDevouredHealthManager = hit.collider.gameObject.GetComponent<HealthManager>();

                        PlayerSoundManager.Instance.PlayDevourSound();
                        debuffTimer.StartDevourTimer(healthManager.DevourTime);

                        CallDevourOnTarget();

                        yield return new WaitForSeconds(healthManager.DevourTime);

                        if (!healthManager.isStunned)
                        {
                            DevouringHasCompleted();
                        }
                    }
                }
            }
        }
    }


    void CallDevourOnTarget()
    {
        //Tell the hitTarget to call OnDevour RPC (inside of targets health manager)
        if (targetBeingDevouredHealthManager.gameObject.tag == "Player")
        {
            isTargetPlayer = true;
            targetBeingDevouredHealthManager.OnDevour(playerController.id);
        }
        else
        {
            isTargetPlayer = false;
            targetBeingDevouredHealthManager.OnDevour(0);
        }

        IsDevouring = true;
        playerController.currentAnim.SetBool("Devouring", true);
        playerController.DisableMovement();

    }

    void DevouringHasCompleted()
    {
        //Reset my controller and animator
        playerController.currentAnim.SetBool("Devouring", false);
        IsDevouring = false;
        playerController.EnableMovement();
        debuffTimer.StopDevourTimer();


        // If the target is a player
        if (isTargetPlayer && targetBeingDevouredHealthManager.GetComponent<DemonKingEvolution>().AmITheDemonKing)
        {
            demonKingEvolution.ChangeToTheDemonKing();
        }
        else if (hitHealthManager.gameObject.transform.CompareTag("DemonKingCrown"))
        {
            demonKingEvolution.ChangeToTheDemonKing();
        }
        else
        {
            experienceManager.AddExpereince(hitHealthManager.MyMinionType, hitHealthManager.MyExperienceWorth);
        }
        //reset the target to null
        targetBeingDevouredHealthManager = null;
        hitHealthManager = null;

        PlayerSoundManager.Instance.StopDevourSound();
    }
    #endregion

    private void OnDrawGizmos()
    {
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        //Shoots ray from center of screen
        if (Physics.SphereCast(ray, 3, out hit, 10, LayersCanDevour))
        {
            if (Vector3.Distance(devourPoint.position, hit.point) < devourRange)
                Gizmos.DrawWireSphere(hit.point, 3);
        }
    }
}
