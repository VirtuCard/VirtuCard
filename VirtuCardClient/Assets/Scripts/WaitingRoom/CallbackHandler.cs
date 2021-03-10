using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace WaitingRoom
{
    public class CallbackHandler : MonoBehaviourPunCallbacks
    {
        public Button exitGame;
        
        private void Start()
        {
            PhotonNetwork.AddCallbackTarget(this);
            exitGame.onClick.AddListener(ONExitButtonClick);
        }

        public void ONExitButtonClick()
        {
            PhotonNetwork.LeaveRoom();
            SceneManager.LoadScene(SceneNames.JoinGamePage);
        }

        public void ChangeGameScene()
        {
            Debug.Log("TAP");
            SceneManager.LoadScene(SceneNames.GameScreen);
 
        }

    }
}