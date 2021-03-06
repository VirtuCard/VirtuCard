﻿using System;
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
            isAnonymous = false;
            errorPanel.SetActive(false);
            DatabaseUtils.getUser(AuthUser.GetUserID(), json =>
            {
                User user = new User(json);
                isAnonymous = user.IsAnonymous;
            });
        }

        void OnSettingsButtonClick()
        {
            SceneManager.LoadScene(SceneNames.SettingsScene, LoadSceneMode.Single);
        }

        void OnProfileButtonClick()
        {
            if (isAnonymous)
            {
                CreateErrorMessage("Not allowed",
                    "You are currently not signed in. You are required to be signed in to view your profile");
            }
            else
            {
                SceneManager.LoadScene(SceneNames.ProfileScene, LoadSceneMode.Single);
            }
        }


        void OnFriendsButtonClick()
        {
            if (isAnonymous)
            {
                CreateErrorMessage("Not allowed",
                    "You are currently not signed in. You are required to be signed in to view your friends list");
            }
            else
            {
                SceneManager.LoadScene(SceneNames.FriendsScene, LoadSceneMode.Single);
            }
        }

        // Update is called once per frame
        void Update()
        {
        }

        void OnApplicationQuit()
        {
            if (isAnonymous)
            {
                AuthUser.DeleteAnonymousAccount(task => { Debug.Log("Completed with return value " + task); });
            }
        }
    }
}