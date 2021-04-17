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

    // Start is called before the first frame update
    void Start()
    {
        searchIcon.onClick.AddListener(delegate { searchPanel.SetActive(true); });
        backSearch.onClick.AddListener(delegate { searchPanel.SetActive(false); });
        searchBtn.onClick.AddListener(delegate { searchingFriend(); });

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

        // goes in if the user exists.
        if (!usernamePlayer.Equals("person does not exist"))
        {

        }
        else 
        {

        }


    }
}
