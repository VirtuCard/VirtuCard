using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using FirebaseScripts;

public class searchFriendScript : MonoBehaviour
{
    public Button searchIcon;
    public Button backSearch;
    public Button searchBtn;
    public InputField username;
    public GameObject searchPanel;
    public string usernamePlayer = "";
    public bool exist;
    public bool doesNotExist;
    public Text postSearchUser;
    public Text addedOrAlreadyFriends;
    public GameObject searchList;
    public Button addBtn;
    public GameObject addBtnObject;
    public CanvasGroup alreadyFriendSign;
    public GameObject stats;
    public Text playerStat;
    public Text playerPlayed;
    public Text playerWon;
    public Text playerLost;
    private User user;
    private User personSearching;
    private List<User> friends = new List<User>();
    private bool nameEqual = false;


    // Start is called before the first frame update
    void Start()
    {
        searchPanel.SetActive(false);
        searchIcon.onClick.AddListener(delegate {
            searchPanel.SetActive(true); 
            });
        backSearch.onClick.AddListener(delegate { backBtnPressed(); });
        searchBtn.onClick.AddListener(delegate
        {
            searchList.SetActive(false);
            addBtnObject.SetActive(false);
            postSearchUser.GetComponent<Text>().text = "";
            if (username.text != "")
            {
                searchingFriend();
            }
        });
        addBtn.onClick.AddListener(delegate { addBtnPressed(); });
    }

    // Update is called once per frame
    void Update()
    {
        if (exist)
        {
            addBtnObject.SetActive(true);
            stats.SetActive(true);
            postSearchUser.GetComponent<Text>().text = usernamePlayer;

            DatabaseUtils.GetUserFromName(usernamePlayer, changePerson);

            playerStat.GetComponent<Text>().text = usernamePlayer + "'s stats";
            playerPlayed.GetComponent<Text>().text = "Games Played: " + personSearching.GamesPlayed.ToString();
            playerWon.GetComponent<Text>().text = "Games Won: " + personSearching.GamesWon.ToString();
            playerLost.GetComponent<Text>().text = "Games Lost: " + personSearching.GamesLost.ToString();
        }
        else if (doesNotExist)
        {
            addBtnObject.SetActive(false);
            stats.SetActive(false);
            postSearchUser.GetComponent<Text>().text = usernamePlayer + " does not exist.";
        }
        else
        {
            playerStat.GetComponent<Text>().text = "";
            playerPlayed.GetComponent<Text>().text = "";
            playerWon.GetComponent<Text>().text = "";
            playerLost.GetComponent<Text>().text = "";
            addBtnObject.SetActive(false);
            stats.SetActive(false);
        }
    }

    public void changePerson(User friend)
    {
        personSearching = friend;
    }

    public void searchingFriend()
    {
        user = ClientData.UserProfile;
        Debug.Log("current username is " + user.Username);

        string usernameSearching = username.text;
        usernamePlayer = username.text;
        Debug.Log("searching for " + usernameSearching);
        searchList.SetActive(true);

        // changeExists(usernameSearching);
        if (usernameSearching.Equals(user.Username))
        {
            // trying to add yourself
            addBtnObject.SetActive(false);
            postSearchUser.GetComponent<Text>().text = "You cannot add yourself.";
            doesNotExist = false;
            exist = false;
        }
        else // the person does exist
        {
            FirebaseInit.InitializeFirebase(task =>
            {
                DatabaseUtils.findUsername(usernameSearching,
                    task =>
                    {
                        Debug.Log("final test the task is " + task);
                        if (task != null)
                        {
                            exist = true;
                            doesNotExist = false;
                            Debug.Log("final test you exist! " + exist);
                        }
                        else
                        {
                            exist = false;
                            doesNotExist = true;
                            Debug.Log("final test you don't exist! " + exist);
                        }
                    });
            });
        }

        Debug.Log("final test " + exist); // return true if the player exists
    }

    public void backBtnPressed()
    {
        exist = false;
        doesNotExist = false;
        username.text = "";
        searchList.SetActive(false);
        searchPanel.SetActive(false);
    }

    public void addBtnPressed()
    {
        user = ClientData.UserProfile;
        string usernameSearching = username.text;
        //List<string> currentFriends = new List<string>();

        int alreadyFriends = 0;

        // this checks if this is person is already friends
        foreach (string friendName in user.Friends)
        {
            // currentFriends.Add(friendName);
            if (friendName == usernameSearching)
            {
                alreadyFriends = 1;
            }
        }

        if (alreadyFriends == 1) // this person is already your friend
        {
            Debug.Log("already friends");
            addedOrAlreadyFriends.GetComponent<Text>().text = "You are already friends with this user";
            alreadyFriendSign.GetComponent<CanvasGroup>().alpha = 1;
            StartCoroutine(FadeCanvas(alreadyFriendSign, alreadyFriendSign.alpha, 0));
        }
        else // adding this person normally
        {
            user.Friends.Add(usernameSearching);

            FirebaseInit.InitializeFirebase(task =>
            {
                DatabaseUtils.updateUser(user,
                    task =>
                    {
                        if (!task)
                        {
                            Debug.Log("Error in adding friend");
                        }
                        else
                        {
                            Debug.Log("Successfully added!");

                            DatabaseUtils.GetUserFromName(usernameSearching, foundFriend =>
                            {
                                if (foundFriend != null)
                                {
                                    FriendsList.Friends.Add(foundFriend);
                                }
                            });
                        }
                    });
            });

            addedOrAlreadyFriends.GetComponent<Text>().text = "You have added this user!";
            alreadyFriendSign.GetComponent<CanvasGroup>().alpha = 1;
            StartCoroutine(FadeCanvas(alreadyFriendSign, alreadyFriendSign.alpha, 0));
        }
    }

    public void changeExists(string usernameSearching)
    {
        FirebaseInit.InitializeFirebase(task =>
        {
            bool test = false;
            DatabaseUtils.findUsername(usernameSearching,
                task =>
                {
                    test = true;
                    Debug.Log("final test the task is " + task);
                    if (task != null)
                    {
                        exist = true;
                        Debug.Log("final test you exist! " + exist);
                    }
                    else
                    {
                        exist = false;
                        Debug.Log("final test you don't exist! " + exist);
                    }
                });
            Debug.Log("this should return true " + test);
        });
    }

    public IEnumerator FadeCanvas(CanvasGroup cg, float start, float end, float lerpTime = 2.0f)
    {
        float _timeStartedLerping = Time.time;
        float timeSinceStarted = Time.time - _timeStartedLerping;
        float percentageComplete = timeSinceStarted / lerpTime;

        while (true)
        {
            timeSinceStarted = Time.time - _timeStartedLerping;
            percentageComplete = timeSinceStarted / lerpTime;

            float currentValue = Mathf.Lerp(start, end, percentageComplete);
            cg.alpha = currentValue;
            if (percentageComplete >= 1) break;
            yield return new WaitForEndOfFrame();
        }
    }
}