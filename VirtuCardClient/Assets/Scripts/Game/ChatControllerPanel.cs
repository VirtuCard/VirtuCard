using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Chat;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class ChatControllerPanel : MonoBehaviour, IChatClientListener
{
    private const int MESSAGE_LIMIT = 44;

    private List<GameObject> currentMessages;

    public InputField messageSend;
    public Button sendBtn;
    public GameObject messageTemplate;
    public GameObject messageParent;

    public string roomcode ;
    private ChatClient _chatClient;
    public string appId = "50b55aec-e283-413b-88eb-c86a27dfb8b2";

    public List<GameObject> placeholders;

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

        ClientData.setJoinCode("ABCDEF"); // TODO: Remove this

        roomcode = ClientData.getJoinCode();
        _chatClient = new ChatClient(this) {ChatRegion = "US"};
        _chatClient.Connect(appId, "0.1b", new AuthenticationValues(PhotonNetwork.NickName));
    }

    // Update is called once per frame
    void Update()
    {
        
        _chatClient.Service();
    }

    public void sendClicked()
    {
        string message = messageSend.text;
        SendMessage(message);
        messageSend.text = "";
    }

    public new void SendMessage(string message)
    {
        _chatClient.PublishMessage(roomcode, message);
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
        SendMessage("hi" + Random.Range(0, 10));
        
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
}