using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class DefaultProjectile : AbilityBase
{
    [Header("LayersForRaycastToIgnore")]
    [SerializeField] private LayerMask PrimaryProjectileLayersToIgnore;
    protected override void PerformAbility()
    {
        ShootPrimaryProjectile();
    }


    public void ShootPrimaryProjectile()
    {
        RaycastHit hit;
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, ~PrimaryProjectileLayersToIgnore))
        {
            SpawnPrimaryProjectile(shootPoint.position, shootPoint.transform.forward, ProjectileSpeed, hit.point);
        }
        else
        {
            SpawnPrimaryProjectile(shootPoint.position, shootPoint.transform.forward, ProjectileSpeed, ray.GetPoint(400f));
        }
    }

    void SpawnPrimaryProjectile(Vector3 pos, Vector3 dir, float power, Vector3 hitPoint)
    {
        GameObject createdPrimaryProjectile = PhotonNetwork.Instantiate("AoeExplosionProjectile", pos, Quaternion.identity);
        createdPrimaryProjectile.transform.forward = dir;

        DefaultProjectileController projectileScript = createdPrimaryProjectile.GetComponent<DefaultProjectileController>();
        projectileScript.Initialize(damage, player.id);
        projectileScript.rb.velocity = (hitPoint - pos).normalized * power;
    }
}
