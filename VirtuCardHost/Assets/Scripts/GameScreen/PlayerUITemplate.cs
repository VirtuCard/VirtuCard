using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUITemplate : MonoBehaviour
{
    public Image avatar;
    public Text cardCount;
    public Text playerName;
    public Text scoreText;

    private void Start()
    {
        
    }

    public int GetCardCount()
    {
        return int.Parse(cardCount.text);
    }
    public void SetCardCount(int value)
    {
        cardCount.text = value.ToString();
    }

    public string GetPlayerName()
    {
        return playerName.text.ToString();
    }
    public void SetPlayerName(string value)
    {
        playerName.text = value;
    }

    public int GetScore()
    {
        return int.Parse(scoreText.text.Substring(7));
    }
    public void SetScore(int value)
    {
        scoreText.text = "Score: " + value;
    }
}
