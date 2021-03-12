using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using System.Collections;
using System.IO;
using System.Linq;

public class CheckConfirm : MonoBehaviour
{
   public string toCheck;
   public string confirmField;
   public GameObject textDisplay;
   public InputField passwordInput;
   public InputField checkPasswordInput;
   public Button signUpButton;
   
   public void updateText()
   {

       toCheck = passwordInput.text;
       confirmField = checkPasswordInput.text;

       if (toCheck != confirmField && confirmField != "")
       {
         textDisplay.GetComponent<Text>().text = "Make sure your passwords match!";
         signUpButton.enabled = false;
       }
       else if (toCheck == confirmField)
       {
          textDisplay.GetComponent<Text>().text = "";
          signUpButton.enabled = true;
       }
       
       if (toCheck == "" || confirmField == "")
       {
          textDisplay.GetComponent<Text>().text = "";
          signUpButton.enabled = false;
       }
       
   }
}
