using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System;
using System.IO;
using GameScreen.GameLogic.Cards;

/// <summary>
/// This class acts as a carousel for the Cards
/// It is based on Shushanta's implementation of a carousel from here https://sushanta1991.blogspot.com/2016/12/how-to-create-carousel-view-with-unity.html
/// </summary>
public class CardMenu : MonoBehaviour
{
    public RectTransform cardTemplate;

    public List<RectTransform> images = new List<RectTransform>();
    public RectTransform viewWindow;

    private bool canSwipe;
    private float imageWidth;
    private float lerpTimer;
    private float lerpPosition;
    private float mousePositionStartX;
    private float mousePositionEndX;
    private float dragAmount;
    private float screenPosition;
    private float lastScreenPosition;

    public float imageSpacing = 10;

    public int swipeThrustHold = 30;

    public int current_index;

    private SpriteRenderer sr;
    private Sprite mySprite;
    private string Path;

    public string backPath;

    private struct CardsToAssignTexture
    {
        public string texturePath;
        public RectTransform cardTransform;
    }

    private List<CardsToAssignTexture> cardsToAssignTextures = new List<CardsToAssignTexture>();

    /// <summary>
    /// Returns the currently selected card or NULL if there are none in the carousel
    /// </summary>
    /// <returns></returns>
    public Card GetCurrentlySelectedCard()
    {
        if (images.Count == 0)
        {
            return null;
        }


        StandardCard stdCard = images[current_index].gameObject.GetComponent<StandardCard>();
        if (stdCard != null)
        {
            return stdCard;
        }

        UnoCard unoCard = images[current_index].gameObject.GetComponent<UnoCard>();
        if (unoCard != null)
        {
            return unoCard;
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
    /// This returns the currently selected index of the card in the carousel
    /// </summary>
    /// <returns></returns>
    public int GetCurrentlySelectedIndex()
    {
        return current_index;
    }

    public bool IsIndexInValidPosition()
    {
        if (current_index >= 0 && current_index < images.Count)
        {
            return true;
        }

        return false;
    }

    public void MoveToValidPosition()
    {
        if (current_index >= images.Count)
        {
            MoveCarouselToIndex(images.Count - 1);
        }
        else if (current_index < 0)
        {
            MoveCarouselToIndex(0);
        }
    }

    public void ResizeAndCompressCards()
    {
    }

    /// <summary>
    /// Adds a card to the rightmost side of the carousel.
    /// It automatically reformats after adding.
    /// </summary>
    /// <param name="newCard"></param>
    /// <param name="whichCardType"></param>
    public void AddCardToCarousel(Card newCard, CardTypes whichCardType)
    {
        RectTransform newImage = Instantiate(cardTemplate, viewWindow);

        Path = "Card UI/";

        if (whichCardType == 0)
        {
            newImage.gameObject.AddComponent<StandardCard>();
            StandardCard cardVals = newImage.gameObject.GetComponent<StandardCard>();
            StandardCardRank rank = ((StandardCard) newCard).GetRank();
            StandardCardSuit suit = ((StandardCard) newCard).GetSuit();
            cardVals.SetRank(rank);
            cardVals.SetSuit(suit);

            Path += suit.ToString();
            Path += "_";
            Path += rank.ToString();


            cardsToAssignTextures.Add(new CardsToAssignTexture {cardTransform = newImage, texturePath = Path});
        }
        else if (whichCardType == CardTypes.UnoCard)
        {
            newImage.gameObject.AddComponent<UnoCard>();
            UnoCard cardVals = newImage.gameObject.GetComponent<UnoCard>();
            UnoCard curr = (UnoCard) newCard;
            cardVals.value = curr.value;
            cardVals.color = curr.color;


            if (curr.value == UnoCardValue.WILD || curr.value == UnoCardValue.PLUS_FOUR)
            {
                Path += curr.value.ToString();
            }
            else
            {
                Path += curr.color.ToString();
                Path += "_";
                Path += curr.value.ToString();
            }


            cardsToAssignTextures.Add(new CardsToAssignTexture {cardTransform = newImage, texturePath = Path});
        }
        // TODO this is where other types of cards would be implemented

        images.Add(newImage);
        ReformatCarousel();
    }


    public void RemoveCardFromCarousel(Card newCard)
    {
        int cardCount = images.Count;
        for (int x = cardCount - 1; x >= 0; x--)
        {
            if (newCard.GetType() == typeof(UnoCard))
            {
                if (images[x].gameObject.GetComponent<UnoCard>().Compare(newCard) == true)
                {
                    GameObject imageToDestroy = images[x].gameObject;
                    // Replacing this:
                    images.RemoveAt(x);
                    // Destroy(imageToDestroy);

                    // With animation that calls Destroy on Exit upon completing
                    string animation = "CardAnimationClientDelete";
                    imageToDestroy.GetComponent<Animator>().Play(animation);

                    //Sets the delay before restructuring the carousel. Currently right after animation completion
                    float delay = imageToDestroy.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length;
                    StartCoroutine(ReformatCarousel(delay, x));

                    // ReformatCarousel();
                    break;
                }
            }
            else if (newCard.GetType() == typeof(StandardCard))
            {
                // if this image is the card we are looking for
                if (images[x].gameObject.GetComponent<StandardCard>().Compare(newCard) == true)
                {
                    GameObject imageToDestroy = images[x].gameObject;
                    // Replacing this:
                    images.RemoveAt(x);
                    // Destroy(imageToDestroy);

                    // With animation that calls Destroy on Exit upon completing
                    string animation = "CardAnimationClientDelete";
                    imageToDestroy.GetComponent<Animator>().Play(animation);

                    //Sets the delay before restructuring the carousel. Currently right after animation completion
                    float delay = imageToDestroy.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length;
                    StartCoroutine(ReformatCarousel(delay, x));

                    // ReformatCarousel();
                    break;
                }
            }
        }
    }

    /// <summary>
    /// IEnumerator version of Reformat Carousel, useful in making the cards before moving around
    /// </summary>
    private IEnumerator ReformatCarousel(float seconds, int index)

    {
        yield return new WaitForSeconds(seconds);
        //images.RemoveAt(index);
        ReformatCarousel();
    }


    /// <summary>
    /// Resets the spacing for the cards.
    /// Call if a new card was added/removed
    /// </summary>
    private void ReformatCarousel()
    {
        for (int x = 1; x < images.Count; x++)
        {
            images[x].anchoredPosition = new Vector2(((imageWidth + imageSpacing) * x), 0);
        }
    }

    // Use this for initialization
    void Start()
    {
        imageWidth = (0.75f) * viewWindow.rect.width;
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
            if (current_index < images.Count)
                OnSwipeComplete();
            else if (current_index == images.Count && dragAmount < 0)
                lerpTimer = 0;
            else if (current_index == images.Count && dragAmount > 0)
                OnSwipeComplete();
        }

        for (int i = 0; i < images.Count; i++)
        {
            images[i].anchoredPosition = new Vector2(screenPosition + ((imageWidth + imageSpacing) * i), 0);
        }

        int cardCountToSet = cardsToAssignTextures.Count;
        for (int x = cardCountToSet - 1; x >= 0; x--)
        {
            cardsToAssignTextures[x].cardTransform.Find("Front").GetComponent<RawImage>().texture =
                Resources.Load<Texture>(cardsToAssignTextures[x].texturePath);
            if (backPath.Length != 0)
            {
                Texture2D tex = null;
                byte[] fileData;
                if (File.Exists(backPath))
                {
                    fileData = File.ReadAllBytes(backPath);
                    tex = new Texture2D(2, 2);
                    tex.LoadImage(fileData); //..this will auto-resize the texture dimensions.
                }

                cardsToAssignTextures[x].cardTransform.Find("Back").GetComponent<RawImage>().texture = tex;
            }
            else
            {
                cardsToAssignTextures[x].cardTransform.Find("Back").GetComponent<RawImage>().texture =
                    Resources.Load<Texture>("Card UI/CardBack");
            }

            cardsToAssignTextures.RemoveAt(x);
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
                if (current_index == images.Count - 1)
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
    /// This method goes to a certain index of the deck
    /// </summary>
    /// <param name="value"></param>
    public void MoveCarouselToIndex(int value)
    {
        if (value < 0 || value >= images.Count)
        {
            // we don't want to move to an undefined area
            return;
        }

        current_index = value;
        lerpTimer = 0;
        lerpPosition = (imageWidth + imageSpacing) * current_index;
    }
}