using FirebaseScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderboardPageManager : MonoBehaviour
{
    private User user;
    private List<User> friends = new List<User>();
    private int targetFriendCount = 0;
    private bool initialPageSetup = false;

    // Start is called before the first frame update
    void Start()
    {
        user = ClientData.UserProfile;
        targetFriendCount = user.Friends.Count;
        foreach (string friendName in user.Friends)
        {
            DatabaseUtils.GetUserFromName(friendName, AddFriend);
        }
    }

    public void AddFriend(User friend)
    {
        friends.Add(friend);
    }

    // Update is called once per frame
    void Update()
    {
        if (friends.Count == targetFriendCount && initialPageSetup == false)
        {
            // friends have all been added
            Debug.LogError("Setting up Page");
            foreach (User friend in friends)
            {
                Debug.LogError(friend.Username);
            }
            initialPageSetup = true;
        }
    }
}
