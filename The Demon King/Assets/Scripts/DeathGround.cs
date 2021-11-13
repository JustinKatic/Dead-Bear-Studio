using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DeathGround : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "PlayerParent")
        {
            PlayerController pc = other.GetComponent<PlayerController>();
            pc.drowningInLava = true;
            pc.playerJumpVelocity.y = 0;
            pc.DisableMovement();
            pc.PlayMyDeathInLavaSound();
            StartCoroutine(RespawnPlayer(other, pc));
        }
    }
    IEnumerator RespawnPlayer(Collider other, PlayerController pc)
    {
        yield return new WaitForSeconds(2f);
        other.gameObject.GetComponent<PlayerHealthManager>().Respawn(false, -1);
        yield return new WaitForSeconds(.5f);
        pc.drowningInLava = false;
    }
}

