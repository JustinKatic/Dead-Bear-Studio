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
    [Header("PhotonInfo")]
    public int id;
    public Player photonPlayer;
    [Header("PlayerStats")]
    [SerializeField] float gravity;
    [SerializeField] float turnSpeed = 15;
    [SerializeField] float MoveSpeed = 5f;
    [SerializeField] float AirSpeed = 5f;
    [SerializeField] float InAirAcceleration = 7f;
    public float InAirDrag = 2f;



    [SerializeField] float jumpHeight;
    [SerializeField] float pushPower = 2f;
    [SerializeField] float SlopeLimit = 45f;
    [SerializeField] float StepOffset = 0.3f;


    [Header("Components")]
    public CharacterInputs CharacterInputs;

    private float startMoveSpeed;
    private float playerYVelocity;
    private Vector2 playerInputs;
    private Vector2 playerLookInput;

    private EvolutionManager evolutionManager;



    //Player Components



    public Animator currentAnim = null;
    private Camera mainCamera;
    public CinemachineVirtualCamera vCam;
    [HideInInspector] public CharacterController cc;


    private Vector3 playerMoveVelocity;
    private bool isJumping;

    private bool isFalling = false;

    [Header("Cinemachine")]
    [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
    public GameObject CinemachineCameraTarget;
    [Tooltip("How far in degrees can you move the camera up")]
    public float TopClamp = 70.0f;
    [Tooltip("How far in degrees can you move the camera down")]
    public float BottomClamp = -30.0f;
    [Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
    public float CameraAngleOverride = 0.0f;
    private float _cinemachineTargetYaw;
    private float _cinemachineTargetPitch;
    public float MouseSensitivity;

    private void Awake()
    {
        vCam = GetComponentInChildren<CinemachineVirtualCamera>();
        //Run following if not local player
        if (!photonView.IsMine)
        {
            //Destroy(GetComponentInChildren<CinemachineVirtualCamera>().gameObject);
            gameObject.layer = LayerMask.NameToLayer("EnemyPlayer");
        }
        //Run following if local player
        else
        {
            //Get Components
            CharacterInputs = new CharacterInputs();
            cc = GetComponent<CharacterController>();
            mainCamera = Camera.main;

            vCam.m_Priority = 11;

            //Set default values
            cc.slopeLimit = SlopeLimit;
            cc.stepOffset = StepOffset;
            startMoveSpeed = MoveSpeed;

            //Jump callback
            CharacterInputs.Player.Jump.performed += OnJump;

            //lock players cursor and set invis.
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            gameObject.layer = LayerMask.NameToLayer("Player");

            evolutionManager = GetComponent<EvolutionManager>();

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
    }


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
            currentAnim.SetBool("Falling", true);
            currentAnim.SetBool("HasLanded", false);
            isFalling = true;

            FMODUnity.RuntimeManager.PlayOneShotAttached(evolutionManager.activeEvolution.JumpSound, gameObject);
            photonView.RPC("PlayJumpOneShot", RpcTarget.Others, evolutionManager.activeEvolution.JumpSound);
        }
    }

    [PunRPC]
    void PlayJumpOneShot(string jumpSound)
    {
        FMODUnity.RuntimeManager.PlayOneShotAttached(jumpSound, gameObject);
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


    private void LateUpdate()
    {
        if (photonView.IsMine)
            CameraRotation();
    }

    private void Update()
    {
        //Run following if local player
        if (photonView.IsMine)
        {
            //pushes player onto ground
            if (cc.isGrounded && playerYVelocity < 0)
            {
                playerYVelocity = -2f;
                //Set jumping false and allow CC to stepUp
                if (isJumping)
                {
                    currentAnim.SetBool("HasLanded", true);
                    isJumping = false;
                    cc.stepOffset = StepOffset;
                    cc.slopeLimit = SlopeLimit;
                    currentAnim.SetBool("JumpStarted", false);
                }
                if (isFalling)
                {
                    currentAnim.SetBool("HasLanded", true);
                    currentAnim.SetBool("Falling", false);
                    isFalling = false;
                }
            }
            else if (!cc.isGrounded && !isJumping)
            {
                currentAnim.SetBool("Falling", true);
                currentAnim.SetBool("HasLanded", false);
                isFalling = true;
            }


            //Get value from input system for directional movement
            playerInputs = CharacterInputs.Player.Move.ReadValue<Vector2>();

            playerLookInput = CharacterInputs.PlayerLook.Look.ReadValue<Vector2>();

            //Add gravity to player
            playerYVelocity += gravity * Time.deltaTime;

            //Get the players current movement velocity based of inputs and relative direction
            if (cc.isGrounded)
            {
                playerMoveVelocity = (transform.right * playerInputs.x + transform.forward * playerInputs.y) * MoveSpeed;
            }
            else
            {
                playerMoveVelocity += (transform.right * playerInputs.x + transform.forward * playerInputs.y) * InAirAcceleration * Time.deltaTime;
                float tempPlayerYVel = playerMoveVelocity.y;
                playerMoveVelocity.y = 0;
                playerMoveVelocity -= Vector3.Normalize(playerMoveVelocity) * InAirDrag * Time.deltaTime;
                playerMoveVelocity = Vector3.ClampMagnitude(playerMoveVelocity, AirSpeed);

                playerMoveVelocity.y = tempPlayerYVel;
            }


            //Add jump and gravity values to current movements Y
            playerMoveVelocity.y = playerYVelocity;

            //Move the character based of the players velocity values
            if (cc.enabled)
                cc.Move(playerMoveVelocity * Time.deltaTime);

            //Set the animators blend tree to correct animation based of PlayerInputs, with 0.1 smooth added
            currentAnim.SetFloat("InputX", playerInputs.x, 0.1f, Time.deltaTime);
            currentAnim.SetFloat("InputY", playerInputs.y, 0.1f, Time.deltaTime);
        }
    }


    private void FixedUpdate()
    {
        //Run following if local player
        if (photonView.IsMine)
        {
            //set the players rotation to the direction of the camera with a slerp smoothness
            float yawCamera = mainCamera.transform.rotation.eulerAngles.y;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, yawCamera, 0), turnSpeed * Time.deltaTime);
        }
    }


    //Push rigidbodys collideded with
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
        }
    }

    public void EnableMovement()
    {
        //Run following if local player
        if (photonView.IsMine)
        {
            //Enable player inputs, CC and set speeds back to starting speeds.
            CharacterInputs.Player.Enable();
            MoveSpeed = startMoveSpeed;
        }
    }

    public void DisableMovement()
    {
        //Run following if local player
        if (photonView)
        {
            //Disable player inputs, CC and set speeds to 0 to prevent movement.
            CharacterInputs.Player.Disable();
            MoveSpeed = 0;
        }
    }
}
