using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class UIMessageDisplay : MonoBehaviourPun, IOnEventCallback
{

    private const byte DisplayPlayerKilledSomeoneMessage = 4;
    private const byte DisplayPlayerKilledSelfMessage = 5;
    private const byte DisplayPlayerLeftLobby = 6;
    private const byte DisplayNormalMessage = 7;


    public enum MessageType { Death, Kill, LeftTheGame, Normal }


    [Header("Message colors:")]
    public Color deathColor;                                // death message text color
    public Color killColor;                                 // kill message text color
    public Color playerLeftTheGameColor;                    // player disconnect message text color
    public Color normalColor;                               // normal message text color

    public Transform messagePanel;                          // the message area
    public TextMeshProUGUI messageTextPrefab;				// the message text prefab that gets spawned in the message area

    public void DisplayMessage(string message, MessageType typeOfMessage)
    {

        // the color that the message will have:
        Color mColor = Color.white;

        // Setting the right color:
        switch (typeOfMessage)
        {
            case MessageType.Death:
                mColor = deathColor;
                break;
            case MessageType.Kill:
                mColor = killColor;
                break;
            case MessageType.LeftTheGame:
                mColor = playerLeftTheGameColor;
                break;
            case MessageType.Normal:
                mColor = normalColor;
                break;
        }

        // the message text itself:
        TextMeshProUGUI m = Instantiate(messageTextPrefab, messagePanel);
        m.color = mColor;
        m.text = message;
    }




    public void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == DisplayPlayerKilledSomeoneMessage)
        {
            object[] data = (object[])photonEvent.CustomData;
            DisplayMessage(data[0].ToString() + " Killed " + data[1].ToString(), MessageType.Kill);
        }
        else if (photonEvent.Code == DisplayPlayerKilledSelfMessage)
        {
            object[] data = (object[])photonEvent.CustomData;
            DisplayMessage(data[0].ToString() + " Suicided ", MessageType.Death);
        }
        else if (photonEvent.Code == DisplayPlayerLeftLobby)
        {

        }
        else if (photonEvent.Code == DisplayNormalMessage)
        {

        }
    }

    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }
    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }
}

