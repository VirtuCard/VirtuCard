using System;
using System.Threading.Tasks;
using Firebase;
using Firebase.Database;
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
                            namesRef.Child(username).Child("userId").SetValueAsync(userId).ContinueWith(copy =>
                            {
                                if (copy.IsFaulted)
                                {
                                    Debug.LogError("Failed to Add User");
                                    callback(false);
                                }
                                else if (copy.IsCompleted)
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
        
        //Works for non-email related details
        /// <summary>
        /// Call this whenever changes needed to be made for statistics
        /// </summary>
        /// <param name="details"></param>
        /// <param name="callback"></param>
        public static void updateUser(User details, Action<bool> callback)
        {
            getUser(details.UserId, s =>
            {
                if (s != null)
                {
                    DatabaseReference usersRef = realtime.GetReference("users/");
                    usersRef.Child(details.UserId).SetRawJsonValueAsync(details.ToString()).ContinueWith(task =>
                    {
                        if (task.IsFaulted)
                        {
                            Debug.LogError("Failed to Update Details");
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
                    return;
                }

                if (task.Result.Value == null)
                {
                    callback(null);
                    return;
                }

                callback((string) task.Result.Value);
            });
        }

        public static async Task<string> searchUsername(string username)
        {

            DatabaseReference namesRef = realtime.GetReference("usernames/" + username).Child("userId");
            var foundEntry = await namesRef.GetValueAsync();
            if (foundEntry == null || foundEntry.Value == null)
            {
                return null;
            }
            return (string)foundEntry.Value;
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

        /// <summary>
        /// Removes a user from the realtime database that has a specific id
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="callback"></param>
        public static void RemoveUserWithID(string userId, Action<bool> callback)
        {
            DatabaseReference usersRef = realtime.GetReference("users/");
            DatabaseReference usernamesRef = realtime.GetReference("usernames/");
                    
            usersRef.Child(userId).Child("Username").GetValueAsync().ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogError("Failed to get username value from Firebase Database");
                }
                else if (task.IsCompleted)
                {
                    usernamesRef.Child(task.Result.Value.ToString()).RemoveValueAsync().ContinueWith(task =>
                    {
                        if (task.IsFaulted)
                        {
                            Debug.LogError("Failed to delete username from usernames/ in Firebase Database");
                            callback(false);
                        }
                        else if (task.IsCompleted)
                        {
                            usersRef.Child(userId).RemoveValueAsync().ContinueWith(task =>
                            {
                                if (task.IsFaulted)
                                {
                                    Debug.LogError("Failed to delete userId from users/ in Firebase Database");
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
        /// removes a user from the realtime database that has a specific username
        /// </summary>
        /// <param name="username"></param>
        /// <param name="callback"></param>
        public static void RemoveUserWithName(string username, Action<bool> callback)
        {
            DatabaseReference usersRef = realtime.GetReference("users/");
            DatabaseReference usernamesRef = realtime.GetReference("usernames/");

            usernamesRef.Child(username).Child("userId").GetValueAsync().ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogError("Failed to get username value from Firebase Database");
                }
                else if (task.IsCompleted)
                {
                    usersRef.Child(task.Result.Value.ToString()).RemoveValueAsync().ContinueWith(task =>
                    {
                        if (task.IsFaulted)
                        {
                            Debug.LogError("Failed to delete usuerId from users/ in Firebase Database");
                            callback(false);
                        }
                        else if (task.IsCompleted)
                        {
                            usernamesRef.Child(username).RemoveValueAsync().ContinueWith(task =>
                            {
                                if (task.IsFaulted)
                                {
                                    Debug.LogError("Failed to delete username from usernames/ in Firebase Database");
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


        public static void GetUserFromName(string name, Action<User> callback)
        {
            DatabaseReference usernamesRef = realtime.GetReference("usernames/");

            usernamesRef.Child(name).Child("userId").GetValueAsync().ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogError("Failed to get username value from Firebase Database");
                }
                else if (task.IsCompleted)
                {
                    string id = task.Result.Value.ToString();
                    //callback(id);

                    DatabaseReference usersRef = realtime.GetReference("users/" + id);

                    usersRef.GetValueAsync().ContinueWith(task =>
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
                            User userFound = new User(values);
                            callback(userFound);
                        }
                    });
                }
            });
            /*

            usernamesRef.Child(name).Child("userId").GetValueAsync().ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogError("Failed to get username value from Firebase Database");
                }
                else if (task.IsCompleted)
                {
                    string userId = task.Result.Value.ToString();
                    return userId;
                    usersRef.Child(userId).GetValueAsync().;
                    
                    
                    
                    .RemoveValueAsync().ContinueWith(task =>
                    {
                        if (task.IsFaulted)
                        {
                            Debug.LogError("Failed to delete usuerId from users/ in Firebase Database");
                            callback(false);
                        }
                        else if (task.IsCompleted)
                        {
                            usernamesRef.Child(username).RemoveValueAsync().ContinueWith(task =>
                            {
                                if (task.IsFaulted)
                                {
                                    Debug.LogError("Failed to delete username from usernames/ in Firebase Database");
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
            });*/
        }
    }
}