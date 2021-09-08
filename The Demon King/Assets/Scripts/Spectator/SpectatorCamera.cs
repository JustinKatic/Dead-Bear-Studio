using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class SpectatorCamera : MonoBehaviourPun
{
    private SpectatorControls spectatorControls;
    [SerializeField] float CameraAngleOverride = 0.0f;
    [SerializeField] float MouseSensitivity;

    private Vector2 playerLookInput;
    private Vector2 playerMoveInput;
    private float _cinemachineTargetYaw;
    private float _cinemachineTargetPitch;
    
    //Photon Components
    [HideInInspector] public int id;
    [HideInInspector] public Player photonPlayer;
    // Start is called before the first frame update
    void Start()
    {
        if (photonView.IsMine)
        {
            spectatorControls = GetComponent<SpectatorControls>();

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
        }
        
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
