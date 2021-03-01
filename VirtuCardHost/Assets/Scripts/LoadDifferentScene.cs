using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadDifferentScene : MonoBehaviour
{
    // scene name that this script changes to when ChangeScene() is called
    public string sceneName;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    } 

    /// <summary>
    /// This method changes to the scene name
    /// </summary>
    /// <param name="sceneName">
    /// This string takes the exact name of the scene to change to
    /// </param>
    public void ChangeScene(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            throw new System.Exception("The attribute sceneName is null in LoadDifferentScene");
        }
        // load the new scene
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }

    /// <summary>
    /// This method changes the scene when it is called.
    /// It changes the scene to the name declared in the sceneName attribute.
    /// (This method is most useful for attaching to GameObjects, use the parametrized version instead when calling from code)
    /// </summary>
    public void ChangeScene()
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            throw new System.Exception("The attribute sceneName is null in LoadDifferentScene");
        }
        // load the new scene
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }
}
