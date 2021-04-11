using FirebaseScripts;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LeaderboardPageManager : MonoBehaviour
{
    private User user;
    private List<User> friends = new List<User>();
    private List<PlayerRankingUIForm> uiForms = new List<PlayerRankingUIForm>();
    private int targetFriendCount = 0;
    private bool initialPageSetup = false;

    public Transform parentPanelForPlayerRankings;
    public Transform playerRankingUITemplate;

    public Color firstPlaceColor;
    public Color secondPlaceColor;
    public Color thirdPlaceColor;

    public Dropdown sortingDropdown;

    public GameObject displayPlayerPanel;
    public Text displayPlayerUsernameText;
    public Text displayPlayerWinsText;
    public Text displayPlayerLossesText;
    public Text displayPlayerPlayedText;
    public Text displayPlayerRankText;

    public Button backButton;

    private enum SetChoice
    {
        WINS, LOSSES, PLAYED
    }

    // Start is called before the first frame update
    void Start()
    {
        user = ClientData.UserProfile;
        targetFriendCount = user.Friends.Count;
        foreach (string friendName in user.Friends)
        {
            DatabaseUtils.GetUserFromName(friendName, AddFriend);
        }
        playerRankingUITemplate.gameObject.SetActive(false);
        sortingDropdown.onValueChanged.AddListener(delegate { OnSortByPanelChanged(sortingDropdown.value); });
        backButton.onClick.AddListener(delegate { SceneManager.LoadScene(SceneNames.JoinGamePage, LoadSceneMode.Single); });
        displayPlayerPanel.SetActive(false);
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
            foreach (User friend in friends)
            {
                // add UI component for each friend
                Transform newUIRanking = Instantiate(playerRankingUITemplate, parentPanelForPlayerRankings);
                newUIRanking.gameObject.SetActive(true);
                uiForms.Add(newUIRanking.GetComponent<PlayerRankingUIForm>());
            }
            // add a UI component for the current user
            Transform playerUIRanking = Instantiate(playerRankingUITemplate, parentPanelForPlayerRankings);
            playerUIRanking.gameObject.SetActive(true);
            uiForms.Add(playerUIRanking.GetComponent<PlayerRankingUIForm>());

            // by default just sort by wins
            SortByWins();
            initialPageSetup = true;
        }
    }

    private void SetUIFormValues(int index, string username, int winCount, int lossCount, int playCount, SetChoice choice)
    {
        if (index < 0 || index >= uiForms.Count)
        {
            Debug.LogError(string.Format("Can't index form {0} in the list of size {1}", index, uiForms.Count));
            return;
        }

        PlayerRankingUIForm targetForm = uiForms[index];
        targetForm.SetRanking(index + 1);
        targetForm.SetUserName(username);
        targetForm.SetWins(winCount);
        targetForm.SetLosses(lossCount);
        targetForm.SetPlayedGames(playCount);
        targetForm.SetActionWhenClicked(OnIndividualClicked);

        switch (choice)
        {
            case SetChoice.WINS:
                targetForm.DisplayWins();
                break;
            case SetChoice.LOSSES:
                targetForm.DisplayLosses();
                break;
            case SetChoice.PLAYED:
                targetForm.DisplayPlayed();
                break;
        }

        switch (index)
        {
            case 0:
                targetForm.SetColor(firstPlaceColor);
                break;
            case 1:
                targetForm.SetColor(secondPlaceColor);
                break;
            case 2:
                targetForm.SetColor(thirdPlaceColor);
                break;
        }
    }

    private void SortByWins()
    {
        List<User> users = new List<User>();
        users.AddRange(friends);
        users.Add(ClientData.UserProfile);

        User[] sortedUsers = users.OrderBy(p => p.GamesWon).Reverse().ToArray();
        for (int x = 0; x < sortedUsers.Length; x++)
        {
            SetUIFormValues(x, sortedUsers[x].Username, (int)sortedUsers[x].GamesWon, (int)sortedUsers[x].GamesLost, (int)sortedUsers[x].GamesPlayed, SetChoice.WINS);
        }
    }

    private void SortByLosses()
    {
        List<User> users = new List<User>();
        users.AddRange(friends);
        users.Add(ClientData.UserProfile);

        User[] sortedUsers = users.OrderBy(p => p.GamesLost).Reverse().ToArray();
        for (int x = 0; x < sortedUsers.Length; x++)
        {
            SetUIFormValues(x, sortedUsers[x].Username, (int)sortedUsers[x].GamesWon, (int)sortedUsers[x].GamesLost, (int)sortedUsers[x].GamesPlayed, SetChoice.LOSSES);
        }
    }

    private void SortByGamesPlayed()
    {
        List<User> users = new List<User>();
        users.AddRange(friends);
        users.Add(ClientData.UserProfile);

        User[] sortedUsers = users.OrderBy(p => p.GamesPlayed).Reverse().ToArray();
        for (int x = 0; x < sortedUsers.Length; x++)
        {
            SetUIFormValues(x, sortedUsers[x].Username, (int)sortedUsers[x].GamesWon, (int)sortedUsers[x].GamesLost, (int)sortedUsers[x].GamesPlayed, SetChoice.PLAYED);
        }
    }

    public void OnSortByPanelChanged(int state)
    {
        switch(state)
        {
            case 0:
                SortByWins();
                break;
            case 1:
                SortByLosses();
                break;
            case 2:
                SortByGamesPlayed();
                break;
        }
    }

    private void OnIndividualClicked(string username, int ranking, int winCount, int lossCount, int playCount)
    {
        displayPlayerPanel.SetActive(true);

        displayPlayerUsernameText.text = username;

        displayPlayerWinsText.text = winCount.ToString();
        displayPlayerLossesText.text = lossCount.ToString();
        displayPlayerPlayedText.text = playCount.ToString();

        string sortingType = "Games Won";
        switch (sortingDropdown.value)
        {
            case 0:
                sortingType = "Games Won";
                break;
            case 1:
                sortingType = "Games Lost";
                break;
            case 2:
                sortingType = "Games Played";
                break;
        }

        displayPlayerRankText.text = string.Format("Ranked {0} of {1} Friends in {2}", ranking, friends.Count + 1, sortingType);
    }
}
