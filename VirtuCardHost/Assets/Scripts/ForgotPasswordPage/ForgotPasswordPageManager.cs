using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ForgotPasswordPageManager : MonoBehaviour
{
    // these are the text boxes for the email input
    public InputField emailInput;

    // this is the button that is pressed to submit the email and recieve the forgot password email
    public Button sendBtn;


    //Error Dialog
    public GameObject failedPanel;
    public Text errorTitle;
    public Text errorMessage;

    //Confirmation Dialog
    public GameObject confirmPanel;
    public Text confirmTitle;
    public Text confirmMessage;

    // this controls what scene to go to
    private LoadDifferentScene sceneLoader;

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

    }

    void CreateErrorMessage(string title, string message)
    {
        errorTitle.GetComponent<Text>().text = title;
        errorMessage.GetComponent<Text>().text = message;
        failedPanel.SetActive(true);
    }

    void CreateConfirmMessage(string title, string message)
    {
        confirmTitle.text = title;
        confirmMessage.text = message;
        confirmPanel.SetActive(true);
    }

    /// <summary>
    /// This is the callback for the send email button. It gathers the inputs from the email field
    /// </summary>
    private void sendBtnClicked()
    {
        // collect email
        string email = emailInput.text;

        // change the scene
        sceneLoader.ChangeScene(SceneNames.LoginPage);
    }
}
