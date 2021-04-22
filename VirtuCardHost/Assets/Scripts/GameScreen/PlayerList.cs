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

    public static List<PlayerDisplay> playerComponents = new List<PlayerDisplay>();
    public RectTransform viewWindow;

    private float imageWidth;

    public float imageSpacing = 10;

    public static List<PlayerDisplay> GetPlayerList()
    {
        return playerComponents;
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

        UpdateUI();
    }

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
    /// Removes a player from the carousel
    /// </summary>
    /// <param name="username">Player to remove</param>
    public void RemovePlayerFromCarousel(string username)
    {
        for (int x = 0; x < playerComponents.Count; x++)
        {
            if (playerComponents[x].playerUI.GetPlayerName().Equals(username))
            {
                GameObject imageToDestroy = playerComponents[x].image.gameObject;
                playerComponents.RemoveAt(x);
                Destroy(imageToDestroy);

                ReformatCarousel();
                return;
            }
        }
    }

    /// <summary>
    /// Resets the spacing for the cards.
    /// Call if a new card was added/removed
    /// </summary>
    public void ReformatCarousel()
    {
        for (int x = 1; x < playerComponents.Count; x++)
        {
            try
            {
                playerComponents[x].image.anchoredPosition = new Vector2(((imageWidth + imageSpacing) * x), 0);
            } catch { /* empty catch block */ };
        }
    }

    /// <summary>
    /// Refreshes the UI for each player with their current score and card count
    /// </summary>
    public void UpdateUI()
    {
        // remove disconnected players
        List<PlayerDisplay> disconnectedPlayers = GetPlayersThatLeft();
        foreach (var player in disconnectedPlayers)
        {
            RemovePlayerFromCarousel(player.playerUI.GetPlayerName());
        }

        List<PlayerInfo> actualPlayerList = HostData.GetGame().GetAllPlayers();

        if (HostData.GetGame().GetGameName().Equals("War"))
        {
            int p1Unplayed = HostData.GetGame().GetDeck(DeckChoices.PONEUNPLAYED).GetCardCount();
            int p1Played = HostData.GetGame().GetDeck(DeckChoices.PONEPLAYED).GetCardCount();
            int p2Unplayed = HostData.GetGame().GetDeck(DeckChoices.PTWOUNPLAYED).GetCardCount();
            int p2Played = HostData.GetGame().GetDeck(DeckChoices.PTWOPLAYED).GetCardCount();

            int p1TotalCards = p1Unplayed + p1Played;
            int p2TotalCards = p2Unplayed + p2Played;

            foreach (var player in actualPlayerList)
            {
                var playerComp = GetDisplayFromUsername(player.username);
                if (player.username.Equals(War.p1Name))
                {
                    playerComp.playerUI.SetCardCount(p1TotalCards);
                    playerComp.playerUI.SetScore(p1TotalCards);
                }
                else
                {
                    playerComp.playerUI.SetCardCount(p2TotalCards);
                    playerComp.playerUI.SetScore(p2TotalCards);
                }
            }
        }
        else
        {
            foreach (var player in actualPlayerList)
            {
                var playerComp = GetDisplayFromUsername(player.username);
                playerComp.playerUI.SetCardCount(player.cards.GetCardCount());
                playerComp.playerUI.SetScore(player.score);
            }
        }
    }

    /// <summary>
    /// Returns all the PlayerDisplays of users that have been disconnected
    /// </summary>
    /// <returns></returns>
    private List<PlayerDisplay> GetPlayersThatLeft()
    {
        List<PlayerDisplay> disconnectedPlayers = new List<PlayerDisplay>();

        List<PlayerInfo> actualPlayerList = HostData.GetGame().GetAllPlayers();
        foreach (var display in playerComponents)
        {
            bool isConnected = false;
            foreach (var actualPlayer in actualPlayerList)
            {
                if (display.playerUI.GetPlayerName().Equals(actualPlayer.username))
                {
                    // if the user is currently connected
                    isConnected = true;
                }
            }
            if (!isConnected)
            {
                disconnectedPlayers.Add(display);
            }
        }
        return disconnectedPlayers;
    }

    /// <summary>
    /// Gets the UI display object associated with a player's username
    /// </summary>
    /// <param name="username"></param>
    /// <returns></returns>
    private PlayerDisplay GetDisplayFromUsername(string username)
    {
        foreach (var playerComp in playerComponents)
        {
            if (playerComp.playerUI.GetPlayerName().Equals(username))
            {
                return playerComp;
            }
        }
        throw new Exception("Player: " + username + " is not associated with a UI component");
    }

}
