using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Photon.Voice.Unity;
using Photon.Voice.PUN;
using UnityEngine.InputSystem;
using System;

public class PushToTalk : MonoBehaviourPun
{
    public Recorder VoiceRecorder;
    private CharacterInputs CharacterInputs;

    private void Start()
    {
        CharacterInputs = InputManager.inputActions;
        CharacterInputs.Enable();
        CharacterInputs.Player.PushForTalk.performed += PushForTalkPerformed;
        CharacterInputs.Player.PushForTalk.canceled += PushForTalkCancelled;

        VoiceRecorder.TransmitEnabled = false;
    }


    private void PushForTalkPerformed(InputAction.CallbackContext obj)
    {
        VoiceRecorder.TransmitEnabled = true;
        Debug.Log("Talking");
    }
    private void PushForTalkCancelled(InputAction.CallbackContext obj)
    {
        VoiceRecorder.TransmitEnabled = false;
        Debug.Log("Stop Talking");
    }


    private void OnDisable()
    {
        CharacterInputs.Disable();
    }
}
