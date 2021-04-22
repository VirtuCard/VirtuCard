using System;
using System.Collections.Generic;
using FirebaseScripts;
using UnityEngine;
using UnityEngine.UI;


public class FriendsList : MonoBehaviour
{
    public GameObject friendTemplate;
    public GameObject friendsPanel;

    public static List<User> friends;

    public void Start()
    {
        friends = new List<User>();
        foreach (var friendName in ClientData.UserProfile.Friends)
        {
            DatabaseUtils.GetUserFromName(friendName, user =>
            {
                if (user != null)
                {
                    friends.Add(user);
                }
            });
        }
    }

    public void Update()
    {
        while (friends.Count > 0)
        {
            User user = friends[0];
            friends.RemoveAt(0);
            GameObject friendObject = Instantiate(friendTemplate, friendsPanel.transform);
            friendObject.transform.Find("FriendName").gameObject.GetComponent<Text>().text = user.Username;
            friendObject.transform.Find("GameStats").Find("GamesPlayed").gameObject.GetComponent<Text>().text =
                user.GamesPlayed.ToString();
            friendObject.transform.Find("GameStats").Find("GamesWon").gameObject.GetComponent<Text>().text =
                user.GamesWon.ToString();
            friendObject.transform.Find("GameStats").Find("GamesLost").gameObject.GetComponent<Text>().text =
                user.GamesLost.ToString();
            friendObject.SetActive(true);
        }
    }

    public void AddToPlaylist(User user)
    {
    }
}