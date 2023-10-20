using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public IntVariable gameScore;
    public IntVariable health;
    public IntVariable waveCounter;
    // timestamp for start of attack wave
    public IntVariable waveTimestamp;

    public TextMeshProUGUI gameScoreText;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI waveText;
    public TextMeshProUGUI countdownText;

    private bool _destroyed = false;
    
    // Start is called before the first frame update
    void Start()
    {
        gameScore.SetValue(0);
        health.SetValue(100);
        UpdateUI();

        StartCoroutine(TimerUpdateLoop());
    }
    
    private void OnDestroy()
    {
        _destroyed = true;
    }

    IEnumerator TimerUpdateLoop()
    {
        while (!_destroyed)
        {
            yield return new WaitForSeconds(0.3f);
            UpdateTimer();
        }

        yield return null;
    }

    public void UpdateTimer() {
        int timestampNow = (int) Time.time;
        int allowedWaveDuration = 20 + 5 * waveCounter.Value;
        int durationPassed = timestampNow - waveTimestamp.Value; 
        int durationLeft = Math.Max(
            allowedWaveDuration - durationPassed, 0
        );
        
        int minutes = durationLeft / 60;
        int seconds = durationLeft % 60;
        countdownText.SetText(
            minutes + ":" + seconds.ToString("00")
        );
    }

    public void UpdateUI()
    {
        // Debug.Log("HEALTH_UPDATE");
        gameScoreText.SetText("SCORE: " + gameScore.Value);
        healthText.SetText("HEALTH: " + health.Value);
        waveText.SetText("WAVE: " + waveCounter.Value);
        UpdateTimer();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
