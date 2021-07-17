using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.InputSystem;

public class AbilityManager : MonoBehaviourPun
{
    [Header("HeavyArcVariables")]
    private LineRenderer lineRenderer;
    //Number of points on the line
    public int numPoints = 50;
    //Distance between points on the line
    public float timeBetweenPoints = .1f;

    [Header("ProjectileVariables")]
    public float primaryProjectilePower;
    public float blastPower = 5;
    public float shootYAngle;
    public int damage;
    private Vector3 shootAngle;

    [Header("InteractableLayers")]
    public LayerMask PrimaryProjectileLayer;
    // The physics layer that will cause the line to stop being drawn
    public LayerMask collidableLayers;


    [Header("ProjectilePrefabs")]
    public GameObject heavyProjectile;
    public GameObject primaryProjectile;

    [Header("GameObjects")]
    public Transform shotPoint;
    public GameObject recticle;
    public GameObject AoeZone;

    [HideInInspector] public bool isAiming = false;

    private PlayerController player;
    private Camera cam;

    void Awake()
    {
        if (!photonView.IsMine)
        {
            Destroy(recticle.gameObject);
            Destroy(AoeZone);
        }

        player = GetComponent<PlayerController>();
        lineRenderer = GetComponent<LineRenderer>();
        cam = GetComponentInChildren<Camera>();

    }
    private void Start()
    {
        if (photonView.IsMine)
        {
            player.CharacterInputs.Player.Ability1.performed += OnAbility1;
            player.CharacterInputs.Player.Ability1.canceled += OnAbility1Cancelled;

            player.CharacterInputs.Player.Ability2.performed += OnAbility2;
            player.CharacterInputs.Player.Ability2.canceled += OnAbility2Cancelled;
        }
    }

    private void OnAbility1(InputAction.CallbackContext obj)
    {
        if (player.isStunned)
            return;

        ShootPrimaryProjectile();
    }

    private void OnAbility1Cancelled(InputAction.CallbackContext obj)
    {
        if (player.isStunned)
            return;
    }

    private void OnAbility2(InputAction.CallbackContext obj)
    {
        if (player.isStunned)
            return;

        HeavyProjectileAim();
    }

    private void OnAbility2Cancelled(InputAction.CallbackContext obj)
    {
        if (player.isStunned)
            return;

        HeavyProjectileAimCancelled();
    }



    private void Update()
    {
        if (!photonView.IsMine)
            return;

        if (isAiming)
        {
            HeavyProjectileAim();
        }
    }

    public void ShootPrimaryProjectile()
    {
        RaycastHit hit;
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, ~PrimaryProjectileLayer))
        {
            player.photonView.RPC("SpawnPrimaryProjectile", RpcTarget.All, shotPoint.position, shotPoint.transform.forward, primaryProjectilePower, hit.point);
        }
        else
        {
            player.photonView.RPC("SpawnPrimaryProjectile", RpcTarget.All, shotPoint.position, shotPoint.transform.forward, primaryProjectilePower, ray.GetPoint(400f));
        }
    }

    [PunRPC]
    void SpawnPrimaryProjectile(Vector3 pos, Vector3 dir, float power, Vector3 hitPoint)
    {
        GameObject createdPrimaryProjectile = Instantiate(primaryProjectile, pos, Quaternion.identity);
        createdPrimaryProjectile.transform.forward = dir;

        PrimaryProjectileController projectileScript = createdPrimaryProjectile.GetComponent<PrimaryProjectileController>();
        projectileScript.Initialize(damage, player.id, player.photonView.IsMine);
        projectileScript.rb.velocity = (hitPoint - pos).normalized * power;
    }


    public void ShootHeavyProjectile()
    {
        if (isAiming)
        {
            player.photonView.RPC("SpawnHeavyProjectile", RpcTarget.All, shotPoint.position, shotPoint.transform.forward, shootAngle, blastPower);
        }
    }

    [PunRPC]
    void SpawnHeavyProjectile(Vector3 pos, Vector3 dir, Vector3 shootDir, float power)
    {
        GameObject createdHeavyProjectile = Instantiate(heavyProjectile, pos, Quaternion.identity);
        createdHeavyProjectile.transform.forward = dir;

        HeavyProjectileController projectileSctipt = createdHeavyProjectile.GetComponent<HeavyProjectileController>();

        projectileSctipt.Initialize(damage, player.id, player.photonView.IsMine);
        projectileSctipt.rb.velocity = shootDir * power;
    }

    public void HeavyProjectileAim()
    {
        shotPoint.transform.rotation = Quaternion.Euler(cam.transform.eulerAngles.x, shotPoint.transform.eulerAngles.y, shotPoint.transform.eulerAngles.z);
        //Enables the Arc for the heavy projectile and disables the primary reticule
        lineRenderer.enabled = true;
        recticle.SetActive(false);
        AoeZone.SetActive(true);

        shootAngle = new Vector3(shotPoint.transform.forward.x, shotPoint.transform.forward.y + shootYAngle, shotPoint.transform.forward.z);

        lineRenderer.positionCount = numPoints;
        List<Vector3> points = new List<Vector3>();
        // Calculate the trajectory of the projectile based off the given position and velocity
        Vector3 startingPosition = shotPoint.position;
        Vector3 startingVelocity = shootAngle * blastPower;

        //Creates a list of points for the Line renderer based off the given points and distance between points
        //Continues adding to the list if an object has not been hit with a collidable layer
        for (float t = 0; t < numPoints; t += timeBetweenPoints)
        {
            Vector3 newPoint = startingPosition + t * startingVelocity;
            newPoint.y = startingPosition.y + startingVelocity.y * t + Physics.gravity.y / 2f * t * t;
            points.Add(newPoint);

            if (Physics.OverlapSphere(newPoint, .4f, collidableLayers).Length > 0)
            {
                lineRenderer.positionCount = points.Count;
                break;
            }
        }
        AoeZone.transform.position = new Vector3(points[points.Count - 1].x, points[points.Count - 1].y, points[points.Count - 1].z);
        lineRenderer.SetPositions(points.ToArray());
        isAiming = true;
    }

    //Set the Aoe zone to and line renderer to inactive 
    //Set the reticule back to active for the primary attack
    public void HeavyProjectileAimCancelled()
    {
        ShootHeavyProjectile();
        AoeZone.SetActive(false);
        isAiming = false;
        lineRenderer.enabled = false;
        recticle.SetActive(true);
    }
}
