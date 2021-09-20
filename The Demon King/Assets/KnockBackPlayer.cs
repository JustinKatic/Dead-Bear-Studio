using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class KnockBackPlayer : MonoBehaviourPun
{
    public LayerMask layersCanHit;
    PlayerController playerController;
    Vector3 KnockBackPos;
    public float knockBackForce = 30;
    public float knockBackRange = 15;
    public float knockBackUpPos = 4;



    private void Start()
    {
        if (photonView.IsMine)
        {
            playerController = GetComponent<PlayerController>();
            playerController.CharacterInputs.Player.Ability2.performed += Ability2_performed;
        }
    }


    private void Ability2_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        Collider[] cols = Physics.OverlapSphere(transform.position, knockBackRange, layersCanHit);
        foreach (var col in cols)
        {
            if (col.CompareTag("PlayerParent"))
            {
                KnockBackPos = col.transform.position + ((col.transform.position - transform.position).normalized * 5);
                KnockBackPos.y = transform.position.y + knockBackUpPos;
                col.GetComponent<PlayerController>().KnockBack(KnockBackPos, playerController.id, knockBackForce);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(KnockBackPos, .5f);
    }
}
