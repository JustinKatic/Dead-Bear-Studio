using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Realtime;

public class UIManager : MonoBehaviour
{
    public enum MessageType { Death, Kill, LeftTheGame, Normal }

    public Transform messagePanel;                          // the message area
    public TextMeshProUGUI messageTextPrefab;				// the message text prefab that gets spawned in the message area

    [Header("Message colors:")]
    public Color deathColor;                                // death message text color
    public Color killColor;                                 // kill message text color
    public Color playerLeftTheGameColor;                    // player disconnect message text color
    public Color normalColor;                               // normal message text color


    // "Someone killed someone" message:
    public void SomeoneKilledSomeone(Player dying, Player killer)
    {
        // You we're killed:
        if (dying.IsLocal)
        {
            // You've committed suicide:
            if (dying == killer)
            {
                DisplayMessage("You've committed suicide!", MessageType.Death);
            }
            // You we're killed by others:
            else
            {
                DisplayMessage("You've been killed by " + killer.NickName, MessageType.Death);
            }
        }
        else
        {
            // You killed someone:
            if (killer.IsLocal)
            {
                DisplayMessage("You killed " + dying.NickName + "!", MessageType.Kill);
            }
            else
            {
                // Someone committed suicide:
                if (killer == dying)
                {
                    DisplayMessage(dying.NickName + " committed suicide", MessageType.Normal);
                }
                // Someone killed someone:
                else
                {
                    DisplayMessage(killer.NickName + " killed " + dying.NickName, MessageType.Normal);
                }
            }
        }
    }


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
}
