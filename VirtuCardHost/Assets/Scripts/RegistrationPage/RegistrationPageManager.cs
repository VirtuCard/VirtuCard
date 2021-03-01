using System;
using System.Collections;
using System.Collections.Generic;
using FirebaseScripts;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RegistrationPageManager : MonoBehaviour
{
    // these are the text boxes for the account info
    public InputField usernameInput;
    public InputField emailInput;
    public InputField passwordInput;
    public InputField confirmPasswordInput;

    // these are the failure texts that popup on user error
    public GameObject failedPanel;
    private static int _successful = 0;

    public Text errorTitle;
    public Text errorMessage;
    public Text failedPasswordText;

    // this is the button that is pressed to submit the username and password
    public Button createBtn;

    // this controls what scene to go to
    private LoadDifferentScene sceneLoader;

    // Start is called before the first frame update
    void Start()
    {
        _successful = 0;
        failedPanel.SetActive(false);
        failedPasswordText.enabled = false;

        // initialize sceneLoader
        sceneLoader = gameObject.AddComponent<LoadDifferentScene>();

        // add an event listner for when the create button is clicked
        createBtn.onClick.AddListener(delegate { onCreateBtnClick(); });

        confirmPasswordInput.onValueChanged.AddListener(delegate { onPasswordInputValueChanged(); });
    }

    // Update is called once per frame
    void Update()
    {
        switch (_successful)
        {
            case 1:
                sceneLoader.ChangeScene(SceneNames.LandingPage);
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

    private void onPasswordInputValueChanged()
    {
        string password = passwordInput.text;
        string confirmPassword = confirmPasswordInput.text;

        if (password.Equals(confirmPassword) && string.IsNullOrWhiteSpace(password) == false)
        {
            failedPasswordText.enabled = false;
        }
        else
        {
            failedPasswordText.enabled = true;
        }
    }

    // ReSharper disable Unity.PerformanceAnalysis
    void CreateErrorMessage(string title, string message)
    {
        errorTitle.GetComponent<Text>().text = title;
        errorMessage.GetComponent<Text>().text = message;
        failedPanel.SetActive(true);
        _successful = 0;
    }


    /// <summary>
    /// This is the even handler for the create button. It handles the registration of the new account to firebase
    /// </summary>
    private void onCreateBtnClick()
    {
        string userName = usernameInput.text;
        string email = emailInput.text;
        string password = passwordInput.text;
        string confirmPassword = confirmPasswordInput.text;

        if (password.Equals(confirmPassword) == true && string.IsNullOrWhiteSpace(password) == false)
        {
            if (string.IsNullOrWhiteSpace(userName) == false && string.IsNullOrWhiteSpace(email) == false)
            {
                failedPanel.SetActive(false);
                // verifiy email is valid and actually register account on firebase.
                // registers the user and then change the scene to landing page
                bool didSucceed = false; ;
                CreateUserAccount(userName, email, password, result => didSucceed = result);
            }
            else
            {
                CreateErrorMessage("Error: Invalid Username", "Must input valid username");
            }
        }
        else

        {
            CreateErrorMessage("Error: Invalid Password", "Must input a valid password");
        }
    }

    /// <summary>
    /// This method creates a user account on firebase with the given email username and password
    /// It sets the boolean successful with its return state. (less than 0 for failure)
    /// </summary>
    public static void CreateUserAccount(string userName, string email, string password, Action<bool> callback)
    {
        FirebaseInit.InitializeFirebase(isInit =>
        {
            if (!isInit)
            {
                _successful = -3;
                callback(false);
                return;
            }

            DatabaseUtils.findUsername(userName, val =>
            {
                if (val != null)
                {
                    _successful = -1;
                    callback(false);
                    return;
                }

                AuthUser.RegisterAccount(userName, email, password, ret =>
                {
                    if (ret)
                    {
                        _successful = 1;
                        callback(true);
                    }
                    else
                    {
                        _successful = -2;
                        callback(false);
                    }
                });
            });
        });
    }
}