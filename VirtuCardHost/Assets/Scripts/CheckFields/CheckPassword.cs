using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using System.IO;
using System.Linq;

public class CheckPassword : MonoBehaviour
{
   private string fixedPassword;
   public GameObject textDisplay;
   public InputField passwordInputField;
   
   public void updateText()
    { 
        fixedPassword = passwordInputField.text;
        if (fixedPassword == "")
        {
            textDisplay.GetComponent<Text>().text = "Password cannot be blank";
        }
        else if (fixedPassword.Length < 6)
        {
            textDisplay.GetComponent<Text>().text = "Please use at least six characters.";
        }
        else if (!fixedPassword.Any(char.IsUpper) && !fixedPassword.Any(char.IsDigit))
        {
            textDisplay.GetComponent<Text>().text = "Add an upper case digit and a number.";
        }
        else if (!fixedPassword.Any(char.IsDigit))
        {
            textDisplay.GetComponent<Text>().text = "Add at least one number.";
        }
        else if (!fixedPassword.Any(char.IsUpper))
        {
            textDisplay.GetComponent<Text>().text = "Add at least one upper case letter.";
        }
        else
        {
            Debug.Log("Password is valid");
            textDisplay.GetComponent<Text>().text = "";
            textDisplay.GetComponent<Text>().enabled = false;
        }
   }
}