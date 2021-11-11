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

    [SerializeField] private GameObject laserChargeUpParent;
    [SerializeField] private ParticleSystem laserGatheringParticle;

    private Material ChargeUpMat;
    [SerializeField] ParticleSystemRenderer chargeUpPS;
    [SerializeField] private float laserShrinkSpeed = 10;

    private void Start()
    {
        ChargeUpMat = Instantiate(chargeUpPS.material);
        chargeUpPS.material = ChargeUpMat;

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
            CancelShooting();
            currentLaserTime = 0;
        }
    }

    private void Update()
    {
        if (photonView.IsMine)
        {
            if (devour.IsDevouring || playerHealthManager.beingDevoured)
            {
                DisableLaser();
            }
            else
            {
                ChargeUpLaser();

                FireingLaser();
            }
        }
    }

    private void OnDisable()
    {
        canShoot = false;
    }

    private void Ability1_performed(InputAction.CallbackContext obj)
    {
        if (canShoot)
        {
            laserGatheringParticle.gameObject.transform.localScale = Vector3.one;
            laserGatheringParticle.gameObject.SetActive(true);
            ChargeUpMat.SetFloat("_RadialEffectTime", 1);
            StartChargeUpLaser();
            chargingUp = true;
        }
    }

    private void DisableLaser()
    {
        isFireing = false;
        LaserLine.enabled = false;
        rayEndVFX.SetActive(false);
        chargedUp = false;
        CancelShooting();
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

            Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            laserChargeUpParent.transform.position = ray.GetPoint(5f);
            laserChargeUpParent.transform.eulerAngles = cam.transform.eulerAngles;

            float valToBeLerped = Mathf.Lerp(0, 1, (chargeUpTimer / ChargeupTime));
            ChargeUpMat.SetFloat("_RadialEffectTime", valToBeLerped);

            UpdateChargeUpEffect(valToBeLerped, laserChargeUpParent.transform.position, laserChargeUpParent.transform.eulerAngles);

            PlayerSoundManager.Instance.PlayRayChargeUpSound();

            if (chargeUpTimer >= ChargeupTime)
            {
                PlayerSoundManager.Instance.StopRayChargeUpSound();
                PlayerSoundManager.Instance.PlayRayFullyChargedUpShootSound();
                SetFireingTrue();
                chargedUp = true;
            }
        }
    }


    void StartChargeUpLaser()
    {
        photonView.RPC("StartChargeUpLaser_RPC", RpcTarget.Others);
    }

    [PunRPC]
    void StartChargeUpLaser_RPC()
    {
        laserGatheringParticle.gameObject.transform.localScale = Vector3.one;
        laserGatheringParticle.gameObject.SetActive(true);
    }


    void FireingLaser()
    {
        //Fireing laser state
        if (isFireing)
        {
            //shoot laser
            ShootLaser();
            currentLaserTime += Time.deltaTime;

            Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            laserChargeUpParent.transform.position = ray.GetPoint(5f);
            laserChargeUpParent.transform.eulerAngles = cam.transform.eulerAngles;

            float valToBeLerped = Mathf.Lerp(1, 0, (ChargeupTime / laserDuration));
            ChargeUpMat.SetFloat("_RadialEffectTime", valToBeLerped);

            UpdateChargeUpEffect(valToBeLerped, laserChargeUpParent.transform.position, laserChargeUpParent.transform.eulerAngles);

            //end laser reset its values ready for next shot
            if (currentLaserTime >= laserDuration || playerHealthManager.isStunned)
            {
                isFireing = false;
                LaserLine.enabled = false;
                rayEndVFX.SetActive(false);
                chargedUp = false;
                CancelShooting();
                currentLaserTime = 0;
                StartCoroutine(CanShoot(shootCooldown));
                damageFrequencyTimer = damageFrequency;
                StartCoroutine(SetChargeUpEffectFalse());
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
            DisplayLaserShooting(hit.point.x, hit.point.y, hit.point.z);


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
        player.currentAnim.SetTrigger("Attack");
    }
    IEnumerator SetChargeUpEffectFalse()
    {
        float timer = 0;

        while (timer < 0.25f)
        {
            timer += Time.deltaTime;
            laserGatheringParticle.gameObject.transform.localScale = Vector3.Lerp(laserGatheringParticle.gameObject.transform.localScale, Vector3.zero, laserShrinkSpeed * Time.deltaTime);

            yield return null;
        }

        laserGatheringParticle.gameObject.SetActive(false);
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

    void UpdateChargeUpEffect(float chargeUpVal, Vector3 chargePos, Vector3 ChargeRot)
    {
        photonView.RPC("UpdateChargeUpEffect_RPC", RpcTarget.Others, chargeUpVal, chargePos.x, chargePos.y, chargePos.z, ChargeRot.x, ChargeRot.y, ChargeRot.z);
    }

    [PunRPC]
    void UpdateChargeUpEffect_RPC(float chargeUpVal, float chargeX, float chargeY, float chargeZ, float chargeRotX, float chargeRotY, float chargeRotZ)
    {
        ChargeUpMat.SetFloat("_RadialEffectTime", chargeUpVal);

        laserChargeUpParent.transform.position = new Vector3(chargeX, chargeY, chargeZ);
        laserChargeUpParent.transform.eulerAngles = new Vector3(chargeRotX, chargeRotY, chargeRotZ);
    }


    #region Display LineRedner
    void DisplayLaserShooting(float L1X, float L1Y, float L1Z)
    {
        photonView.RPC("DisplayLaserShooting_RPC", RpcTarget.Others, L1X, L1Y, L1Z);
    }

    [PunRPC]
    void DisplayLaserShooting_RPC(float L1X, float L1Y, float L1Z)
    {
        rayEndVFX.SetActive(true);
        LaserLine.SetPosition(0, shootPoint.position);
        LaserLine.SetPosition(1, new Vector3(L1X, L1Y, L1Z));
        rayEndVFX.transform.position = new Vector3(L1X, L1Y, L1Z);
        LaserLine.enabled = true;
    }


    void CancelShooting()
    {
        photonView.RPC("CancelShooting_RPC", RpcTarget.Others);
    }

    [PunRPC]
    void CancelShooting_RPC()
    {
        StartCoroutine(SetChargeUpEffectFalse());
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
