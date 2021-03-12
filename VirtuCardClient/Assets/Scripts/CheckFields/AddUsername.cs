using System;
using Firebase.Auth;
using FirebaseScripts;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace CheckFields
{
    public class AddUsername : MonoBehaviour
    {
        private int value = 0;
        public InputField inputField;

        public GameObject failedPanel;
        public Text errorTitle;
        public Text errorMessage;

        private void Start()
        {

        }

        private void Update()
        {
            if (value == 1)
            {
                SceneManager.LoadScene(SceneNames.JoinGamePage);
            } else if (value == -1)
            {
                CreateErrorMessage("Username Already Present!", "Try another username.");
            }

            value = 0;
        }

        void CreateErrorMessage(string title, string message)
        {
            errorTitle.GetComponent<Text>().text = title;
            errorMessage.GetComponent<Text>().text = message;
            failedPanel.SetActive(true);
        }
        
        public void OnUsernameButtonClick()
        {
            Debug.Log("Hi");
            string username = inputField.text;
            Debug.Log(username);
            User user = new User(username, "virtu@card.com", AuthUser.GetUserID());
            DatabaseUtils.addUser(user, b =>
            {
                if (!b)
                {
                    value = -1;
                }
                else
                {
                    value = 1;
                }
            });
        }
    }
}