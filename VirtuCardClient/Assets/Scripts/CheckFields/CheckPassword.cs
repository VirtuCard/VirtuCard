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
   public GameObject password;
   public GameObject inputField;
   public GameObject textDisplay;
   
   public void updateText()
   {

      fixedPassword = inputField.GetComponent<Text>().text;
      
      if (fixedPassword.Length < 6)
      {
         textDisplay.GetComponent<Text>().text = "Please use at least six characters.";
      }
      else 
      {
        textDisplay.GetComponent<Text>().text = "";
      }
      if (fixedPassword == "")
      {
         textDisplay.GetComponent<Text>().text = "";
      }

      /* This is for if we get the password field working. As of now 
      Unity returns astericks when retrieving text from a password text
      field.
       if (fixedPassword.Length < 6)
       {
         textDisplay.GetComponent<Text>().text = "Please use at least six characters.";
       }
       else if (!fixedPassword.Any(char.IsUpper) && !fixedPassword.Any(char.IsDigit))
       {
          textDisplay.GetComponent<Text>().text = "Please use an upper case digit and a number.";
       }
       else if (!fixedPassword.Any(char.IsDigit))
       {
           textDisplay.GetComponent<Text>().text = "Please use at least one number.";
       }
       else if (!fixedPassword.Any(char.IsUpper))
       {
          textDisplay.GetComponent<Text>().text = "Please use at least one upper case letter.";
       }
       else
       {
          textDisplay.GetComponent<Text>().text = "";
       }
      */
   }
}
