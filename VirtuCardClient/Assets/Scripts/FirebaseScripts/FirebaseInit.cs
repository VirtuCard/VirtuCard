using System;
using System.IO;
using Firebase;
using Firebase.Auth;
using UnityEngine;

namespace FirebaseScripts
{
    public class FirebaseInit : MonoBehaviour
    {
        static bool isConfigured = false;
        static FirebaseApp app;

        //Should be run during start of app
        public static void InitializeFirebase(Action<bool> action)
        {
            if (isConfigured)
            {
                action(true);
                return;
            }

            Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
            {
                var dependencyStatus = task.Result;
                if (dependencyStatus == Firebase.DependencyStatus.Available)
                {
                    // Create and hold a reference to your FirebaseApp,
                    // where app is a Firebase.FirebaseApp property of your application class.
                    string json = File.ReadAllText("Assets/google-services.json");
                    app = Firebase.FirebaseApp.Create((AppOptions.LoadFromJsonConfig(json)));
                    AuthUser.SetAuth(FirebaseAuth.GetAuth(app));
                    DatabaseUtils.setApp(app);

                    // To test if this works
                    // User.RegisterAccount("test@purdue.edu", "hidkasidjoiajci!", b => { print("HI"); });

                    // Set a flag here to indicate whether Firebase is ready to use by your app.
                    isConfigured = true;
                }
                else
                {
                    UnityEngine.Debug.LogError($"Could not resolve all Firebase dependencies: {dependencyStatus}");
                    // Firebase Unity SDK is not safe to use here.
                }

                action(true);
            });
        }

        public static bool IsInitialized()
        {
            return isConfigured;
        }


        // Start is called before the first frame update
        // Not sure if this needs to exhibit MonoBehaviour?
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
        }
    }
}