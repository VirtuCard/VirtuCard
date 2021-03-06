using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class LandingPageTest : MonoBehaviour
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
    void Start()
    {
        SignInBtn.onClick.AddListener(delegate { signInClicked(); });
        RegisterBtn.onClick.AddListener(delegate { registerClicked(); });
        GoogleBtn.onClick.AddListener(delegate { googleClicked(); });
        FacebookBtn.onClick.AddListener(delegate { facebookClicked(); });
        AnonBtn.onClick.AddListener(delegate { anonClicked(); });
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void signInClicked() {
        sign = true;
        Assert.IsTrue(sign);
    }

    public void registerClicked() {
        register = true;
        Assert.IsTrue(register);
    }

    public void googleClicked() {
        google = true;
        Assert.IsTrue(google);
    }

    public void facebookClicked() {
        facebook = true;
        Assert.IsTrue(facebook);    
    }
    public void anonClicked() {
        anon = true;
        Assert.IsTrue(anon);
    }
}
