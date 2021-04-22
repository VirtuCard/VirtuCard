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
        Friends = new List<User>();
        if (HostData.UserProfile == null)
        {
            DatabaseUtils.getUser(AuthUser.GetUserID(), s =>
            {
                HostData.UserProfile = new User(s);
                foreach (var friendName in HostData.UserProfile.Friends)
                {
                    DatabaseUtils.GetUserFromName(friendName, user =>
                    {
                        if (user != null)
                        {
                            Friends.Add(user);
                        }
                    });
                }
            });
        }
        else
        {
            foreach (var friendName in HostData.UserProfile.Friends)
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
    }

    public void Update()
    {
        while (Friends.Count > 0)
        {
            User user = Friends[0];
            GameObject friendObject = Instantiate(friendTemplate, friendsPanel.transform);
            if (friendObject != null)
            {
                friendObject.transform.Find("FriendName").gameObject.GetComponent<Text>().text = user.Username;
                friendObject.transform.Find("GamesPlayed").gameObject.GetComponent<Text>().text =
                    user.GamesPlayed.ToString();
                friendObject.transform.Find("GamesWon").gameObject.GetComponent<Text>().text =
                    user.GamesWon.ToString();
                friendObject.transform.Find("GamesLost").gameObject.GetComponent<Text>().text =
                    user.GamesLost.ToString();
                friendObject.SetActive(true);
                Friends.RemoveAt(0);
            }
            else
            {
                Debug.Log("Issue with adding Friend");
            }
        }
    }
}