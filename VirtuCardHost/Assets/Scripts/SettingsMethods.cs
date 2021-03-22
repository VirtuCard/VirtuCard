using System.Collections;
using System.Collections.Generic;
using FirebaseScripts;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SettingsMethods : MonoBehaviour
{
    private int successful = 0;

    // Update is called once per frame
    void Update()
    {
        if (successful == 1)
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        if (successful == 2)
        {
            SceneManager.LoadScene(SceneNames.WelcomePage);
        }

        successful = 0;
    }

    public void OnExitGameButtonClick()
    {
        successful = 1;
    }

    public void OnLogoutButtonClick()
    {
        HostData.UserProfile = null;
        AuthUser.Logout();
        successful = 2;
    }
}
