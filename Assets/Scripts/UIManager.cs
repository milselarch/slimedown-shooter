using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class UIManager: MonoBehaviour {
    public IntVariable gameScore;
    public IntVariable health;
    public IntVariable waveCounter;
    // timestamp for start of attack wave
    public IntVariable waveTimestamp;
    public CanvasGroup gameOverCanvas;

    public TextMeshProUGUI gameScoreText;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI waveText;
    public TextMeshProUGUI countdownText;

    public GameObject gameOverScreen;
    public GameObject pauseScreen;

    private bool _destroyed = false;
	private bool _exiting = false;
    
    // Start is called before the first frame update
    void Start() {
        gameScore.SetValue(0);
        health.SetValue(100);
        UpdateUI();

        gameOverScreen.SetActive(false);
        pauseScreen.SetActive(false);
        StartCoroutine(TimerUpdateLoop());
    }

    public void TogglePause() {
        if (gameOverScreen.activeSelf) {
            // don't pause game if game over
            return;
        }

        GameState.paused = !GameState.paused;
        
        if (GameState.paused) {
            pauseScreen.SetActive(true);
            Time.timeScale = 1.0f;
        } else {
            pauseScreen.SetActive(false);
            Time.timeScale = 0.0f;   
        }
    }

    public void OnPlayerHealthUpdate() {
        if (health.Value == 0)
        {
            gameOverScreen.SetActive(true);
        }
    }

    public void ReturnToMainMenu() {
        if (this._exiting) {
            return;
        }

        this._exiting = true;
        StartCoroutine(FadeAndExit());
    }
    
    IEnumerator FadeAndExit() {
        for (var alpha = 1f; alpha >= -0.05f; alpha -= 0.05f) {
            gameOverCanvas.alpha = alpha;
            yield return new WaitForSecondsRealtime(0.1f);
        }

        // once done, go to next scene
        SceneManager.LoadSceneAsync("Menu", LoadSceneMode.Single);
    }
    
    private void OnDestroy() {
        _destroyed = true;
    }

    private IEnumerator TimerUpdateLoop() {
        while (!_destroyed) {
            yield return new WaitForSeconds(0.3f);
            UpdateTimer();
        }

        yield return null;
    }

    private void UpdateTimer() {
        var timestampNow = (int) Time.time;
        var allowedWaveDuration = 20 + 5 * waveCounter.Value;
        var durationPassed = timestampNow - waveTimestamp.Value; 
        var durationLeft = Math.Max(
            allowedWaveDuration - durationPassed, 0
        );
        
        var minutes = durationLeft / 60;
        var seconds = durationLeft % 60;
        countdownText.SetText(
            minutes + ":" + seconds.ToString("00")
        );
    }

    public void UpdateUI() {
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
