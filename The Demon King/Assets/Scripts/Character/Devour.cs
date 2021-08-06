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
            if (hit.transform.CompareTag("Player") || hit.transform.CompareTag("Minion"))
            {
                if (Vector3.Distance(devourPoint.position, hit.point) > devourRange)
                    return;

                //Get the healthManager of hit target
                HealthManager hitPlayerHealth = hit.transform.gameObject.GetComponent<HealthManager>();

                //check if the target can be devoured
                if (hitPlayerHealth.canBeDevoured)
                {
                    //Get the photon view of hit target
                    targetBeingDevouredPV = hit.collider.gameObject.GetPhotonView();

                    //Tell the hitTarget to call OnDevour RPC (inside of targets health manager)
                    targetBeingDevouredPV.RPC("OnDevour", RpcTarget.All);

                    //Disable and Enable the player devourings movement for duration
                    StartCoroutine(DevourCorutine());
                    IEnumerator DevourCorutine()
                    {
                        IsDevouring = true;
                        playerController.currentAnim.SetBool("Devouring", true);
                        playerController.DisableMovement();

                        yield return new WaitForSeconds(healthManager.DevourTime);

                        if (!healthManager.isStunned)
                        {
                            playerController.currentAnim.SetBool("Devouring", false);
                            IsDevouring = false;
                            playerController.EnableMovement();
                            targetBeingDevouredPV = null;
                            
                            healthManager.MyMinionType = hitPlayerHealth.MyMinionType;
                            experienceManager.AddExpereince(healthManager.MyMinionType, hitPlayerHealth.ExperienceValue);
                            
                        }
                    }
                }
            }
        }
    }
}
