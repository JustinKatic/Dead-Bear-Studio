using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.InputSystem;
using System;

public class LaserAbility : MonoBehaviourPun
{
    [Header("Damage")]
    [SerializeField] private int damage;
    [SerializeField] private int ChargedUpDamage;
    [Header("Timers")]
    [SerializeField] private float shootCooldown = 1f;
    [SerializeField] private float ChargeupTime = 2f;
    [SerializeField] private float ShootAutomaticallyAt = 3f;

    [Header("Extended Laser Duration")]
    [SerializeField] private bool shouldLaserDurationIncrease = false;
    [SerializeField] private float laserExtendedDurationTime = 2f;
    [SerializeField] private float damageFrequency = .5f;

    [Header("Laser")]
    private float laserDuration = .4f;


    [Header("Layers For Raycast To Ignore")]
    [SerializeField] private LayerMask LayersForRaycastToIgnore;

    [Header("Game Objects")]
    [SerializeField] protected Transform shootPoint;


    private bool canShoot = true;
    private bool isFireing = false;
    private bool chargingUp;
    private float MaxRayRange = 200f;

    //Timers
    private float chargeUpTimer = 0;
    private float currentLaserTime;
    private bool chargedUp = false;

    private float damageFrequencyTimer;

    //Components
    private LineRenderer LaserLine;
    PlayerHealthManager playerHealthManager;
    private Camera cam;
    private PlayerController player;


    private void Start()
    {
        LaserLine = GetComponent<LineRenderer>();
        if (photonView.IsMine)
        {
            cam = Camera.main;
            player = GetComponentInParent<PlayerController>();
            player.CharacterInputs.Player.Ability1.performed += Ability1_performed;
            player.CharacterInputs.Player.Ability1.canceled += Ability1_cancelled;
            playerHealthManager = GetComponentInParent<PlayerHealthManager>();
        }
    }

    private void OnEnable()
    {
        canShoot = true;
        damageFrequencyTimer = damageFrequency;
    }

    private void Update()
    {
        ChargeUpLaser();

        FireingLaser();
    }

    private void Ability1_performed(InputAction.CallbackContext obj)
    {
        if (canShoot)
        {
            chargingUp = true;
            ChargeUpLaser();
        }
    }

    private void Ability1_cancelled(InputAction.CallbackContext obj)
    {
        if (!isFireing && canShoot && chargingUp)
        {
            SetFireingTrue();
            PlayerSoundManager.Instance.StopRayChargeUpSound();

            if (chargedUp)
            {
                PlayerSoundManager.Instance.PlayRayFullyChargedUpShootSound();
            }
            else
            {
                PlayerSoundManager.Instance.PlayCastAbilitySound();
            }

            if (shouldLaserDurationIncrease && chargedUp)
            {
                laserDuration = chargeUpTimer;
            }
            else
                laserDuration = .4f;
        }
    }


    void ChargeUpLaser()
    {
        //Charge up state
        if (chargingUp)
        {
            chargeUpTimer += Time.deltaTime;
            PlayerSoundManager.Instance.PlayRayChargeUpSound();

            if (chargeUpTimer >= ShootAutomaticallyAt)
            {
                PlayerSoundManager.Instance.StopRayChargeUpSound();
                PlayerSoundManager.Instance.PlayRayFullyChargedUpShootSound();
                SetFireingTrue();
            }

            //Shoot laser if max charge time was reached
            if (chargeUpTimer >= ChargeupTime)
            {
                chargedUp = true;

                if (shouldLaserDurationIncrease)
                {
                    laserDuration = laserExtendedDurationTime;
                }
            }
        }
    }

    void FireingLaser()
    {
        //Fireing laser state
        if (isFireing)
        {
            //shoot laser
            ShootLaser();
            currentLaserTime += Time.deltaTime;

            //end laser reset its values ready for next shot
            if (currentLaserTime >= laserDuration || playerHealthManager.isStunned)
            {
                isFireing = false;
                LaserLine.enabled = false;
                chargedUp = false;
                CancelLinerender();
                currentLaserTime = 0;
                chargeUpTimer = 0;
                StartCoroutine(CanShoot(shootCooldown));
                damageFrequencyTimer = damageFrequency;
            }
        }
    }

    public void ShootLaser()
    {
        damageFrequencyTimer += Time.deltaTime;

        RaycastHit hit;
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, ~LayersForRaycastToIgnore))
        {
            LaserLine.SetPosition(0, shootPoint.position);
            LaserLine.SetPosition(1, hit.point);
            DisplayLinerender(hit.point.x, hit.point.y, hit.point.z);


            if (damageFrequencyTimer >= damageFrequency)
            {
                PhotonNetwork.Instantiate("RayImpactFX", hit.point, Quaternion.identity);

                if (chargedUp)
                    DealDamageToPlayersAndMinions(hit.collider, ChargedUpDamage);
                else
                    DealDamageToPlayersAndMinions(hit.collider, damage);

                damageFrequencyTimer = 0;
            }
        }
        else
        {
            LaserLine.SetPosition(0, shootPoint.position);
            LaserLine.SetPosition(1, ray.GetPoint(MaxRayRange));
        }
    }

    void SetFireingTrue()
    {
        LaserLine.enabled = true;
        chargingUp = false;
        canShoot = false;
        chargeUpTimer = 0;
        isFireing = true;
    }

    public IEnumerator CanShoot(float timer)
    {
        canShoot = false;
        yield return new WaitForSeconds(timer);
        canShoot = true;
    }

    void DealDamageToPlayersAndMinions(Collider other, int DamageToDeal)
    {
        //stores refrence to tag collided with
        string objTag = other.transform.tag;

        if (objTag.Equals("Player"))
        {
            //tell the player who was hit to take damage
            PlayerHealthManager playerHealth = other.GetComponent<PlayerHealthManager>();
            if (playerHealth.PlayerId != player.id)
                playerHealth.TakeDamage(DamageToDeal, player.id);
        }
        //If tag is Minion
        else if (objTag.Equals("Minion"))
        {
            //tell the minion who was hit to take damage
            MinionHealthManager minionHealth = other.gameObject.GetComponent<MinionHealthManager>();
            minionHealth.TakeDamage(DamageToDeal, player.id);
        }
    }

    void DisplayLinerender(float L1X, float L1Y, float L1Z)
    {
        photonView.RPC("DisplayLinerender_RPC", RpcTarget.Others, L1X, L1Y, L1Z);
    }

    [PunRPC]
    void DisplayLinerender_RPC(float L1X, float L1Y, float L1Z)
    {
        LaserLine.SetPosition(0, shootPoint.position);
        LaserLine.SetPosition(1, new Vector3(L1X, L1Y, L1Z));
        LaserLine.enabled = true;
    }

    void CancelLinerender()
    {
        photonView.RPC("CancelLinerender_RPC", RpcTarget.Others);
    }

    [PunRPC]
    void CancelLinerender_RPC()
    {
        LaserLine.enabled = false;
    }
}
