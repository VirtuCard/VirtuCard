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

        /// <summary>
        /// This method is for logging in to an account assuming that the account is in the firebase.
        /// If the account is not in the firebase, you will not be able to log in.
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        public static void Login(string email, string password, Action<bool> callback)
        {
            /// checks if the credentials are correct
            auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWith(task =>
            {
                if (task.IsCanceled)
                {
                    //Throw error for cancellation here 
                    Debug.LogError("SignInWithEmailAndPasswordAsync was canceled.");
                    callback(false);
                    return;
                }

                if (task.IsFaulted)
                {
                    //Throw error for other error here
                    Debug.LogError("SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                    callback(false);
                    return;
                }

                // Firebase user has been created.
                firebaseUser = task.Result;

                //Put callback here to return to when done. 
                callback(true);
                Debug.LogFormat("Firebase user logged in successfully: {0} ({1})",
                    firebaseUser.DisplayName, firebaseUser.UserId);
            });
        }


        /// <summary>
        /// This method creates a TEMPORARY testing account. It just creates the account and then immediately deletes it for testing purposes.
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <param name="callback"></param>
        public static void RegisterTestingAccount(String email, String password, Action<bool> callback)
        {
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

                //Put callback here to return to when done. 
                callback(true);
                Debug.LogFormat("Firebase user created successfully: {0} ({1})",
                    firebaseUser.DisplayName, firebaseUser.UserId);
                firebaseUser.DeleteAsync();
            });
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

                User user = new User(username, email, task.Result.UserId);

                //Put callback here to return to when done.
                Debug.LogFormat("Firebase user created successfully: {0} ({1})",
                    firebaseUser.DisplayName, firebaseUser.UserId);
                DatabaseUtils.addUser(user, c =>
                {
                    if (!c)
                    {
                        Debug.Log("Failed to Add into Realtime Database");
                        firebaseUser.DeleteAsync();
                    }

                    callback(c);
                });
            });
        }

        public static void PlayAnonymously(Action<bool> callback)
        {
            if (!FirebaseInit.IsInitialized())
            {
                Debug.LogError("Firebase not initialized!");
                callback(false);
                return;
            }

            AnonymousAuth.CreateAnonymousAccount(auth, c =>
            {
                if (c)
                {
                    firebaseUser = auth.CurrentUser;
                }

                callback(c);
            });
        }

        private static void AuthStateChanged(object sender, System.EventArgs eventArgs)
        {
            if (auth.CurrentUser != firebaseUser)
            {
                bool signedIn = auth.CurrentUser != null && firebaseUser != auth.CurrentUser;
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

        /// <summary>
        /// This method is used to send a given user a password reset email for their account
        /// </summary>
        /// <param name="username"></param>
        /// <param name="email"></param>
        private static void ResetPassword(String username, String email)
        {
            if (username != null)
            {
                auth.SendPasswordResetEmailAsync(email).ContinueWith(task => {
                    if (task.IsCanceled)
                    {
                        Debug.LogError("SendPasswordResetEmailAsync was canceled.");
                        return;
                    }
                    if (task.IsFaulted)
                    {
                        Debug.LogError("SendPasswordResetEmailAsync encountered an error: " + task.Exception);
                        return;
                    }

                    Debug.Log("Password reset email sent successfully.");
                });
            }
        }
    }
}