using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using ExitGames.Client.Photon;
using Photon.Chat;
using Photon.Chat.Demo;
using Photon.Pun;
using TMPro;
using UnityEngine;
using Random = System.Random;

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

    [SerializeField] private Color listOfColorsForNames;
    

    private Dictionary<string, string> playerTextColors = new Dictionary<string, string>();


    // Start is called before the first frame update
    void Start()
    {
        //DontDestroyOnLoad(this.gameObject); 
        StartChat(roomData.RoomName, PhotonNetwork.NickName);
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

        foreach (var message in messages)
        {
            Debug.Log(message.ToString());
        }
    }

    public void OnPrivateMessage(string sender, object message, string channelName)
    {
        throw new System.NotImplementedException();
    }

    public void OnSubscribed(string[] channels, bool[] results)
    {

        // in this demo, we simply send a message into each channel. This is NOT a must have!
        foreach (string channel in channels)
        {
            chatClient.PublishMessage(channel,  joinRoomMessage); // you don't HAVE to send a msg on join but you could.
        }

        chatClient.TryGetChannel(currentChatRoom, out SubscribedChannel);
        if (SubscribedChannel.Subscribers.Count > 0)
        {
            foreach (var user in SubscribedChannel.Subscribers)
            {
                playerTextColors.Add(user,CreateRandomColor());
            }
        }
        else
        {
            playerTextColors.Add(PhotonNetwork.NickName,CreateRandomColor());
        }
    }

    public void OnUnsubscribed(string[] channels)
    {
        playerTextColors.Remove(PhotonNetwork.NickName);
    }

    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
        throw new System.NotImplementedException();
    }

    public void OnUserSubscribed(string channel, string user)
    {
        playerTextColors.Add(user,CreateRandomColor());
        Debug.LogFormat("OnUserSubscribed: channel=\"{0}\" userId=\"{1}\"", currentChatRoom, user);
    }

    public void OnUserUnsubscribed(string channel, string user)
    {
        playerTextColors.Remove(user);
        Debug.LogFormat("OnUserUnsubscribed: channel=\"{0}\" userId=\"{1}\"", currentChatRoom, user);

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
        {
            return;
        }

        if (SubscribedChannel == null)
        {
            bool found = this.chatClient.TryGetChannel(channelName, out SubscribedChannel);
            if (!found)
            {
                Debug.Log("ShowChannel failed to find channel: " + channelName);
                return;
            }
        }

        this.currentChatRoom = channelName;
        ToStringMessages(SubscribedChannel);
    }

    public void ToStringMessages(ChatChannel currentChannel)
    {
        //reset the string
        CurrentChannelText.text = null;
        for (int i = 0; i < currentChannel.Messages.Count; i++)
        {
            foreach (var player in playerTextColors)
            {
                if (player.Key == currentChannel.Senders[i])
                {
                    string nameMessage = player.Value + player.Key + " : ";
                    CurrentChannelText.text += nameMessage;

                }
            }
            //Makes the messaage color white
            message = "<color=white>" + currentChannel.Messages[i];
            CurrentChannelText.text += message;
            //creates a new line
            CurrentChannelText.text += Environment.NewLine;
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
        Debug.Log(roomName);
        Debug.Log(playerName);
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
        return string.Format("#{0:X2}{1:X2}{2:X2}", ToByte(c.r), ToByte(c.g), ToByte(c.b));
    }
    private static byte ToByte(float f)
    {
        f = Mathf.Clamp01(f);
        return (byte)(f * 255);
    }

}
