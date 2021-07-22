using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Devour : MonoBehaviourPun
{
    private PlayerController playerController;
    private HealthManager healthManager;
    private Camera cam;
    private bool IsDevouring;

    public float devourRange;
    private void Awake()
    {
        if (photonView.IsMine)
        {
            playerController = GetComponent<PlayerController>();
            cam = GetComponentInChildren<Camera>();
            playerController.CharacterInputs.Player.Interact.performed += OnInteract;
            healthManager = GetComponent<HealthManager>();
        }
    }
    private void OnInteract(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (IsDevouring)
            return;

        CheckForDevourTarget();
    }

    private void CheckForDevourTarget()
    {
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, devourRange))
        {
            if (hit.transform.CompareTag("Player"))
            {
                PhotonView hitPlayer = hit.collider.gameObject.GetPhotonView();
                HealthManager hitPlayerHealth = hit.transform.gameObject.GetComponent<HealthManager>();

                if (hitPlayerHealth.canBeDevoured)
                {
                    hitPlayer.RPC("OnDevour", RpcTarget.All);

                    StartCoroutine(DevourCorutine());

                    IEnumerator DevourCorutine()
                    {
                        photonView.RPC("UpdateOverheadText", RpcTarget.All, "Devouring " + GameManager.instance.GetPlayer(hitPlayer.gameObject).photonPlayer.NickName);
                        playerController.DisableMovement();
                        yield return new WaitForSeconds(healthManager.DevourTime);
                        playerController.EnableMovement();
                        photonView.RPC("UpdateOverheadText", RpcTarget.All, gameObject.GetComponent<HealthManager>().CurrentHealth.ToString());
                    }
                }
            }
        }
    } 
}
