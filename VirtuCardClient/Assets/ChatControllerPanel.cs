using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatControllerPanel : MonoBehaviour
{
    private const int MESSAGE_LIMIT = 44;

    private List<GameObject> currentMessages;

    public InputField messageSend;
    public Button sendBtn;
    public GameObject messageTemplate;
    public GameObject messageParent;

    public List<GameObject> placeholders;

    /// <summary>
    /// This class contains all the methods and fields that are within a single message.
    /// When the constructor is called, it creates a new message from the messageTemplate and places it into the messageParent
    /// </summary>
    private class MessageUI
    {
        private Text messageText;
        private Text username;        private GameObject gameObject;

        public MessageUI(GameObject messageTemplate, GameObject messageParent)
        {
            gameObject = GameObject.Instantiate(messageTemplate, messageParent.transform);
            gameObject.SetActive(true);
            messageText = gameObject.transform.Find("Text").gameObject.GetComponent<Text>();
            username = gameObject.transform.Find("Username").gameObject.GetComponent<Text>();
        }

        public GameObject GetGameObject() { return gameObject; }
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

        sendBtn.onClick.AddListener(delegate {
            string message = messageSend.text;
            // does not send the message if it's blank
            if (message != "") {
                sendClicked();
            }
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void sendClicked() {
        string message = messageSend.text;
        CreateNewMessage(message, "June");
        messageSend.text = "";
    }
}
