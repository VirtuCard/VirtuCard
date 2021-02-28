using System;
using Firebase.Auth;
using FirebaseScripts;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SignInPage
{
    public class AnonButton : MonoBehaviour
    {
        private void Start()
        {
            FirebaseInit.InitializeFirebase(ret => AuthUser.RegisterAccount("MrBrightside", "illinois@indiana.edu",
                "password!",
                cb => { Debug.Log("Hi!" + cb); }));
        }

        public void AttemptCreateAccount()
        {
            Debug.Log("Hi");
            FirebaseInit.InitializeFirebase(ret =>
            {
                if (ret)
                {
                    AuthUser.PlayAnonymously(task =>
                    {
                        if (task)
                        {
                            SceneManager.LoadScene(SceneNames.JoinGamePage, LoadSceneMode.Single);
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
    }
}