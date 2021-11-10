using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Cinemachine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviourPun
{
    [Header("Grounded Varaibles")]
    public float MaxGroundMoveSpeed = 5f;
    [SerializeField] float GroundAcceleration = 3f;
    [SerializeField] float SlopeLimit = 45f;
    [SerializeField] float StepOffset = 0.3f;
    [SerializeField] float jumpHeight;
    [SerializeField] float maximumSlidingAngle = 70;


    private float currentMoveSpeed = 0;
    private bool isJumping;

    [Header("In Air Varaibles")]
    [SerializeField] float gravity;
    [SerializeField] float DrowningInLavaGravity;
    public float MaxAirMoveSpeed = 5f;
    [SerializeField] float InAirAcceleration = 7f;
    [SerializeField] float InAirDrag = 2f;
    [SerializeField] float VelocityToStartFalling = -7f;
    [SerializeField] float VelocityNeededToPlayGroundSlam = -20f;
    [SerializeField] GameObject LandingEffectSpawnPos = null;

    [HideInInspector] public Vector3 playerJumpVelocity;

    private bool isFalling = false;

    [Header("Physics")]
    [SerializeField] float pushPower = 2f;

    [Header("Camera Controls")]
    [SerializeField] GameObject CinemachineCameraTarget;
    [SerializeField] float RotationSpeed = 15;
    [SerializeField] float TopClamp = 70.0f;
    [SerializeField] float BottomClamp = -30.0f;
    [SerializeField] float CameraAngleOverride = 0.0f;
    public float MouseSensitivity;
    [HideInInspector] public float _cinemachineTargetYaw;
    [HideInInspector] public float _cinemachineTargetPitch;
    private Camera mainCamera;

    public bool drowningInLava = false;

    //Input System
    public CharacterInputs CharacterInputs;
    public InputDevice CurrentInputDevice;
    private Vector2 playerInputs;
    private Vector2 playerLookInput;

    [HideInInspector] public Vector3 playerMoveVelocity;

    //Photon Components
    public int id;
    [HideInInspector] public Player photonPlayer;

    //Player Components
    [HideInInspector] public Animator currentAnim = null;
    [HideInInspector] public CinemachineVirtualCamera vCam;
    [HideInInspector] public CharacterController cc;

    [SerializeField] private GameObject reticle;
    [SerializeField] private Animator reticleAnimator;

    [HideInInspector] public bool onLaunchPad = false;
    [HideInInspector] public bool allowedToEnableMovement = true;
    [HideInInspector] public bool inMenus = false;
    [HideInInspector] public bool cameraRotation = true;
    private Vector3 hitNormal;
    private bool onSlope;
    private float slideFriction = 0.3f;

    private bool coyoteGronded;
    private float coyoteTimer;
    public float coyoteTime;

    public DemonKingInGameAnalytics demonKingInGameAnalytics;

    public void IncreaseSlimeDamage(int value)
    {
        demonKingInGameAnalytics.SlimeDamageOutput += value;
    }
    public void IncreaseLionDamage(int value)
    {
        demonKingInGameAnalytics.LionDamageOutput += value;
    }
    public void IncreaseRayDamage(int value)
    {
        demonKingInGameAnalytics.RayDamageOutput += value;
    }
    public void IncreaseDragonDamage(int value)
    {
        demonKingInGameAnalytics.DragonDamageOutput += value;
    }

    [HideInInspector] public bool knockback;

    #region Start Up
    private void Awake()
    {
        vCam = GetComponentInChildren<CinemachineVirtualCamera>();
        //Run following if not local player
        if (!photonView.IsMine)
        {
            Collider[] cols = GetComponentsInChildren<Collider>(true);
            foreach (var col in cols)
            {
                col.gameObject.layer = LayerMask.NameToLayer("EnemyPlayer");
            }
            gameObject.layer = LayerMask.NameToLayer("EnemyParent");
            Destroy(reticle.gameObject);
        }
        //Run following if local player
        else
        {
            //Get Components
            CharacterInputs = InputManager.inputActions;
            cc = GetComponent<CharacterController>();
            mainCamera = Camera.main;

            vCam.m_Priority = 11;

            //Set default values
            cc.slopeLimit = SlopeLimit;
            cc.stepOffset = StepOffset;

            //Jump callback
            CharacterInputs.Player.Jump.performed += OnJump;

            //lock players cursor and set invis.
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            Collider[] cols = GetComponentsInChildren<Collider>(true);
            foreach (var col in cols)
            {
                col.gameObject.layer = LayerMask.NameToLayer("Player");
            }
            gameObject.layer = LayerMask.NameToLayer("PlayerParent");
            MouseSensitivity = PlayerPrefs.GetFloat("MouseSensitivity", MouseSensitivity);

            // Re-enable when controllers are reimplemented
            
            //Subsribes to the on action change event and detects what the current activecontroller is
            //InputSystem.onActionChange += (obj, change) =>
           //{
                //if (change == InputActionChange.ActionPerformed)
                //{
                    //var inputAction = (InputAction)obj;
                    //var lastControl = inputAction.activeControl;
                    //CurrentInputDevice = lastControl.device;
                //}
            //};
        }
    }

    [PunRPC]
    public void Initialize(Player player, float spawnY, float spawnZ)
    {
        if (photonView.IsMine)
        {
            _cinemachineTargetYaw = spawnY;
            _cinemachineTargetPitch = spawnZ;
        }
        //Set photon player
        photonPlayer = player;
        //Sets player id inside of gameManager = to this
        id = photonPlayer.ActorNumber;
        GetComponent<PlayerHealthManager>().PlayerId = id;
    }


    public void PlayRectAnim()
    {
        photonView.RPC("PlayRectAnim_RPC", RpcTarget.All);
    }

    [PunRPC]
    void PlayRectAnim_RPC()
    {
        if (photonView.IsMine)
            reticleAnimator.Play("ReticleAnimation");
    }

    #endregion

    #region Update loops
    private void Update()
    {
        //Run following if local player
        if (photonView.IsMine)
        {
            //Get value from input system for directional movement
            playerInputs = CharacterInputs.Player.Move.ReadValue<Vector2>();

            playerLookInput = CharacterInputs.PlayerLook.Look.ReadValue<Vector2>();

            //Add gravity to player
            if (!drowningInLava)
            {
                playerJumpVelocity.y += gravity * Time.deltaTime;
            }
            else
            {
                playerJumpVelocity.y += DrowningInLavaGravity * Time.deltaTime;
                onLaunchPad = false;
                knockback = false;
            }

            AccelerateMoveSpeed();

            //pushes player onto ground
            if (cc.isGrounded && playerJumpVelocity.y < 0)
            {
                if (!onSlope)
                {
                    playerJumpVelocity.y = -2f;
                    //Set jumping false and allow CC to stepUp
                    if (isJumping)
                    {
                        IsJumpingAndGrounded();
                    }
                    if (isFalling)
                    {
                        IsFallingAndGrounded();
                    }
                }
                if (onLaunchPad)
                    onLaunchPad = false;
                if (knockback)
                    knockback = false;
            }
            else if (playerMoveVelocity.y <= VelocityToStartFalling)
            {
                SetFallingTrue();
            }


            //Get the players current movement velocity based of inputs and relative direction
            if (cc.isGrounded && !onSlope)
            {
                GroundMovement();

                coyoteGronded = true;
                coyoteTimer = 0;
            }
            else
            {
                InAirMovement();

                if (!isJumping && coyoteGronded)
                {
                    coyoteTimer += Time.deltaTime;
                    if (coyoteTimer >= coyoteTime)
                        coyoteGronded = false;
                }
            }

            //Add jump and gravity values to current movements Y
            playerMoveVelocity.y = playerJumpVelocity.y;
            if (onLaunchPad)
                playerMoveVelocity = playerJumpVelocity;

            if (knockback)
            {
                playerMoveVelocity = playerJumpVelocity;
                //if (Vector3.Distance(playerWhoKnockedMeBack.transform.position, transform.position) >= 15)
                //    knockback = false;
            }


            if (onSlope && cc.isGrounded)
            {
                playerMoveVelocity.x += (1f - hitNormal.y) * hitNormal.x * (1f - slideFriction);
                playerMoveVelocity.z += (1f - hitNormal.y) * hitNormal.z * (1f - slideFriction);
            }

            //Move the character based of the players velocity values
            if (cc.enabled)
                cc.Move(playerMoveVelocity * Time.deltaTime);

            onSlope = (Vector3.Angle(Vector3.up, hitNormal) >= maximumSlidingAngle);
            if (playerInputs.magnitude >= 0.1f)
                currentAnim.SetFloat("Speed", currentMoveSpeed);
            else
            {
                currentAnim.SetFloat("Speed", 5);
            }

            if (!onSlope)
            {
                SetAnimInputs();
            }
        }
        if (photonView.IsMine)
            RotatePlayerToFaceCamDirection();
    }

    private void FixedUpdate()
    {
        //Run following if local player
    }

    private void LateUpdate()
    {
        if (photonView.IsMine)
        {
            CameraRotation();
        }
    }
    #endregion

    #region OnEnable/Diable
    //Enable character Input
    private void OnEnable()
    {
        //Run following if local player 
        if (photonView.IsMine)
        {
            //Enable the player inputs
            CharacterInputs.Player.Enable();
            CharacterInputs.PlayerLook.Enable();
            CharacterInputs.DisplayScoreBoard.Enable();
            CharacterInputs.EmoteWheel.Enable();
            CharacterInputs.Settings.Enable();
        }
    }

    //Disable character input
    private void OnDisable()
    {
        //Run following if local player
        if (photonView.IsMine)
        {
            //Disable the player inputs
            CharacterInputs.Player.Disable();
            CharacterInputs.PlayerLook.Disable();
            CharacterInputs.DisplayScoreBoard.Disable();
            CharacterInputs.EmoteWheel.Disable();
            CharacterInputs.Settings.Disable();
        }
    }

    private void OnDestroy()
    {
        if (photonView.IsMine)
            CharacterInputs.Player.Jump.performed -= OnJump;
    }

    #endregion

    #region Collision Interactions
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (photonView.IsMine)
        {
            Rigidbody body = hit.collider.attachedRigidbody;

            hitNormal = hit.normal;


            // no rigidbody
            if (body == null || body.isKinematic)
                return;

            // We dont want to push objects below us
            if (hit.moveDirection.y < -0.3f)
                return;

            // Calculate push direction from move direction,
            // we only push objects to the sides never up and down
            Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);

            // Apply the push
            body.velocity = pushDir * pushPower;
        }
    }
    #endregion

    #region Camera Controlls
    void RotatePlayerToFaceCamDirection()
    {
        if (!cameraRotation)
            return;
        //set the players rotation to the direction of the camera with a slerp smoothness
        float yawCamera = mainCamera.transform.rotation.eulerAngles.y;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, yawCamera, 0), RotationSpeed * Time.deltaTime);
    }

    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }

    private void CameraRotation()
    {
        // if there is an input and camera position is not fixed
        if (playerLookInput.sqrMagnitude >= 0.01)
        {
            _cinemachineTargetYaw += playerLookInput.x * MouseSensitivity * Time.deltaTime;
            _cinemachineTargetPitch += playerLookInput.y * MouseSensitivity * Time.deltaTime;
        }

        // clamp our rotations so our values are limited 360 degrees
        _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
        _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

        // Cinemachine will follow this target
        CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride, _cinemachineTargetYaw, 0.0f);
    }

    #endregion

    #region Set Anims
    void SetAnimInputs()
    {
        //Set the animators blend tree to correct animation based of PlayerInputs, with 0.1 smooth added
        currentAnim.SetFloat("InputX", playerInputs.x, 0.1f, Time.deltaTime);
        currentAnim.SetFloat("InputY", playerInputs.y, 0.1f, Time.deltaTime);
    }
    #endregion

    #region Player Movement

    void AccelerateMoveSpeed()
    {
        if (playerInputs.magnitude >= 0.2f)
        {
            if (currentMoveSpeed >= MaxGroundMoveSpeed)
                currentMoveSpeed = MaxGroundMoveSpeed;
            else
                currentMoveSpeed += GroundAcceleration * Time.deltaTime;
        }
        else
            currentMoveSpeed = 0;
    }

    void GroundMovement()
    {
        playerMoveVelocity = (transform.right * playerInputs.x + transform.forward * playerInputs.y) * currentMoveSpeed;
    }
    void InAirMovement()
    {
        playerMoveVelocity += (transform.right * playerInputs.x + transform.forward * playerInputs.y) * InAirAcceleration * Time.deltaTime;
        float tempPlayerYVel = playerMoveVelocity.y;
        playerMoveVelocity.y = 0;
        playerMoveVelocity -= Vector3.Normalize(playerMoveVelocity) * InAirDrag * Time.deltaTime;
        playerMoveVelocity = Vector3.ClampMagnitude(playerMoveVelocity, MaxAirMoveSpeed);
        playerMoveVelocity.y = tempPlayerYVel;
    }



    public void EnableMovement()
    {
        //Run following if local player
        if (photonView.IsMine)
        {
            if (!inMenus)
            {
                //Enable player inputs, CC and set speeds back to starting speeds.
                CharacterInputs.Player.Enable();
            }
            allowedToEnableMovement = true;
        }
    }

    public void DisableMovement()
    {
        //Run following if local player
        if (photonView.IsMine)
        {
            //Disable player inputs, CC and set speeds to 0 to prevent movement.
            CharacterInputs.Player.Disable();
            allowedToEnableMovement = false;
        }
    }

    public void DisableAllInputs()
    {
        //Run following if local player
        if (photonView.IsMine)
        {
            cameraRotation = false;
            CharacterInputs.Disable();
        }
    }

    #endregion

    #region Jump/Falling
    private void OnJump(InputAction.CallbackContext obj)
    {
        if (coyoteGronded && !isJumping)
        {
            //Sets Y velocity to jump value
            playerJumpVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            isJumping = true;
            //Sets CC not to try and stepUp while in air
            cc.stepOffset = 0;
            cc.slopeLimit = 0;

            currentAnim.SetBool("JumpStarted", true);
            isFalling = true;
            currentAnim.SetBool("Falling", true);
            PlayerSoundManager.Instance.PlayJumpSound();
        }
    }

    void IsJumpingAndGrounded()
    {
        isJumping = false;
        cc.stepOffset = StepOffset;
        cc.slopeLimit = SlopeLimit;
        currentAnim.SetBool("JumpStarted", false);
    }

    void IsFallingAndGrounded()
    {
        currentAnim.SetBool("Falling", false);
        isFalling = false;

        PlayerSoundManager.Instance.StopFallingSound();

        if (playerMoveVelocity.y <= VelocityNeededToPlayGroundSlam || onLaunchPad)
        {
            PlayerSoundManager.Instance.PlayJumpLandBigSound();
            PhotonNetwork.Instantiate("LandingFX", LandingEffectSpawnPos.transform.position, LandingEffectSpawnPos.transform.rotation);
        }
        else
            PlayerSoundManager.Instance.PlayJumpLandNormalSound();

        onLaunchPad = false;
        knockback = false;
    }


    void SetFallingTrue()
    {
        currentAnim.SetBool("Falling", true);
        isFalling = true;
        PlayerSoundManager.Instance.PlayFallingSound();
    }

    public void LaunchPad(Vector3 launchVelocity)
    {
        if (!photonView.IsMine)
            return;

        playerJumpVelocity = launchVelocity;

        isJumping = true;
        //Sets CC not to try and stepUp while in air
        cc.stepOffset = 0;
        cc.slopeLimit = 0;

        SetFallingTrue();

        onLaunchPad = true;
    }

    public void KnockBack(Vector3 KnockBackPos, int IdOfPlayerWhoCalledKnockBack, float knockBackForce)
    {
        photonView.RPC("KnockBack_RPC", RpcTarget.All, KnockBackPos.x, KnockBackPos.y, KnockBackPos.z, IdOfPlayerWhoCalledKnockBack, knockBackForce);
    }

    [PunRPC]
    public void KnockBack_RPC(float x, float y, float z, int IdOfPlayerWhoCalledKnockBack, float knockBackForce)
    {
        if (!photonView.IsMine)
            return;

        Vector3 ObjB = new Vector3(x, y, z);

        Vector3 dir = -(transform.position - ObjB).normalized * knockBackForce;

        playerJumpVelocity = dir;

        isJumping = true;
        //Sets CC not to try and stepUp while in air
        cc.stepOffset = 0;
        cc.slopeLimit = 0;

        SetFallingTrue();

        knockback = true;
    }

    #endregion

    public void PlayMyDeathInLavaSound()
    {
        if (photonView.IsMine)
            PlayerSoundManager.Instance.PlayDeathByLavaSound();
    }
}
