using System;
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

public class Menu : MonoBehaviourPunCallbacks, ILobbyCallbacks
{
      [Header("Screens")]
    [SerializeField] private GameObject mainScreen;
    [SerializeField] private GameObject createRoomScreen;
    [SerializeField] private GameObject lobbyScreen;
    [SerializeField] private GameObject lobbyBrowserScreen;

    [Header("Main Screen")]
    [SerializeField] private Button createRoomButton;
    [SerializeField] private Button findRoomButton;    
    [SerializeField] private GameObject mainSelectableItem;


    [Header("Create Room Screen")]
    [SerializeField] private Button createNewRoomButton;
    [SerializeField] private Toggle privateRoomToggle;
    [SerializeField] private TMP_Text maxPlayerInput;
    [SerializeField] private GameObject createRoomSelectableItem;


    [Header("Lobby")]
    [SerializeField] private TextMeshProUGUI playerListText;
    [SerializeField] private TextMeshProUGUI roomInfoText;
    [SerializeField] private Button startGameButton;
    [SerializeField] private TMP_Dropdown sceneDropdown;
    [SerializeField] private Button RoomPrivacyButton;
    [SerializeField] private TextMeshProUGUI privacyRoomText;
    public List<SceneInformation> scenes;
    [SerializeField] private GameObject lobbySelectableItem;


    [Header("Lobby Browser")]
    [SerializeField] private RectTransform roomListContainer;
    [SerializeField] private GameObject roomButtonPrefab;
    [SerializeField] private TMP_InputField roomSearchBar;
    [SerializeField] private Button searchRoomButton;
    [SerializeField] private GameObject lobbyBrowserSelectableItem;

    private List<GameObject> roomButtons = new List<GameObject>();
    private List<RoomInfo> roomList = new List<RoomInfo>();
    private List<RoomInfo> allOpenRooms = new List<RoomInfo>();

    public bool roomIsPublic = true;
    [HideInInspector] public string sceneName;
    private string currentRoomName;
    private float roomMaxPlayers = 2;

    void Start()
    {
   
        List<string> sceneNames = new List<string>();

        foreach (var scene in scenes)
        {
            sceneNames.Add(scene.SceneName);
        }
        
        sceneDropdown.AddOptions(sceneNames);
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
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(mainSelectableItem);
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
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(mainSelectableItem);
    }

    // MAIN SCREEN

    // called when the player name input field has been changed
    public void OnPlayerNameValueChanged(TMP_InputField playerNameInput)
    {
        PhotonNetwork.NickName = playerNameInput.text;

        createRoomButton.interactable = !string.IsNullOrEmpty(playerNameInput.text);
        findRoomButton.interactable = !string.IsNullOrEmpty(playerNameInput.text);
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

    // called when the "Create Room" button has been pressed
    public void OnCreateRoomButton()
    {
        SetScreen(createRoomScreen);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(createRoomSelectableItem);
        
    }

    // called when the "Find Room" button has been pressed
    public void OnFindRoomButton()
    {
        SetScreen(lobbyBrowserScreen);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(lobbyBrowserScreen);
    }

    // CREATE ROOM SCREEN
    public void OnPlayerRoomValueChanged(TMP_InputField playerRoomNameInput)
    {
        createNewRoomButton.interactable = !string.IsNullOrEmpty(playerRoomNameInput.text);
    }

    public void OnCreateButton(TMP_InputField roomNameInput)
    {
        currentRoomName = roomNameInput.text.ToUpper();
        NetworkManager.instance.CreateRoom(roomNameInput.text, (int)roomMaxPlayers);
    }

    // LOBBY SCREEN

    // called when we join a room
    // set the screen to be the Lobby and update the UI for all players
    public override void OnJoinedRoom()
    {
        SetScreen(lobbyScreen);
        
        photonView.RPC("UpdateLobbyUI", RpcTarget.All);
        ChatManager.instance.StartChat(currentRoomName, PhotonNetwork.NickName);
         PhotonNetwork.CurrentRoom.IsVisible = roomIsPublic;
        
        if (roomIsPublic)
        {
            privacyRoomText.text = "Public";
        }
        else
        {
            privacyRoomText.text = "Private";

        }
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(lobbySelectableItem);
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
        sceneDropdown.gameObject.SetActive(PhotonNetwork.IsMasterClient);
        RoomPrivacyButton.gameObject.SetActive(PhotonNetwork.IsMasterClient);

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
        startGameButton.interactable = false;

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
        
        startGameButton.interactable = NetworkManager.instance.levelNotLoading;
    }

    // called when the "Leave Lobby" button has been pressed
    public void OnLeaveLobbyButton()
    {
        PhotonNetwork.LeaveRoom();
        ChatManager.instance.chatClient.Unsubscribe(new string[] { currentRoomName });
        currentRoomName = null;
        SetScreen(mainScreen);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(mainSelectableItem);
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
        currentRoomName = roomName;

        PhotonNetwork.JoinRoom(roomName);
    }


    public override void OnRoomListUpdate(List<RoomInfo> allRooms)
    {
        Debug.Log("UPDATING ROOMLIST");

        foreach (RoomInfo roomInfo in allRooms)
        {
            //Adds rooms to a list that is used for searching private rooms
            if (roomInfo.IsOpen && !allOpenRooms.Contains(roomInfo))
                allOpenRooms.Add(roomInfo);
            else if(roomInfo.RemovedFromList && roomInfo.PlayerCount > 0 && roomInfo.PlayerCount < roomInfo.MaxPlayers)
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
            Debug.Log("Public");
        }
        else
        {
            roomIsPublic = false;
            privacyRoomText.text = "Private";
            PhotonNetwork.CurrentRoom.IsVisible = false;
            Debug.Log("Private");

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
            if (allOpenRooms[i].Name.Contains(generatedString.ToString()) )
                GenerateRandomRoomCode();
        }

        return generatedString.ToString();
    }
    
}