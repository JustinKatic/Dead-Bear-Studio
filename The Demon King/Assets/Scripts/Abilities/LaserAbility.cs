using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.InputSystem;
using System;

public class LaserAbility : MonoBehaviourPun
{
    [Header("Modifiable Stats")]
    [SerializeField] private int damage;
    [SerializeField] private float shootCooldown = 0.3f;
    [SerializeField] private float MaxChargeupTime = 2f;
    [SerializeField] private float damageFrequency = .5f;

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
    private float laserDuration;

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
            isFireing = true;
            canShoot = false;
            LaserLine.enabled = true;
            chargingUp = false;
            if (chargeUpTimer <= .5f)
                laserDuration = .5f;
            else
                laserDuration = chargeUpTimer;
            chargeUpTimer = 0;
        }
    }

    private void OnEnable()
    {
        canShoot = true;
        damageFrequencyTimer = damageFrequency;
    }


    void ChargeUpLaser()
    {
        //Charge up state
        if (chargingUp)
        {
            chargeUpTimer += Time.deltaTime;

            //Shoot laser if max charge time was reached
            if (chargeUpTimer >= MaxChargeupTime)
            {
                laserDuration = MaxChargeupTime;
                LaserLine.enabled = true;
                chargingUp = false;
                canShoot = false;
                chargeUpTimer = 0;
                isFireing = true;
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
                CancelLinerender();
                currentLaserTime = 0;
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
                DealDamageToPlayersAndMinions(hit.collider);
                damageFrequencyTimer = 0;
            }
        }
        else
        {
            LaserLine.SetPosition(0, shootPoint.position);
            LaserLine.SetPosition(1, ray.GetPoint(MaxRayRange));
        }
    }

    public IEnumerator CanShoot(float timer)
    {
        canShoot = false;
        yield return new WaitForSeconds(timer);
        canShoot = true;
    }

    void DealDamageToPlayersAndMinions(Collider other)
    {
        //stores refrence to tag collided with
        string objTag = other.transform.tag;

        if (objTag.Equals("Player"))
        {
            //tell the player who was hit to take damage
            PlayerHealthManager playerHealth = other.GetComponent<PlayerHealthManager>();
            if (playerHealth.PlayerId != player.id)
                playerHealth.TakeDamage(damage, player.id);
        }
        //If tag is Minion
        else if (objTag.Equals("Minion"))
        {
            //tell the minion who was hit to take damage
            MinionHealthManager minionHealth = other.gameObject.GetComponent<MinionHealthManager>();
            minionHealth.TakeDamage(damage, player.id);
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
