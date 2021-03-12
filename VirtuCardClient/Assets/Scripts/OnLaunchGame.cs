using System;
using FirebaseScripts;
using Photon.Pun;
using UnityEngine;

namespace DefaultNamespace
{
    public class OnLaunchGame : MonoBehaviour
    {
        private void Start()
        {
            FirebaseInit.InitializeFirebase(c => Debug.Log("Returned Init with " + c));
            PhotonNetwork.ConnectUsingSettings();
        }
    }
}