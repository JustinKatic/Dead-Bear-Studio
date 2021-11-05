using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpectatorCamera : MonoBehaviourPun
{
    private SpectatorControls spectatorControls;
    [SerializeField] float CameraAngleOverride = 0.0f;
    [SerializeField] float MouseSensitivity;
    [SerializeField] private float spectatorMoveSpeed = 10;
    private Vector2 playerLookInput;
    private Vector2 playerMoveInput;
    private float _cinemachineTargetYaw;
    private float _cinemachineTargetPitch;
    [HideInInspector] public CinemachineVirtualCamera vCam;


    //Photon Components
    [HideInInspector] public int id;
    [HideInInspector] public Player photonPlayer;
    // Start is called before the first frame update

    private void OnEnable()
    {
        //Run following if local player 
        if (photonView.IsMine)
        {
            vCam = GetComponentInChildren<CinemachineVirtualCamera>();
            spectatorControls = new SpectatorControls();

            spectatorControls.Movement.IncreaseSpeed.started += OnShiftDown;
            spectatorControls.Movement.IncreaseSpeed.canceled += OnShiftUp;
            vCam.m_Priority = 12;
            //Enable the player inputs
            spectatorControls.Enable();
            //lock players cursor and set invis.
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private void OnShiftUp(InputAction.CallbackContext obj)
    {
        spectatorMoveSpeed = 20;
    }

    private void OnShiftDown(InputAction.CallbackContext obj)
    {
        spectatorMoveSpeed = 40;
    }


    //Disable character input
    private void OnDisable()
    {
        //Run following if local player
        if (photonView.IsMine)
        {
            //Disable the player inputs
            spectatorControls.Disable();
            spectatorControls.Movement.IncreaseSpeed.started -= OnShiftDown;
            spectatorControls.Movement.IncreaseSpeed.canceled -= OnShiftUp;
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
        //GameManager.instance.players[id - 1] = this;
    }

    private void Update()
    {
        if (photonView.IsMine)
        {
            playerLookInput = spectatorControls.Movement.Rotate.ReadValue<Vector2>();
            playerMoveInput = spectatorControls.Movement.Move.ReadValue<Vector2>();

            MoveCamera();

        }
    }

    void MoveCamera()
    {
        // movement
        float y = 0;

        y += spectatorControls.Movement.Raise.ReadValue<float>();
        y -= spectatorControls.Movement.Lower.ReadValue<float>();

        Vector3 dir = transform.right * playerMoveInput.x + transform.up * y + transform.forward * playerMoveInput.y;
        transform.position += dir * spectatorMoveSpeed * Time.deltaTime;
    }

    private void LateUpdate()
    {
        if (photonView.IsMine)
            CameraRotation();
    }

    private void CameraRotation()
    {
        // if there is an input and camera position is not fixed
        if (playerLookInput.sqrMagnitude >= 0.01)
        {
            _cinemachineTargetYaw += playerLookInput.x * MouseSensitivity * Time.deltaTime;
            _cinemachineTargetPitch += playerLookInput.y * MouseSensitivity * Time.deltaTime;
        }

        // Cinemachine will follow this target
        transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride, _cinemachineTargetYaw, 0.0f);
    }
}
