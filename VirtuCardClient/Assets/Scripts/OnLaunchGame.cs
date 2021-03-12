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
            Debug.Log("Connected with: + " +  PhotonNetwork.ConnectUsingSettings());
            FirebaseInit.InitializeFirebase(c => Debug.Log("Returned Init with " + c));
        }
    }
}