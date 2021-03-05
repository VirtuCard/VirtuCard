using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClientGameController : MonoBehaviour
{
    public Button skipBtn;


    private bool wasCurrentlyTurn = false;

    // Start is called before the first frame update
    void Start()
    {
        skipBtn.onClick.AddListener(delegate() {
            SkipBtnClicked();
        });
        SetCanSkipBtn(ClientData.isCurrentTurn());
    }

    // Update is called once per frame
    void Update()
    {
        // check to see if the player can skip their turn once per frame
        if (ClientData.isCurrentTurn())
        {
            if (!wasCurrentlyTurn)
            {
                wasCurrentlyTurn = true;
                SetupTurn();
            }
        }
        else
        {
            wasCurrentlyTurn = false;
        }
    }

    /// <summary>
    /// This method should called every time the game swaps to the player's turn
    /// It should setup all the necessary things like enabling skip btn, etc...
    /// </summary>
    private void SetupTurn()
    {
        SetCanSkipBtn(ClientData.isCurrentTurn());
    }

    /// <summary>
    /// This method sets the skip btn to interactable or not
    /// </summary>
    /// <param name="value">true if the button should be enabled, false otherwise</param>
    private void SetCanSkipBtn(bool value)
    {
        skipBtn.interactable = value;
    }

    /// <summary>
    /// This method is run everythime the skip button is clicked
    /// </summary>
    private void SkipBtnClicked()
    {
        Debug.Log("Skipping...");
    }
}
