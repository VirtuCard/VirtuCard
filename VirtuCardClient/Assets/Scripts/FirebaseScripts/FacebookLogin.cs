using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Start is called before the first frame update
using Firebase.Auth;
using Facebook.Unity;
using FirebaseScripts;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FacebookLogin : MonoBehaviour
{
// Start function from Unity's MonoBehavior
    private int successful = 0;
    public GameObject failedPanel;
    public Text errorTitle;
    public Text errorMessage;

    void Start()
    {
        if (!FB.IsInitialized)
        {
            Debug.Log("Hi");
            FB.Init(initCallback, onHideUnity);
        }

        // Already initialized
        FB.ActivateApp();
    }

    private void initCallback()
    {
        if (FB.IsInitialized)
        {
            // Signal an app activation App Event
            FB.ActivateApp();
            // Continue with Facebook SDK
            // ...
        }
        else
        {
            Debug.Log("Something went wrong to Initialize the Facebook SDK");
        }
    }

    private void onHideUnity(bool isGameScreenShown)
    {
        if (!isGameScreenShown)
        {
            // Pause the game - we will need to hide
            Time.timeScale = 0;
        }
        else
        {
            // Resume the game - we're getting focus again
            Time.timeScale = 1;
        }
    }

    public void OnFacebookLoginClick()
    {
        FirebaseInit.InitializeFirebase(c =>
        {
// Permission option list      https://developers.facebook.com/docs/facebook-login/permissions/
            var perms = new List<string> {"email", "public_profile", "user_friends"};
            FB.LogInWithReadPermissions(perms, ret =>
            {
                if (FB.IsLoggedIn)
                {
                    // AccessToken class will have session details

                    var accessToken = ret.AccessToken;

                    // current access token's User ID : aToken.UserId
                    Debug.Log("Called Login");
                    if (accessToken == null)
                    {
                        successful = -1;
                        return;
                    }
                    AuthUser.FacebookLogin(accessToken.TokenString, val => { successful = val; });
                }
                else
                {
                    Debug.Log("User cancelled login" + ret.Error);
                    successful = -1;
                }
            });
        });
    }

    void CreateErrorMessage(string title, string message)
    {
        errorTitle.GetComponent<Text>().text = title;
        errorMessage.GetComponent<Text>().text = message;
        failedPanel.SetActive(true);
    }

    private void Update()
    {
        if (successful == -1)
        {
            CreateErrorMessage("Login Error", "Failed to Sign In with Facebook!");
        }
        else if (successful == 1)
        {
            SceneManager.LoadScene(SceneNames.SetUpAccount);
        }
        else if (successful == 2)
        {
            SceneManager.LoadScene(SceneNames.JoinGamePage);
        }

        successful = 0;
    }
}