using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PhotonScripts;

public class ShuffleButtonScript : MonoBehaviour
{

    public GameObject shufflingPanel;

    private IEnumerator coroutine;

    public AudioSource shuffleSound;

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

        shuffleSound.Play();

        //Setting the Shuffling... Panel to visible
        shufflingPanel.SetActive(true);

        NetworkController.setIsShuffle(true);

        //Calling the Shuffle Method
        HostData.GetGame().AddCardsToDeck(HostData.GetGame().GetDeck(DeckChoices.PLAYED).GetAllCards().ToArray(), DeckChoices.UNDEALT);
        HostData.GetGame().GetDeck(DeckChoices.PLAYED).RemoveAllCards();
        HostData.GetGame().ShuffleDeck(DeckChoices.UNDEALT);

        NetworkController.setIsShuffle(false);

        //Setting the Shuffling... Panel to invisible
        shufflingPanel.SetActive(false);
    }


    /// <summary>
    /// Coroutine that displays Shuffling... Panel with an in-built delay
    /// </summary>
    /// <returns> The panel active for a few seconds </returns>
    IEnumerator ShuffleRoutine()
    {
        Debug.Log("Shuffle Routine invoked");

        shufflingPanel.SetActive(true);

        HostData.GetGame().AddCardsToDeck(HostData.GetGame().GetDeck(DeckChoices.PLAYED).GetAllCards().ToArray(), DeckChoices.UNDEALT);
        HostData.GetGame().GetDeck(DeckChoices.PLAYED).RemoveAllCards();
        HostData.GetGame().ShuffleDeck(DeckChoices.UNDEALT);
        
        HostData.SetLastPlayedCardTexture("SingleCardBack");

        yield return new WaitForSeconds(2);

        shufflingPanel.SetActive(false);
    }

    /// <summary>
    /// Testing with this method
    /// </summary>
    public void ShuffleDeckClicked2()
    {
        coroutine = ShuffleRoutine();
        StartCoroutine(coroutine);
    }

    

}