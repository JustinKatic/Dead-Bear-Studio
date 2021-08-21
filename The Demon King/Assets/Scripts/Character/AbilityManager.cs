using System.Collections;
using UnityEngine;
using Photon.Pun;
using UnityEngine.InputSystem;

public class AbilityManager : MonoBehaviourPun
{
    [Header("ProjectileVariables")]
    [SerializeField] private float primaryProjectilePower;
    [SerializeField] private int damage;
    [SerializeField] private float shootTimer = 0.3f;

    [Header("LayersForRaycastToIgnore")]
    [SerializeField] private LayerMask PrimaryProjectileLayersToIgnore;

    [Header("ProjectilePrefabs")]
    [SerializeField] private GameObject primaryProjectile;

    [Header("GameObjects")]
    [SerializeField] private Transform shootPoint;
    [SerializeField] private GameObject recticle;

    private bool canShoot = true;

    //Components
    private EvolutionManager evolutionManager;
    private PlayerController player;
    private Camera cam;

    #region Start Up
    void Awake()
    {
        if (photonView.IsMine)
        {
            evolutionManager = GetComponent<EvolutionManager>();
            player = GetComponent<PlayerController>();
            cam = Camera.main;
        }
        else
            Destroy(recticle.gameObject);
    }
    private void Start()
    {
        if (photonView.IsMine)
            player.CharacterInputs.Player.Ability1.performed += OnAbility1;
    }
    #endregion

    #region Shoot Projectile
    private void OnAbility1(InputAction.CallbackContext obj)
    {
        if (canShoot)
        {
            ShootPrimaryProjectile();
            player.currentAnim.SetTrigger("Attack");
            StartCoroutine(CanCast(shootTimer));
            PlayerSoundManager.Instance.PlayAbility1Sound();
        }
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
        canShoot = false;
        yield return new WaitForSeconds(timer);
        canShoot = true;
    }
    #endregion
}
