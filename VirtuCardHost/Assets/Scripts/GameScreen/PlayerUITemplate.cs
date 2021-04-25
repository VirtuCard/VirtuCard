using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using FirebaseScripts;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUITemplate : MonoBehaviour
{
    public RawImage avatar;
    public Text cardCount;
    public Text playerName;
    public Text scoreText;
    private static Mutex locked;

    private byte[] image;

    private void Start()
    {
        if (locked == null)
        {
            locked = new Mutex();
        }
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

        if (locked == null)
        {
            locked = new Mutex();
        }
        locked.WaitOne();
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
        locked.ReleaseMutex();
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