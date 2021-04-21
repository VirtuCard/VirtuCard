using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using FirebaseScripts;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUITemplate : MonoBehaviour
{
    public RawImage avatar;
    public Text cardCount;
    public Text playerName;
    public Text scoreText;

    private byte[] image;

    private void Start()
    {
    }

    private void Update()
    {
        if (image != null)
        {
            Texture2D t = new Texture2D(2, 2);
            t.LoadImage(image);
            t.Apply();
            avatar.texture = t;
            Debug.Log("Height:" + t.height);
            image = null;
        }
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

        FirebaseInit.InitializeFirebase(res =>
        {
            DatabaseUtils.GetUserFromName(value, user =>
            {
                ImageStorage.getImage(user.Avatar, bytes =>
                {
                    Debug.Log("Got Image? " + (bytes != null));
                    if (bytes == null) return;
                    image = bytes;
                });
            });
        });
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