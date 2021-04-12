using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerRankingUIForm : MonoBehaviour
{
    public Text winsText;
    private int wins;
    private int losses;
    private int played;

    public Text userNameText;

    public Text rankingText;

    private Action<string, int, int, int, int> actionWhenClicked;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<Button>().onClick.AddListener(delegate { ButtonClicked(); });
    }

    public void DisplayWins()
    {
        winsText.text = string.Format("Won: {0}", wins);
    }
    public void DisplayLosses()
    {
        winsText.text = string.Format("Lost: {0}", losses);
    }
    public void DisplayPlayed()
    {
        winsText.text = string.Format("Played: {0}", played);
    }

    public void ButtonClicked()
    {
        actionWhenClicked?.Invoke(GetUserName(), GetRanking(), GetWins(), GetLosses(), GetPlayedGames());
    }

    public void SetActionWhenClicked(Action<string, int, int, int, int> action)
    {
        actionWhenClicked = action;
    }

    public void SetColor(Color color)
    {
        gameObject.GetComponent<Image>().color = color;
    }

    public void SetRanking(int rank)
    {
        rankingText.text = rank.ToString();
    }
    public int GetRanking()
    {
        return int.Parse(rankingText.text);
    }

    public void SetWins(int winCount)
    {
        wins = winCount;
    }

    public int GetWins()
    {
        return wins;
    }

    public void SetLosses(int lossCount)
    {
        losses = lossCount;
    }

    public int GetLosses()
    {
        return losses;
    }
    public void SetPlayedGames(int playedGamesCount)
    {
        played = playedGamesCount;
    }

    public int GetPlayedGames()
    {
        return played;
    }

    public string GetUserName()
    {
        return userNameText.text;
    }
    public void SetUserName(string value)
    {
        userNameText.text = value;
    }
}
