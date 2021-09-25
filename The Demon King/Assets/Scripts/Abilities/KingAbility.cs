using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class KingAbility : MonoBehaviourPun
{
    //[Header("Modifiable Stats")]
    [SerializeField] private int damage;
    [SerializeField] private float damageFrequency;
    [SerializeField] private float abilityDuration;
    [SerializeField] private string kingAbilityEffectName;



    [SerializeField] private float shootCooldown = 1f;
    [SerializeField] private GameObject LionKingAbilityMarker = null;
    [SerializeField] private float abilityMarkerSize = 4;
    [SerializeField] private LayerMask AimerLayersToIgnore;

    private Vector3 targetPos;


    private Camera cam;

    private bool canShoot = true;
    private bool aiming = false;
    private bool hittingTarget = false;

    protected PlayerController player;


    private void Start()
    {
        if (photonView.IsMine)
        {
            cam = Camera.main;
            player = GetComponentInParent<PlayerController>();
            player.CharacterInputs.Player.Ability2.performed += Ability2_performed;
            player.CharacterInputs.Player.Ability2.canceled += Ability2_canceled;
        }
    }

    private void Update()
    {
        if (aiming)
        {
            RaycastHit hit;
            Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, ~AimerLayersToIgnore))
            {
                LionKingAbilityMarker.SetActive(true);
                LionKingAbilityMarker.transform.position = hit.point;
                targetPos = hit.point;
                hittingTarget = true;
            }
            else
            {
                LionKingAbilityMarker.SetActive(false);
                hittingTarget = false;
            }
        }
    }

    private void OnDestroy()
    {
        if (photonView.IsMine)
        {
            player.CharacterInputs.Player.Ability2.canceled -= Ability2_canceled;
            player.CharacterInputs.Player.Ability2.performed -= Ability2_performed;
        }
    }

    private void OnEnable()
    {
        canShoot = true;
    }

    private void Ability2_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (!canShoot || !gameObject.activeSelf)
            return;

        LionKingAbilityMarker.transform.SetParent(null);
        LionKingAbilityMarker.transform.localScale = new Vector3(abilityMarkerSize, abilityMarkerSize, abilityMarkerSize);
        LionKingAbilityMarker.transform.SetParent(transform);

        aiming = true;
    }

    private void Ability2_canceled(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (!aiming || !hittingTarget)
            return;
        aiming = false;
        LionKingAbilityMarker.SetActive(false);

        player.currentAnim.SetTrigger("Attack");
        PlayerSoundManager.Instance.PlayCastAbilitySound();
        PerformAbility();
        StartCoroutine(CanShoot(shootCooldown));
    }

    void PerformAbility()
    {
        GameObject createdAbility = PhotonNetwork.Instantiate(kingAbilityEffectName, targetPos, Quaternion.identity);
        KingAbilityEffect lionKingAbilityEffect = createdAbility.GetComponentInChildren<KingAbilityEffect>();
        lionKingAbilityEffect.Initialize(player.id, damageFrequency, abilityDuration, damage);
    }

    public IEnumerator CanShoot(float timer)
    {
        canShoot = false;
        yield return new WaitForSeconds(timer);
        canShoot = true;
    }
}


