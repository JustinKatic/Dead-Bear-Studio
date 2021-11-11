using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;
using UnityEngine.EventSystems;
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
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(PauseMenu.GetComponentInChildren<Selectable>().gameObject);
            
            if (optionsCanOpenOnPress)
            {
                playerController.inMenus = true;
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
                playerController.inMenus = false;

                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;

                if (!playerController.GetComponent<PlayerHealthManager>().isStunned && playerController.allowedToEnableMovement)
                {
                    playerController.CharacterInputs.Player.Enable();
                    playerController.CharacterInputs.Player.Ability1.Enable();

                }
                playerController.CharacterInputs.PlayerLook.Enable();
                
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
            playerController.inMenus = false;

            if (playerController.allowedToEnableMovement)
            {
                playerController.CharacterInputs.Player.Enable();
                playerController.CharacterInputs.Player.Ability1.Enable();
            }
            playerController.CharacterInputs.PlayerLook.Enable();

            
        }
    }
    public void OnButtonClickActivateMenu(GameObject menuToActivate)
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(menuToActivate.GetComponentInChildren<Selectable>().gameObject);
        
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
        playerController.inMenus = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        if (playerController.allowedToEnableMovement)
        {
            playerController.CharacterInputs.Player.Enable();
            playerController.CharacterInputs.Player.Ability1.Enable();
        }
        playerController.CharacterInputs.PlayerLook.Enable();

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
