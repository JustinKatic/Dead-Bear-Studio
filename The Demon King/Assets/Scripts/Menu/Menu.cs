﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;
using Hashtable = ExitGames.Client.Photon.Hashtable;


public class Menu : MonoBehaviourPunCallbacks, ILobbyCallbacks
{
    [SerializeField] private SOMenuData roomData;

    [SerializeField] private List<GameObject> screens;
    [SerializeField] private GameObject ActiveMenu;

    [Header("Main Screen")]
    [SerializeField] private Button createRoomButton;
    [SerializeField] private Button findRoomButton;
    [SerializeField] private GameObject mainSelectableItem;
    [SerializeField] private TMP_InputField playerNameInput;


    [Header("Create Room Screen")]
    [SerializeField] private Button createNewRoomButton;
    [SerializeField] private TMP_Text maxPlayerInput;
    [SerializeField] private TextMeshProUGUI createRoomTimeLimitText;
    [SerializeField] private TextMeshProUGUI createRoomPointsToWinText;


    [Header("Lobby")]
    [SerializeField] private TextMeshProUGUI playerListText;
    [SerializeField] private GameObject lobbyScreen;
    [SerializeField] private TextMeshProUGUI roomInfoText;
    [SerializeField] private List<Button> masterClientButtons;

    [SerializeField] private Button startGameButton;
    [SerializeField] private TextMeshProUGUI privacyRoomText;
    [SerializeField] private List<SceneInformation> scenes;
    [SerializeField] private Image CurrentSceneDisplayImg;

    [SerializeField] private TextMeshProUGUI lobbyTimeLimitText;
    [SerializeField] private TextMeshProUGUI lobbyPointsToWinText;
    [SerializeField] private TextMeshProUGUI currentLevelSelectedText;

    [Header("Lobby Browser")]
    [SerializeField] private RectTransform roomListContainer;
    [SerializeField] private GameObject roomButtonPrefab;
    [SerializeField] private TMP_InputField roomSearchBar;

    private List<GameObject> roomButtons = new List<GameObject>();
    private List<RoomInfo> roomList = new List<RoomInfo>();
    private List<RoomInfo> allOpenRooms = new List<RoomInfo>();

    public bool roomIsPublic = true;
    [HideInInspector] public string sceneName;
    private string currentRoomName;
    private float roomMaxPlayers = 8;

    public bool spectatorMode = false;
    private GameObject lastActiveMenu;


    void Start()
    {
        // connect to the master server
        PhotonNetwork.ConnectUsingSettings();

        createRoomTimeLimitText.text = FormatTime(roomData.GameTimeLimit).ToString();
        lobbyTimeLimitText.text = FormatTime(roomData.GameTimeLimit).ToString();
        createRoomPointsToWinText.text = roomData.PointsToWin.ToString();
        lobbyPointsToWinText.text = roomData.PointsToWin.ToString();
        CurrentSceneDisplayImg.sprite = scenes[roomData.CurrentSceneIndex].SceneDisplayImage;
        currentLevelSelectedText.text = scenes[roomData.CurrentSceneIndex].SceneName;

        // disable the menu buttons at the start
        createRoomButton.interactable = false;
        findRoomButton.interactable = false;

        // enable the cursor since we hide it when we play the game
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        // are we in a game?
        if (PhotonNetwork.InRoom)
        {
            // go to the lobby
            SetScreen(lobbyScreen);
            UpdateLobbyUI();

            // make the room visible again
            PhotonNetwork.CurrentRoom.IsVisible = true;
            PhotonNetwork.CurrentRoom.IsOpen = true;
        }

        playerNameInput.text = PlayerPrefs.GetString("PlayerName", null);
    }


    // joins a room of the requested room name
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    // changes the currently visible screen
    void SetScreen(GameObject newScreen)
    {
        if (ActiveMenu != null)
        {
            foreach (var screen in screens)
            {
                if (screen == newScreen)
                    screen.SetActive(true);
                else
                    screen.SetActive(false);
            }
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(mainSelectableItem);
        }
    }

    // called when the "Back" button gets pressed
    public void OnButtonClick(GameObject ScreenToActivate)
    {
        //store the last screen in case we have entered the settings
        lastActiveMenu = ActiveMenu;
        foreach (var screen in screens)
        {
            if (ScreenToActivate == screen)
            {
                ActiveMenu = ScreenToActivate;
                SetScreen(ActiveMenu);

                return;
            }
        }
    }

    //Only used in Settings due to multiple screens potentially to go back to
    public void OnSettingsBackButton()
    {
        OnButtonClick(lastActiveMenu);
    }
    // MAIN SCREEN

    // called when the player name input field has been changed
    public void OnPlayerNameValueChanged(TMP_InputField playerNameInput)
    {
        PhotonNetwork.NickName = playerNameInput.text;

        createRoomButton.interactable = !string.IsNullOrEmpty(playerNameInput.text);
        findRoomButton.interactable = !string.IsNullOrEmpty(playerNameInput.text);
        PlayerPrefs.SetString("PlayerName", playerNameInput.text);
    }


    public void OnMaxPlayerSliderValueChanged(Slider maxPlayerValueChange)
    {
        roomMaxPlayers = maxPlayerValueChange.value;
        maxPlayerInput.text = roomMaxPlayers.ToString();
    }
    public void OnPrivateRoomValueChanged(Toggle roomPrivacyChange)
    {
        roomIsPublic = roomPrivacyChange.isOn;
    }

    public void OnQuitGameButton()
    {
        Application.Quit();
    }

    // CREATE ROOM SCREEN
    public void OnPlayerRoomValueChanged(TMP_InputField playerRoomNameInput)
    {
        createNewRoomButton.interactable = !string.IsNullOrEmpty(playerRoomNameInput.text);
    }

    public void OnCreateButton(TMP_InputField roomNameInput)
    {
        currentRoomName = roomNameInput.text.ToUpper();

        RoomOptions options = new RoomOptions();
        options.MaxPlayers = (byte)roomMaxPlayers;

        options.CustomRoomProperties = new Hashtable { { roomData.GameTimeLimitString, roomData.GameTimeLimit }, { roomData.PointsToWinString, roomData.PointsToWin }, { roomData.CurrentSceneIndexString, roomData.CurrentSceneIndex } };

        roomNameInput.text = roomNameInput.text.ToUpper();

        PhotonNetwork.CreateRoom(roomNameInput.text, options);
    }

    // LOBBY SCREEN

    // called when we join a room
    // set the screen to be the Lobby and update the UI for all players

    public void SetSpectatorMode()
    {
        spectatorMode = !spectatorMode;
        Hashtable SpectatorMode = new Hashtable();
        SpectatorMode.Add("IsSpectator", spectatorMode);
        PhotonNetwork.LocalPlayer.SetCustomProperties(SpectatorMode);
    }




    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        if (propertiesThatChanged.ContainsKey(roomData.GameTimeLimitString))
        {
            roomData.GameTimeLimit = (float)propertiesThatChanged[roomData.GameTimeLimitString];
            createRoomTimeLimitText.text = FormatTime(roomData.GameTimeLimit).ToString();
            lobbyTimeLimitText.text = FormatTime(roomData.GameTimeLimit).ToString();
        }
        if (propertiesThatChanged.ContainsKey(roomData.PointsToWinString))
        {
            roomData.PointsToWin = (int)propertiesThatChanged[roomData.PointsToWinString];
            createRoomPointsToWinText.text = roomData.PointsToWin.ToString();
            lobbyPointsToWinText.text = roomData.PointsToWin.ToString();
        }
        if (propertiesThatChanged.ContainsKey(roomData.CurrentSceneIndexString))
        {
            roomData.CurrentSceneIndex = (int)propertiesThatChanged[roomData.CurrentSceneIndexString];
            CurrentSceneDisplayImg.sprite = scenes[roomData.CurrentSceneIndex].SceneDisplayImage;
            currentLevelSelectedText.text = scenes[roomData.CurrentSceneIndex].SceneName;
        }
    }

    public void OnPointsToWinChanged(bool IncreasePoints)
    {
        if (IncreasePoints)
            roomData.PointsToWin += 10;
        else
            roomData.PointsToWin -= 10;

        createRoomPointsToWinText.text = roomData.PointsToWin.ToString();
        lobbyPointsToWinText.text = roomData.PointsToWin.ToString();
    }


    public void OnGameTimeChanged(bool InccreaseTime)
    {
        if (InccreaseTime)
            roomData.GameTimeLimit += 60;
        else
            roomData.GameTimeLimit -= 60;

        createRoomTimeLimitText.text = FormatTime(roomData.GameTimeLimit).ToString();
        lobbyTimeLimitText.text = FormatTime(roomData.GameTimeLimit).ToString();
    }

    public void UpdateRoomProperties()
    {
        PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable { { roomData.GameTimeLimitString, roomData.GameTimeLimit }, { roomData.PointsToWinString, roomData.PointsToWin } });
    }

    public override void OnJoinedRoom()
    {
        photonView.RPC("UpdateLobbyUI", RpcTarget.All);
        ChatManager.instance.StartChat(currentRoomName, PhotonNetwork.NickName);
        PhotonNetwork.CurrentRoom.IsVisible = roomIsPublic;

        Hashtable SpectatorMode = new Hashtable();
        SpectatorMode.Add("IsSpectator", spectatorMode);
        PhotonNetwork.LocalPlayer.SetCustomProperties(SpectatorMode);

        roomData.GameTimeLimit = (float)PhotonNetwork.CurrentRoom.CustomProperties[roomData.GameTimeLimitString];
        roomData.PointsToWin = (int)PhotonNetwork.CurrentRoom.CustomProperties[roomData.PointsToWinString];
        roomData.CurrentSceneIndex = (int)PhotonNetwork.CurrentRoom.CustomProperties[roomData.CurrentSceneIndexString];

        Invoke("UpdateRoomHashsOnJoinInvoke", 0.2f);

        if (roomIsPublic)
            privacyRoomText.text = "Public";
        else
            privacyRoomText.text = "Private";

    }

    void UpdateRoomHashsOnJoinInvoke()
    {
        createRoomTimeLimitText.text = FormatTime(roomData.GameTimeLimit).ToString();
        lobbyTimeLimitText.text = FormatTime(roomData.GameTimeLimit).ToString();

        createRoomPointsToWinText.text = roomData.PointsToWin.ToString();
        lobbyPointsToWinText.text = roomData.PointsToWin.ToString();

        CurrentSceneDisplayImg.sprite = scenes[roomData.CurrentSceneIndex].SceneDisplayImage;
        currentLevelSelectedText.text = scenes[roomData.CurrentSceneIndex].SceneName;
    }

    // called when a player leaves the room - update the lobby UI
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdateLobbyUI();
    }

    // updates the lobby player list and active buttons
    [PunRPC]
    void UpdateLobbyUI()
    {
        // enable or disable the start game button depending on if we're the host
        startGameButton.interactable = PhotonNetwork.IsMasterClient;
        foreach (var button in masterClientButtons)
        {
            button.gameObject.SetActive(PhotonNetwork.IsMasterClient);
        }

        // display all the players
        playerListText.text = "";

        foreach (Player player in PhotonNetwork.PlayerList)
            playerListText.text += player.NickName + "\n";

        // set the room info text
        roomInfoText.text = PhotonNetwork.CurrentRoom.Name;
    }

    // called when the "Start Game" button has been pressed
    public void OnStartGameButton()
    {
        // hide the room
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.CurrentRoom.IsVisible = false;

        sceneName = scenes[roomData.CurrentSceneIndex].SceneName;

        // tell everyone to load into the Game scene
        NetworkManager.instance.photonView.RPC("ChangeScene", RpcTarget.All, sceneName);
    }

    public void OnSceneChangeRightButton()
    {
        if (roomData.CurrentSceneIndex >= scenes.Count - 1)
            roomData.CurrentSceneIndex = 0;
        else
            roomData.CurrentSceneIndex++;

        PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable { { roomData.CurrentSceneIndexString, roomData.CurrentSceneIndex } });
    }
    public void OnSceneChangeLeftButton()
    {
        if (roomData.CurrentSceneIndex <= 0)
            roomData.CurrentSceneIndex = scenes.Count - 1;
        else
            roomData.CurrentSceneIndex--;

        PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable { { roomData.CurrentSceneIndexString, roomData.CurrentSceneIndex } });
    }


    // called when the "Leave Lobby" button has been pressed
    public void OnLeaveLobbyButton()
    {
        PhotonNetwork.LeaveRoom();
        currentRoomName = null;
    }
    // LOBBY BROWSER SCREEN
    GameObject CreateRoomButton()
    {
        GameObject buttonObj = Instantiate(roomButtonPrefab, roomListContainer.transform);
        roomButtons.Add(buttonObj);

        return buttonObj;
    }

    // joins a room of the requested room name
    public void OnJoinRoomButton(string roomName)
    {
        SetScreen(lobbyScreen);
        currentRoomName = roomName;
        PhotonNetwork.JoinRoom(roomName);
    }

    public override void OnRoomListUpdate(List<RoomInfo> allRooms)
    {

        foreach (RoomInfo roomInfo in allRooms)
        {
            //Adds rooms to a list that is used for searching private rooms
            if (roomInfo.IsOpen && !allOpenRooms.Contains(roomInfo))
                allOpenRooms.Add(roomInfo);
            else if (roomInfo.RemovedFromList && roomInfo.PlayerCount > 0 && roomInfo.PlayerCount < roomInfo.MaxPlayers)
                allOpenRooms.Remove(roomInfo);

            //Updates a list of rooms with open and visible rooms
            if (roomInfo.RemovedFromList)
                roomList.Remove(FindRoom(roomInfo.Name));
            else if (FindRoom(roomInfo.Name) == null)
                roomList.Add(roomInfo);
            else
            {
                roomList.Remove(FindRoom(roomInfo.Name));
                roomList.Add(roomInfo);
            }
        }

        // disable all room buttons
        foreach (GameObject button in roomButtons)
            Destroy(button);

        roomButtons.Clear();

        // display all current rooms in the master server
        for (int x = 0; x < roomList.Count; ++x)
        {
            // get or create the button object
            GameObject button = CreateRoomButton();

            // set the room name and player count texts
            button.transform.Find("RoomNameText").GetComponent<TextMeshProUGUI>().text = roomList[x].Name;
            button.transform.Find("PlayerCountText").GetComponent<TextMeshProUGUI>().text = roomList[x].PlayerCount + " / " + roomList[x].MaxPlayers;

            // set the button Onclick event
            Button buttonComp = button.GetComponent<Button>();

            string roomName = roomList[x].Name;

            buttonComp.onClick.RemoveAllListeners();
            buttonComp.onClick.AddListener(() => { OnJoinRoomButton(roomName); });
        }
    }
    RoomInfo FindRoom(string name)
    {
        for (int i = 0; i < roomList.Count; i++)
        {
            if (roomList[i].Name == name)
                return roomList[i];
        }

        return null;
    }
    //Button Event that will change to the bool true if currently false and false if true
    public void SetRoomPrivacy()
    {
        if (!roomIsPublic)
        {
            roomIsPublic = true;
            privacyRoomText.text = "Public";
            PhotonNetwork.CurrentRoom.IsVisible = true;
        }
        else
        {
            roomIsPublic = false;
            privacyRoomText.text = "Private";
            PhotonNetwork.CurrentRoom.IsVisible = false;

        }
    }

    public void SearchForRoom(string codeEntered)
    {
        if (codeEntered == null)
            return;

        for (int i = 0; i < allOpenRooms.Count; i++)
        {
            if (allOpenRooms[i].Name.Contains(codeEntered.ToUpper()))
            {
                OnJoinRoomButton(allOpenRooms[i].Name);
            }
        }
    }

    public void GetSearchRoomCodeValue()
    {
        if (roomSearchBar.text == "")
            return;

        SearchForRoom(roomSearchBar.text);
    }

    //Generates a random Code at the given length
    public string GenerateRandomRoomCode()
    {
        int charactersToGenerate = 4;
        StringBuilder generatedString = new StringBuilder(charactersToGenerate);

        for (int i = 0; i < charactersToGenerate; i++)
        {
            char newCharacter = (char)Random.Range(65, 90);
            generatedString.Append(newCharacter);
        }
        //Check if the room code matches another room, recreate room Code
        //Until it Doesnt match another room
        for (int i = 0; i < allOpenRooms.Count; i++)
        {
            if (allOpenRooms[i].Name.Contains(generatedString.ToString()))
                GenerateRandomRoomCode();
        }

        return generatedString.ToString();
    }


    public string FormatTime(float time)
    {
        int minutes = (int)time / 60;
        int seconds = (int)time - 60 * minutes;
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}