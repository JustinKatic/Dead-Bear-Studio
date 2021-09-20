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
    public GameObject pushBackEffectObj;





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
        photonView.RPC("SpawnPushbackEffect_RPC", RpcTarget.All, transform.position.x, transform.position.y, transform.position.z, knockBackForce, knockBackRange, knockBackUpPos, playerController.id);
    }

    [PunRPC]
    void SpawnPushbackEffect_RPC(float x, float y, float z, float knockBackForce, float knockBackRange, float knockBackUpPos, int IdOfPlayerWhoSpawnedKnockback)
    {
        GameObject pushBackEffect = Instantiate(pushBackEffectObj, new Vector3(x, y, z), Quaternion.identity);
        PushBackAbility PushBackAbility = pushBackEffect.GetComponent<PushBackAbility>();
        PushBackAbility.Init(IdOfPlayerWhoSpawnedKnockback, knockBackForce, knockBackRange, knockBackUpPos);
    }
}
