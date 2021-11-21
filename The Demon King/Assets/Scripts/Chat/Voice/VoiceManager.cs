using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using Photon.Voice.PUN;
using Photon.Pun;
using Photon.Voice.Unity;
using Photon.Voice.Unity.UtilityScripts;

public class VoiceManager : MonoBehaviourPun
{
    public Recorder Recorder;
    [SerializeField] private Speaker Speaker;

    public static VoiceManager instance;
    private PhotonView PV;
    private PhotonVoiceView VV;
    private CharacterInputs CharacterInputs;
    public MicAmplifier mic;

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
                PV.ViewID = 999;
                VV = gameObject.AddComponent<PhotonVoiceView>();
                VV.RecorderInUse = Recorder;
                VV.SpeakerInUse = Speaker;
                VV.UsePrimaryRecorder = true;

                CharacterInputs = InputManager.inputActions;
                CharacterInputs.VoiceChat.Enable();
                CharacterInputs.VoiceChat.PushForTalk.performed += PushForTalkPerformed;
                CharacterInputs.VoiceChat.PushForTalk.canceled += PushForTalkCancelled;

                Recorder.TransmitEnabled = PlayerPrefs.GetInt("PushToTalkOn") == 1;
                mic.AmplificationFactor = 1;
            }
        }
        DontDestroyOnLoad(gameObject);
    }

    private void PushForTalkPerformed(InputAction.CallbackContext obj)
    {
        if (PlayerPrefs.GetInt("PushToTalkOn") == 1)
            Recorder.TransmitEnabled = true;

        Debug.Log("Talking");
    }
    private void PushForTalkCancelled(InputAction.CallbackContext obj)
    {
        if (PlayerPrefs.GetInt("PushToTalkOn") == 1)
            Recorder.TransmitEnabled = false;

        Debug.Log("Stop Talking");
    }
}
