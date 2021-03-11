using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FirebaseScripts;

public class ForgotPasswordManager : MonoBehaviour
{
    // Start is called before the first frame update
    public InputField emailInput;
    // this is the button that is pressed to submit the email and recieve the forgot password email
    public Button sendBtn;


    //Error Dialog
    public GameObject failedPanel;
    public Text errorTitle;
    public Text errorMessage;

    //Confirmation Dialog
    public GameObject confirmPanel;
    public Text confirmMessage;
    public Text confirmTitle;

    // this controls what scene to go to
    private LoadDifferentScene sceneLoader;

    public bool CorrectCred = false;
    public bool IncorrectCred = false;

    // Start is called before the first frame update
    void Start()
    {
        // deactivating error and confirm panels
        failedPanel.SetActive(false);
        confirmPanel.SetActive(false);

        // initialize sceneLoader
        sceneLoader = gameObject.AddComponent<LoadDifferentScene>();

        // add an event listner for when the login button is clicked
        sendBtn.onClick.AddListener(delegate { sendBtnClicked(); });

    }

    // Update is called once per frame
    void Update()
    {
        if (CorrectCred)
        {
            confirmPanel.SetActive(true);
            emailInput.text = "";
            CorrectCred = false;

        }
        else if (IncorrectCred)
        {
            failedPanel.SetActive(true);
            emailInput.text = "";
            IncorrectCred = false;
        }

    }

    public void CreateErrorMessage(string title, string message)
    {
        errorTitle.GetComponent<Text>().text = title;
        errorMessage.GetComponent<Text>().text = message;
        failedPanel.SetActive(true);
    }

    public void CreateConfirmMessage(string title, string message)
    {
        confirmTitle.text = title;
        confirmMessage.text = message;
        confirmPanel.SetActive(true);
    }

    /// <summary>
    /// This is the callback for the send email button. It gathers the inputs from the email field
    /// </summary>
    public void sendBtnClicked()
    {
        // collect email
        string email = emailInput.text;
        Debug.Log(email);

        // change the scene
        FirebaseInit.InitializeFirebase(task =>
        {
            AuthUser.ResetPassword(email,
                task => {
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
