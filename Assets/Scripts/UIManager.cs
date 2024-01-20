using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using TMPro;

public class UIManager: MonoBehaviour {
    public IntVariable gameScore;
    public IntVariable health;
    public IntVariable waveCounter;
    // timestamp for start of attack wave
    public IntVariable waveTimestamp;
    public CanvasGroup gameOverCanvas;

    public UIDocument statsUI;
    public GameObject gameOverScreen;

    private Label _scoreText;
    private Label _waveText;
    private Label _healthText;
    private Label _countdownText;
    
    private bool _destroyed = false;
	private bool _exiting = false;
    
    // Start is called before the first frame update
    void Start() {
        var root = statsUI.rootVisualElement;
        this._scoreText = root.Q<Label>("Score");
        this._waveText = root.Q<Label>("WaveCounter");
        this._healthText = root.Q<Label>("Health");
        this._countdownText = root.Q<Label>("Timer");
        
        gameScore.SetValue(0);
        health.SetValue(100);
        UpdateUI();

        gameOverScreen.SetActive(false);
        StartCoroutine(TimerUpdateLoop());
    }

    public void OnPlayerHealthUpdate() {
        if (GameState.dead) {
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
        _countdownText.text = minutes + ":" + seconds.ToString("00");
    }

    public void UpdateUI() {
        // Debug.Log("HEALTH_UPDATE");
        _scoreText.text = "SCORE: " + gameScore.Value;
        _waveText.text = "WAVE: " + waveCounter.Value;
        _healthText.text = "HEALTH: " + health.Value;
        UpdateTimer();
    }
}
