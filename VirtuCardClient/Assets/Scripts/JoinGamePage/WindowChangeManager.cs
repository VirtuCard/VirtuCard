using System;
using Firebase.Auth;
using FirebaseScripts;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace JoinGamePage
{
    public class WindowChangeManager : MonoBehaviour
    {
        public GameObject errorPanel;
        public GameObject errorTitle;
        public GameObject errorMessage;

        public Button profileButton;
        public Button friendsButton;
        public Button settingsButton;

        public bool isAnonymous;

        void CreateErrorMessage(string title, string message)
        {
            errorTitle.GetComponent<Text>().text = title;
            errorMessage.GetComponent<Text>().text = message;
            errorPanel.SetActive(true);
        }

        private void Start()
        {
            errorPanel.SetActive(false);
            DatabaseUtils.getUser(AuthUser.GetUserID(), json =>
            {
                User user = new User(json);
                isAnonymous = user.IsAnonymous;
            });
        }

        void onSettingsButtonClick()
        {
            SceneManager.LoadScene(SceneNames.SettingsScene, LoadSceneMode.Single);
        }
        
        // Update is called once per frame
        void Update()
        {
        }
    }
}