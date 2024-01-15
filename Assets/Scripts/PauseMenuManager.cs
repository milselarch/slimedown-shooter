using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PauseMenuManager: MonoBehaviour {
    public UIDocument pauseUI;
    public GameObject pauseScreen;

    private void Initialize() {
        var root = pauseUI.rootVisualElement;
        var playButton = (Button) root.Q<VisualElement>("PlayButton");
        playButton.clicked += () => {
            GameState.paused = false;
            pauseScreen.SetActive(false);
        };
    }
    
    private IEnumerator LaunchInitializer() {
        // wait for UI document to render
        yield return new WaitForEndOfFrame();
        Initialize();
    }
    
    private void OnEnable() {
        StartCoroutine(LaunchInitializer());
    }
}

