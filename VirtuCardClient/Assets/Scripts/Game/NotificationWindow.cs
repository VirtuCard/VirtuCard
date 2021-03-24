using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NotificationWindow : MonoBehaviour
{
    public GameObject panel;
    private const int NUM_OF_MESSAGE_TEXTS = 4;
    public List<Text> messageTexts;
    public List<bool> currentlySetTexts = new List<bool>();

    private const int TOTAL_SECONDS_DISPLAYED = 3;
    private float secondsRemaining;

    private bool isCountingDown = false;

    // Start is called before the first frame update
    void Start()
    {
        for (int x = 0; x < NUM_OF_MESSAGE_TEXTS; x++)
        {
            currentlySetTexts.Add(false);
        }

        secondsRemaining = TOTAL_SECONDS_DISPLAYED;
        panel.SetActive(false);
        ClearAllTexts();
    }

    /// <summary>
    /// Shows the notification window with the specified <paramref name="message"/> for a specific amount of <paramref name="seconds"/>.
    /// If seconds is not specified, it uses the default constant TOTAL_SECONDS_DISPLAYED
    /// </summary>
    /// <param name="message"></param>
    /// <param name="seconds"></param>
    public void ShowNotification(string message, int seconds = TOTAL_SECONDS_DISPLAYED)
    {
        int index = GetIndexOfFirstAvailableText();
        SetText(index, message);
        secondsRemaining = seconds;
        isCountingDown = true;
        panel.SetActive(true);
    }

    private int GetIndexOfFirstAvailableText()
    {
        for(int x = 0; x < NUM_OF_MESSAGE_TEXTS; x++)
        {
            if (currentlySetTexts[x] == false)
            {
                return x;
            }
        }
        // return the top one because they were all full, so that one can be overridden
        return 0;
    }

    private void SetText(int index, string message)
    {
        Debug.Log("Setting NotificationWindow: " + index + ", " + message);
        messageTexts[index].text = message.Trim();
        currentlySetTexts[index] = true;
    }

    private void ClearAllTexts()
    {
        for (int x = 0; x < NUM_OF_MESSAGE_TEXTS; x++)
        {
            currentlySetTexts[x] = false;
            messageTexts[x].text = string.Empty;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isCountingDown)
        {
            secondsRemaining -= Time.deltaTime;
            if (secondsRemaining < 0)
            {
                ClearAllTexts();
                isCountingDown = false;
                panel.SetActive(false);
            }
        }
    }
}
