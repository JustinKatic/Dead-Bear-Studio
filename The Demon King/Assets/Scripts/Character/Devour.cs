using Photon.Pun;
using System.Collections;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;


public class Devour : MonoBehaviourPun
{
    [SerializeField] private float devourRange;
    [SerializeField] private Transform devourPoint;
    [SerializeField] private LayerMask LayersCanDevour;

    public bool IsDevouring;
    private bool isTargetPlayer = false;

    [SerializeField]private int demonKingPointExtraPoints;
    //Components
    private Camera cam;
    private PlayerController playerController;
    private HealthManager healthManager;
    private ExperienceManager experienceManager;
    public HealthManager targetCanDevour = null;
    public HealthManager targetBeingDevourd = null;

    private PlayerTimers debuffTimer;

    DemonKingEvolution demonKingEvolution;
    private LeaderboardManager leaderboardManager;

    protected IEnumerator myDevourCo;

    private int playerKills = 0;
    private int minionKills = 0;


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
            leaderboardManager = FindObjectOfType<LeaderboardManager>();
            //Interact callback
            playerController.CharacterInputs.Player.Interact.performed += OnInteract;

            Hashtable PlayerKills = new Hashtable();
            PlayerKills.Add("PlayerKills", playerKills);
            PhotonNetwork.LocalPlayer.SetCustomProperties(PlayerKills);

            Hashtable MinionsKills = new Hashtable();
            MinionsKills.Add("MinionKills", minionKills);
            PhotonNetwork.LocalPlayer.SetCustomProperties(MinionsKills);
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
                InteruptDevouring();
            }

            CheckForDevour();
        }
    }
    #endregion

    public void InteruptDevouring()
    {
        targetBeingDevourd.InterruptDevourOnSelf();
        IsDevouring = false;
        targetCanDevour = null;
        targetBeingDevourd = null;
        playerController.cameraRotation = true;

        PlayerSoundManager.Instance.StopDevourSound();
        debuffTimer.StopDevourTimer();
    }

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
            DevourTarget();
        }
    }

    private void OnDestroy()
    {
        if (photonView.IsMine)
            playerController.CharacterInputs.Player.Interact.performed -= OnInteract;
    }

    void CheckForDevour()
    {
        GameObject ClosestTarget = null;
        
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit, devourRange,LayersCanDevour))
        {
            if (hit.transform.CompareTag("PlayerParent") ||hit.transform.CompareTag("Minion") || hit.transform.CompareTag("DemonKingCrown"))
            {
                if (hit.transform.GetComponent<HealthManager>().canBeDevoured)
                {
                    ClosestTarget = hit.transform.gameObject;
                }
            }
        }

        if (ClosestTarget == null)
        {
            Collider[] targets = Physics.OverlapSphere(devourPoint.position, 4);

            foreach (var target in targets)
            {
                //If raycast hits player/minion/crown
                if (target.CompareTag("PlayerParent") || target.CompareTag("Minion") || target.CompareTag("DemonKingCrown"))
                {
                    if (target.gameObject.GetComponent<HealthManager>().canBeDevoured)
                    {
                        if (ClosestTarget == null)
                        {
                            ClosestTarget = target.gameObject;
                        }
                        if (Vector3.Distance(devourPoint.position, target.transform.position) < Vector3.Distance(devourPoint.position, ClosestTarget.transform.position))
                        {
                            ClosestTarget = target.gameObject;
                        }
                    }
                }
            
            }
        }
       
        if (targetCanDevour != null)
        {
            targetCanDevour.DevourTargetIcon.SetActive(false);
            targetCanDevour = null;
        }

        if (ClosestTarget != null)
        {
            HealthManager hitHealthManager = ClosestTarget.transform.gameObject.GetComponent<HealthManager>();

            //check if a target was hit and target can be devoured
            if (hitHealthManager != null && hitHealthManager.canBeDevoured)
            {
                //If dont currently have a devour target
                if (targetCanDevour == null)
                {
                    targetCanDevour = hitHealthManager;
                    targetCanDevour.DevourTargetIcon.SetActive(true);
                }
                //If target is already our target
                else if (targetCanDevour == hitHealthManager)
                {
                }
                //If hit target but not equal to current target we have
                else if (targetCanDevour != hitHealthManager)
                {
                    targetCanDevour.DevourTargetIcon.SetActive(false);
                    targetCanDevour = hitHealthManager;
                    targetCanDevour.DevourTargetIcon.SetActive(true);
                }
            }
            else if (hitHealthManager != null && !hitHealthManager.canBeDevoured && targetCanDevour != null)
            {
                targetCanDevour.DevourTargetIcon.SetActive(false);
                targetCanDevour = null;
            }
        }
        else
        {
            if (targetCanDevour != null)
            {
                targetCanDevour.DevourTargetIcon.SetActive(false);
                targetCanDevour = null;
            }
        }
    }

    private void DevourTarget()
    {
        if (targetCanDevour != null)
        {
            targetBeingDevourd = targetCanDevour;
            //Disable and Enable the player devourings movement for duration
            myDevourCo = DevourCorutine();
            StartCoroutine(myDevourCo);
            IEnumerator DevourCorutine()
            {
                PlayerSoundManager.Instance.PlayDevourSound();
                debuffTimer.StartDevourTimer(targetBeingDevourd.TimeTakenToBeDevoured);
                playerController.cameraRotation = false;

                CallDevourOnTarget();

                yield return new WaitForSeconds(targetBeingDevourd.TimeTakenToBeDevoured);

                if (!healthManager.isStunned || IsDevouring)
                {
                    Debug.Log("devouring completed via devour script");
                    DevouringHasCompleted(false);
                }
            }
        }
    }


    void CallDevourOnTarget()
    {
        //Tell the hitTarget to call OnDevour RPC (inside of targets health manager)
        if (targetBeingDevourd.gameObject.tag == "PlayerParent")
        {
            isTargetPlayer = true;
            targetBeingDevourd.OnDevour(playerController.id);
        }
        else
        {
            isTargetPlayer = false;
            targetBeingDevourd.OnDevour(playerController.id);
        }

        IsDevouring = true;
        playerController.currentAnim.SetBool("Devouring", true);
        playerController.DisableMovement();
    }

    public void DevouringHasCompleted(bool interupted)
    {
        if (!photonView.IsMine)
            return;
        //Reset my controller and animator
        playerController.currentAnim.SetBool("Devouring", false);
        IsDevouring = false;
        playerController.EnableMovement();
        playerController.cameraRotation = true;
        debuffTimer.StopDevourTimer();


        if (!interupted)
        {
            // If the target is a player
            if (isTargetPlayer && targetBeingDevourd.GetComponent<DemonKingEvolution>().AmITheDemonKing)
            {
                demonKingEvolution.ChangeToTheDemonKing();
            }
            else if (targetBeingDevourd.gameObject.transform.CompareTag("DemonKingCrown"))
            {
                demonKingEvolution.ChangeToTheDemonKing();
            }
            else
            {
                experienceManager.AddExpereince(targetBeingDevourd.MyMinionType, targetBeingDevourd.MyExperienceWorth);
                if (targetBeingDevourd.gameObject.CompareTag("PlayerParent"))
                {
                    playerKills++;
                    Hashtable PlayerKills = new Hashtable();
                    PlayerKills.Add("PlayerKills", playerKills);
                    PhotonNetwork.LocalPlayer.SetCustomProperties(PlayerKills);
                }
                else if (targetBeingDevourd.gameObject.CompareTag("Minion"))
                {
                    minionKills++;
                    Debug.Log("Devoured minion");
                    Hashtable MinionsKills = new Hashtable();
                    MinionsKills.Add("MinionKills", minionKills);
                    PhotonNetwork.LocalPlayer.SetCustomProperties(MinionsKills);
                }
            }

            if (demonKingEvolution.AmITheDemonKing)
            {
                leaderboardManager.UpdatePlayerScore(targetBeingDevourd.myDemonKingScoreWorth);
            }
            else
            {
                leaderboardManager.UpdatePlayerScore(targetBeingDevourd.myScoreWorth);
            }

            healthManager.healthRegenTimer = healthManager.timeForHealthRegenToActivate;
        }
        else
            StopCoroutine(myDevourCo);

        //reset the target to null
        targetBeingDevourd.DevourTargetIcon.SetActive(false);
        targetCanDevour = null;
        targetBeingDevourd = null;


        PlayerSoundManager.Instance.StopDevourSound();
    }
    #endregion

    void CheckForDevourTarget()
    {
        Collider[] targets = Physics.OverlapSphere(transform.position, 2);
    }
}
