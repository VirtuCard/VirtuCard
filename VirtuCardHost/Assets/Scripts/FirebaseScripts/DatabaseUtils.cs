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
            callback(true);
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