using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu]
public class SOMenuData : ScriptableObject
{
    [SerializeField] private int baseMaxPlayers = 10;
    [SerializeField] private string baseRoomName = "";
    [SerializeField] private float baseGameTimeLimit = 900;
    [SerializeField] private int basePointsToWin = 200;
    [SerializeField] private int baseCurrentSceneIndex = 0;


    //Internal Values
    [HideInInspector] public int MaxPlayers;
    [HideInInspector] public string RoomName;
    [HideInInspector] public float GameTimeLimit;
    [HideInInspector] public int PointsToWin;
    [HideInInspector] public int CurrentSceneIndex;


    [HideInInspector] private const string gameTimeLimitString = "T";
    [HideInInspector] private const string pointsToWinString = "P";
    [HideInInspector] private const string currentSceneIndexString = "S";

    public string GameTimeLimitString { get { return gameTimeLimitString; } }
    public string PointsToWinString { get { return pointsToWinString; } }
    public string CurrentSceneIndexString { get { return currentSceneIndexString; } }


    // Initialize coolDown with editor's value
    private void OnEnable()
    {
        MaxPlayers = baseMaxPlayers;
        RoomName = baseRoomName;
        GameTimeLimit = baseGameTimeLimit;
        PointsToWin = basePointsToWin;
        CurrentSceneIndex = baseCurrentSceneIndex;
    }
}
