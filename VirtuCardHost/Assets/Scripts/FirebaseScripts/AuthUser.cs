﻿using System;
using Firebase.Auth;
using UnityEngine;

namespace FirebaseScripts
{
    public class AuthUser
    {
        private static FirebaseAuth auth;
        private static FirebaseUser firebaseUser;

        public static void SetAuth(FirebaseAuth value)
        {
            auth = value;
            auth.SignOut();
            auth.StateChanged += AuthStateChanged;
            AuthStateChanged(null, null);
        }

        public static void RegisterAccount(String username, String email, String password, Action<bool> callback)
        {
            if (!FirebaseInit.IsInitialized())
            {
                Debug.LogError("Firebase not initialized!");
                callback(false);
                return;
            }

            auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith(task =>
            {
                if (task.IsCanceled)
                {
                    //Throw error for cancellation here 
                    Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
                    callback(false);
                    return;
                }

                if (task.IsFaulted)
                {
                    //Throw error for other error here
                    Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                    callback(false);
                    return;
                }

                // Firebase user has been created.
                firebaseUser = task.Result;
                User user = new User(username, email, firebaseUser.UserId);

                //Put callback here to return to when done.
                Debug.LogFormat("Firebase user created successfully: {0} ({1})",
                    firebaseUser.DisplayName, firebaseUser.UserId);
                DatabaseUtils.addUser(user, callback);
            });
        }

        private static void AuthStateChanged(object sender, System.EventArgs eventArgs)
        {
            if (auth.CurrentUser != firebaseUser)
            {
                bool signedIn = firebaseUser != auth.CurrentUser && auth.CurrentUser != null;
                if (!signedIn && firebaseUser != null)
                {
                    Debug.Log("Signed out " + firebaseUser.UserId);
                }

                firebaseUser = auth.CurrentUser;
                if (signedIn)
                {
                    Debug.Log("Signed in " + firebaseUser.UserId);
                }
            }
        }
    }
}