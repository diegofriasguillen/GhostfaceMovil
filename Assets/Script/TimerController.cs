using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TimerController : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    public GameObject timerObject;
    public float maxTime = 60f;
    private float currentTime;

    public GameObject loseCanvas;

    private void Start()
    {
        currentTime = maxTime;
        UpdateTimerText();

    }

    private void Update()
    {
        currentTime -= Time.deltaTime;
        UpdateTimerText();

        if( currentTime <= 0 )
        {
            PlayerLost();
        }
    }

    private void UpdateTimerText()
    {
        int minutes = Mathf.FloorToInt(currentTime / 60);
        int seconds = Mathf.FloorToInt(currentTime % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    private void PlayerLost()
    {
        timerObject.SetActive(false);
        loseCanvas.SetActive(true);
        Time.timeScale = 0f;
    }
}
