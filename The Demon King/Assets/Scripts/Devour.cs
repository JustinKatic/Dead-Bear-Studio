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
                    hitPlayer.photonView.RPC("OnDevourStart", hitPlayer.photonPlayer,playerController.id);
                }
            }
        }
    }

    [PunRPC]
    void OnDevourStart(int attackerId)
    {
        Debug.Log(playerController.id + "IM THE HIT PLAYER");
        Debug.Log("Devoured by" + attackerId);
    }
}
