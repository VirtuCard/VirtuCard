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

        public void ConnectClientClicked()
        {
            joinCode = inputField.GetComponent<Text>().text;
            Debug.Log("Join Code is: " + joinCode);
  
        }
    }
}
