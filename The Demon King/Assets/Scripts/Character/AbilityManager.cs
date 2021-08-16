using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.InputSystem;

public class AbilityManager : MonoBehaviourPun
{
    [Header("ProjectileVariables")]
    public float primaryProjectilePower;
    public int damage;
    public float primaryProjectileDelay = 0.3f;

    [Header("LayersForRaycastToIgnore")]
    public LayerMask PrimaryProjectileLayersToIgnore;

    private EvolutionManager evolutionManager;


    [Header("ProjectilePrefabs")]
    public GameObject primaryProjectile;

    [Header("GameObjects")]
    public Transform shootPoint;
    public GameObject recticle;

    private bool canCast = true;


    private PlayerController player;
    private Camera cam;

    void Awake()
    {
        player = GetComponent<PlayerController>();
        cam = Camera.main;

        if (photonView.IsMine)
        {
            evolutionManager = GetComponent<EvolutionManager>();
        }

        if (!photonView.IsMine)
        {
            Destroy(recticle.gameObject);
        }
    }
    private void Start()
    {
        if (photonView.IsMine)
        {
            player.CharacterInputs.Player.Ability1.performed += OnAbility1;
            player.CharacterInputs.Player.Ability1.canceled += OnAbility1Cancelled;
        }
    }

    private void OnAbility1(InputAction.CallbackContext obj)
    {
        if (canCast)
        {
            ShootPrimaryProjectile();
            player.currentAnim.SetTrigger("Attack");
            StartCoroutine(CanCast(primaryProjectileDelay));
            PlayerSoundManager.Instance.PlayAbility1Sound();
        }
    }

    private void OnAbility1Cancelled(InputAction.CallbackContext obj)
    {

    }

    public void ShootPrimaryProjectile()
    {
        RaycastHit hit;
        shootPoint = evolutionManager.currentActiveShootPoint;
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, ~PrimaryProjectileLayersToIgnore))
        {
            SpawnPrimaryProjectile(shootPoint.position, shootPoint.transform.forward, primaryProjectilePower, hit.point);
        }
        else
        {
            SpawnPrimaryProjectile(shootPoint.position, shootPoint.transform.forward, primaryProjectilePower, ray.GetPoint(400f));
        }
    }

    [PunRPC]
    void SpawnPrimaryProjectile(Vector3 pos, Vector3 dir, float power, Vector3 hitPoint)
    {
        GameObject createdPrimaryProjectile = PhotonNetwork.Instantiate("PrimaryProjectile", pos, Quaternion.identity);
        createdPrimaryProjectile.transform.forward = dir;

        PrimaryProjectileController projectileScript = createdPrimaryProjectile.GetComponent<PrimaryProjectileController>();
        projectileScript.Initialize(damage, player.id);
        projectileScript.rb.velocity = (hitPoint - pos).normalized * power;
    }

    public IEnumerator CanCast(float timer)
    {
        canCast = false;
        yield return new WaitForSeconds(timer);
        canCast = true;

    }
}
