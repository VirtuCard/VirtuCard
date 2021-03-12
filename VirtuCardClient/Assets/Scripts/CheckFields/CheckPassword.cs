using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using System.Collections;
using System.IO;
using System.Linq;

public class CheckPassword : MonoBehaviour
{
   public string fixedPassword;
   public GameObject textDisplay;
   public InputField passwordInputField;
   public Button signUpButton;

   
   public void updateText()
   {

      //fixedPassword = inputField.GetComponent<Text>().text;

      fixedPassword = passwordInputField.text;

       if (fixedPassword.Length < 6)
       {
         textDisplay.GetComponent<Text>().text = "Please use at least six characters.";
         signUpButton.enabled = false;
       }
       else if (!fixedPassword.Any(char.IsUpper) && !fixedPassword.Any(char.IsDigit))
       {
          textDisplay.GetComponent<Text>().text = "Add an upper case digit and a number.";
          signUpButton.enabled = false;
       }
       else if (!fixedPassword.Any(char.IsDigit))
       {
           textDisplay.GetComponent<Text>().text = "Add at least one number.";
           signUpButton.enabled = false;
       }
       else if (!fixedPassword.Any(char.IsUpper))
       {
          textDisplay.GetComponent<Text>().text = "Add at least one upper case letter.";
          signUpButton.enabled = false;
       }
       else
       {
          textDisplay.GetComponent<Text>().text = "";
          signUpButton.enabled = true;
       }

       if (fixedPassword == "")
       {
          textDisplay.GetComponent<Text>().text = "";
          signUpButton.enabled = false;
       }

   }
}
