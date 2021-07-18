using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Devour : MonoBehaviourPun
{
    private PlayerController playerController;
    private PlayerHealthManager playerHealthManager;
    private Camera cam;
    private bool IsDevouring;

    public float devourRange;


    private void Awake()
    {
        if (photonView.IsMine)
        {
            playerController = GetComponent<PlayerController>();
            playerHealthManager = GetComponent<PlayerHealthManager>();
            cam = GetComponentInChildren<Camera>();
            playerController.CharacterInputs.Player.Interact.performed += OnInteract;
        }
    }


    private void OnInteract(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (IsDevouring || playerController.isStunned)
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
                PlayerController hitPlayer = GameManager.instance.GetPlayer(hit.collider.gameObject).GetComponent<PlayerController>();
                if (hitPlayer.isStunned)
                {
                    hitPlayer.photonView.RPC("OnDevour", hitPlayer.photonPlayer, playerController.id);
                }
            }
        }
    }

    [PunRPC]
    void OnDevour(int attackerId)
    {
        StartCoroutine(DevourCorutine());
        IEnumerator DevourCorutine()
        {
            playerHealthManager.beingDevoured = true;
            Debug.Log(GameManager.instance.GetPlayer(playerController.id).photonPlayer.NickName + " Is being devoured");

            yield return new WaitForSeconds(3);

            Debug.Log("Devoured by: " + GameManager.instance.GetPlayer(attackerId).photonPlayer.NickName);
            playerHealthManager.Respawn();
        }
    }
}
