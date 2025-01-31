using System;
using System.Collections;
using System.Collections.Generic;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Events;

internal class StatsOverlay {
    private readonly UIDocument _statsUI;

    private readonly Label _scoreText;
    private readonly Label _waveText;
    private readonly Label _healthText;
    private readonly Label _countdownText;
    
    public StatsOverlay(UIDocument statsUI) {
        this._statsUI = statsUI;
        
        var root = statsUI.rootVisualElement;
        this._scoreText = root.Q<Label>("Score");
        this._waveText = root.Q<Label>("WaveCounter");
        this._healthText = root.Q<Label>("Health");
        this._countdownText = root.Q<Label>("Timer");
    }

    public void AssignOutlines() {
        var root = this._statsUI.rootVisualElement;
        var labels = root.Query<Label>().Build();

        foreach (var label in labels) {
            label.AddToClassList("outlined");
        }
    }

    public void UpdateUI(UIManager uiManager) {
        UpdateUI(uiManager, (int) Time.time);
    }

    public void UpdateUI(UIManager uiManager, int timestampNow) {
        _scoreText.text = "SCORE: " + uiManager.gameScore.value;
        _waveText.text = "WAVE: " + uiManager.waveCounter.value;
        _healthText.text = "HEALTH: " + uiManager.health.value;
        UpdateTimer(uiManager, timestampNow);
    }

    public void UpdateTimer(UIManager uiManager) {
        UpdateTimer(uiManager, (int) Time.time);
    }
    
    public void UpdateTimer(UIManager uiManager, int timestampNow) {
        var allowedWaveDuration = 20 + 5 * uiManager.waveCounter.value;
        var durationPassed = timestampNow - uiManager.waveTimestamp.value; 
        var durationLeft = Math.Max(
            allowedWaveDuration - durationPassed, 0
        );
        
        var minutes = durationLeft / 60;
        var seconds = durationLeft % 60;
        _countdownText.text = minutes + ":" + seconds.ToString("00");
    }
}


internal class MultipleStatsOverlay {
    private readonly List<StatsOverlay> _overlays = new();

    public void AddOverlay(
        UIDocument uiDocument, bool outline=false
    ) {
        var overlay = new StatsOverlay(uiDocument);
        if (outline) { overlay.AssignOutlines(); }
        _overlays.Add(overlay);
    }

    public void UpdateUI(UIManager uiManager) {
        var timestampNow = (int) Time.time;
        foreach (var overlay in _overlays) {
            overlay.UpdateUI(uiManager, timestampNow);
        }
    }
    
    public void UpdateTimer(UIManager uiManager) {
        var timestampNow = (int) Time.time;
        foreach (var overlay in _overlays) {
            overlay.UpdateTimer(uiManager, timestampNow);
        }
    }
}

public class UIManager : MonoBehaviour {
    public IntVariable gameScore;
    public IntVariable health;
    public IntVariable waveCounter;
    // timestamp for start of attack wave
    public IntVariable waveTimestamp; 
    public UnityEvent restartEvent;

    public UIDocument statsUI;
    public UIDocument outlineStatsUI;
    
    private bool _destroyed = false;
    private MultipleStatsOverlay _statsOverlays;
    
    // Start is called before the first frame update
    private void Start() {
        _statsOverlays = new MultipleStatsOverlay();
        _statsOverlays.AddOverlay(outlineStatsUI, true);
        _statsOverlays.AddOverlay(statsUI);
        UpdateUI();
        
        StartCoroutine(TimerUpdateLoop());
        GameRestart();
    }

    public void GameRestart() {
        /* I set UIManager script execution order to be after
         * EnemySpawner script execution order. That is the only thing
         * guaranteeing that the wave count is reset before the UI
         */
        UpdateUI();
    }

    public void InitiateRestart() {
        Debug.Log("Initiating restart...");
        restartEvent.Invoke();
    }

    private void OnDestroy() {
        _destroyed = true;
    }

    private IEnumerator TimerUpdateLoop() {
        while (!_destroyed) {
            yield return new WaitForSeconds(0.1f);
            _statsOverlays.UpdateTimer(this);
        }

        yield return null;
    }

    public void UpdateUI() {
        _statsOverlays?.UpdateUI(this);
    }
}
