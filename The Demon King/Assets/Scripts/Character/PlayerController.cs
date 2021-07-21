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
    public float gravity;
    public float turnSpeed = 15;
    public float speed = 5f;

    public CharacterInputs CharacterInputs;

    [SerializeField] float jumpHeight;
    Vector3 velocity;
    private Vector2 input;

    //Player Components
    private Animator animator;
    private Camera mainCamera;
    private CharacterController cc;

    public Player photonPlayer;
    private float pushPower = 2f;

    [SerializeField] float slopeForce;
    [SerializeField] float slopeForceRayLength;
    private bool isJumping;

    private void Awake()
    {
        if (photonView.IsMine)
            CharacterInputs = new CharacterInputs();
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

            CharacterInputs.Player.Jump.performed += OnJump;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }



    private void OnJump(InputAction.CallbackContext obj)
    {
        if (cc.isGrounded)
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
    }


    private void Update()
    {
        if (!photonView.IsMine)
            return;

        //if the player is on the ground and players y velocity is less then 0 
        if (cc.isGrounded && velocity.y < 0)
            velocity.y = -2f;           //-2 instead of 0 to force the player on the ground a bit more

        //Get value from input system for directional movement
        input = CharacterInputs.Player.Move.ReadValue<Vector2>();

        //movement
        Vector3 move = transform.right * input.x + transform.forward * input.y;
        cc.Move(move * speed * Time.deltaTime);
        //Set the animators blend tree to correct animation based of inputs, with 0.1 smooth added
        // animator.SetFloat("InputX", input.x, 0.1f, Time.deltaTime);
        // animator.SetFloat("InputY", input.y, 0.1f, Time.deltaTime);

        //falling physics velocity.y increased every second
        velocity.y += gravity * Time.deltaTime;
        //apply falling gravity to character controller
        cc.Move(velocity * Time.deltaTime);

    }
    private void FixedUpdate()
    {
        if (!photonView.IsMine)
            return;

        //set the players rotation to the direction of the camera with a slerp smoothness
        float yawCamera = mainCamera.transform.rotation.eulerAngles.y;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, yawCamera, 0), turnSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enviroment"))
            transform.SetParent(other.transform);
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Enviroment"))
            transform.SetParent(null);
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

    public void EnableMovement()
    {
        CharacterInputs.Player.Enable();
        animator.speed = 1;
    }
    public void DisableMovement()
    {
        CharacterInputs.Player.Disable();
        animator.speed = 1;
    }
}
