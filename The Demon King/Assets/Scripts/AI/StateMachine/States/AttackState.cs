using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class AttackState : State
{
    public bool CanAttack = false;
    [SerializeField] private GameObject[] attackVFX;

    public override void RunCurrentState()
    {
        PlayAttackState();
    }

    private void PlayAttackState()
    {
        var towardsPlayer = target.transform.position - minionTransform.position;

        minionTransform.rotation = Quaternion.RotateTowards(
            minionTransform.rotation,
            Quaternion.LookRotation(towardsPlayer),
            Time.deltaTime * 180
        );

        agent.SetDestination(target.transform.position + Vector3.one);

        if (CanAttack)
        {
            target.GetComponent<PlayerHealthManager>().TakeDamage(1, 0);
            int randVfx = Random.Range(0, attackVFX.Length);
            AttackVFX(randVfx);
        }
    }

    void AttackVFX(int VfxToPlay)
    {
        photonView.RPC("PlayAttackVFX_RPC", RpcTarget.All, VfxToPlay);
    }

    [PunRPC]
    void PlayAttackVFX_RPC(int VfxToPlay)
    {
        StartCoroutine(PlayVFX(VfxToPlay));
    }

    IEnumerator PlayVFX(int VfxToPlay)
    {
        attackVFX[VfxToPlay].SetActive(true);
        yield return new WaitForSeconds(0.5f);
        attackVFX[VfxToPlay].SetActive(false);
    }
}
