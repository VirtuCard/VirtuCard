using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using FirebaseScripts;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class RegisterButton : MonoBehaviour
{
    /// <summary>
    /// `successful` is an int describing various states for the Register page
    /// 0: Default state (Show nothing)
    /// 1: Successful state (Go to Main Page)
    /// -1: Invalid Username
    /// -2
    /// </summary>
    public static int successful = 0;
    public GameObject usernameField;
    public GameObject emailField;
    public GameObject passwordField;
    public GameObject confirmPasswordField;
    public GameObject errorPanel;
    public GameObject errorTitle;
    
    public GameObject loadingPanel;
    public GameObject errorMessage;

    void Start()
    {
        errorPanel.SetActive(false);
    }

    public void AttemptCreateAccount()
    {
        loadingPanel.SetActive(true);
        string username = usernameField.GetComponent<InputField>().text;
        string email = emailField.GetComponent<InputField>().text;
        string password = passwordField.GetComponent<InputField>().text;

        CreateUserAccount(username, email, password);
    }

    /// <summary>
    /// This method creates a user account on firebase with the given email username and password
    /// It sets the boolean successful with its return state. (less than 0 for failure)
    /// </summary>
    public static void CreateUserAccount(string username, string email, string password)
    {
        FirebaseInit.InitializeFirebase(isInit =>
        {
            if (!isInit)
            {
                successful = -3;
                return;
            }

            DatabaseUtils.findUsername(username, val =>
            {
                if (val != null)
                {
                    successful = -1;
                    return;
                }
                else
                {
                    AuthUser.RegisterAccount(username, email, password, ret =>
                    {
                        if (ret)
                        {
                            successful = 1;
                        }
                        else
                        {
                            successful = -2;
                        }
                    });
                }
            });
        });
    }

    // ReSharper disable Unity.PerformanceAnalysis
    void CreateErrorMessage(string title, string message)
    {
        errorTitle.GetComponent<Text>().text = title;
        errorMessage.GetComponent<Text>().text = message;
        loadingPanel.SetActive(false);
        errorPanel.SetActive(true);
        successful = 0;
    }

    // Update is called once per frame
    void Update()
    {
        switch (successful)
        {
            case 1:
                loadingPanel.SetActive(false);
                SceneManager.LoadScene(SceneNames.JoinGamePage, LoadSceneMode.Single);
                break;
            case -1:
                CreateErrorMessage("Invalid Username", "Sorry! Username is already taken.");
                break;
            case -2:
                CreateErrorMessage("Email Already Exists", "This email is already taken. Try another email.");
                break;
            case -3:
                CreateErrorMessage("Error", "Something Unexpected Happened");
                break;
        }
    }
}