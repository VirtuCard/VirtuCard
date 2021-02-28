using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using FirebaseScripts;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

public class RegisterButton : MonoBehaviour
{
    public bool successful = false;
    public GameObject usernameField;
    public GameObject emailField;
    public GameObject passwordField;
    public GameObject confirmPasswordField;

    public void AttemptCreateAccount()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (successful)
        {
            SceneManager.LoadScene(SceneNames.JoinGamePage, LoadSceneMode.Single);
        }
    }
}