using System.Collections;
using System.Collections.Generic;
using System.Threading;
using FirebaseScripts;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;

public class GUITesting
{
    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator LoginGUITest()
    {
        SceneManager.LoadScene(SceneNames.WelcomePage);
        yield return new WaitForSeconds(1);
        var currentScene = SceneManager.GetActiveScene();
        Assert.IsTrue(currentScene.name.Equals(SceneNames.WelcomePage),
            "currentScene.name.Equals(SceneNames.WelcomePage)");

        //Click sign in button
        Button signInButton = GameObject.Find("SigninBtn").GetComponent<Button>();
        Assert.NotNull(signInButton, "signInButton != null");
        signInButton.onClick.Invoke();
        yield return new WaitForSeconds(2);

        //Check if reached login page
        currentScene = SceneManager.GetActiveScene();
        Assert.IsTrue(currentScene.name.Equals(SceneNames.LoginPage),
            "currentScene.name.Equals(SceneNames.LoginPage)");

        //Access fields
        InputField emailField = GameObject.Find("Username").GetComponent<InputField>();
        InputField passwordField = GameObject.Find("Password").GetComponentInChildren<InputField>();
        Button loginButton = GameObject.Find("LoginBtn").GetComponent<Button>();
        Assert.NotNull(emailField, "emailField != null");
        Assert.NotNull(passwordField, "passwordField != null");
        Assert.NotNull(loginButton, "loginButton != null");

        //Attempt login
        emailField.SetTextWithoutNotify("micro@soft.com");
        passwordField.SetTextWithoutNotify("Password!123");
        loginButton.onClick.Invoke();
        yield return new WaitForSeconds(2);
        
        Button successButton = GameObject.Find("SuccessButton").GetComponent<Button>();
        successButton.onClick.Invoke();
        yield return new WaitForSeconds(2);

        //Check if reached landing page
        currentScene = SceneManager.GetActiveScene();
        Assert.IsTrue(currentScene.name.Equals(SceneNames.LandingPage),
            "currentScene.name.Equals(SceneNames.LandingPage)");
    }

    [UnityTest]
    public IEnumerator RegisterGUITest()
    {
        SceneManager.LoadScene(SceneNames.WelcomePage);
        yield return new WaitForSeconds(2);
        var currentScene = SceneManager.GetActiveScene();
        Assert.IsTrue(currentScene.name.Equals(SceneNames.WelcomePage),
            "currentScene.name.Equals(SceneNames.WelcomePage)");

        //Click register button
        Button registerBtn = GameObject.Find("RegisterBtn").GetComponent<Button>();
        Assert.NotNull(registerBtn, "RegisterBtn != null");
        registerBtn.onClick.Invoke();
        yield return new WaitForSeconds(2);

        //Check if reached register page
        currentScene = SceneManager.GetActiveScene();
        Assert.IsTrue(currentScene.name.Equals(SceneNames.RegistrationPage),
            "currentScene.name.Equals(SceneNames.RegistrationPage)");

        //Access fields
        InputField usernameField =
            GameObject.Find("UserInfo").transform.Find("UsernameInput").GetComponent<InputField>();
        InputField emailField = GameObject.Find("UserInfo").transform.Find("EmailInput").GetComponent<InputField>();
        InputField passwordField =
            GameObject.Find("PassInfo").transform.Find("PasswordInput").GetComponent<InputField>();
        InputField confirmPasswordField = GameObject.Find("PassInfo").transform.Find("ConfirmPasswordInput")
            .GetComponent<InputField>();
        Button createAccountButton = GameObject.Find("CreateBtn").GetComponent<Button>();
        Assert.NotNull(usernameField, "usernameField != null");
        Assert.NotNull(emailField, "emailField != null");
        Assert.NotNull(passwordField, "passwordField != null");
        Assert.NotNull(confirmPasswordField, "confirmPasswordField != null");
        Assert.NotNull(createAccountButton, "loginButton != null");

        //Attempt login
        emailField.SetTextWithoutNotify("bright@space.com");
        usernameField.SetTextWithoutNotify("blackboard1");
        passwordField.SetTextWithoutNotify("Password!123");
        confirmPasswordField.SetTextWithoutNotify("Password!123");
        createAccountButton.onClick.Invoke();
        yield return new WaitForSeconds(3);

        //Check if reached landing page
        currentScene = SceneManager.GetActiveScene();
        Assert.IsTrue(currentScene.name.Equals(SceneNames.LandingPage),
            "currentScene.name.Equals(SceneNames.LandingPage)");

        AuthUser.UnregisterAccount("bright@space.com", "Password!123",
            b => { Assert.IsTrue(b, "Account Deletion Failed."); });

        DatabaseUtils.RemoveUserWithName("blackboard1", b => { Assert.IsTrue(b, "Account Deletion Failed."); });
        yield return new WaitForSeconds(2);
    }

    [UnityTest]
    public IEnumerator LandingPageClickProfileTest()
    {
        SceneManager.LoadScene(SceneNames.WelcomePage);
        yield return new WaitForSeconds(2);
        var currentScene = SceneManager.GetActiveScene();
        Assert.IsTrue(currentScene.name.Equals(SceneNames.WelcomePage),
            "currentScene.name.Equals(SceneNames.WelcomePage)");

        //Click sign in button
        Button signInButton = GameObject.Find("SigninBtn").GetComponent<Button>();
        Assert.NotNull(signInButton, "signInButton != null");
        signInButton.onClick.Invoke();
        yield return new WaitForSeconds(2);

        //Check if reached login page
        currentScene = SceneManager.GetActiveScene();
        Assert.IsTrue(currentScene.name.Equals(SceneNames.LoginPage),
            "currentScene.name.Equals(SceneNames.LoginPage)");

        //Access fields
        InputField emailField = GameObject.Find("Username").GetComponent<InputField>();
        InputField passwordField = GameObject.Find("Password").GetComponentInChildren<InputField>();
        Button loginButton = GameObject.Find("LoginBtn").GetComponent<Button>();
        Assert.NotNull(emailField, "emailField != null");
        Assert.NotNull(passwordField, "passwordField != null");
        Assert.NotNull(loginButton, "loginButton != null");

        //Attempt login
        emailField.SetTextWithoutNotify("micro@soft.com");
        passwordField.SetTextWithoutNotify("Password!123");
        loginButton.onClick.Invoke();
        yield return new WaitForSeconds(2);

        Button successButton = GameObject.Find("SuccessButton").GetComponent<Button>();
        successButton.onClick.Invoke();
        yield return new WaitForSeconds(2);
        
        //Check if reached landing page
        currentScene = SceneManager.GetActiveScene();
        Assert.IsTrue(currentScene.name.Equals(SceneNames.LandingPage),
            "currentScene.name.Equals(SceneNames.LandingPage)");

        //Click Profile
        Button profileButton = GameObject.Find("ProfileBtn").GetComponent<Button>();
        Assert.NotNull(profileButton, "profileButton != null");
        profileButton.onClick.Invoke();
        yield return new WaitForSeconds(2);

        //Check if reached Profile Page
        currentScene = SceneManager.GetActiveScene();
        Assert.IsTrue(currentScene.name.Equals(SceneNames.ProfilePage),
            "currentScene.name.Equals(SceneNames.ProfilePage)");

        //Click Profile
        Button backButton = GameObject.Find("BackButton").GetComponent<Button>();
        Assert.NotNull(backButton, "backButton != null");
        backButton.onClick.Invoke();
        yield return new WaitForSeconds(3);

        //Check if reached Profile Page
        currentScene = SceneManager.GetActiveScene();
        Assert.IsTrue(currentScene.name.Equals(SceneNames.LandingPage),
            "currentScene.name.Equals(SceneNames.LandingPage)");
    }

    [UnityTest]
    public IEnumerator LandingPageClickSettingsTest()
    {
        SceneManager.LoadScene(SceneNames.WelcomePage);
        yield return new WaitForSeconds(2);
        var currentScene = SceneManager.GetActiveScene();
        Assert.IsTrue(currentScene.name.Equals(SceneNames.WelcomePage),
            "currentScene.name.Equals(SceneNames.WelcomePage)");

        //Click sign in button
        Button signInButton = GameObject.Find("SigninBtn").GetComponent<Button>();
        Assert.NotNull(signInButton, "signInButton != null");
        signInButton.onClick.Invoke();
        yield return new WaitForSeconds(2);

        //Check if reached login page
        currentScene = SceneManager.GetActiveScene();
        Assert.IsTrue(currentScene.name.Equals(SceneNames.LoginPage),
            "currentScene.name.Equals(SceneNames.LoginPage)");

        //Access fields
        InputField emailField = GameObject.Find("Username").GetComponent<InputField>();
        InputField passwordField = GameObject.Find("Password").GetComponentInChildren<InputField>();
        Button loginButton = GameObject.Find("LoginBtn").GetComponent<Button>();
        Assert.NotNull(emailField, "emailField != null");
        Assert.NotNull(passwordField, "passwordField != null");
        Assert.NotNull(loginButton, "loginButton != null");

        //Attempt login
        emailField.SetTextWithoutNotify("micro@soft.com");
        passwordField.SetTextWithoutNotify("Password!123");
        loginButton.onClick.Invoke();
        yield return new WaitForSeconds(2);

        Button successButton = GameObject.Find("SuccessButton").GetComponent<Button>();
        successButton.onClick.Invoke();
        yield return new WaitForSeconds(2);
        
        //Check if reached landing page
        currentScene = SceneManager.GetActiveScene();
        Assert.IsTrue(currentScene.name.Equals(SceneNames.LandingPage),
            "currentScene.name.Equals(SceneNames.LandingPage)");

        //Click Profile
        Button settingsButton = GameObject.Find("SettingsBtn").GetComponent<Button>();
        Assert.NotNull(settingsButton, "settingsButton != null");
        settingsButton.onClick.Invoke();
        yield return new WaitForSeconds(2);

        //Check if reached Profile Page
        currentScene = SceneManager.GetActiveScene();
        Assert.IsTrue(currentScene.name.Equals(SceneNames.SettingsPage),
            "currentScene.name.Equals(SceneNames.SettingsPage)");

        //Click Profile
        Button backButton = GameObject.Find("BackButton").GetComponent<Button>();
        Assert.NotNull(backButton, "backButton != null");
        backButton.onClick.Invoke();
        yield return new WaitForSeconds(3);

        //Check if reached Profile Page
        currentScene = SceneManager.GetActiveScene();
        Assert.IsTrue(currentScene.name.Equals(SceneNames.LandingPage),
            "currentScene.name.Equals(SceneNames.LandingPage)");
    }

    [UnityTest]
    public IEnumerator LandingPageCreateGameGUITest()
    {
        SceneManager.LoadScene(SceneNames.WelcomePage);
        yield return new WaitForSeconds(2);
        var currentScene = SceneManager.GetActiveScene();
        Assert.IsTrue(currentScene.name.Equals(SceneNames.WelcomePage),
            "currentScene.name.Equals(SceneNames.WelcomePage)");

        //Click sign in button
        Button signInButton = GameObject.Find("SigninBtn").GetComponent<Button>();
        Assert.NotNull(signInButton, "signInButton != null");
        signInButton.onClick.Invoke();
        yield return new WaitForSeconds(2);

        //Check if reached login page
        currentScene = SceneManager.GetActiveScene();
        Assert.IsTrue(currentScene.name.Equals(SceneNames.LoginPage),
            "currentScene.name.Equals(SceneNames.LoginPage)");

        //Access fields
        InputField emailField = GameObject.Find("Username").GetComponent<InputField>();
        InputField passwordField = GameObject.Find("Password").GetComponentInChildren<InputField>();
        Button loginButton = GameObject.Find("LoginBtn").GetComponent<Button>();
        Assert.NotNull(emailField, "emailField != null");
        Assert.NotNull(passwordField, "passwordField != null");
        Assert.NotNull(loginButton, "loginButton != null");

        //Attempt login
        emailField.SetTextWithoutNotify("micro@soft.com");
        passwordField.SetTextWithoutNotify("Password!123");
        loginButton.onClick.Invoke();
        yield return new WaitForSeconds(2);

        Button successButton = GameObject.Find("SuccessButton").GetComponent<Button>();
        successButton.onClick.Invoke();
        yield return new WaitForSeconds(2);
        
        //Check if reached landing page
        currentScene = SceneManager.GetActiveScene();
        Assert.IsTrue(currentScene.name.Equals(SceneNames.LandingPage),
            "currentScene.name.Equals(SceneNames.LandingPage)");


        //Select Freeplay and join game
        Button joinGameButton = GameObject.Find("CreateGameBtn").GetComponent<Button>();
        Dropdown gameChoices = GameObject.Find("GameDropdown").GetComponent<Dropdown>();
        Assert.NotNull(gameChoices, "gameChoices != null");
        Assert.NotNull(joinGameButton, "joinGameButton != null");

        gameChoices.value = 0;
        Assert.AreEqual(gameChoices.captionText.text, "Freeplay", "Game Dropdowns provided unexpected value");
        gameChoices.value = 1;
        Assert.AreEqual(gameChoices.captionText.text, "Uno", "Game Dropdowns provided unexpected value");
        gameChoices.value = 2;
        Assert.AreEqual(gameChoices.captionText.text, "Go Fish", "Game Dropdowns provided unexpected value");

        joinGameButton.onClick.Invoke();
        yield return new WaitForSeconds(5);

        //Check if reached Profile Page
        currentScene = SceneManager.GetActiveScene();
        Assert.IsTrue(currentScene.name.Equals(SceneNames.WaitingRoomScreen),
            "currentScene.name.Equals(SceneNames.WaitingRoomScreen)");

        //Check if Go Fish is selected
        Text selectedGame = GameObject.Find("SelectedGame").GetComponent<Text>();
        Button settingsButton = GameObject.Find("Settings").GetComponent<Button>();

        Assert.NotNull(selectedGame, "selectedGame != null");
        Assert.NotNull(settingsButton, "settingsButton != null");
        Assert.AreEqual(selectedGame.text, "Go Fish", "Dropdown choice not shown.");
        settingsButton.onClick.Invoke();
        yield return new WaitForSeconds(1);
        
        //See if panel has opened, Exit Game then
        Button exitGame = GameObject.Find("ExitGameButton").GetComponent<Button>();
        Assert.IsTrue(exitGame.IsActive());
        exitGame.onClick.Invoke();
        yield return new WaitForSeconds(3);

        //Check if it returned
        currentScene = SceneManager.GetActiveScene();
        Assert.IsTrue(currentScene.name.Equals(SceneNames.LandingPage),
            "currentScene.name.Equals(SceneNames.LandingPage)");
    }
}