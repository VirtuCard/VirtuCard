using UnityEngine;
using UnityEngine.UI;

//using Photon.Pun; eventually

namespace JoinGamePage
{
    public class JoinGameMethod : MonoBehaviour
    {
        /// <summary>
        /// This method grabs the code that the user input
        /// then puts it in to the code to join a Photon room.
        /// <summary>
       
        public string joinCode;
        public GameObject inputField;
        public GameObject textDisplay;

        // This string is for testing purposes, eventually we will use Photon to 
        // connect the user and see if the code input is valid
        public string testCode = "123456";

        public void ConnectClientClicked()
        {
            joinCode = inputField.GetComponent<Text>().text;
            //Debug.Log("Join Code is: " + joinCode);

            if (joinCode == testCode)
            {
                // for local testing on the sprint review this is what we can do
                // this will change the scene to the waiting screen in here

                Debug.Log("Successful code entered.");
                textDisplay.GetComponent<Text>().text = "";
            }
            else
            {
                Debug.Log("Incorrect code");
                textDisplay.GetComponent<Text>().text = "Please input a valid code.";
            }

            if (joinCode == "")
            {
                textDisplay.GetComponent<Text>().text = "";
            }
  
        }
    }
}
