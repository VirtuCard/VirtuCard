using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using UnityEngine.UI;
using FirebaseScripts;

public class AvatarSelectionScript : MonoBehaviour
{
    public GameObject avatarPanel;
    public RectTransform playerAvatar;

    public List<RectTransform> AvatarList = new List<RectTransform>();
    public RectTransform avatarTemplate;
    public RectTransform viewWindow;

    public int success = 0;

    private bool canSwipe;
    private float lerpTimer;
    private float lerpPosition;
    private float mousePositionStartX;
    private float mousePositionEndX;
    private float dragAmount;
    private float screenPosition;
    private float lastScreenPosition;

    int current_index;
    public int swipeThrustHold = 30;

    private float imageWidth;
    private float imageHeight;
    public float imageSpacing = 10;

    public int numAvatars = 6;

    //Custom Avatar Fields
    [Header("Custom Avatar Fields")]
    string filePath;
    private CardMenu cardMenu;

    [Header("AvatarButton for Disabling")]
    //AvatarButton
    public Button EditAvatarButton;
    public GameObject errorPanel;
    
    /* private struct AvatarsToAssignTexture
    {
        public string texturePath;
        public RectTransform AvatarTransform;
    } */

    // Start is called before the first frame update
    void Start()
    {
        errorPanel.SetActive(false);
           
        if (ClientData.ImageData != null)
        {
            Texture2D t = new Texture2D(2, 2);
            t.LoadImage(ClientData.ImageData);
            t.Apply();
            playerAvatar.gameObject.GetComponent<RawImage>().texture = t;
            Debug.Log("Height:" + t.height);
        }

        avatarPanel.SetActive(false);
        imageWidth = (0.75f) * viewWindow.rect.width;
        imageHeight = (0.75f) * viewWindow.rect.height;
    }

    // Update is called once per frame
    void Update()
    {
        lerpTimer += Time.deltaTime;

        if (lerpTimer < 0.333f)
        {
            screenPosition = Mathf.Lerp(lastScreenPosition, lerpPosition * -1, lerpTimer * 3);
            lastScreenPosition = screenPosition;
        }

        RectTransform transform = this.gameObject.GetComponent<RectTransform>();

        if (Input.GetMouseButtonDown(0) &&
            Input.mousePosition.y <= transform.position.y + (transform.rect.height / 2) &&
            Input.mousePosition.y >= transform.position.y - (transform.rect.height / 2))
        {
            canSwipe = true;
            mousePositionStartX = Input.mousePosition.x;
        }


        if (Input.GetMouseButton(0))
        {
            if (canSwipe)
            {
                mousePositionEndX = Input.mousePosition.x;
                dragAmount = mousePositionEndX - mousePositionStartX;
                screenPosition = lastScreenPosition + dragAmount;
            }
        }

        if (Mathf.Abs(dragAmount) > swipeThrustHold && canSwipe)
        {
            canSwipe = false;
            lastScreenPosition = screenPosition;
            if (current_index < AvatarList.Count)
                OnSwipeComplete();
            else if (current_index == AvatarList.Count && dragAmount < 0)
                lerpTimer = 0;
            else if (current_index == AvatarList.Count && dragAmount > 0)
                OnSwipeComplete();
        }

        for (int i = 0; i < AvatarList.Count; i++)
        {
            AvatarList[i].anchoredPosition = new Vector2(screenPosition + ((imageWidth + imageSpacing) * i), 0);
        }
    }

    /// <summary>
    /// Method to add all the Avatars to the Carousel
    /// </summary>
    void addAvatarsToCarousel()
    {
        Debug.Log("Loading Avatars into Carousel");
        for (int i = 0; i < numAvatars; i++)
        {
            RectTransform Avatar = Instantiate(avatarTemplate, viewWindow);
            Avatar.gameObject.SetActive(true);

            AvatarList.Add(Avatar);
            ReformatCarousel();
        }
    }

    /// <summary>
    /// Resets the spacing for the cards.
    /// Call if a new card was added/removed
    /// </summary>
    private void ReformatCarousel()
    {
        for (int x = 1; x < AvatarList.Count; x++)
        {
            AvatarList[x].anchoredPosition = new Vector2(((imageWidth + imageSpacing) * x), 0);
        }
    }

    /// <summary>
    /// Function to handle swipe functionality on Card Carousel
    /// </summary>
    private void OnSwipeComplete()
    {
        lastScreenPosition = screenPosition;

        if (dragAmount > 0)
        {
            if (dragAmount >= swipeThrustHold)
            {
                if (current_index == 0)
                {
                    lerpTimer = 0;
                    lerpPosition = 0;
                }
                else
                {
                    current_index--;
                    lerpTimer = 0;
                    if (current_index < 0)
                        current_index = 0;
                    lerpPosition = (imageWidth + imageSpacing) * current_index;
                }
            }
            else
            {
                lerpTimer = 0;
            }
        }
        else if (dragAmount < 0)
        {
            if (Mathf.Abs(dragAmount) >= swipeThrustHold)
            {
                if (current_index == AvatarList.Count - 1)
                {
                    lerpTimer = 0;
                    lerpPosition = (imageWidth + imageSpacing) * current_index;
                }
                else
                {
                    lerpTimer = 0;
                    current_index++;
                    lerpPosition = (imageWidth + imageSpacing) * current_index;
                }
            }
            else
            {
                lerpTimer = 0;
            }
        }

        dragAmount = 0;
    }

    /// <summary>
    /// Method to get the currently selected Avatar
    /// </summary>
    /// <returns>The texture of the Avatar the player is looking at</returns>
    public RectTransform GetCurrentlySelectedAvatar()
    {
        if (AvatarList.Count == 0)
        {
            return null;
        }

        RectTransform temp_img = AvatarList[current_index].gameObject.GetComponent<RectTransform>();
        Debug.Log("Current_Index: " + current_index);
        if (temp_img != null)
        {
            return temp_img;
        }

        // ------- USE THIS TEMPLATE WHEN A NEW CARD TYPE IS ADDED --------------

        //OtherCard otherCard = images[current_index].gameObject.GetComponent<OtherCard>();
        //if (otherCard != null)
        //{
        //    return otherCard;
        //}

        // -----------------------------------------------------------------------

        throw new Exception("No Currently Selected Card");
    }

    /// <summary>
    /// Method to handle the Choose Avatar button functionality
    /// that lets users choose from preset Avatars
    /// </summary>
    public void onChooseAvatarButtonClick()
    {
        int newAvatarIndex = GetCurrentlySelectedIndex();

        string imageName = "Avatar" + (current_index + 1) + ".png";
        Texture t = Resources.Load<Texture>("Avatars/Avatar" + (current_index + 1));
        playerAvatar.gameObject.GetComponent<RawImage>().texture = t;

        //Reading in Image and obtaining base64 string

        byte[] imageBytes = ((Texture2D) t).EncodeToPNG();
        ClientData.ImageData = imageBytes;

        ClientData.UserProfile.Avatar = imageName;
        DatabaseUtils.updateUser(ClientData.UserProfile, b => { Debug.Log("Updated image info"); });

        ImageStorage.uploadImage(imageName, imageBytes, b => { Debug.Log("Uploaded with " + b); });
        avatarPanel.SetActive(false);
    }

    /// <summary>
    /// Method to activate the Edit Avatar panel
    /// </summary>
    public void onEditAvatarButtonClick()
    {
        if (ClientData.UserProfile.IsAnonymous)
        {
            
            errorPanel.SetActive(true);
        }
        else
        {
            avatarPanel.SetActive(true);
        }
    }

    /// <summary>
    /// Method to retrieve the index of the card the player is looking
    /// at in the Avatar presets carousel.
    /// </summary>
    /// <returns>The index of the Avatar the player has selected</returns>
    public int GetCurrentlySelectedIndex()
    {
        if (AvatarList.Count == 0)
        {
            Debug.Log("There are no Avatars in the list.");
            return -1;
        }

        RectTransform temp_img = AvatarList[current_index].gameObject.GetComponent<RectTransform>();
        Debug.Log("Current_Index: " + current_index);
        if (temp_img != null)
        {
            return current_index;
        }


        throw new Exception("No Currently Selected Card");
    }

    /// <summary>
    /// Method to facilitate Custom Avatar selection button
    /// </summary>
    public void onCustomAvatarClick()
    {
        filePath = "";

        Debug.Log(filePath);
    
        NativeGallery.Permission permission = NativeGallery.GetImageFromGallery((path) =>
        {
            Debug.Log("Image path: " + path);
            if (path != null)
            {
                filePath = path;
                UpdateAvatarImage();
            }
        }, "Select a custom Avatar image", "image/*");
        Debug.Log("Permission result: " + permission);
    }

    /// <summary>
    /// Method to update image after Custom Avatar is selected
    /// </summary>
    private void UpdateAvatarImage()
    {
        if (filePath.Length != 0)
        {
            Texture2D tex = null;
            byte[] fileData;
            if (File.Exists(filePath))
            {
                fileData = File.ReadAllBytes(filePath);
                
                tex = new Texture2D(2, 2);
                tex.LoadImage(fileData); //..this will auto-resize the texture dimensions.
            }

            //Loading in image File
            playerAvatar.gameObject.GetComponent<RawImage>().texture = tex;

            //Uploading File to Firebase
            byte[] imageBytes = ((Texture2D) tex).EncodeToPNG();
            ClientData.ImageData = imageBytes;

            ClientData.UserProfile.Avatar = ClientData.UserProfile.Username;
            DatabaseUtils.updateUser(ClientData.UserProfile, b => { Debug.Log("Updated image info"); });

            ImageStorage.uploadImage(ClientData.UserProfile.Username, imageBytes, b => { Debug.Log("Uploaded with " + b); });

            //Deactivating panel
            avatarPanel.SetActive(false);
        }
    }

    /// <summary>
    /// Method to handle close button click
    /// </summary>
    public void onCloseButtonClick()
    {
        avatarPanel.SetActive(false);
    }

   
}