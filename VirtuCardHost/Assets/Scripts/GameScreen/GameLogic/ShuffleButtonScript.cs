using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PhotonScripts;

public class ShuffleButtonScript : MonoBehaviour
{

    public GameObject shufflingPanel;

    // Start is called before the first frame update
    void Start()
    {
        shufflingPanel.SetActive(false);
    }

    // Update is called once per frame
    /* void Update()
    {
        
    } */

    /// <summary>
    /// This method will Shuffle the deck when called.
    /// </summary>
    public void ShuffleDeckClicked()
    {
        Debug.Log("Shuffle Deck Button Clicked");

        //Setting the Shuffling... Panel to visible
        shufflingPanel.SetActive(true);

        NetworkController.setIsShuffle(true);

        //Calling the Shuffle Method
        HostData.GetGame().ShuffleDeck(DeckChoices.UNDEALT);

        NetworkController.setIsShuffle(false);

        //Setting the Shuffling... Panel to invisible
        shufflingPanel.SetActive(false);
    }
}