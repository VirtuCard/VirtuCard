using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Timer : MonoBehaviour
{
    public Text secondsText;
    public Text minutesText;
    public GameObject timerPanel;

    public GameObject warningPanel;
    public Text warningText;

    public Color timerColor;
    public Color timerBelowWarningColor;

    private bool isInPlay = false;

    private bool isCountingDown = false;

    private float secondsRemaining;
    private int totalSeconds;

    private int earlyWarningThreshold;
    private bool alreadySentEarlyWarning = false;
    private float secondsToShowWarningPanel = 3;
    private float secondsWarningPanelHasBeenVisible = 0;
    private Action earlyWarningCallback;
    private Action finishedCallback;

    //Settings Panel options
    private bool hideTimer;
    public Button hideButton;
    public Button unHideButton;

    // Start is called before the first frame update
    void Start()
    {
        hideTimer = false;
        hideButton.gameObject.SetActive(true);
        unHideButton.gameObject.SetActive(false);

        // these values are set in SetupTimer()
        minutesText.text = "-1";
        secondsText.text = "-1";
    }

    public void onButtonClick()
    {
        if (hideTimer)
        {
            timerPanel.SetActive(false);
            hideButton.gameObject.SetActive(false);
            unHideButton.gameObject.SetActive(true);
        }
        else
        {
            timerPanel.SetActive(true);
            hideButton.gameObject.SetActive(true);
            unHideButton.gameObject.SetActive(false);
        }

        hideTimer = !hideTimer;
    }


    /// <summary>
    /// Sets up the timer
    /// </summary>
    /// <param name="isInPlay"></param>
    /// <param name="seconds"></param>
    /// <param name="minutes"></param>
    /// <param name="earlyWarningCallback"></param>
    /// <param name="finishedCallback"></param>
    public void SetupTimer(bool isInPlay, int seconds, int minutes, int warningThreshold, Action earlyWarningCallback,
        Action finishedCallback)
    {
        // set it active if it is in play
        this.isInPlay = isInPlay;
        timerPanel.SetActive(isInPlay);

        earlyWarningThreshold = warningThreshold;
        warningText.text = "Warning Only " + earlyWarningThreshold + " Seconds Left!";

        totalSeconds = seconds + (60 * minutes);

        UpdateText();

        // setup callbacks
        this.finishedCallback = finishedCallback;
        this.earlyWarningCallback = earlyWarningCallback;

        // set panel color
        timerPanel.GetComponent<Image>().color = timerColor;

        // hide warning panel
        warningPanel.SetActive(false);
    }


    /// <summary>
    /// Restarts the timer.
    /// It does not resume it, it resets the time and starts over.
    /// User ResumeTimer() if you want to resume.
    /// </summary>
    public void StartTimer()
    {
        UpdateText();
        secondsRemaining = (float) totalSeconds;
        isCountingDown = true;
        alreadySentEarlyWarning = false;

        // set panel color
        timerPanel.GetComponent<Image>().color = timerColor;

        // hide warning panel
        warningPanel.SetActive(false);
    }

    /// <summary>
    /// Resumes the timer from where it was paused.
    /// Use StartTimer() if you want to start the timer over
    /// </summary>
    public void ResumeTimer()
    {
        isCountingDown = true;
    }

    /// <summary>
    /// Pauses the timer
    /// </summary>
    public void StopTimer()
    {
        isCountingDown = false;
    }

    /// <summary>
    /// Enables or disables the timer visibility and functionality depending on the value of <paramref name="enable"/>
    /// </summary>
    /// <param name="enable"></param>
    public void EnableTimer(bool enable)
    {
        isInPlay = enable;
        timerPanel.SetActive(enable);
        if (enable)
        {
            // reset the timer to full values
            secondsRemaining = (float) totalSeconds;

            // set panel color
            timerPanel.GetComponent<Image>().color = timerColor;

            // hide warning panel
            warningPanel.SetActive(false);
        }
    }

    /// <summary>
    /// Returns the time remaining in seconds
    /// </summary>
    /// <returns></returns>
    public float GetTimeRemaining()
    {
        return secondsRemaining;
    }

    /// <summary>
    /// Returns the time threshold that the early warning callback is triggered
    /// </summary>
    /// <returns></returns>
    public int GetEarlyWarningThreshold()
    {
        return earlyWarningThreshold;
    }

    /// <summary>
    /// Decrements the timer by <paramref name="seconds"/> seconds.
    /// Returns false if the time has dipped below 0 seconds.
    /// Returns true if it is still above 30 seconds.
    /// </summary>
    /// <param name="seconds"></param>
    private bool DecrementTimer(float seconds)
    {
        secondsRemaining -= seconds;
        if (secondsRemaining <= 0)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Updates the text on the timer panel to reflect current minutes and seconds remaining
    /// </summary>
    private void UpdateText()
    {
        minutesText.text = ((int) (secondsRemaining / 60)).ToString();
        secondsText.text = ((int) (secondsRemaining % 60)).ToString();
    }

    // Update is called once per frame
    [Obsolete] // this is just so it does give me depracated warnings
    void Update()
    {
        if (isInPlay)
        {
            if (isCountingDown)
            {
                // decrement the timer by the time passed since last frame
                if (!DecrementTimer(Time.deltaTime))
                {
                    // if it is below 0 seconds left
                    StopTimer();
                    finishedCallback();
                    return;
                }

                // check if we need to send early warning
                if (GetTimeRemaining() <= GetEarlyWarningThreshold() && alreadySentEarlyWarning == false)
                {
                    // set panel color
                    timerPanel.GetComponent<Image>().color = timerBelowWarningColor;

                    // show warning panel
                    warningPanel.SetActive(true);
                    secondsWarningPanelHasBeenVisible = 0;

                    alreadySentEarlyWarning = true;
                    earlyWarningCallback();
                }

                // update the UI
                UpdateText();
            }
        }

        // disable warning panel after secondsToShowWarningPanel number of seconds
        if (warningPanel.active)
        {
            secondsWarningPanelHasBeenVisible += Time.deltaTime;
            if (secondsWarningPanelHasBeenVisible >= secondsToShowWarningPanel)
            {
                warningPanel.SetActive(false);
            }
        }
    }
}