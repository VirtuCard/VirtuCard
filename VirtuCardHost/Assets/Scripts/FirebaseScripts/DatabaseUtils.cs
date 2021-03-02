using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Firebase;
using Firebase.Database;
using Firebase.Firestore;
using UnityEngine;

namespace FirebaseScripts
{
    public class DatabaseUtils
    {
        private static FirebaseDatabase realtime;

        public static void setApp(FirebaseApp firebaseApp)
        {
            realtime = FirebaseDatabase.GetInstance(firebaseApp);
        }

        public static void addUser(User user, Action<bool> callback)
        {
            string json = user.ToString();
            string userId = user.UserId;
            DatabaseReference usersRef = realtime.GetReference("users/");
            usersRef.Child(userId).SetRawJsonValueAsync(json).ContinueWith(task =>
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