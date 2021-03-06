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
        SignInBtn.onClick.AddListener(delegate {
            signInClicked(); 
            Assert.IsTrue(sign);
            
        });
        RegisterBtn.onClick.AddListener(delegate {
            registerClicked();
            Assert.IsTrue(register);
        });
        GoogleBtn.onClick.AddListener(delegate {
            googleClicked();
            Assert.IsTrue(google);
        });
        FacebookBtn.onClick.AddListener(delegate {
            facebookClicked();
            Assert.IsTrue(facebook);
        });
        AnonBtn.onClick.AddListener(delegate {
            anonClicked();
            Assert.IsTrue(anon);
        });
    }

    // Update is called once per frame
    void Update()
    {
        
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
