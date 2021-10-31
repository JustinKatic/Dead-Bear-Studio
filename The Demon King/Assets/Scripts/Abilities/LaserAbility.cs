using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.InputSystem;
using System;

public class LaserAbility : MonoBehaviourPun
{
    public PlayerControllerRuntimeSet playerControllerRuntimeSet;

    //[Header("Damage")]
    [SerializeField] private int damage = 1;
    //[Header("Timers")]
    [SerializeField] private float shootCooldown = 1f;
    //[SerializeField] private float ChargeupTime = 2f;
    [SerializeField] private float ChargeupTime = 3f;


    //[Header("Laser")]
    [SerializeField] private float baseLaserDuration = 1f;
    private float laserDuration;

    [SerializeField] private float damageFrequency = .2f;
    [SerializeField] private GameObject rayEndVFX;


    [FMODUnity.EventRef]
    [SerializeField] string OnHitSound;


    [Header("Layers For Raycast To Ignore")]
    [SerializeField] private LayerMask LayersForRaycastToIgnore;

    [Header("Game Objects")]
    [SerializeField] protected Transform shootPoint;


    private bool canShoot = true;
    private bool isFireing = false;
    private bool chargingUp = false;
    private float MaxRayRange = 500f;

    //Timers
    private float chargeUpTimer = 0;
    private float currentLaserTime;
    private bool chargedUp = false;

    private float damageFrequencyTimer;

    //Components
    private LineRenderer LaserLine;
    PlayerHealthManager playerHealthManager;
    private Devour devour;
    private Camera cam;
    private PlayerController player;
    [SerializeField] private PlayerTimers timers;


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
            devour = GetComponentInParent<Devour>();
        }
    }

    private void OnEnable()
    {
        canShoot = true;
        damageFrequencyTimer = damageFrequency;
        if (isFireing)
        {
            chargedUp = false;
            isFireing = false;
            LaserLine.enabled = false;
            rayEndVFX.SetActive(false);
            CancelLinerender();
            currentLaserTime = 0;
        }
    }

    private void Update()
    {
        if (photonView.IsMine)
        {
            if (devour.IsDevouring || playerHealthManager.beingDevoured)
            {
                timers.StopRayAbilityTimer();
                DisableLaser();
            }
            else
            {
                ChargeUpLaser();

                FireingLaser();
            }
        }

    }

    private void Ability1_performed(InputAction.CallbackContext obj)
    {
        if (canShoot)
        {
            chargingUp = true;
            timers.StartRayAbilityTimer(ChargeupTime);
        }
    }

    private void DisableLaser()
    {
        isFireing = false;
        LaserLine.enabled = false;
        rayEndVFX.SetActive(false);
        chargedUp = false;
        CancelLinerender();
        currentLaserTime = 0;
        StartCoroutine(CanShoot(shootCooldown));
        damageFrequencyTimer = damageFrequency;
    }

    private void Ability1_cancelled(InputAction.CallbackContext obj)
    {
        if (!isFireing && chargingUp)
        {
            SetFireingTrue();
            PlayerSoundManager.Instance.StopRayChargeUpSound();

            timers.StartRayAbilityBackwardsTimer(chargeUpTimer);

            if (chargedUp)
                PlayerSoundManager.Instance.PlayRayFullyChargedUpShootSound();
            else
                PlayerSoundManager.Instance.PlayCastAbilitySound();
        }
    }


    void ChargeUpLaser()
    {
        if (chargingUp)
        {
            chargeUpTimer += Time.deltaTime;

            PlayerSoundManager.Instance.PlayRayChargeUpSound();

            if (chargeUpTimer >= ChargeupTime)
            {
                PlayerSoundManager.Instance.StopRayChargeUpSound();
                PlayerSoundManager.Instance.PlayRayFullyChargedUpShootSound();
                timers.StartRayAbilityBackwardsTimer(chargeUpTimer);
                SetFireingTrue();
            }

            //Shoot laser if max charge time was reached
            if (chargeUpTimer >= ChargeupTime)
            {
                chargedUp = true;
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
                rayEndVFX.SetActive(false);
                chargedUp = false;
                CancelLinerender();
                currentLaserTime = 0;
                StartCoroutine(CanShoot(shootCooldown));
                damageFrequencyTimer = damageFrequency;
                timers.StopRayAbilityTimer();
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
            if (rayEndVFX.activeInHierarchy)
                rayEndVFX.transform.position = hit.point;
            DisplayLinerender(hit.point.x, hit.point.y, hit.point.z);


            if (damageFrequencyTimer >= damageFrequency)
            {
                PhotonNetwork.Instantiate("RayImpactFX", hit.point, Quaternion.identity);
                FMODUnity.RuntimeManager.PlayOneShot(OnHitSound, hit.point);
                PlayHitSound(hit.point.x, hit.point.y, hit.point.z);

                if (chargeUpTimer >= 1)
                    laserDuration = chargeUpTimer;
                else
                    laserDuration = baseLaserDuration;

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
        rayEndVFX.SetActive(true);
        chargingUp = false;
        canShoot = false;
        isFireing = true;
    }


    //Shoot cooldown
    public IEnumerator CanShoot(float timer)
    {
        canShoot = false;
        yield return new WaitForSeconds(timer);
        canShoot = true;
        chargeUpTimer = 0;
    }

    //Deal dmg if ray hits player or minion
    void DealDamageToPlayersAndMinions(Collider other, int DamageToDeal)
    {
        //stores refrence to tag collided with
        string objTag = other.transform.tag;

        if (objTag.Equals("Player"))
        {
            //tell the player who was hit to take damage
            PlayerHealthManager playerHealth = other.GetComponentInParent<PlayerHealthManager>();
            if (playerHealth.PlayerId != player.id)
            {
                playerHealth.TakeDamage(DamageToDeal, player.id);
                playerControllerRuntimeSet.GetPlayer(player.id).PlayRectAnim();

                playerControllerRuntimeSet.GetPlayer(player.id).IncreaseRayDamage(damage);
            }
        }
        //If tag is Minion
        else if (objTag.Equals("Minion"))
        {
            //tell the minion who was hit to take damage
            MinionHealthManager minionHealth = other.GetComponentInParent<MinionHealthManager>();
            minionHealth.TakeDamage(DamageToDeal, player.id);
            playerControllerRuntimeSet.GetPlayer(player.id).PlayRectAnim();
            playerControllerRuntimeSet.GetPlayer(player.id).IncreaseRayDamage(damage);
        }
    }


    #region Display LineRedner
    void DisplayLinerender(float L1X, float L1Y, float L1Z)
    {
        photonView.RPC("DisplayLinerender_RPC", RpcTarget.Others, L1X, L1Y, L1Z);
    }

    [PunRPC]
    void DisplayLinerender_RPC(float L1X, float L1Y, float L1Z)
    {
        rayEndVFX.SetActive(true);
        LaserLine.SetPosition(0, shootPoint.position);
        LaserLine.SetPosition(1, new Vector3(L1X, L1Y, L1Z));
        rayEndVFX.transform.position = new Vector3(L1X, L1Y, L1Z);
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
        rayEndVFX.SetActive(false);
    }
    #endregion


    #region Laser Hit Sounds
    void PlayHitSound(float hitX, float hitY, float hitZ)
    {
        photonView.RPC("PlayHitSound_RPC", RpcTarget.Others, hitX, hitY, hitZ);
    }

    [PunRPC]
    void PlayHitSound_RPC(float hitX, float hitY, float hitZ)
    {
        FMODUnity.RuntimeManager.PlayOneShot(OnHitSound, new Vector3(hitX, hitY, hitZ));
    }
    #endregion

    private void OnDestroy()
    {
        if (photonView.IsMine)
        {
            player.CharacterInputs.Player.Ability1.performed -= Ability1_performed;
            player.CharacterInputs.Player.Ability1.performed -= Ability1_cancelled;
        }
    }
}
