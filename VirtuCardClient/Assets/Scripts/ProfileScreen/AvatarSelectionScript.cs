using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using UnityEngine.UI;

public class AvatarSelectionScript : MonoBehaviour
{

    public GameObject avatarPanel;
    public RectTransform playerAvatar;

    public List<RectTransform> AvatarList = new List<RectTransform>();
    public RectTransform avatarTemplate;
    public RectTransform viewWindow;

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

    /* private struct AvatarsToAssignTexture
    {
        public string texturePath;
        public RectTransform AvatarTransform;
    } */

    // Start is called before the first frame update
    void Start()
    {
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

   
    public void onChooseAvatarButtonClick()
    {
        int newAvatarIndex = GetCurrentlySelectedIndex();

        playerAvatar.gameObject.GetComponent<RawImage>().texture = Resources.Load<Texture>("Avatars/Avatar" + (current_index + 1));

        avatarPanel.SetActive(false);
    }

    public void onEditAvatarButtonClick()
    {
        avatarPanel.SetActive(true);
    }

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

        // ------- USE THIS TEMPLATE WHEN A NEW CARD TYPE IS ADDED --------------

        //OtherCard otherCard = images[current_index].gameObject.GetComponent<OtherCard>();
        //if (otherCard != null)
        //{
        //    return otherCard;
        //}

        // -----------------------------------------------------------------------

        throw new Exception("No Currently Selected Card");
    }


}
