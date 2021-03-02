using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginPageManager : MonoBehaviour
{
    // these are the text boxes for the username and password inputs
    public InputField usernameInput;
    public InputField passwordInput;

    // this is the button that is pressed to submit the username and password
    public Button loginBtn;


    //Error Dialog
    public GameObject failedPanel;
    public Text errorTitle;
    public Text errorMessage;

    // this controls what scene to go to
    private LoadDifferentScene sceneLoader;

    // Start is called before the first frame update
    void Start()
    {
        failedPanel.SetActive(false);
        // initialize sceneLoader
        sceneLoader = gameObject.AddComponent<LoadDifferentScene>();

        // add an event listner for when the login button is clicked
        loginBtn.onClick.AddListener(delegate { loginBtnClicked(); });
    }

    // Update is called once per frame
    void Update()
    {
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
        // collect username and password
        string username = usernameInput.text;
        string password = passwordInput.text;

        // change the scene
        sceneLoader.ChangeScene(SceneNames.LandingPage);
    }
}