using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RegistrationPageManager : MonoBehaviour
{
    // these are the text boxes for the account info
    public InputField usernameInput;
    public InputField emailInput;
    public InputField passwordInput;
    public InputField confirmPasswordInput;

    // these are the failure texts that popup on user error
    public Text failedText;
    public Text failedPasswordText;

    // this is the button that is pressed to submit the username and password
    public Button createBtn;

    // this controls what scene to go to
    private LoadDifferentScene sceneLoader;

    // Start is called before the first frame update
    void Start()
    {
        failedText.enabled = false;
        failedPasswordText.enabled = false;

        // initialize sceneLoader
        sceneLoader = gameObject.AddComponent<LoadDifferentScene>();

        // add an event listner for when the create button is clicked
        createBtn.onClick.AddListener(delegate {
            onCreateBtnClick();
        });

        confirmPasswordInput.onValueChanged.AddListener(delegate
        {
            onPasswordInputValueChanged();
        });

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void onPasswordInputValueChanged()
    {
        string password = passwordInput.text;
        string confirmPassword = confirmPasswordInput.text;

        if (password.Equals(confirmPassword) && string.IsNullOrWhiteSpace(password) == false)
        {
            failedPasswordText.enabled = false;
        }
        else {
            failedPasswordText.enabled = true;
        }
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
                failedText.enabled = false;

                // TODO verifiy email is valid and actually register account on firebase.

                // TODO sign in the user and then change the scene to landing page
                sceneLoader.ChangeScene(SceneNames.LandingPage);
            }
            else
            {
                failedText.enabled = true;
                failedText.text = "Must input valid username";
            }
        }
        else
        {
            failedText.enabled = true;
            failedText.text = "Must input a valid password";
        }
    }
}
