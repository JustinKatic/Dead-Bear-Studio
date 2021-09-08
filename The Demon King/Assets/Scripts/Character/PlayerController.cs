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

    [HideInInspector] public float playerYVelocity;
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
    private float _cinemachineTargetYaw;
    private float _cinemachineTargetPitch;
    private Camera mainCamera;

    public bool drowningInLava = false;

    //Input System
    [HideInInspector] public CharacterInputs CharacterInputs;
    private Vector2 playerInputs;
    private Vector2 playerLookInput;

    [HideInInspector] public Vector3 playerMoveVelocity;

    //Photon Components
    [HideInInspector] public int id;
    [HideInInspector] public Player photonPlayer;

    //Player Components
    [HideInInspector] public Animator currentAnim = null;
    [HideInInspector] public CinemachineVirtualCamera vCam;
    [HideInInspector] public CharacterController cc;

    [SerializeField] private GameObject recticle;


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
            Destroy(recticle.gameObject);
        }
        //Run following if local player
        else
        {
            //Get Components
            CharacterInputs = new CharacterInputs();
            cc = GetComponent<CharacterController>();
            //playerSoundManager = GetComponent<PlayerSoundManager>();
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
        }
    }

    [PunRPC]
    public void Initialize(Player player)
    {
        //gives player an ID
        id = player.ActorNumber;
        //Set photon player
        photonPlayer = player;
        //Sets player id inside of gameManager = to this
        GameManager.instance.players[id - 1] = this;

        if (player.NickName == "Justin" || player.NickName == "justin" || player.NickName == "j" || player.NickName == "J")
            player.NickName = "JUICY TEEN";
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
                playerYVelocity += gravity * Time.deltaTime;
            else
                playerYVelocity += DrowningInLavaGravity * Time.deltaTime;

            AccelerateMoveSpeed();

            //pushes player onto ground
            if (cc.isGrounded && playerYVelocity < 0)
            {
                playerYVelocity = -2f;
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
            else if (playerMoveVelocity.y <= VelocityToStartFalling)
            {
                SetFallingTrue();
            }


            //Get the players current movement velocity based of inputs and relative direction
            if (cc.isGrounded)
            {
                GroundMovement();
            }
            else
            {
                InAirMovement();
            }

            //Add jump and gravity values to current movements Y
            playerMoveVelocity.y = playerYVelocity;

            //Move the character based of the players velocity values
            if (cc.enabled)
                cc.Move(playerMoveVelocity * Time.deltaTime);

            if (playerInputs.magnitude >= 0.1f)
                currentAnim.SetFloat("Speed", currentMoveSpeed);
            else
            {
                currentAnim.SetFloat("Speed", 5);
            }

            SetAnimInputs();
        }
    }

    private void FixedUpdate()
    {
        //Run following if local player
        if (photonView.IsMine)
            RotatePlayerToFaceCamDirection();
    }

    private void LateUpdate()
    {
        if (photonView.IsMine)
            CameraRotation();
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
            //Enable player inputs, CC and set speeds back to starting speeds.
            CharacterInputs.Player.Enable();
        }
    }

    public void DisableMovement()
    {
        //Run following if local player
        if (photonView.IsMine)
        {
            //Disable player inputs, CC and set speeds to 0 to prevent movement.
            CharacterInputs.Player.Disable();
        }
    }
    #endregion

    #region Jump/Falling
    private void OnJump(InputAction.CallbackContext obj)
    {
        if (cc.isGrounded)
        {
            //Sets Y velocity to jump value
            playerYVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
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

        if (playerMoveVelocity.y <= VelocityNeededToPlayGroundSlam)
            PlayerSoundManager.Instance.PlayJumpLandBigSound();

        else
            PlayerSoundManager.Instance.PlayJumpLandNormalSound();
    }

    void SetFallingTrue()
    {
        currentAnim.SetBool("Falling", true);
        isFalling = true;
        PlayerSoundManager.Instance.PlayFallingSound();
    }
    #endregion

    public void PlayMyDeathInLavaSound()
    {
        if (photonView.IsMine)
            PlayerSoundManager.Instance.PlayDeathByLavaSound();
    }
}
