using System;
using Firebase;
using Firebase.Database;
using FirebaseScripts;
using UnityEngine;

namespace FirebaseScripts
{
    public static class DatabaseUtils
    {
        private static FirebaseDatabase realtime;

        public static void setApp(FirebaseApp firebaseApp)
        {
            realtime = FirebaseDatabase.GetInstance(firebaseApp);
        }

        public static void addUser(FirebaseScripts.User user, Action<bool> callback)
        {
            string json = user.ToString();
            string userId = user.UserId;
            string username = user.Username;
            DatabaseReference usersRef = realtime.GetReference("users/");
            DatabaseReference namesRef = realtime.GetReference("usernames/");
            findUsername(username, b =>
            {
                if (b != null)
                {
                    Debug.Log("Username already present in Firebase!");
                    callback(false);
                }
                else
                {
                    usersRef.Child(userId).SetRawJsonValueAsync(json).ContinueWith(task =>
                    {
                        if (task.IsFaulted)
                        {
                            Debug.LogError("Failed to Add User");
                            callback(false);
                        }
                        else if (task.IsCompleted)
                        {
                            namesRef.Child(username).Child("userId").SetValueAsync(userId).ContinueWith(task =>
                            {
                                if (task.IsFaulted)
                                {
                                    Debug.LogError("Failed to Add User");
                                    callback(false);
                                }
                                else if (task.IsCompleted)
                                {
                                    callback(true);
                                }
                            });
                        }
                    });
                }
            });
        }

        /// <summary>
        /// Updates a property of the account with the given userId
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="property">The property to be updated("Name", "Email", etc.)</param>
        /// <param name="value">A JSON version of the property (If a string, no work needed, If an array, use JsonConvert.SerializeObject()</param>
        /// <param name="callback"></param>
        public static void updateProperty(string userId, string property, string value, Action<bool> callback)
        {
            getUser(userId, s =>
            {
                if (s != null)
                {
                    DatabaseReference usersRef = realtime.GetReference("users/");
                    usersRef.Child(userId).Child(property).SetRawJsonValueAsync(value).ContinueWith(task =>
                    {
                        if (task.IsFaulted)
                        {
                            Debug.LogError("Failed to Update Name");
                            callback(false);
                        }
                        else if (task.IsCompleted)
                        {
                            callback(true);
                        }
                    });
                }
            });
        }


        /// <summary>
        /// Returns true if username is present. Returns false if not.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="callback"></param>
        public static void findUsername(string username, Action<string> callback)
        {
            DatabaseReference namesRef = realtime.GetReference("usernames/" + username).Child("userId");
            namesRef.GetValueAsync().ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogError("Failed to Connect to Firebase Database");
                    callback(null);
                }

                callback((string) task.Result.Value);
            });
        }

        public static void getUser(string userId, Action<string> callback)
        {
            DatabaseReference usersList = realtime.GetReference("users/" + userId);
            usersList.GetValueAsync().ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogError("Failed to Connect to Firebase Database");
                    callback(null);
                }
                else if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;
                    // We know this will be a dictionary
                    string values = snapshot.GetRawJsonValue();
                    callback(values);
                }
            });
        }
    }

}