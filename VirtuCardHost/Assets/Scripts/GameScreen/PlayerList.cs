using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System;
public class PlayerList : MonoBehaviour
{
    public struct PlayerDisplay
    {
        public RectTransform image;
        public PlayerUITemplate playerUI;
    }

    public RectTransform playerTemplate;

    public List<PlayerDisplay> playerComponents = new List<PlayerDisplay>();
    public RectTransform viewWindow;

    private float imageWidth;

    public float imageSpacing = 10;

    /// <summary>
    /// Adds a player to the rightmost side of the carousel.
    /// It automatically reformats after adding.
    /// </summary>
    public void AddPlayerToCarousel(PlayerInfo newPlayer)
    {
        Debug.Log("Trying to add to carousel");
        RectTransform newImage = Instantiate(playerTemplate, viewWindow);
        newImage.gameObject.SetActive(true);

        PlayerDisplay display = new PlayerDisplay();
        display.image = newImage;
        display.playerUI = newImage.GetComponent<PlayerUITemplate>();
        display.playerUI.SetPlayerName(newPlayer.username);
        display.playerUI.SetScore(newPlayer.score);
        display.playerUI.SetCardCount(newPlayer.cards.GetCardCount());

        playerComponents.Add(display);
        ReformatCarousel();
    }

    /// <summary>
    /// Resets the spacing for the cards.
    /// Call if a new card was added/removed
    /// </summary>
    private void ReformatCarousel()
    {
        for (int x = 1; x < playerComponents.Count; x++)
        {
            playerComponents[x].image.anchoredPosition = new Vector2(((imageWidth + imageSpacing) * x), 0);
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
        for (int i = 0; i < playerComponents.Count; i++)
        {
            playerComponents[i].image.anchoredPosition = new Vector2(((imageWidth + imageSpacing) * i), 0);
        }
    }
}
