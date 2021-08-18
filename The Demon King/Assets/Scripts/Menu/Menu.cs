using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class Menu : MonoBehaviourPunCallbacks, ILobbyCallbacks
{
    [Header("Screens")]
    public GameObject mainScreen;
    public GameObject createRoomScreen;
    public GameObject lobbyScreen;
    public GameObject lobbyBrowserScreen;

    [Header("Main Screen")]
    public Button createRoomButton;
    public Button findRoomButton;

    [Header("Create Room Screen")]
    public Button createNewRoomButton;

    [Header("Lobby")]
    public TextMeshProUGUI playerListText;
    public TextMeshProUGUI roomInfoText;
    public Button startGameButton;
    [SerializeField] private TMP_Dropdown sceneDropdown;


    [Header("Lobby Browser")]
    public RectTransform roomListContainer;
    public GameObject roomButtonPrefab;

    private List<GameObject> roomButtons = new List<GameObject>();
    private List<RoomInfo> roomList = new List<RoomInfo>();

    private string sceneName;
    private string roomName;

    void Start()
    {

        // disable the menu buttons at the start
        createRoomButton.interactable = false;
        findRoomButton.interactable = false;

        // enable the cursor since we hide it when we play the game
        Cursor.lockState = CursorLockMode.None;

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


    }



    // changes the currently visible screen
    void SetScreen(GameObject screen)
    {
        // disable all other screens
        mainScreen.SetActive(false);
        createRoomScreen.SetActive(false);
        lobbyScreen.SetActive(false);
        lobbyBrowserScreen.SetActive(false);

        // activate the requested screen
        screen.SetActive(true);
    }

    // called when the "Back" button gets pressed
    public void OnBackButton()
    {
        SetScreen(mainScreen);
        ChatManager.instance.chatClient.Unsubscribe(new string[] { roomName });
    }

    // MAIN SCREEN

    // called when the player name input field has been changed
    public void OnPlayerNameValueChanged(TMP_InputField playerNameInput)
    {
        PhotonNetwork.NickName = playerNameInput.text;

        createRoomButton.interactable = !string.IsNullOrEmpty(playerNameInput.text);
        findRoomButton.interactable = !string.IsNullOrEmpty(playerNameInput.text);
    }


    // called when the "Create Room" button has been pressed
    public void OnCreateRoomButton()
    {
        SetScreen(createRoomScreen);
    }

    // called when the "Find Room" button has been pressed
    public void OnFindRoomButton()
    {
        SetScreen(lobbyBrowserScreen);
    }

    // CREATE ROOM SCREEN
    public void OnPlayerRoomValueChanged(TMP_InputField playerRoomNameInput)
    {
        createNewRoomButton.interactable = !string.IsNullOrEmpty(playerRoomNameInput.text);
    }
    public void OnLevelSelectionChanged(Dropdown.OptionData sceneNameSelection)
    {
        sceneName = sceneNameSelection.text;

    }

    public void OnCreateButton(TMP_InputField roomNameInput)
    {
        roomName = roomNameInput.text;
        NetworkManager.instance.CreateRoom(roomNameInput.text);
    }

    // LOBBY SCREEN

    // called when we join a room
    // set the screen to be the Lobby and update the UI for all players
    public override void OnJoinedRoom()
    {
        SetScreen(lobbyScreen);
        photonView.RPC("UpdateLobbyUI", RpcTarget.All);
        ChatManager.instance.StartChat(roomName, PhotonNetwork.NickName);

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

        // display all the players
        playerListText.text = "";

        foreach (Player player in PhotonNetwork.PlayerList)
            playerListText.text += player.NickName + "\n";

        // set the room info text
        roomInfoText.text = "<b>Room Name</b>\n" + PhotonNetwork.CurrentRoom.Name;
    }

    // called when the "Start Game" button has been pressed
    public void OnStartGameButton()
    {
        // hide the room
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.CurrentRoom.IsVisible = false;

        //Name of scene is the current dropdown selection
        if (sceneDropdown != null)
        {
            sceneName = sceneDropdown.options[sceneDropdown.value].text;
        }
        else
        {
            sceneName = "Game";
        }
        // tell everyone to load into the Game scene
        NetworkManager.instance.photonView.RPC("ChangeScene", RpcTarget.All, sceneName);
    }

    // called when the "Leave Lobby" button has been pressed
    public void OnLeaveLobbyButton()
    {
        PhotonNetwork.LeaveRoom();

        SetScreen(mainScreen);
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
        PhotonNetwork.JoinRoom(roomName);
    }


    public override void OnRoomListUpdate(List<RoomInfo> allRooms)
    {
        Debug.Log("UPDATING ROOMLIST");

        foreach (RoomInfo roomInfo in allRooms)
        {
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
}