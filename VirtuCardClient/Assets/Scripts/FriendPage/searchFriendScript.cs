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
    public string usernamePlayer;
    public Text postSearchUser;
    public Text addedOrAlreadyFriends;
    public GameObject searchList;
    public Button addBtn;
    public GameObject addBtnObject;
    public CanvasGroup alreadyFriendSign;
    private User user;
    private List<User> friends = new List<User>();

    // Start is called before the first frame update
    void Start()
    {
        searchIcon.onClick.AddListener(delegate { searchPanel.SetActive(true); });
        backSearch.onClick.AddListener(delegate { backBtnPressed(); });
        searchBtn.onClick.AddListener(delegate { 
                if (username.text != "" )
                {
                    searchingFriend();
                }
            });
        addBtn.onClick.AddListener(delegate { addBtnPressed(); });

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void searchingFriend()
    {
        string usernameSearching = username.text;
        Debug.Log("searching for " + usernameSearching);

        FirebaseInit.InitializeFirebase(task =>
        {
            // Task<string> task = Task.Run<string>(async () => await DatabaseUtils.searchUsername(usernameSearching));
            // string foundUsername = task.Result;
            DatabaseUtils.findUsername(usernameSearching,
                task =>
                {
                    if (task == null) {
                        Debug.Log("there's nobody named that!");
                        usernamePlayer = "person does not exist";
                    }
                    else {
                        Debug.Log("you exist! " + task);
                        usernamePlayer = (string) task;
                    } 
                });
        });

        Debug.Log("final test " + usernamePlayer);

        searchList.SetActive(true);
        string searchResult;
        // goes in if the user exists.
        if (!usernamePlayer.Equals("person does not exist"))
        {
            addBtnObject.SetActive(false);
            searchResult = usernameSearching + " does not exist.";

        }
        else 
        {
            addBtnObject.SetActive(true);
            searchResult = usernameSearching;
        }

        postSearchUser.GetComponent<Text>().text = searchResult;

    }

    public void backBtnPressed()
    {
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
            addedOrAlreadyFriends.GetComponent<Text>().text = "You have added this user!";
            alreadyFriendSign.GetComponent<CanvasGroup>().alpha = 1;
            StartCoroutine(FadeCanvas(alreadyFriendSign, alreadyFriendSign.alpha, 0));

        }
    }

    public IEnumerator FadeCanvas(CanvasGroup cg, float start, float end, float lerpTime = 1.0f)
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
