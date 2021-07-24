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
    [SerializeField] float gravity;
    [SerializeField] float turnSpeed = 15;
    [SerializeField] float MoveSpeed = 5f;


    public CharacterInputs CharacterInputs;

    [SerializeField] float jumpHeight;
    private float yVelovity;
    private Vector2 input;

    //Player Components
    private Animator animator;
    private Camera mainCamera;
    private CharacterController cc;

    public Player photonPlayer;
    [SerializeField] float pushPower = 2f;

    private bool isJumping;

    public float SlopeLimit = 45f;
    public float StepOffset = 0.3f;


    private void Awake()
    {
        if (!photonView.IsMine)
        {
            Destroy(GetComponentInChildren<Camera>().gameObject);
            Destroy(GetComponentInChildren<CinemachineFreeLook>().gameObject);
        }
        else
        {
            CharacterInputs = new CharacterInputs();
            cc = GetComponent<CharacterController>();

            cc.slopeLimit = SlopeLimit;
            cc.stepOffset = StepOffset;

            animator = GetComponent<Animator>();
            mainCamera = Camera.main;

            CharacterInputs.Player.Jump.performed += OnJump;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    [PunRPC]
    public void Initialize(Player player)
    {
        id = player.ActorNumber;
        photonPlayer = player;

        //Sets player id inside of gamemanager = to this
        GameManager.instance.players[id - 1] = this;
    }


    private void OnJump(InputAction.CallbackContext obj)
    {
        if (cc.isGrounded)
        {
            yVelovity = Mathf.Sqrt(jumpHeight * -2f * gravity);
            isJumping = true;
            cc.stepOffset = 0;
            cc.slopeLimit = 0;
        }
    }

    private void Update()
    {
        if (!photonView.IsMine)
            return;


        if (cc.isGrounded && yVelovity < 0)
        {
            yVelovity = -2f;
            if (isJumping)
            {
                isJumping = false;
                cc.stepOffset = StepOffset;
                cc.slopeLimit = SlopeLimit;
            }
        }


        //Get value from input system for directional movement
        input = CharacterInputs.Player.Move.ReadValue<Vector2>();

        yVelovity += gravity * Time.deltaTime;

        //movement
        Vector3 move = transform.right * input.x * MoveSpeed + transform.forward * input.y * MoveSpeed;
        move.y = yVelovity;
        cc.Move(move * Time.deltaTime);

        //Set the animators blend tree to correct animation based of inputs, with 0.1 smooth added
        animator.SetFloat("InputX", input.x, 0.1f, Time.deltaTime);
        animator.SetFloat("InputY", input.y, 0.1f, Time.deltaTime);
    }
    private void FixedUpdate()
    {
        if (photonView.IsMine)
        {
            //set the players rotation to the direction of the camera with a slerp smoothness
            float yawCamera = mainCamera.transform.rotation.eulerAngles.y;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, yawCamera, 0), turnSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (photonView.IsMine)
        {
            if (other.gameObject.CompareTag("Enviroment"))
                transform.SetParent(other.transform);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (photonView.IsMine)
        {
            if (other.gameObject.CompareTag("Enviroment"))
                transform.SetParent(null);
        }
    }


    //Push rigidbodys collideded with..
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
        if (photonView.IsMine)
            CharacterInputs.Player.Enable();
    }

    //Disable character input
    private void OnDisable()
    {
        if (photonView.IsMine)
            CharacterInputs.Player.Disable();
    }

    public void EnableMovement()
    {
        if (photonView.IsMine)
        {
            CharacterInputs.Player.Enable();
            cc.enabled = true;
            animator.speed = 1;
            MoveSpeed = 5;
        }
    }

    public void DisableMovement()
    {
        if (photonView)
        {
            cc.enabled = false;
            CharacterInputs.Player.Disable();
            animator.speed = 0;
            MoveSpeed = 0;
        }
    }
}
