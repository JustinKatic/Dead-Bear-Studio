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
    [Header("PlayerStats")]
    public float jumpHeight;
    public float gravity;
    public float stepDown;
    public float airControl;
    public float pushPower = 2f;
    public float groundSpeed = 1f;
    public float turnSpeed = 15;
    public float jumpDamp;

    Vector3 PlayerVelocity;
    private float jumpVelovity;
    private Vector3 rootMotion;
    private Vector2 input;

    private bool isJumping;
    private Animator animator;
    private Camera mainCamera;
    private CharacterInputs CharacterInputs;
    private CharacterController cc;
    private ShootController shootController;

    public Player photonPlayer;
    public bool isStunned = false;

    private void Awake()
    {
        if (photonView.IsMine)
        {
            CharacterInputs = new CharacterInputs();
        }

        shootController = GetComponent<ShootController>();
    }

    [PunRPC]
    public void Initialize(Player player)
    {
        id = player.ActorNumber;
        photonPlayer = player;

        //Sets player id inside of gamemanager = to this
        GameManager.instance.players[id - 1] = this;

        // If not local player
        if (!photonView.IsMine)
        {
            Destroy(GetComponentInChildren<Camera>().gameObject);
            Destroy(GetComponentInChildren<CinemachineFreeLook>().gameObject);
        }
        //If local player
        else
        {
            cc = GetComponent<CharacterController>();
            animator = GetComponent<Animator>();
            mainCamera = Camera.main;
            jumpVelovity = Mathf.Sqrt(2 * gravity * jumpHeight);

            CharacterInputs.Player.Jump.performed += OnJump;
            CharacterInputs.Player.Shoot.performed += OnShoot;
            CharacterInputs.Player.Aim.performed += OnAim;

            CharacterInputs.Player.Aim.canceled += OnAimCancelled;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            //GameUI.instance.Initialize(this);
        }
    }
    private void OnAim(InputAction.CallbackContext obj)
    {
        if (isStunned)
            return;

        if (shootController.HeavyProjectileActive)
            shootController.HeavyProjectileAim();
    }

    private void OnAimCancelled(InputAction.CallbackContext obj)
    {
        if (isStunned)
            return;

        if (shootController.HeavyProjectileActive)
            shootController.HeavyProjectileAimCancelled();
    }


    private void OnShoot(InputAction.CallbackContext obj)
    {
        if (isStunned)
            return;
        if (shootController.HeavyProjectileActive)
            shootController.ShootHeavyProjectile();
    }

    private void OnJump(InputAction.CallbackContext obj)
    {
        if (isStunned)
            return;
        Jump();
    }


    private void Update()
    {
        if (!photonView.IsMine)
            return;


        if (!isStunned)
        {
            //Get value from input system for directional movement
            input = CharacterInputs.Player.Move.ReadValue<Vector2>();

            //Set the animators blend tree to correct animation based of inputs, with 0.1 smooth added
            animator.SetFloat("InputX", input.x, 0.1f, Time.deltaTime);
            animator.SetFloat("InputY", input.y, 0.1f, Time.deltaTime);
        }
    }
    private void FixedUpdate()
    {
        if (!photonView.IsMine)
            return;

        if (isStunned)
            return;

        //Is in air state
        if (isJumping)
        {
            UpdateInAir();
        }
        //IsGrounded state
        else
        {
            UpdateOnGround();
        }

        //set the players rotation to the direction of the camera with a slerp smoothness
        float yawCamera = mainCamera.transform.rotation.eulerAngles.y;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, yawCamera, 0), turnSpeed * Time.deltaTime);
    }


    //Movement while on ground
    private void UpdateOnGround()
    {
        Vector3 stepForwardAmount = rootMotion * groundSpeed;
        Vector3 stepDownAmount = Vector3.down * stepDown;

        cc.Move(stepForwardAmount + stepDownAmount);
        rootMotion = Vector3.zero;

        if (!cc.isGrounded)
        {
            SetInAir(0);
        }
    }

    //Movement while in air
    private void UpdateInAir()
    {
        PlayerVelocity.y -= gravity * Time.fixedDeltaTime;
        Vector3 displacement = PlayerVelocity * Time.fixedDeltaTime;
        displacement += CalculateAirControl();
        cc.Move(displacement);
        isJumping = !cc.isGrounded;
        rootMotion = Vector3.zero;
        animator.SetBool("isJumping", isJumping);
    }
    //Adds air control to player
    Vector3 CalculateAirControl()
    {
        return ((transform.forward * input.y) + (transform.right * input.x)) * (airControl / 100);
    }

    //Increase the delta of animation 
    private void OnAnimatorMove()
    {
        if (!photonView.IsMine)
            return;
        if (!isStunned)
            rootMotion += animator.deltaPosition;
    }

    //player Jump
    void Jump()
    {
        if (!isJumping)
        {
            SetInAir(jumpVelovity);
        }
    }

    //Jumping logic 
    private void SetInAir(float jumpVelocity)
    {
        isJumping = true;
        PlayerVelocity = animator.velocity * jumpDamp * groundSpeed;
        PlayerVelocity.y = jumpVelocity;
        animator.SetBool("isJumping", true);
    }


    //Push rigidbodys collideded with..
    void OnControllerColliderHit(ControllerColliderHit hit)
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


    //Enable character Input
    private void OnEnable()
    {
        if (!photonView.IsMine)
            return;

        CharacterInputs.Player.Enable();
    }

    //Disable character input
    private void OnDisable()
    {
        if (!photonView.IsMine)
            return;

        CharacterInputs.Player.Disable();
    }
}
