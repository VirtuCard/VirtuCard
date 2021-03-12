using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Start is called before the first frame update
using Firebase.Auth;
using Facebook.Unity;
using FirebaseScripts;

public class FacebookLogin : MonoBehaviour
{
// Start function from Unity's MonoBehavior
    private int successful = 0;

    void Start()
    {
        if (!FB.IsInitialized)
        {
            FB.Init(initCallback, onHideUnity);
        }
        else
        {
            // Already initialized
            FB.ActivateApp();
        }
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
// Permission option list      https://developers.facebook.com/docs/facebook-login/permissions/
        var perms = new List<string> {"email", "user_birthday", "public_profile"};
        FB.LogInWithReadPermissions(perms, ret =>
        {
            if (FB.IsLoggedIn)
            {
                // AccessToken class will have session details
                var accessToken = Facebook.Unity.AccessToken.CurrentAccessToken;
                // current access token's User ID : aToken.UserId
                AuthUser.FacebookLogin(accessToken.TokenString, val => { successful = val; });
            }
            else
            {
                Debug.Log("User cancelled login");
                successful = -1;
            }
        });
    }

    private void Update()
    {
        if (successful != 0)
        {
            Debug.Log("Return value:" + successful);
            successful = 0;
        }
    }
}