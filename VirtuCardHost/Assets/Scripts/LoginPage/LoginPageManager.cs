using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FirebaseScripts;
using System.Threading;

public class LoginPageManager : MonoBehaviour
{
    // these are the text boxes for the username and password inputs
    public InputField usernameInput;
    public InputField passwordInput;

    // this is the button that is pressed to submit the username and password
    public Button loginBtn;
    public Button successBtn;
    public GameObject successPanel;

    //Error Dialog
    public GameObject failedPanel;
    public Text errorTitle;
    public Text errorMessage;
    public Text successMessage;

    // this controls what scene to go to
    private LoadDifferentScene sceneLoader;

    public bool CorrectCred = false;
    public bool IncorrectCred = false;


    // Start is called before the first frame update
    void Start()
    {
        successPanel.SetActive(false);
        failedPanel.SetActive(false);
        // initialize sceneLoader
        sceneLoader = gameObject.AddComponent<LoadDifferentScene>();

        // add an event listner for when the login button is clicked
        loginBtn.onClick.AddListener(delegate { loginBtnClicked(); });
    }

    // Update is called once per frame
    void Update()
    {
        if (CorrectCred)
        {
            Debug.Log("login success");
            CreateSuccessMessage("Login Success!");
            successBtn.onClick.AddListener(delegate { sceneLoader.ChangeScene(SceneNames.LandingPage); });
        }

        if (IncorrectCred) 
        {
            IncorrectCred = false;
            CreateErrorMessage("ERROR", "Invalid email or Incorrect password!");
            Debug.Log("login failed");
        }
    }

    void CreateSuccessMessage(string title) {
        successMessage.GetComponent<Text>().text = title;
        successPanel.SetActive(true);
    }

    void CreateErrorMessage(string title, string message)
    {
        errorTitle.GetComponent<Text>().text = title;
        errorMessage.GetComponent<Text>().text = message;
        failedPanel.SetActive(true);
    }

    /// <summary>
    /// This is the callback for the login button. It gathers the inputs from both the username and password fields
    /// </summary>

    private void loginBtnClicked()
    {
        FirebaseInit.InitializeFirebase(task =>
        {
            // collect username and password
            string username = usernameInput.text;
            string password = passwordInput.text;
            
            AuthUser.Login(username, password,
                task=> {
                if (task)
                {
                    Debug.Log("task bool is " + task);
                    CorrectCred = true;
                }
                else
                {
                    Debug.Log("task bool is " + task);
                    IncorrectCred = true;
                }
                
            });
        });
    }
}