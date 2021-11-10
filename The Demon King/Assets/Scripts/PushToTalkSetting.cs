using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PushToTalkSetting : MonoBehaviour
{
    [SerializeField]private Toggle toggleCheck;

    private VoiceManager voiceManager;
    // Start is called before the first frame update
    void Start()
    {
        voiceManager = FindObjectOfType<VoiceManager>();
        toggleCheck.isOn = PlayerPrefs.GetInt("PushToTalkOn", 0) > 0;
        toggleCheck.onValueChanged.AddListener(PushToTalkChange);
    }

    public void PushToTalkChange(bool newPushToTalkValue)
    {
        if (newPushToTalkValue)
            PlayerPrefs.SetInt("PushToTalkOn", 1);
        else
            PlayerPrefs.SetInt("PushToTalkOn", 0);

        voiceManager.Recorder.TransmitEnabled = PlayerPrefs.GetInt("PushToTalkOn") == 0;
    }

    private void OnDestroy()
    {
        toggleCheck.onValueChanged.RemoveListener(PushToTalkChange);

    }
}
