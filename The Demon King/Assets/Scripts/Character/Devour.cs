using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Devour : MonoBehaviourPun
{
    [Tooltip("Range the player can devour from")]
    public float devourRange;

    private PlayerController playerController;
    private HealthManager healthManager;
    private Camera cam;
    private bool IsDevouring;
    public Transform devourPoint;

    public LayerMask LayersForDevourToIgnore;

    private ExperienceManager experienceManager;

    [HideInInspector] public PhotonView targetBeingDevouredPV = null;
    private HealthManager hitPlayerHealth;
    private bool isTargetPlayer = false;

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

            //Interact callback
            playerController.CharacterInputs.Player.Interact.performed += OnInteract;
        }
    }

    private void Update()
    {
        if (photonView.IsMine)
        {
            if (IsDevouring && healthManager.isStunned)
            {
                //Tell the hitTarget to call CancelDevour RPC (inside of targets health manager)
                targetBeingDevouredPV.RPC("InterruptDevourOnSelf", RpcTarget.All);
                PhotonNetwork.SendAllOutgoingCommands();
                IsDevouring = false;
                targetBeingDevouredPV = null;
            }
        }
    }

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
        if (Physics.Raycast(ray, out hit, 20, ~LayersForDevourToIgnore))
        {
            //If raycast hits player or minion
            if (hit.transform.CompareTag("Player") || hit.transform.CompareTag("Minion") || hit.transform.CompareTag("DemonKingCrown"))
            {
                if (Vector3.Distance(devourPoint.position, hit.point) > devourRange)
                    return;

                //Get the healthManager of hit target
                hitPlayerHealth = hit.transform.gameObject.GetComponent<HealthManager>();

                //check if the target can be devoured
                if (hitPlayerHealth.canBeDevoured)
                {
                    //Disable and Enable the player devourings movement for duration
                    StartCoroutine(DevourCorutine());
                    IEnumerator DevourCorutine()
                    {
                        //Get the photon view of hit target
                        targetBeingDevouredPV = hit.collider.gameObject.GetPhotonView();
                        
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
        if (targetBeingDevouredPV.gameObject.tag == "Player")
        {
            isTargetPlayer = true;
            targetBeingDevouredPV.RPC("OnDevour", RpcTarget.All, playerController.id);
        }
        else
        {
            isTargetPlayer = false;
            targetBeingDevouredPV.RPC("OnDevour", RpcTarget.All, 0);
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

        // If the target is a player
        if (isTargetPlayer)
        {
            //If the target is the demon king, become the king and remove the other player as king
            if (targetBeingDevouredPV.GetComponent<DemonKingEvolution>().AmITheDemonKing)
            {
                targetBeingDevouredPV.GetComponent<DemonKingEvolution>().ChangeFromTheDemonKing();
                gameObject.GetComponent<DemonKingEvolution>().ChangeToTheDemonKing();
            }
        }
        else if (hitPlayerHealth.gameObject.transform.CompareTag("DemonKingCrown"))
        {
            gameObject.GetComponent<DemonKingEvolution>().ChangeToTheDemonKing();
        }
        // ADd experience to my bar and reset the target to null
        experienceManager.AddExpereince(hitPlayerHealth.MyMinionType, hitPlayerHealth.ExperienceValue);
        targetBeingDevouredPV = null;
        hitPlayerHealth = null;
    }
}
