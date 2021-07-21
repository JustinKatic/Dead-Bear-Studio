using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Devour : MonoBehaviourPun
{
    private PlayerController playerController;
    private Camera cam;
    private bool IsDevouring;

    public float devourRange;
    public float DevourTime = 3f;
    private void Awake()
    {
        if (photonView.IsMine)
        {
            playerController = GetComponent<PlayerController>();
            cam = GetComponentInChildren<Camera>();
            playerController.CharacterInputs.Player.Interact.performed += OnInteract;
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
                PlayerController hitPlayer = GameManager.instance?.GetPlayer(hit.collider.gameObject).GetComponent<PlayerController>();
                HealthManager hitPlayerHealth = hit.transform.gameObject.GetComponent<HealthManager>();

                if (hitPlayerHealth.canBeDevoured)
                {
                    hitPlayer.photonView.RPC("OnDevour", hitPlayer.photonPlayer);

                    StartCoroutine(DevourCorutine());

                    IEnumerator DevourCorutine()
                    {
                        photonView.RPC("UpdateOverheadText", RpcTarget.All, "Devouring " + GameManager.instance.GetPlayer(hitPlayer.gameObject).photonPlayer.NickName);
                        playerController.DisableMovement();
                        yield return new WaitForSeconds(DevourTime);
                        playerController.EnableMovement();
                    }
                }
            }
        }
    }

    [PunRPC]
    void OnDevour()
    {
        StartCoroutine(DevourCorutine());
        IEnumerator DevourCorutine()
        {
            photonView.RPC("UpdateOverheadText", RpcTarget.All, GameManager.instance.GetPlayer(playerController.id).photonPlayer.NickName + " Is being devoured"); 
            
            yield return new WaitForSeconds(DevourTime);

            photonView.RPC("Respawn", RpcTarget.All);
        }
    }
}
