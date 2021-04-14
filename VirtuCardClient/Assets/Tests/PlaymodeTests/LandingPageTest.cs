using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NUnit.Framework;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;

public class LandingPageTests
{

    [UnityTest]
    public IEnumerator LandingPageTest()
    {
        SceneManager.LoadScene(SceneNames.LandingPage);
        yield return new WaitForSeconds(1);

        Button signInBtn = GameObject.Find("SignInButton").GetComponent<Button>();
        signInBtn.onClick.Invoke();
        yield return new WaitForSeconds(2);
        Assert.IsTrue(SceneManager.GetActiveScene().name.Equals(SceneNames.SignInPage), "Did not transition scenes");

        Button backBtn = GameObject.Find("BackButton").GetComponent<Button>();
        backBtn.onClick.Invoke();
        yield return new WaitForSeconds(2);
        Assert.IsTrue(SceneManager.GetActiveScene().name.Equals(SceneNames.LandingPage), "Did not transition scenes");

        Button registerBtn = GameObject.Find("RegisterButton").GetComponent<Button>();
        registerBtn.onClick.Invoke();
        yield return new WaitForSeconds(2);
        Assert.IsTrue(SceneManager.GetActiveScene().name.Equals(SceneNames.RegisterPage), "Did not transition scenes");

        backBtn = GameObject.Find("BackButton").GetComponent<Button>();
        backBtn.onClick.Invoke();
        yield return new WaitForSeconds(2);
        Assert.IsTrue(SceneManager.GetActiveScene().name.Equals(SceneNames.LandingPage), "Did not transition scenes");

        Button anonBtn = GameObject.Find("AnonButton").GetComponent<Button>();
        anonBtn.onClick.Invoke();
        yield return new WaitForSeconds(2);
        Assert.IsTrue(SceneManager.GetActiveScene().name.Equals(SceneNames.JoinGamePage), "Did not transition scenes");
    }
}
