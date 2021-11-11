using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExitGames.Client.Photon;
using Photon.Chat;
using Photon.Chat.Demo;
using Photon.Pun;
using TMPro;
using UnityEngine;
using Random = System.Random;

public class PlayersTextColor
{
    public bool colorUsed = false;
    public string colorString;
    public string user = null;

}
public class ChatManager : MonoBehaviourPun, IChatClientListener
{
    [SerializeField] private SOMenuData roomData;

    [HideInInspector] public ChatClient chatClient;
    protected internal ChatAppSettings chatAppSettings;
    // instance
    public static ChatManager instance;

    [HideInInspector] public string currentChatRoom;
    private ChatChannel SubscribedChannel;


    public TMP_InputField InputFieldChat;   // set in inspector
    public TMP_Text CurrentChannelText;     // set in inspector
    public int HistoryLengthToFetch; // set in inspector. Up to a certain degree, previously sent messages can be fetched for context

    private string joinRoomMessage = "Has Joined The Chat";
    private string leaveRoomMessage = "Has Left";
    private string message;

    [HideInInspector] public string userID;

    [SerializeField] private List<Color> listOfColorsForNames;
    string assignedTextColor;


    private List<PlayersTextColor> listOfAssignedColors = new List<PlayersTextColor>();


    // Start is called before the first frame update
    void Start()
    {
        //DontDestroyOnLoad(this.gameObject); 
        StartChat(roomData.RoomName, PhotonNetwork.NickName);
        //Get colors from the list and assign to text colors
        foreach (var color in listOfColorsForNames)
        {
            PlayersTextColor colorToAdd = new PlayersTextColor();
            colorToAdd.colorString = ToRGBHex(color);
            listOfAssignedColors.Add(colorToAdd);
        }
    }

    void Awake()
    {
        instance = this;
    }
    // Update is called once per frame
    void Update()
    {
        if (this.chatClient != null)
        {
            this.chatClient.Service(); // make sure to call this regularly! it limits effort internally, so calling often is ok!
        }

    }
    private void OnDisable()
    {
        chatClient.Unsubscribe(new string[] { currentChatRoom });
    }
    public void DebugReturn(DebugLevel level, string message)
    {
        if (level == ExitGames.Client.Photon.DebugLevel.ERROR)
        {
            Debug.LogError(message);
        }
        else if (level == ExitGames.Client.Photon.DebugLevel.WARNING)
        {
            Debug.LogWarning(message);
        }
        else
        {
            Debug.Log(message);
        }
    }

    public void OnDisconnected()
    {
        throw new System.NotImplementedException();
    }

    public void OnConnected()
    {
        this.chatClient.SetOnlineStatus(ChatUserStatus.Online);
        ChannelCreationOptions channelOptions = new ChannelCreationOptions();
        channelOptions.MaxSubscribers = 40;
        channelOptions.PublishSubscribers = true;
        chatClient.Subscribe(currentChatRoom,0,-1, channelOptions);

    }

    public void OnChatStateChange(ChatState state)
    {
    }

    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        if (channelName.Equals(this.currentChatRoom))
        {
            // update text
            this.ShowChannel(this.currentChatRoom);
        }
    }

    public void OnPrivateMessage(string sender, object message, string channelName)
    {
        throw new System.NotImplementedException();
    }

    public void OnSubscribed(string[] channels, bool[] results)
    {
        // Send a message when joining
        foreach (string channel in channels)
        {
            chatClient.PublishMessage(channel,  joinRoomMessage);
        }

        chatClient.TryGetChannel(currentChatRoom, out SubscribedChannel);
        //Checks if room already has people
        if (SubscribedChannel.Subscribers.Count > 0)
        {
            AssignColorsToUsersInChannelOnSubscribe();
        }
        else
        {
            //If the first person in room
            listOfAssignedColors[0].colorUsed = true;
            listOfAssignedColors[0].user = PhotonNetwork.NickName;
        }
    }

    private void AssignColorsToUsersInChannelOnSubscribe()
    {
        //Go through the subscribers and assign colors to all players
        foreach (var user in SubscribedChannel.Subscribers.Reverse())
        {
            AssignTextColor(user);
        }
    }

    private void AssignTextColor(string newUser)
    {
        foreach (var textColors in listOfAssignedColors)
        {
            if (!textColors.colorUsed)
            {
                textColors.colorUsed = true;
                textColors.user = newUser;
                break;
            }

        }
    }

    private void RemoveUserFromColorTextList(string userToRemove)
    {
        //Remove the user from the list of colors
        foreach (var colorText in listOfAssignedColors)
        {
            if (colorText.user == userToRemove)
            {
                colorText.colorUsed = false;
                colorText.user = null;
                break;
            }
        }
    }

    public void OnUnsubscribed(string[] channels)
    {
        CurrentChannelText.text = null;
        //Clear the list of players and colors
        foreach (var colorText in listOfAssignedColors)
        {
            colorText.colorUsed = false;
            colorText.user = null;
        }
    }

    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
        throw new System.NotImplementedException();
    }

    public void OnUserSubscribed(string channel, string user)
    {
        AssignTextColor(user);
    }

    public void OnUserUnsubscribed(string channel, string user)
    {
        RemoveUserFromColorTextList(user);
        
    }
    /// <summary>To avoid that the Editor becomes unresponsive, disconnect all Photon connections in OnDestroy.</summary>
    public void OnDestroy()
    {
        if (this.chatClient != null)
        {
            this.chatClient.Disconnect();
        }
    }

    /// <summary>To avoid that the Editor becomes unresponsive, disconnect all Photon connections in OnApplicationQuit.</summary>
    public void OnApplicationQuit()
    {
        if (this.chatClient != null)
        {
            this.chatClient.Disconnect();
        }
    }
    public void OnEnterSend()
    {
        this.SendChatMessage(this.InputFieldChat.text);
        this.InputFieldChat.text = "";
        InputFieldChat.ActivateInputField();
    }
    private void SendChatMessage(string inputLine)
    {
        if (string.IsNullOrEmpty(inputLine))
            return;


        this.chatClient.PublishMessage(currentChatRoom, inputLine);

    }
    public void ShowChannel(string channelName)
    {
        
        if (string.IsNullOrEmpty(channelName))
            return;
        

        if (SubscribedChannel == null)
        {
            bool found = this.chatClient.TryGetChannel(channelName, out SubscribedChannel);
            if (!found)
            {
                return;
            }
        }

        currentChatRoom = channelName;
        ToStringMessages(SubscribedChannel);
    }

    public void ToStringMessages(ChatChannel currentChannel)
    {
        //reset the string
        CurrentChannelText.text = null;
        //Go through the channels messsages to rewrite to channel text
        for (int i = 0; i < currentChannel.Messages.Count; i++)
        {
            foreach (var player in listOfAssignedColors)
            {
                //If the player is still in the room add message to text field
                if (player.user == currentChannel.Senders[i])
                {
                    string nameMessage = $"{player.colorString}{player.user} :";
                    CurrentChannelText.text += nameMessage;
                    //Makes the messaage color white
                    message = $"<color=white> {currentChannel.Messages[i]}";
                    CurrentChannelText.text += message;
                    //creates a new line
                    CurrentChannelText.text += Environment.NewLine;
                    break;
                }
            }
        }
    }

    public void Connect()
    {
        chatClient = new ChatClient(this);
        chatAppSettings = PhotonNetwork.PhotonServerSettings.AppSettings.GetChatSettings();
        chatClient.UseBackgroundWorkerForSending = true;
        chatClient.AuthValues = new AuthenticationValues(this.userID);
        chatClient.ConnectUsingSettings(this.chatAppSettings);

    }
    public void StartChat(string roomName, string playerName)
    {

        ChatManager chatNewComponent = this;
        chatNewComponent.userID = playerName;
        currentChatRoom = roomName;
        chatNewComponent.Connect();
        this.enabled = true;
    }

    private string CreateRandomColor()
    {
        Random random = new Random();
        var color = String.Format("#{0:X6}", random.Next(0x1000000));
        color = "<color=" + color + ">";
        return color;
    }
    public static string ToRGBHex(Color c)
    {
        // Turns the rgb format into a hex string that is made into the tag needed for TMP
        string color = "<color=" + string.Format("#{0:X2}{1:X2}{2:X2}", ToByte(c.r), ToByte(c.g), ToByte(c.b)) + ">";

        return color;
    }
    private static byte ToByte(float f)
    {
        f = Mathf.Clamp01(f);
        return (byte)(f * 255);
    }

}
