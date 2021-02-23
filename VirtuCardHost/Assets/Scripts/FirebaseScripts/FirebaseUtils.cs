using System.Collections;
using System.Collections.Generic;
using Firebase;
using UnityEngine;

public class FirebaseUtils : MonoBehaviour
{
    private static bool isConfigured = false;
    private static FirebaseApp app;

    //Should be run during start of app
    public static void initializeFirebase()
    {
        if (isConfigured)
        {
            return;
        }

        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                // Create and hold a reference to your FirebaseApp,
                // where app is a Firebase.FirebaseApp property of your application class.
                app = Firebase.FirebaseApp.DefaultInstance;
                // Set a flag here to indicate whether Firebase is ready to use by your app.
                isConfigured = false;
            }
            else
            {
                UnityEngine.Debug.LogError(System.String.Format(
                    "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                // Firebase Unity SDK is not safe to use here.
            }
        });
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