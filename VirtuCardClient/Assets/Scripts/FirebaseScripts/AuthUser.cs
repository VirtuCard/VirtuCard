using System;
using Firebase;
using Firebase.Auth;
using Photon.Pun;
using UnityEditor.PackageManager;
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
        public static void Login(String email, String password, Action<bool> callback)
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
                //Put callback here to return to when done.
                Firebase.Auth.FirebaseUser newUser = task.Result;
                Debug.LogFormat("User logged in successfully: {0} ({1})",
                    newUser.DisplayName, newUser.UserId);
                DatabaseUtils.getUser(newUser.UserId, s =>
                {
                    User user1 = new User(s);
                    ClientData.UserProfile = user1;
                    PhotonNetwork.NickName = user1.Username;
                    DatabaseUtils.updateUser(user1, b => { Debug.Log("Updated with " + b); });
                    callback(true);
                });
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

                    PhotonNetwork.NickName = username;

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

        public static void DeleteAnonymousAccount(Action<bool> callback)
        {
            if (!FirebaseInit.IsInitialized())
            {
                Debug.LogError("Firebase not initialized!");
                callback(false);
                return;
            }

            var prevUser = auth.CurrentUser;
            DatabaseUtils.getUser(prevUser.UserId, res =>
            {
                User user = new User(res);
                if (user.IsAnonymous)
                {
                    DatabaseUtils.RemoveUserWithID(prevUser.UserId, task =>
                    {
                        prevUser.DeleteAsync().ContinueWith(task =>
                        {
                            if (task.IsFaulted)
                            {
                                callback(false);
                            }

                            if (task.IsCompleted)
                            {
                                callback(true);
                            }
                        });
                    });
                }
                else
                {
                    callback(false);
                }
            });
        }

        public static void AuthStateChanged(object sender, System.EventArgs eventArgs)
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
        /// This unregisters a user's account from the firebase authentication
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <param name="callback"></param>
        public static void UnregisterAccount(string email, string password, Action<bool> callback)
        {
            if (!FirebaseInit.IsInitialized())
            {
                Debug.LogError("Firebase not initialized!");
                callback(false);
                return;
            }

            var prevUser = auth.CurrentUser;
            auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWith(task =>
            {
                var user = auth.CurrentUser;
                user.DeleteAsync().ContinueWith(task =>
                {
                    if (task.IsFaulted)
                    {
                        callback(false);
                    }

                    if (task.IsCompleted)
                    {
                        callback(true);
                    }
                });
            });
        }

        /// This methods sends a confirmation email to the current user after they have registered successfully
        /// </summary>
        public static void SendConfirmationEmail()
        {
            FirebaseUser user = auth.CurrentUser;
            if (user != null)
            {
                user.SendEmailVerificationAsync().ContinueWith(task =>
                {
                    if (task.IsCanceled)
                    {
                        Debug.LogError("SendEmailVerificationAsync was canceled.");
                        return;
                    }

                    if (task.IsFaulted)
                    {
                        Debug.LogError("SendEmailVerificationAsync encountered an error: " + task.Exception);
                        return;
                    }

                    Debug.Log("Email sent successfully.");
                });
            }
        }

        /// <summary>
        /// This method is used to send a given user a password reset email for their account
        /// </summary>
        /// <param name="username"></param>
        /// <param name="email"></param>
        public static void ResetPassword(String email, Action<bool> callback)
        {
            if (!FirebaseInit.IsInitialized())
            {
                Debug.LogError("Firebase not initialized!");
                return;
            }

            if (email != null)
            {
                Debug.Log(email);
                auth.SendPasswordResetEmailAsync(email).ContinueWith(task =>
                {
                    if (task.IsCanceled)
                    {
                        Debug.LogError("SendPasswordResetEmailAsync was canceled.");
                        callback(false);
                        return;
                    }

                    if (task.IsFaulted)
                    {
                        Debug.LogError("SendPasswordResetEmailAsync encountered an error: " + task.Exception);
                        callback(false);
                        return;
                    }

                    Debug.Log("Password reset email sent successfully.");
                    callback(true);
                });
            }
        }

        public static string GetUserID()
        {
            return auth.CurrentUser.UserId;
        }

        public static void Logout()
        {
            if (auth.CurrentUser != null)
            {
                auth.SignOut();
                // Null check required for account deletion cases.
            }
        }

        public static void FacebookLogin(String accessToken, Action<int> callback)
            //returns -1, 1 or 2 for incorrect, new account and old account.
        {
            var credential = FacebookAuthProvider.GetCredential(accessToken);
            Debug.Log("Retrieved Credential");
            auth.SignInWithCredentialAsync(credential).ContinueWith(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.LogError("SignInWithCredentialAsync was canceled.");
                    callback(-1);
                    return;
                }

                if (task.IsFaulted)
                {
                    Debug.LogError("SignInWithCredentialAsync encountered an error: " + task.Exception);
                    callback(-1);
                    return;
                }

                Debug.Log("Attempt Login: " + task.Result);
                firebaseUser = task.Result;
                Debug.LogFormat("User signed in successfully: {0} ({1})",
                    firebaseUser.DisplayName, firebaseUser.UserId);
                DatabaseUtils.getUser(firebaseUser.UserId, ret =>
                {
                    if (ret == null)
                    {
                        Debug.Log("New Account");
                        callback(1);
                    }
                    else
                    {
                        User user = new User(ret);
                        PhotonNetwork.NickName = user.Username;
                        callback(2);
                    }
                });
            });
        }

        public static void SignInWithGoogle(string idToken, Action<int> callback)
        {
            Credential credential = GoogleAuthProvider.GetCredential(idToken, null);
            Debug.Log("iSValid?" + (credential.IsValid()));

            auth.SignInWithCredentialAsync(credential).ContinueWith(task =>
            {
                Debug.Log("HDFSAJKDHASDjklohfsDVjkhl;vbfsjkhl;vbfsdjh");
                if (task.IsCanceled)
                {
                    Debug.LogError("SignInWithCredentialAsync was canceled.");
                    callback(-1);
                    return;
                }

                if (task.IsFaulted)
                {
                    Debug.LogError("SignInWithCredentialAsync encountered an error: " + task.Exception);
                    callback(-1);
                    return;
                }

                Debug.Log("Attempt Login: " + task.Result);
                firebaseUser = task.Result;
                Debug.LogFormat("User signed in successfully: {0} ({1})",
                    firebaseUser.DisplayName, firebaseUser.UserId);
                DatabaseUtils.getUser(firebaseUser.UserId, ret =>
                {
                    if (ret == null)
                    {
                        Debug.Log("New Account");
                        callback(1);
                    }
                    else
                    {
                        User user = new User(ret);
                        PhotonNetwork.NickName = user.Username;
                        callback(2);
                    }
                });
            });
        }
    }
}