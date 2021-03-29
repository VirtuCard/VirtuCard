using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Chat;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class ChatControllerPanel : MonoBehaviourPunCallbacks, IChatClientListener
{
    private const int MESSAGE_LIMIT = 44;

    private List<GameObject> currentMessages;

    public InputField messageSend;
    public Button sendBtn;
    public GameObject messageTemplate;
    public GameObject messageParent;

    public string roomcode;
    private ChatClient _chatClient;
    public string appId = "50b55aec-e283-413b-88eb-c86a27dfb8b2";

    public List<GameObject> placeholders;

    public Button[] defaultChats;
    public Text[] defaultChatMessages;

    public Dropdown privChatOption;
    public RectTransform privChatSize;


    /// <summary>
    /// This class contains all the methods and fields that are within a single message.
    /// When the constructor is called, it creates a new message from the messageTemplate and places it into the messageParent
    /// </summary>
    private class MessageUI
    {
        private Text messageText;
        private Text username;
        private GameObject gameObject;

        public MessageUI(GameObject messageTemplate, GameObject messageParent)
        {
            gameObject = GameObject.Instantiate(messageTemplate, messageParent.transform);
            gameObject.SetActive(true);
            messageText = gameObject.transform.Find("Text").gameObject.GetComponent<Text>();
            username = gameObject.transform.Find("Username").gameObject.GetComponent<Text>();
        }

        public GameObject GetGameObject()
        {
            return gameObject;
        }

        public void SetText(string message)
        {
            if (message.Length > 44)
            {
                message = message.Substring(0, 41);
                message += "...";
            }

            messageText.text = message;
        }

        public void SetUsername(string username)
        {
            this.username.text = username;
        }
    }

    /// <summary>
    /// This method creates a new message and places it into the message box
    /// </summary>
    /// <param name="message">The message string to be displayed. Limited to MESSAGE_LIMIT chars</param>
    /// <param name="username">The username of the person sending the message</param>
    public void CreateNewMessage(string message, string username)
    {
        MessageUI ui = new MessageUI(messageTemplate, messageParent);
        ui.SetText(message);
        ui.SetUsername(username);
        // remove the oldest message to make room for the new one
        GameObject oldestMessage = currentMessages[0];
        currentMessages.RemoveAt(0);
        oldestMessage.SetActive(false);
        Destroy(oldestMessage);
        // add the new message
        currentMessages.Add(ui.GetGameObject());
    }

    // Start is called before the first frame update
    void Start()
    {

        // set the currentMessages to contain the placeholders
        currentMessages = new List<GameObject>();
        currentMessages.AddRange(placeholders);

        sendBtn.onClick.AddListener(delegate
        {
            string message = messageSend.text;
            // does not send the message if it's blank
            if (message != "")
            {
                sendClicked();
            }
        });

        roomcode = ClientData.getJoinCode();
        _chatClient = new ChatClient(this) {ChatRegion = "US"};
        PhotonNetwork.AddCallbackTarget(this);
        _chatClient.Connect(appId, "0.1b", new AuthenticationValues(PhotonNetwork.NickName));

        // private message UI
        privChatSize.offsetMin = new Vector2(privChatSize.offsetMin.x, 995);
        privChatSize.offsetMax = new Vector2(privChatSize.offsetMax.x, 1085);
        privChatOption.options.Add(new Dropdown.OptionData("Public chat")); // default
        List<string> privName = ClientData.GetAllConnectedPlayers();
        foreach (string namePlayer in privName)
        {
            // only add the name if it is not this person
            if (!namePlayer.Equals(PhotonNetwork.NickName))
            {
                Debug.Log("should've added ${namePlayer}" + namePlayer);
                privChatOption.options.Add(new Dropdown.OptionData(namePlayer));
            }
        }

        // testing purpose - don't delete for now.
        //privChatOption.AddOptions(privName);

        // default chats 
        defaultChats[0].onClick.AddListener( delegate { defaultClicked("Outstanding Move"); });
        defaultChats[1].onClick.AddListener( delegate { defaultClicked("Big OOF"); });
        defaultChats[2].onClick.AddListener( delegate { defaultClicked("Well Played"); });
        // end of default chat
        
    }

    // Update is called once per frame
    void Update()
    {
        _chatClient.Service();
    }
    
    /// <summary>
    /// This function gets the person the text of what is inside the dropdown
    /// </summary> 
    public string privChatPlayer() {
        Debug.Log(privChatOption.options[privChatOption.value].text);
        return privChatOption.options[privChatOption.value].text;
    }


    public void sendClicked()
    {

        string message = messageSend.text;
        if (String.Compare(privChatPlayer(), "Public chat") == 0) { // public chat
            SendMessage(message);
        }
        else // text is a private message
        {
            Debug.Log("Private message! " + privChatPlayer());
            SendPrivMessage(message);
        }

        messageSend.text = "";
    }

    public void defaultClicked(string message) {
        if (String.Compare(privChatPlayer(), "Public chat") == 0) { // public chat
            SendMessage(message);
        }
        else // text is a private message
        {
            SendPrivMessage(message);
        }
    }

    public new void SendMessage(string message)
    {
        _chatClient.PublishMessage(roomcode, message);
    }

    public new void SendPrivMessage(string message)
    {
        Debug.Log("Sending private message to " + privChatPlayer());
        _chatClient.SendPrivateMessage(privChatPlayer(), message);
    }

    public void DebugReturn(DebugLevel level, string message)
    {
        Debug.Log(message);
    }

    public void OnDisconnected()
    {
        Debug.Log("Chat Disconnected!\n");
    }

    public void OnConnected()
    {
        Debug.Log("Chat Connected!\n");
        _chatClient.Subscribe(new[] {roomcode});

        /* Enable chatbox here */
    }

    public void OnChatStateChange(ChatState state)
    {
        /* Ignore */
    }

    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        for (int i = 0; i < messages.Length; i++)
        {
            Debug.Log(messages[i]);
            CreateNewMessage(messages[i].ToString(), senders[i]);
        }
    }

    public void OnPrivateMessage(string sender, object message, string channelName)
    {
        Debug.Log("OnPrivateMessage: {0} ({1}) > {2}" + channelName + " " + sender + " " + message);

        string text = "<color=red>(Private) </color>";
        text += sender;
        CreateNewMessage(message.ToString(), text);

        /* Ignore */
    }

    public void OnSubscribed(string[] channels, bool[] results)
    {
        /* Ignore */
    }

    public void OnUnsubscribed(string[] channels)
    {
        /* Ignore */
    }

    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
        /* Ignore */
    }

    public void OnUserSubscribed(string channel, string user)
    {
        /* Ignore */
    }

    public void OnUserUnsubscribed(string channel, string user)
    {
        /* Ignore */
    }

    public override void OnLeftRoom()
    {
        _chatClient.Disconnect();
    }
}