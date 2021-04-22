using System;
using System.Collections.Generic;
using FirebaseScripts;
using UnityEngine;
using UnityEngine.UI;


public class FriendsList : MonoBehaviour
{
    public GameObject friendTemplate;
    public GameObject friendsPanel;

    public static List<User> Friends;

    public void OnEnable()
    {
        while (friendsPanel.transform.childCount > 0)
        {
            DestroyImmediate(friendsPanel.transform.GetChild(0).gameObject);
        }


        Friends = new List<User>();
        foreach (var friendName in ClientData.UserProfile.Friends)
        {
            DatabaseUtils.GetUserFromName(friendName, user =>
            {
                if (user != null)
                {
                    Friends.Add(user);
                }
            });
        }
    }

    public void Update()
    {
        if (Friends.Count > 0)
        {
            User user = Friends[0];
            Friends.RemoveAt(0);

            GameObject friendObject = Instantiate(friendTemplate, friendsPanel.transform);

            friendObject.SetActive(true);
            friendObject.transform.Find("FriendName").gameObject.GetComponent<Text>().text = user.Username;
            friendObject.transform.Find("GameStats").Find("GamesPlayed").gameObject.GetComponent<Text>().text =
                user.GamesPlayed.ToString();
            friendObject.transform.Find("GameStats").Find("GamesWon").gameObject.GetComponent<Text>().text =
                user.GamesWon.ToString();
            friendObject.transform.Find("GameStats").Find("GamesLost").gameObject.GetComponent<Text>().text =
                user.GamesLost.ToString();
        }
    }
}