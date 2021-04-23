using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Chat;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class ChatControllerPanel : MonoBehaviourPunCallbacks, IChatClientListener
{
    private const int MESSAGE_LIMIT = 44;

    private List<GameObject> currentMessages;

    public InputField messageSend;
    public GameObject messageSendObject;
    public Button sendBtn;
    public GameObject sendBtnObject;
    public GameObject messageTemplate;
    public GameObject messageParent;
    public CanvasGroup bannedSign;

    public string roomcode;
    private ChatClient _chatClient;
    public string appId = "50b55aec-e283-413b-88eb-c86a27dfb8b2";

    public List<GameObject> placeholders;

    public Button[] defaultChats;
    public Text[] defaultChatMessages;

    public Dropdown privChatOption;
    public RectTransform privChatSize;

    private int profanityChecker = 0;
    private int warningCounter = 0;


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
            if (message.Length > 56)
            {
                message = message.Substring(0, 53);
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
        // reset when the new game starts
        warningCounter = 0;
        // set the currentMessages to contain the placeholders
        currentMessages = new List<GameObject>();
        currentMessages.AddRange(placeholders);

        sendBtn.onClick.AddListener(delegate { sendBtnClicked(); });

        roomcode = ClientData.getJoinCode();
        _chatClient = new ChatClient(this) {ChatRegion = "US"};
        PhotonNetwork.AddCallbackTarget(this);
        _chatClient.Connect(appId, "0.1b", new AuthenticationValues(PhotonNetwork.NickName));

        // private message UI
        privChatSize.offsetMin = new Vector2(privChatSize.offsetMin.x, 995);
        privChatSize.offsetMax = new Vector2(privChatSize.offsetMax.x, 1085);
        privChatOption.options.Add(new Dropdown.OptionData("Public chat")); // default
        var privName = ClientData.GetAllConnectedPlayers();
        foreach (string namePlayer in privName)
        {
            // only add the name if it is not this person
            if (!namePlayer.Equals(PhotonNetwork.NickName))
            {
                Debug.Log("should've added ${namePlayer}" + namePlayer);
                privChatOption.options.Add(new Dropdown.OptionData(namePlayer));
            }
        }

        privChatOption.value = 0;
        privChatOption.RefreshShownValue();

        // default chats 
        defaultChats[0].onClick.AddListener(delegate { defaultClicked("Outstanding Move"); });
        defaultChats[1].onClick.AddListener(delegate { defaultClicked("Big OOF"); });
        defaultChats[2].onClick.AddListener(delegate { defaultClicked("Well Played"); });
        // end of default chat
    }

    // Update is called once per frame
    void Update()
    {
        _chatClient.Service();

        // probably better way to do this, but this is here for now
        // btn is not available if the chat is hidden
        sendBtnObject.SetActive(!ClientData.getHideChat());
        messageSendObject.SetActive(!ClientData.getHideChat());

        privChatOption.options.RemoveAll(optionData =>
            !ClientData.GetAllConnectedPlayers().Contains(optionData.text) && !optionData.text.Equals("Public chat")
        );
    }

    /// <summary>
    /// This function gets the person the text of what is inside the dropdown
    /// </summary> 
    public string privChatPlayer()
    {
        Debug.Log(privChatOption.options[privChatOption.value].text);
        return privChatOption.options[privChatOption.value].text;
    }

    public void sendBtnClicked()
    {
        if (warningCounter < 3)
        {
            string message = messageSend.text;
            profanityChecker = 0;

            // makes everything lowercase to check the swear words
            string tempMessage = message.ToLower();

            // checks if it has a bad word
            for (int i = 0; i < badWords.Count; i++)
            {
                if (tempMessage.Contains(badWords[i]))
                {
                    profanityChecker = 1;
                    break;
                }
            }

            // for UMANG
            // if profanity is allowed 
            if (ClientData.IsProfanityAllowed())
            {
                profanityChecker = 0;
            }

            // does not send the message if it's blank
            if ((message != "") && (profanityChecker == 0))
            {
                sendClicked();
            }

            string warningMessage;

            switch (warningCounter)
            {
                case 0:
                    warningMessage = "WATCH YOUR LANGUAGE. THIS IS A WARNING";
                    break;
                case 1:
                    warningMessage = "WATCH YOUR LANGUAGE. THIS IS THE LAST WARNING";
                    break;
                default:
                    warningMessage = "YOU ARE NOW BANNED FROM CHAT";
                    break;
            }

            // when there is a bad message
            if (profanityChecker == 1)
            {
                Debug.Log("Naught word detected!");
                _chatClient.SendPrivateMessage(PhotonNetwork.NickName, warningMessage);
                messageSend.text = "";
                warningCounter++;
            }
        }
        else // the person has been banned from chatting for the rest of the game.
        {
            bannedSign.GetComponent<CanvasGroup>().alpha = 1;
            StartCoroutine(FadeCanvas(bannedSign, bannedSign.alpha, 0));
            messageSend.text = "";
        }
    }


    public void sendClicked()
    {
        string message = messageSend.text;
        if (String.Compare(privChatPlayer(), "Public chat") == 0)
        {
            // public chat
            SendMessage(message);
        }
        else // text is a private message
        {
            Debug.Log("Private message! " + privChatPlayer());
            SendPrivMessage(message);
        }

        messageSend.text = "";
    }

    public void defaultClicked(string message)
    {
        if (warningCounter < 3)
        {
            if (String.Compare(privChatPlayer(), "Public chat") == 0)
            {
                // public chat
                SendMessage(message);
            }
            else // text is a private message
            {
                SendPrivMessage(message);
            }
        }
        else // person is banned from chat
        {
            bannedSign.GetComponent<CanvasGroup>().alpha = 1;
            StartCoroutine(FadeCanvas(bannedSign, bannedSign.alpha, 0));
            messageSend.text = "";
        }
    }

    public new void SendMessage(string message)
    {
        if (message.StartsWith("!play "))
        {
            SendSystemMessage(message.Substring("!play ".Length));
            // Check if the message starts with "!play ", if it does,
            // send it to the system specifically instead.
        }
        else
        {
            _chatClient.PublishMessage(roomcode, message);
        }
    }

    public new void SendPrivMessage(string message)
    {
        Debug.Log("Sending private message to " + privChatPlayer());
        _chatClient.SendPrivateMessage(privChatPlayer(), message);
    }

    private new void SendSystemMessage(string message)
    {
        _chatClient.SendPrivateMessage("System (Host) " + ClientData.getJoinCode(), message);
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

        string text;
        if (channelName.Contains("System (Host)"))
        {
            text = "<color=fuchsia>(Song Request) </color>";
            text += sender;
            // If this is a song request, show it as so in the chat too. 
        }
        else
        {
            if (profanityChecker == 0)
            {
                text = "<color=red>(Private) </color>";
                text += sender;
                if (sender.Equals(PhotonNetwork.NickName))
                {
                    text += "<color=red> to </color>";
                    text += channelName.Split(':')[1];
                    // If you're the sender, indicate who the message is being sent to.
                }
            }
            else // this is a swear word message to the person
            {
                text = "<color=red>HOST</color>";
            }
        }


        CreateNewMessage(message.ToString(), text);
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

    /// <summary>
    /// This is the code to update the fade in or fade out for the Canvas Group
    /// </summary>
    public IEnumerator FadeCanvas(CanvasGroup cg, float start, float end, float lerpTime = 1.0f)
    {
        float _timeStartedLerping = Time.time;
        float timeSinceStarted = Time.time - _timeStartedLerping;
        float percentageComplete = timeSinceStarted / lerpTime;

        while (true)
        {
            timeSinceStarted = Time.time - _timeStartedLerping;
            percentageComplete = timeSinceStarted / lerpTime;

            float currentValue = Mathf.Lerp(start, end, percentageComplete);
            cg.alpha = currentValue;
            if (percentageComplete >= 1) break;
            yield return new WaitForEndOfFrame();
        }
    }

    // don't use the word hell because I don't want Hello being a bad word
    // make it all lower case
    private List<string> badWords = new List<string>(new string[]
    {
        "fuck", "shit", "bitch", "cunt", "purdue sucks", "@ss",
        "b!tch", "sh!t", "arse", "asshole", "bastard", "damn", "d@mn",
        "prick", "slut"
    });
}