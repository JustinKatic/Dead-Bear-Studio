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
    [Header("Info")]
    public int id;
    private int curAttackerId;

    public float jumpHeight;
    public float gravity;

    Vector3 velocity;
    bool isJumping;
    public float stepDown;
    public float airControl;

    private float jumpVelovity;

    public float pushPower = 2f;


    public float groundSpeed = 1f;
    public float turnSpeed = 15;
    public float jumpDamp;
    Animator animator;
    Camera mainCamera;
    CharacterInputs CharacterInputs;
    CharacterController cc;

    private HeavyProjectile heavyProjectile;

    Vector3 rootMotion;


    Vector2 input;

    public Player photonPlayer;
    PhotonView PV;

    private void Awake()
    {
        CharacterInputs = new CharacterInputs();
    }

    [PunRPC]
    public void Initialize(Player player)
    {
        id = player.ActorNumber;
        photonPlayer = player;

        GameManager.instance.players[id - 1] = this;

        // is this not our local player?
        if (!photonView.IsMine)
        {
            Destroy(GetComponentInChildren<Camera>().gameObject);
            Destroy(GetComponentInChildren<CinemachineFreeLook>().gameObject);
        }
        else
        {
            cc = GetComponent<CharacterController>();
            animator = GetComponent<Animator>();
            mainCamera = Camera.main;
            jumpVelovity = Mathf.Sqrt(2 * gravity * jumpHeight);
            
            heavyProjectile = GetComponent<HeavyProjectile>();

            CharacterInputs.Player.Jump.performed += OnJump;
            CharacterInputs.Player.Shoot.performed += OnShoot;
            CharacterInputs.Player.Aim.performed += OnAim;

            CharacterInputs.Player.Aim.canceled += OnAimCancelled;
            
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            //GameUI.instance.Initialize(this);
        }
    }

    private void OnAimCancelled(InputAction.CallbackContext obj)
    {
        heavyProjectile.AimCancelled();
    }

    private void OnAim(InputAction.CallbackContext obj)
    {
        
       heavyProjectile.Aim();
    }

    private void OnShoot(InputAction.CallbackContext obj)
    {
        heavyProjectile.Shoot();
    }

    private void OnJump(InputAction.CallbackContext obj)
    {
        Jump();
    }


    private void Update()
    {
        if (!photonView.IsMine)
            return;

        //Get value from input system for directional movement
        input = CharacterInputs.Player.Move.ReadValue<Vector2>();

        //Set the animators blend tree to correct animation based of inputs, with 0.1 smooth added
        animator.SetFloat("InputX", input.x, 0.1f, Time.deltaTime);
        animator.SetFloat("InputY", input.y, 0.1f, Time.deltaTime);
        
    }
    private void FixedUpdate()
    {
        if (!photonView.IsMine)
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

    private void UpdateInAir()
    {
        velocity.y -= gravity * Time.fixedDeltaTime;
        Vector3 displacement = velocity * Time.fixedDeltaTime;
        displacement += CalculateAirControl();
        cc.Move(displacement);
        isJumping = !cc.isGrounded;
        rootMotion = Vector3.zero;
        animator.SetBool("isJumping", isJumping);
    }

    private void OnAnimatorMove()
    {
        if (!photonView.IsMine)
            return;
        rootMotion += animator.deltaPosition;
    }




    private void OnEnable()
    {
        if (photonView.IsMine)
            
            CharacterInputs.Player.Enable();

    }

    private void OnDisable()
    {
        if (photonView.IsMine)
            CharacterInputs.Player.Disable();
    }

    void Jump()
    {
        if (!isJumping)
        {
            SetInAir(jumpVelovity);
        }
    }

    private void SetInAir(float jumpVelocity)
    {
        isJumping = true;
        velocity = animator.velocity * jumpDamp * groundSpeed;
        velocity.y = jumpVelocity;
        animator.SetBool("isJumping", true);
    }


    Vector3 CalculateAirControl()
    {
        return ((transform.forward * input.y) + (transform.right * input.x)) * (airControl / 100);
    }

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

        // If you know how fast your character is trying to move,
        // then you can also multiply the push velocity by that.

        // Apply the push
        body.velocity = pushDir * pushPower;
    }
    
}
