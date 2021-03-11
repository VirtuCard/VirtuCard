using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using System.IO;

public class CheckEmail : MonoBehaviour
{
    private string emailInput;
    public GameObject inputField;
    public GameObject textDisplay;

    // This is updated when the user leaves the email box
    public void updateText()
    {
        emailInput = inputField.GetComponent<Text>().text;
        if (emailInput == "")
        {
            textDisplay.GetComponent<Text>().text = "Email is required";
        }
        if (!emailInput.Contains("@") || !emailInput.Contains("."))
        {
          textDisplay.GetComponent<Text>().text = "Please input a valid email.";
        }
        else
        {
            Debug.Log("Email is valid");
            textDisplay.GetComponent<Text>().text = "";
            textDisplay.GetComponent<Text>().enabled = false;

        }
    }
}
