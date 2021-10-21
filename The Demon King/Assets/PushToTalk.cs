using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Photon.Voice.Unity;
using Photon.Voice.PUN;
using UnityEngine.InputSystem;
using System;
using UnityEngine.SceneManagement;

public class PushToTalk : MonoBehaviourPun
{
    public Recorder VoiceRecorder;
    private CharacterInputs CharacterInputs;

    private void Start()
    {
        CharacterInputs = InputManager.inputActions;
        CharacterInputs.VoiceChat.Enable();
        CharacterInputs.VoiceChat.PushForTalk.performed += PushForTalkPerformed;
        CharacterInputs.VoiceChat.PushForTalk.canceled += PushForTalkCancelled;

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
        CharacterInputs.VoiceChat.Disable();
    }
}
