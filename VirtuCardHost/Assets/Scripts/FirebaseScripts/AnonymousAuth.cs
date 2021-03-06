using System;
using Firebase.Auth;
using UnityEngine;
using Random = System.Random;

namespace FirebaseScripts
{
    public class AnonymousAuth
    {
        public static void CreateAnonymousAccount(FirebaseAuth auth, Action<bool> callback)
        {
            auth.SignInAnonymouslyAsync().ContinueWith(task =>
            {
                if (task.IsCanceled)
                {
                    //Throw error for cancellation here 
                    Debug.LogError("SignInAnonymously was canceled.");
                    callback(false);
                    return;
                }

                if (task.IsFaulted)
                {
                    //Throw error for other error here
                    Debug.LogError("SignInAnonymously encountered an error: " + task.Exception);
                    callback(false);
                    return;
                }

                GenerateFirebaseUsername(username =>
                {
                    User user = new User(username, username.Substring(0, 18), task.Result.UserId,
                        true);
                    DatabaseUtils.addUser(user, c =>
                    {
                        if (!c)
                        {
                            Debug.LogError("Error updating Database!");
                            task.Result.DeleteAsync();
                        }

                        callback(c);
                    });
                });
            });
        }

        private static void GenerateFirebaseUsername(Action<string> returnVal)
        {
            String username = "Anonymous";
            Random rand = new Random();
            for (int i = 0; i < 20; i++)
            {
                username += rand.Next(10);
            }

            DatabaseUtils.findUsername(username, b =>
            {
                if (b != null)
                {
                    Debug.Log("Found Duplicate Username. Creating New Username");
                    GenerateFirebaseUsername(returnVal);
                }
                else
                {
                    returnVal(username);
                }
            });
        }
    }
}