using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class ShootController : MonoBehaviourPun
{
    private LineRenderer lineRenderer;

    //Number of points on the line
    public int numPoints = 50;
    //Distance between points on the line
    public float timeBetweenPoints = .1f;
    public float blastPower = 5;
    public float shootYAngle;
    public int damage;

    private Vector3 shootAngle;
    public GameObject heavyProjectile;
    public Transform shotPoint;
    public GameObject recticle;
    public GameObject AoeZone;

    public bool PrimaryProjectileActive = false;
    public bool HeavyProjectileActive = false;

    [HideInInspector] public bool isAiming = false;

    // The physics layer that will cause the line to stop being drawn
    public LayerMask collidableLayers;
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

    private void Update()
    {
        if (!photonView.IsMine)
            return;

        if (isAiming)
        {
            HeavyProjectileAim();
        }
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
        lineRenderer.enabled = true;
        recticle.SetActive(false);
        AoeZone.SetActive(true);

        shootAngle = new Vector3(shotPoint.transform.forward.x, shotPoint.transform.forward.y + shootYAngle, shotPoint.transform.forward.z);

        lineRenderer.positionCount = numPoints;
        List<Vector3> points = new List<Vector3>();
        Vector3 startingPosition = shotPoint.position;
        Vector3 startingVelocity = shootAngle * blastPower;

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

    public void HeavyProjectileAimCancelled()
    {
        AoeZone.SetActive(false);
        isAiming = false;
        lineRenderer.enabled = false;
        recticle.SetActive(true);
    }
}
