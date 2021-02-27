using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Firebase.Auth;
using FirebaseScripts;
using UnityEngine;
using UnityEngine.UI;

public class LandingPageManager : MonoBehaviour
{
    // this dropdown should be populated with all the game names
    public Dropdown gameChoiceDropdown;

    // this is the UI toggle component that allows the host to join or not
    public Toggle canHostJoinToggle;

    // Start is called before the first frame update
    void Start()
    {
        // add event listener for the value changing
        canHostJoinToggle.onValueChanged.AddListener(delegate {
            CanHostJoinToggleValueChanged(canHostJoinToggle.isOn);
        });

        gameChoiceDropdown.options.Add(new Dropdown.OptionData("Freeplay"));
        gameChoiceDropdown.options.Add(new Dropdown.OptionData("Uno"));
        gameChoiceDropdown.options.Add(new Dropdown.OptionData("Go Fish"));
        //Console.WriteLine("HELLLLLOOOOO");



        FirebaseInit.InitializeFirebase(task =>
        {
            AuthUser.RegisterAccount("testing", "hello@ohio.edu", "topsecret",
                task => { Debug.Log("Hi " + task); 
                if (AuthUser.Login("testing", "topsecret")) {
                        Console.WriteLine("True");
                    }
                    else {
                        Console.WriteLine("False");
                    }
                });
        });

        FirebaseInit.InitializeFirebase(task =>
        {
            AuthUser.RegisterAccount("juniebear", "seodongjune00@gmail.com", "chicken",
                task => { 
                    if (AuthUser.Login("juniebear27", "chicken")) {
                        Console.WriteLine("True");
                    }
                    else {
                        Console.WriteLine("False");
                    }
                 });
        });
    }

    // Update is called once per frame
    void Update()
    {
    }

    /// <summary>
    /// This is the callback for when the canHostJoinToggle's value is changed.
    /// It sets the corresponding boolean in the HostData class
    /// </summary>
    /// <param name="state"></param>
    private void CanHostJoinToggleValueChanged(bool state)
    {
        HostData.canHostJoinGame = state;
    }
}