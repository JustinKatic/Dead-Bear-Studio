using Photon.Pun;
using UnityEngine;

public class DragonAbility : AbilityBase
{
    [SerializeField] private float damageFrequency;
    [SerializeField] private float gasDuration;
    [SerializeField] private float gasSize;

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
            SpawnPrimaryProjectile(shootPoint.position, shootPoint.transform.forward, hit.point);
        }
        else
        {
            SpawnPrimaryProjectile(shootPoint.position, shootPoint.transform.forward, ray.GetPoint(400f));
        }
    }

    void SpawnPrimaryProjectile(Vector3 pos, Vector3 dir, Vector3 hitPoint)
    {
        GameObject createdPrimaryProjectile = PhotonNetwork.Instantiate("DragonProjectile", pos, Quaternion.identity);
        createdPrimaryProjectile.transform.forward = dir;

        DragonProjectileController projectileScript = createdPrimaryProjectile.GetComponent<DragonProjectileController>();
        projectileScript.Initialize(player.id,damage,damageFrequency,gasDuration,gasSize);
        projectileScript.rb.velocity = (hitPoint - pos).normalized * ProjectileSpeed;
    }
}
