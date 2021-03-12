using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using System.Collections;
using System.IO;

public class CheckEmail : MonoBehaviour
{

   public string emailInput;
   public GameObject inputField;
   public GameObject textDisplay;
   public Button signUpButton;

   
   public void updateText()
   {
       emailInput = inputField.GetComponent<Text>().text;
       if (!emailInput.Contains("@") || !emailInput.Contains("."))
       {
           textDisplay.GetComponent<Text>().text = "Please input a valid email.";
           signUpButton.enabled = false;
       }
       else
       {
           textDisplay.GetComponent<Text>().text = "";
           signUpButton.enabled = true;
       }
       if (emailInput == "")
       {
          textDisplay.GetComponent<Text>().text = "";
          signUpButton.enabled = false;
       }
   }
}
