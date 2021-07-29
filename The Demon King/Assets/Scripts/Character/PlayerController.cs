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
    [SerializeField] float jumpHeight;
    [SerializeField] float pushPower = 2f;
    [SerializeField] float SlopeLimit = 45f;
    [SerializeField] float StepOffset = 0.3f;


    [Header("Components")]
    public CharacterInputs CharacterInputs;

    private float startMoveSpeed;
    private float playerYVelocity;
    private Vector2 playerInputs;


    //Player Components



    public Animator currentAnim = null;
    private Camera mainCamera;
    [HideInInspector] public CharacterController cc;


    private Vector3 playerMoveVelocity;
    private bool isJumping;


    private void Awake()
    {
        //Run following if not local player
        if (!photonView.IsMine)
        {
            Destroy(GetComponentInChildren<Camera>().gameObject);
            Destroy(GetComponentInChildren<CinemachineFreeLook>().gameObject);
        }
        //Run following if local player
        else
        {
            //Get Components
            CharacterInputs = new CharacterInputs();
            cc = GetComponent<CharacterController>();
            mainCamera = Camera.main;

            //Set default values
            cc.slopeLimit = SlopeLimit;
            cc.stepOffset = StepOffset;
            startMoveSpeed = MoveSpeed;

            //Jump callback
            CharacterInputs.Player.Jump.performed += OnJump;

            //lock players cursor and set invis.
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
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
        }
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
                    isJumping = false;
                    cc.stepOffset = StepOffset;
                    cc.slopeLimit = SlopeLimit;
                }
            }


            //Get value from input system for directional movement
            playerInputs = CharacterInputs.Player.Move.ReadValue<Vector2>();

            //Add gravity to player
            playerYVelocity += gravity * Time.deltaTime;

            //Get the players current movement velocity based of inputs and relative direction
            playerMoveVelocity = transform.right * playerInputs.x * MoveSpeed + transform.forward * playerInputs.y * MoveSpeed;
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
            //Enable the player inputs
            CharacterInputs.Player.Enable();
    }

    //Disable character input
    private void OnDisable()
    {
        //Run following if local player
        if (photonView.IsMine)
            //Disable the player inputs
            CharacterInputs.Player.Disable();
    }

    public void EnableMovement()
    {
        //Run following if local player
        if (photonView.IsMine)
        {
            //Enable player inputs, CC and set speeds back to starting speeds.
            CharacterInputs.Player.Enable();
            currentAnim.speed = 1;
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
            currentAnim.speed = 0;
            MoveSpeed = 0;
        }
    }
}
