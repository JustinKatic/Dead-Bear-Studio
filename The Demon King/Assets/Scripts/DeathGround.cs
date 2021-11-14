using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DeathGround : MonoBehaviour
{

    public float drowningSpeed = -0.2f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "PlayerParent")
        {
            PlayerController pc = other.GetComponent<PlayerController>();
            if (pc.drowningInLava)
                return;

            pc.drowningInLava = true;
            pc.DisableMovement();
            pc.playerJumpVelocity.y = 0;
            pc.DrowningInLavaGravity = drowningSpeed;
            pc.playerMoveVelocity = Vector3.zero;
            pc.PlayMyDeathInLavaSound();
            StartCoroutine(RespawnPlayer(other, pc));
        }
    }
    IEnumerator RespawnPlayer(Collider other, PlayerController pc)
    {
        yield return new WaitForSeconds(2f);
        other.gameObject.GetComponent<PlayerHealthManager>().Respawn(false, -1);
        yield return new WaitForSeconds(1.2f);
        pc.drowningInLava = false;
    }
}

