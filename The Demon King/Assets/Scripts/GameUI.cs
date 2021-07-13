using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class GameUI : MonoBehaviour
{
    private PlayerController player;

    // instance
    public static GameUI instance;

    void Awake ()
    {
        instance = this;
    }

    public void Initialize (PlayerController localPlayer)
    {
        player = localPlayer;

    }

}