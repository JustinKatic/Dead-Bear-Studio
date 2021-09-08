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
    public GameObject PauseMenu;
    private PlayerController playerController;
    public bool optionsCanOpenOnPress = true;
    public List<GameObject> menus = new List<GameObject>();
    private void Start()
    {
        if (photonView.IsMine)
        {
            playerController = GetComponentInParent<PlayerController>();
            playerController.CharacterInputs.Settings.OpenSettings.performed += DisplaySettings_started;
        }
    }

    private void OnDestroy()
    {
        if (photonView.IsMine)
        {
            playerController.CharacterInputs.Settings.OpenSettings.performed -= DisplaySettings_started;
        }
    }

    private void DisplaySettings_started(InputAction.CallbackContext obj)
    {
        if (photonView.IsMine)
        {
            if (optionsCanOpenOnPress)
            {
                optionsCanOpenOnPress = false;
                playerController.CharacterInputs.Player.Disable();
                playerController.CharacterInputs.PlayerLook.Disable();
                playerController.CharacterInputs.Player.Ability1.Disable();

                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                
                PauseMenu.SetActive(true);

            }
            else
            {
                optionsCanOpenOnPress = true;

                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;

                playerController.CharacterInputs.Player.Enable();
                playerController.CharacterInputs.PlayerLook.Enable();
                playerController.CharacterInputs.Player.Ability1.Enable();
                
                PauseMenu.SetActive(false);
                
                foreach (var menu in menus)
                {
                    menu.SetActive(false);
                }
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
            
        }
    }
    public void OnButtonClickActivateMenu(GameObject menuToActivate)
    {
        menuToActivate.SetActive(true);
    }
    public void OnButtonClickDeactivateMenu(GameObject menuToActivate)
    {
        menuToActivate.SetActive(false);
    }
    public void OnButtonClickResumeGame(GameObject menuToActivate)
    {
        menuToActivate.SetActive(false);
        
        optionsCanOpenOnPress = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        playerController.CharacterInputs.Player.Enable();
        playerController.CharacterInputs.PlayerLook.Enable();
        playerController.CharacterInputs.Player.Ability1.Enable();
    }
    public void OnClickQuitGame()
    {
        PauseMenu.SetActive(false);
        
        foreach (var menu in menus)
        {
            menu.SetActive(false);
        }
        
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LoadLevel("Menu");
    }
}
