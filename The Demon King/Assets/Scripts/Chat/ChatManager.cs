using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Chat;
using Photon.Chat.Demo;
using Photon.Pun;
using TMPro;
using UnityEngine;

public class ChatManager : MonoBehaviourPun, IChatClientListener
{
    [HideInInspector] public ChatClient chatClient;
    
    // instance
    public static ChatManager instance;
    
    [HideInInspector] public string userID;

    [HideInInspector]public string currentChatRoom;
    public TMP_InputField InputFieldChat;   // set in inspector
    public TMP_Text CurrentChannelText;     // set in inspector
    public int HistoryLengthToFetch; // set in inspector. Up to a certain degree, previously sent messages can be fetched for context

    protected internal ChatAppSettings chatAppSettings;


    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        
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
        Debug.Log(userID + "Connection Successful");
        this.chatClient.SetOnlineStatus(ChatUserStatus.Online); // You can set your online state (without a mesage).
        chatClient.Subscribe(currentChatRoom, HistoryLengthToFetch);

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
            chatClient.PublishMessage(channel, userID + ":" + "says 'hi'."); // you don't HAVE to send a msg on join but you could.
        }

        Debug.Log("OnSubscribed: " + string.Join(", ", channels));    
    }

    public void OnUnsubscribed(string[] channels)
    {
        throw new System.NotImplementedException();

    }

    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
        throw new System.NotImplementedException();
    }

    public void OnUserSubscribed(string channel, string user)
    {
        Debug.LogFormat("OnUserSubscribed: channel=\"{0}\" userId=\"{1}\"", currentChatRoom, user);
    }

    public void OnUserUnsubscribed(string channel, string user)
    {
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
    }
    private void SendChatMessage(string inputLine)
    {
        if (string.IsNullOrEmpty(inputLine))
        {
            return;
        }

        inputLine = userID + ":" + inputLine;
        CurrentChannelText.text = inputLine;
        this.chatClient.PublishMessage(currentChatRoom, inputLine);

    }
    public void ShowChannel(string channelName)
    {
        if (string.IsNullOrEmpty(channelName))
        {
            return;
        }

        ChatChannel channel = null;
        bool found = this.chatClient.TryGetChannel(channelName, out channel);
        if (!found)
        {
            Debug.Log("ShowChannel failed to find channel: " + channelName);
            return;
        }

        this.currentChatRoom = channelName;
        this.CurrentChannelText.text = channel.ToStringMessages();
        Debug.Log("ShowChannel: " + this.currentChatRoom);

    }
    public void Connect()
    {
        chatClient = new ChatClient(this);
        chatAppSettings = PhotonNetwork.PhotonServerSettings.AppSettings.GetChatSettings();
        chatClient.UseBackgroundWorkerForSending = true;
        chatClient.AuthValues = new AuthenticationValues(this.userID);
        chatClient.ConnectUsingSettings(this.chatAppSettings);
        
        Debug.Log("Connecting as: " + this.userID);
    }
    public void StartChat(string roomName, string playerName)
    {
        ChatManager chatNewComponent = FindObjectOfType<ChatManager>();
        chatNewComponent.userID = playerName;
        currentChatRoom = roomName;
        chatNewComponent.Connect();
        this.enabled = true;
    }
    
}
