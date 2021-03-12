using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Firebase;
using Firebase.Auth;
using FirebaseScripts;
using Google;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GoogleSignInScript : MonoBehaviour
{
    public string webClientId = "<your client id here>";

    private GoogleSignInConfiguration configuration;

    //Associated fields with confirming login/sign-in
    private int successful = 0;
    public GameObject failedPanel;
    public Text errorTitle;
    public Text errorMessage;

    private void Awake()
    {
        configuration = new GoogleSignInConfiguration
            {WebClientId = webClientId, RequestEmail = true, RequestIdToken = true};
     
    }


    public void SignInWithGoogle()
    {
        OnSignIn();
    }

    public void SignOutFromGoogle()
    {
        OnSignOut();
    }

    private void OnSignIn()
    {
        GoogleSignIn.Configuration = configuration;
        GoogleSignIn.Configuration.UseGameSignIn = false;
        GoogleSignIn.Configuration.RequestIdToken = true;
        AddToInformation("Calling SignIn");

        GoogleSignIn.DefaultInstance.SignIn().ContinueWith(OnAuthenticationFinished);
    }

    private void OnSignOut()
    {
        AddToInformation("Calling SignOut");
        GoogleSignIn.DefaultInstance.SignOut();
    }

    public void OnDisconnect()
    {
        AddToInformation("Calling Disconnect");
        GoogleSignIn.DefaultInstance.Disconnect();
    }

    internal void OnAuthenticationFinished(Task<GoogleSignInUser> task)
    {
        if (task.IsFaulted)
        {
            using (IEnumerator<Exception> enumerator = task.Exception.InnerExceptions.GetEnumerator())
            {
                if (enumerator.MoveNext())
                {
                    GoogleSignIn.SignInException error = (GoogleSignIn.SignInException) enumerator.Current;
                    AddToInformation("Got Error: " + error.Status + " " + error.Message);
                    successful = -1;
                }
                else
                {
                    AddToInformation("Got Unexpected Exception?!?" + task.Exception);
                    successful = -1;
                }
            }
        }
        else if (task.IsCanceled)
        {
            AddToInformation("Canceled");
            successful = -1;
        }
        else
        {
            AddToInformation("Welcome: " + task.Result.DisplayName + "!");
            AddToInformation("Email = " + task.Result.Email);
            AddToInformation("Google ID Token = " + task.Result.IdToken);
            AddToInformation("Email = " + task.Result.Email);
            AuthUser.SignInWithGoogle(task.Result.IdToken, i => successful = i);
        }
    }

    private void AddToInformation(string str)
    {
        Debug.Log(str);
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
            CreateErrorMessage("Login Error", "Failed to Sign In with Google!");
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