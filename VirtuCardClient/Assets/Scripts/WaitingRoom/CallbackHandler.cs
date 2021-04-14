using System;
using ExitGames.Client.Photon;
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

        private void OnSignalSent(EventData photonEvent)
        {
            if (photonEvent.Code == (int)NetworkEventCodes.ExitGame) //Exit Game
            {
                PhotonNetwork.LeaveRoom();
                SceneManager.LoadScene(SceneNames.JoinGamePage);
            }
        }

        public override void OnEnable()
        {
            PhotonNetwork.NetworkingClient.EventReceived += OnSignalSent;
        }

        public override void OnDisable()
        {
            PhotonNetwork.NetworkingClient.EventReceived -= OnSignalSent;
        }

        public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
        {
            ClientData.FromHashtable(propertiesThatChanged);
        }
    }
}