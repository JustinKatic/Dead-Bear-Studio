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

    [HideInInspector] public bool isAiming = false;

    // The physics layer that will cause the line to stop being drawn
    public LayerMask collidableLayers;
    private PlayerController player;

    void Awake()
    {
        if (!photonView.IsMine)
        {
            Destroy(recticle.gameObject);
        }

        player = GetComponent<PlayerController>();
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        if (!photonView.IsMine)
            return;

        if (isAiming)
        {
            Aim();
        }
    }

    public void Shoot()
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

    public void Aim()
    {
        lineRenderer.enabled = true;
        recticle.SetActive(false);

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

            if (Physics.OverlapSphere(newPoint, 1, collidableLayers).Length > 0)
            {
                lineRenderer.positionCount = points.Count;
                break;
            }
        }
        lineRenderer.SetPositions(points.ToArray());
        isAiming = true;
    }

    public void AimCancelled()
    {
        isAiming = false;
        lineRenderer.enabled = false;
        recticle.SetActive(true);
    }
}
