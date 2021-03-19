using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DefaultNamespace.Game
{
    public class SettingsPanelMethods : MonoBehaviour
    {
        public void DisconnectUser()
        {
            PhotonNetwork.LeaveRoom();
            ClientData.setJoinCode(null);
            SceneManager.LoadScene(SceneNames.JoinGamePage);
        }
    }
}