using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditAvatarButton : MonoBehaviour
{

    public static GameObject avatarPanel;

    // Start is called before the first frame update
    void Start()
    {
        avatarPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void onEditAvatarButtonClick()
    {
        avatarPanel.SetActive(true);
    }

    public static void onChooseAvatarButtonClick()
    {
        avatarPanel.SetActive(false);
    }

}
