using Photon.Pun;
using UnityEngine;

public class AoeExplosionAbility : AbilityBase
{
    [SerializeField] float aoeRadius;
    [SerializeField] int aoeDamage;


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
        GameObject createdPrimaryProjectile = PhotonNetwork.Instantiate("LionProjectile", pos, Quaternion.identity);
        createdPrimaryProjectile.transform.forward = dir;

        AoeExplosionProjectileController projectileScript = createdPrimaryProjectile.GetComponent<AoeExplosionProjectileController>();
        projectileScript.Initialize(damage, aoeDamage, player.id, aoeRadius);
        projectileScript.rb.velocity = (hitPoint - pos).normalized * power;
    }
}
