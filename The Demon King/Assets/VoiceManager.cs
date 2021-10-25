using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using Photon.Voice.PUN;
using Photon.Pun;
using Photon.Voice.Unity;

public class VoiceManager : MonoBehaviourPun
{
    [SerializeField] private Recorder Recorder;
    [SerializeField] private Speaker Speaker;

    private VoiceManager instance;
    private PhotonView PV;
    private PhotonVoiceView VV;
    private CharacterInputs CharacterInputs;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            if (PV == null)
            {
                PV = gameObject.AddComponent<PhotonView>();
                photonView.ViewID = 999;
                VV = gameObject.AddComponent<PhotonVoiceView>();
                VV.RecorderInUse = Recorder;
                VV.SpeakerInUse = Speaker;

                CharacterInputs = InputManager.inputActions;
                CharacterInputs.VoiceChat.Enable();
                CharacterInputs.VoiceChat.PushForTalk.performed += PushForTalkPerformed;
                CharacterInputs.VoiceChat.PushForTalk.canceled += PushForTalkCancelled;

                Recorder.TransmitEnabled = false;
            }
        }
        DontDestroyOnLoad(gameObject);
    }

    private void PushForTalkPerformed(InputAction.CallbackContext obj)
    {
        Recorder.TransmitEnabled = true;
        Debug.Log("Talking");
    }
    private void PushForTalkCancelled(InputAction.CallbackContext obj)
    {
        Recorder.TransmitEnabled = false;
        Debug.Log("Stop Talking");
    }


    private void OnDisable()
    {
        CharacterInputs.VoiceChat.Disable();
    }
}
