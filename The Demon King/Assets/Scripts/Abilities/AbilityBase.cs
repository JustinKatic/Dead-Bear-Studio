using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


[System.Serializable]
public class AbilityBase : MonoBehaviourPun
{
    [Header("Modifiable Stats")]
    [SerializeField] protected float ProjectileSpeed;
    [SerializeField] protected int damage;
    [SerializeField] protected float shootCooldown = 0.3f;

    private bool canShoot = true;

    [Header("GameObjects")]
    [SerializeField] protected Transform shootPoint;

    protected Camera cam;

    protected PlayerController player;


    private void Start()
    {
        if (photonView.IsMine)
        {
            cam = Camera.main;
            player = GetComponentInParent<PlayerController>();
            player.CharacterInputs.Player.Ability1.performed += Ability1_performed;
        }
    }

    private void OnDestroy()
    {
        if (photonView.IsMine)
            player.CharacterInputs.Player.Ability1.performed -= Ability1_performed;
    }

    private void OnEnable()
    {
        canShoot = true;
    }

    private void Ability1_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (!canShoot || !gameObject.activeSelf)
            return;

        player.currentAnim.SetTrigger("Attack");
        PlayerSoundManager.Instance.PlayCastAbilitySound();
        PerformAbility();
        StartCoroutine(CanShoot(shootCooldown));
    }

    protected virtual void PerformAbility()
    {

    }

    public IEnumerator CanShoot(float timer)
    {
        canShoot = false;
        yield return new WaitForSeconds(timer);
        canShoot = true;
    }
}
