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

   public string passwordInput;
   public GameObject inputField;
   public GameObject textDisplay;
   
   public void updateText()
   {

      passwordInput = inputField.GetComponent<Text>().text;
      Debug.Log(passwordInput);
       if (passwordInput.Length < 6 || !passwordInput.Any(char.IsUpper))
       {
         textDisplay.GetComponent<Text>().text = "Please use at least six characters!";
       }
       else if (passwordInput.Length > 6 && passwordInput.Any(char.IsUpper)
       && passwordInput.Any(char.IsDigit))
       {
          textDisplay.GetComponent<Text>().text = "";
       }
       else
       {
           textDisplay.GetComponent<Text>().text = "";
       }
       
       if (passwordInput == "")
       {
          textDisplay.GetComponent<Text>().text = "";
       }
   }
}
