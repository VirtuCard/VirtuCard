using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;


public class BackgroundUploadPageManager : MonoBehaviour
{
    public Button bgUploadBtn;

    public void onUploadButtonClicked()
    {
        string path = EditorUtility.OpenFilePanel("Hi", "", "jpg");
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
