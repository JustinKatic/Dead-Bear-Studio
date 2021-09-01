using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class EmoteWheel : MonoBehaviourPun
{
    private PlayerController playerController;
    public List<Button> emoteButtons = new List<Button>();
    public List<EmoteButton> emotes = new List<EmoteButton>();

    public Image floatingImage;
    // Start is called before the first frame update
    void Start()
    {
        if (photonView.IsMine)
        {
            for (int i = 0; i < emoteButtons.Count; i++)
            {
                emotes.Add(emoteButtons[i].GetComponent<EmoteButton>());
            }
            
            playerController = GetComponentInParent<PlayerController>();
            playerController.CharacterInputs.EmoteWheel.Display.started += DisplayEmoteWheel_started;
            playerController.CharacterInputs.EmoteWheel.Display.canceled += DisplayEmoteWheel_canceled;
        }
    }

    private void DisplayEmoteWheel_canceled(InputAction.CallbackContext obj)
    {
        if (photonView.IsMine)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            playerController.CharacterInputs.PlayerLook.Enable();
            playerController.CharacterInputs.Player.Ability1.Enable();
            foreach (var emote in emoteButtons)
            {
                emote.gameObject.SetActive(false);
            }   
        }
 
    }
    private void DisplayEmoteWheel_started(InputAction.CallbackContext obj)
    {
        if (photonView.IsMine)
        {
            playerController.CharacterInputs.PlayerLook.Disable();
            playerController.CharacterInputs.Player.Ability1.Disable();

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            foreach (var emote in emoteButtons)
            {
                emote.gameObject.SetActive(true);
                emote.interactable = true;
            }
        }

    }

    public void ActivateEmote(int buttonNumber)
    {
        if (photonView.IsMine)
        {
            photonView.RPC("SendEmote_RPC", RpcTarget.All, buttonNumber);
        }
            
    }

    [PunRPC]
    public void SendEmote_RPC(int buttonNumber)
    {
        floatingImage.sprite = emotes[buttonNumber].Emote.emoteImage;

        floatingImage.gameObject.SetActive(true);
    }

}
