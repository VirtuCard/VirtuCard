using System.Collections.Generic;
using ExitGames.Client.Photon;
using Music;
using Photon.Chat;
using Photon.Pun;
using Photon.Realtime;
using PhotonScripts;
using UnityEngine;
using UnityEngine.UI;
using AuthenticationValues = Photon.Chat.AuthenticationValues;

namespace GameScreen.ChatPanel
{
    public class ChatPanelController : MonoBehaviourPunCallbacks, IChatClientListener
    {
        private const int MESSAGE_LIMIT = 44;

        private List<GameObject> currentMessages;

        public string roomcode;
        private ChatClient _chatClient;
        public string appId = "50b55aec-e283-413b-88eb-c86a27dfb8b2";

        public GameObject messageTemplate;
        public GameObject messageParent;

        public List<GameObject> placeholders;
        public static List<string> systemMessages;
        private int messageCounter = 0;

        public PlaylistController songController;

        /// <summary>
        /// This class contains all the methods and fields that are within a single message.
        /// When the constructor is called, it creates a new message from the messageTemplate and places it into the messageParent
        /// </summary>
        public class MessageUI
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

            /// this is for testing purposes
            public string GetText()
            {
                return messageText.text;
            }

            public void SetUsername(string username)
            {
                this.username.text = username;
            }
        } // end of MessageUI class

        /// <summary>
        /// This method creates a new message and places it into the message box
        /// </summary>
        /// <param name="message">The message string to be displayed. Limited to MESSAGE_LIMIT chars</param>
        /// <param name="username">The username of the person sending the message</param>
        public void CreateNewMessage(string message, string username)
        {
            messageCounter++;
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
            systemMessages = new List<string>();
            // set the currentMessages to contain the placeholders
            currentMessages = new List<GameObject>();
            currentMessages.AddRange(placeholders);
            roomcode = HostData.GetJoinCode();

            _chatClient = new ChatClient(this) {ChatRegion = "US"};
            //For left room callback
            PhotonNetwork.AddCallbackTarget(this);
            _chatClient.Connect(appId, "0.1b", new AuthenticationValues("System (Host) " + HostData.GetJoinCode()));
        }

        // Update is called once per frame

        private void Update()
        {
            _chatClient.Service();
            if (systemMessages.Count > 0)
            {
                SendMessage(systemMessages[0]);
                systemMessages.RemoveAt(0);
            }
        }

        public int getMessageCount()
        {
            return messageCounter;
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
            /* Enable chatbox here */
        }

        public void OnChatStateChange(ChatState state)
        {
            /* Ignore */
        }

        public void OnGetMessages(string channelName, string[] senders, object[] messages)
        {
            if (channelName != HostData.GetJoinCode())
            {
                return;
            }

            Debug.Log("message");
            for (int i = 0; i < messages.Length; i++)
            {
                Debug.Log(messages[i]);
                CreateNewMessage(messages[i].ToString(), senders[i]);
            }
        }

        public void OnPrivateMessage(string sender, object message, string channelName)
        {
            //Any private messages that come here are considered song requests currently.
            songController.SearchAndAddSongAsync(message.ToString(), sender);
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

        /// START UNIT TEST
        /// The next two methods are for unit tests only
        /*
        private string strTest = "";
        private GameObject tmp;
        private MessageUI uiTest;
        public void setTextTest(string text)
        {
            tmp = new GameObject();

            GameObject text1 = new GameObject();
            text1.AddComponent<Text>();
            text1.GetComponent<Text>().name = "Text";
            text1.GetComponent<Text>().gameObject.name = "Text";

            GameObject text2 = new GameObject();
            text2.AddComponent<Text>();
            text2.GetComponent<Text>().name = "Username";
            text2.GetComponent<Text>().gameObject.name = "Username";

            var par1 = Instantiate(text1, tmp.transform);
            var par2 = Instantiate(text2, tmp.transform);

            uiTest = new MessageUI(tmp, new GameObject());
            uiTest.SetText(text);
        }

        public string getTextTest()
        {
            return uiTest.GetText();
        }*/
        public string setAndGetTextTest(string text)
        {
            MessageUI ui = new MessageUI(messageTemplate, messageParent);
            ui.SetText(text);
            return ui.GetText();
        }

        /// end of testing
    }
}