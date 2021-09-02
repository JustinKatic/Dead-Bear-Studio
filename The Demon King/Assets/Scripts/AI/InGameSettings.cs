using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InGameSettings : MonoBehaviourPun
{
    public Canvas Options;
    private PlayerController playerController;
    public List<Button> buttons ;
    private void Start()
    {
        if (photonView.IsMine)
        {
            playerController = GetComponentInParent<PlayerController>();
            playerController.CharacterInputs.Settings.OpenSettings.started += DisplaySettings_started;
            playerController.CharacterInputs.Settings.OpenSettings.canceled += DisplaySettings_canceled;
        }    
    }

    private void OnDestroy()
    {
        playerController.CharacterInputs.Settings.OpenSettings.started -= DisplaySettings_started;
        playerController.CharacterInputs.Settings.OpenSettings.canceled -= DisplaySettings_canceled;
    }

    private void DisplaySettings_started(InputAction.CallbackContext obj)
    {
        if (photonView.IsMine)
        {
            playerController.CharacterInputs.Player.Disable();
            playerController.CharacterInputs.PlayerLook.Disable();
            playerController.CharacterInputs.Player.Ability1.Disable();
            
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            
            foreach (var button in buttons)
            {
                button.gameObject.SetActive(true);
            }
        }
    }
    private void DisplaySettings_canceled(InputAction.CallbackContext obj)
    {
        if (photonView.IsMine)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            
            playerController.CharacterInputs.Player.Enable();
            playerController.CharacterInputs.PlayerLook.Enable();
            playerController.CharacterInputs.Player.Ability1.Enable();
            
            foreach (var button in buttons)
            {
                button.gameObject.SetActive(false);
            }
            
        }
    }

    // Start is called before the first frame update
    public void ONClickResumeGame()
    {
        Options.gameObject.SetActive(false);
        foreach (var button in buttons)
        {
            button.gameObject.SetActive(false);
        }
        
    }

    public void OnClickOpenSettings()
    {
        foreach (var button in buttons)
        {
            button.gameObject.SetActive(false);
        }
        Options.gameObject.SetActive(true);
    }
    public void OnClickQuitGame()
    {
        foreach (var button in buttons)
        {
            button.gameObject.SetActive(false);
        }
        
        PhotonNetwork.LeaveRoom();
        Destroy(NetworkManager.instance.gameObject);
        PhotonNetwork.LoadLevel("Menu");
    }
}
