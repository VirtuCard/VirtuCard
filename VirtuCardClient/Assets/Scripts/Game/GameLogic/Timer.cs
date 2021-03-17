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

    private bool isInPlay = false;

    private bool isCountingDown = false;

    private int secondsRemaining;
    private int minutesRemaining;
    private int totalSeconds;
    private int totalMinutes;

    private int timeLeftBeforeWarning = 10;
    private Action earlyWarningCallback;
    private Action finishedCallback;

    // Start is called before the first frame update
    void Start()
    {
        // these values are set in SetupTimer()
        minutesText.text = "-1";
        secondsText.text = "-1";
        timerPanel.SetActive(false);
    }


    /// <summary>
    /// Sets up the timer
    /// </summary>
    /// <param name="isInPlay"></param>
    /// <param name="seconds"></param>
    /// <param name="minutes"></param>
    /// <param name="earlyWarningCallback"></param>
    /// <param name="finishedCallback"></param>
    public void SetupTimer(bool isInPlay, int seconds, int minutes, Action earlyWarningCallback, Action finishedCallback)
    {
        this.isInPlay = isInPlay;
        timerPanel.SetActive(isInPlay);
        minutesText.text = ClientData.GetTimerMinutes().ToString();
        secondsText.text = ClientData.GetTimerSeconds().ToString();
        totalSeconds = secondsRemaining;
        totalMinutes = minutesRemaining;
        this.finishedCallback = finishedCallback;
        this.earlyWarningCallback = earlyWarningCallback;
    }


    /// <summary>
    /// Starts the timer fresh
    /// </summary>
    public void StartTimer()
    {
        secondsRemaining = totalSeconds;
        minutesRemaining = totalMinutes;
        isCountingDown = true;
    }

    /// <summary>
    /// Resumes the timer from where it was paused off
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
    /// Returns the time remaining in seconds
    /// </summary>
    /// <returns></returns>
    public int GetTimeRemaining()
    {
        return secondsRemaining + (minutesRemaining * 60);
    }

    // Update is called once per frame
    void Update()
    {
        if (isInPlay)
        {
            if (isCountingDown)
            {

            }
        }
    }
}
