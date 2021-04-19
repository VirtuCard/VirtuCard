using System.Collections;
using System.Collections.Generic;
using System;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Photon.Chat;
using Photon.Pun;
using PhotonScripts;
using GameScreen.ChatPanel;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MessageTest
{
    [UnityTest]
    public IEnumerator MessageTestWithEnumeratorPasses()
    {
        /*
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
        Assert.AreEqual(gameChoices.captionText.text, "TestGame", "Game Dropdowns provided unexpected value");

        joinGameButton.onClick.Invoke();
        yield return new WaitForSeconds(7);

        //Check if reached Profile Page
        currentScene = SceneManager.GetActiveScene();
        Assert.IsTrue(currentScene.name.Equals(SceneNames.WaitingRoomScreen),
            "currentScene.name.Equals(SceneNames.WaitingRoomScreen)");*/

        // checks if the message puts ... after 44 characters
        string message = "junieseojunieseojunieseojunieseojunieseojunieseo";
        ChatPanelController p = new ChatPanelController();
        string edited = p.setAndGetTextTest(message);
        //p.setTextTest(message);
        //yield return new WaitForSeconds(2);
        //string edited = p.getTextTest();
        string expected = "junieseojunieseojunieseojunieseojunieseoj...";
        bool check = false;
        if (String.Compare(expected, edited) == 0) {
            check = true;
        }
        yield return new WaitForSeconds(2);
        Assert.IsTrue(check);

        // checks if the message has been sent
        p.CreateNewMessage("Testing123", "UsernameTesting");
        yield return new WaitForSeconds(2);
        Assert.IsTrue(p.getMessageCount() == 1);

        yield return new WaitForSeconds(2);;
    }
}
