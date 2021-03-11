using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class LandingPageTests
{
    // All the buttons in the page
    public Button SignInBtn;
    public bool sign = false;
    
    public Button RegisterBtn;
    public bool register = false;
    
    public Button GoogleBtn;
    public bool google = false;

    public Button FacebookBtn;
    public bool facebook = false;

    public Button AnonBtn;
    public bool anon = false;

    // Start is called before the first frame update

    [UnityTest]
    public IEnumerator LandingPageTest()
    {
        // unit test for sign in
        SignInBtn.onClick.AddListener(delegate { signInClicked(); });
        SignInBtn.onClick.Invoke();
        yield return new WaitForSeconds(2);
        Assert.IsTrue(sign);

        // unit test for register
        RegisterBtn.onClick.AddListener(delegate { registerClicked(); });
        RegisterBtn.onClick.Invoke();
        yield return new WaitForSeconds(2);
        Assert.IsTrue(register);

        // unit test for google
        GoogleBtn.onClick.AddListener(delegate { googleClicked(); });
        GoogleBtn.onClick.Invoke();
        yield return new WaitForSeconds(2);
        Assert.IsTrue(google);

        // unit test for facebook
        FacebookBtn.onClick.AddListener(delegate { facebookClicked(); });
        FacebookBtn.onClick.Invoke();
        yield return new WaitForSeconds(2);
        Assert.IsTrue(facebook);
        
        // unit test for anon
        AnonBtn.onClick.AddListener(delegate { anonClicked(); });
        AnonBtn.onClick.Invoke();
        yield return new WaitForSeconds(2);
        Assert.IsTrue(anon);
    }
    public void signInClicked() {
        sign = true;
    }

    public void registerClicked() {
        register = true;
    }

    public void googleClicked() {
        google = true;
    }

    public void facebookClicked() {
        facebook = true;
    }
    public void anonClicked() {
        anon = true;
    }
}
