using System;
using Firebase.Auth;
using FirebaseScripts;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SignInPage
{
    public class AnonButton : MonoBehaviour
    {
        public bool successful = false;

        public void AttemptCreateAccount()
        {
            FirebaseInit.InitializeFirebase(ret =>
            {
                if (ret)
                {
                    AuthUser.PlayAnonymously(task =>
                    {
                        if (task)
                        {
                            successful = true;
                        }
                        else
                        {
                            Debug.LogError("Failed to Create Guest Account!");
                        }
                    });
                }
                else
                {
                    Debug.LogError("Failed to Create Guest Account!");
                }
            });
        }

        public void Update()
        {
            if (successful)
            {
                SceneManager.LoadScene(SceneNames.JoinGamePage, LoadSceneMode.Single);
            }
        }
    }
}