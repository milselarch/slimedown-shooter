using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using Cursor = UnityEngine.Cursor;

public class PauseMenuManager : MonoBehaviour {
    public GameObject pauseCanvas;
    public UIDocument pauseUI;
    public UnityEvent restartEvent;

    private Button _playButton;
    private Button _manuMenuButton;
    private Button _restartButton;

    private void OnEnable() {
        var root = pauseUI.rootVisualElement;
        this._playButton = root.Q<Button>("PlayButton");
        this._manuMenuButton = root.Q<Button>("MainMenuButton");
        this._restartButton = root.Q<Button>("RestartButton");
            
        Assert.IsNotNull(this._playButton);
        // Debug.Log("PLAY_BUTTON" + this._playButton);
        this._playButton.clickable.clicked += OnPlayButtonClicked;
        this._manuMenuButton.clickable.clicked += OnMainMenuButtonClicked;
        this._restartButton.clickable.clicked += OnRestartButtonClicked;
        Hide();
    }
    
    private void OnRestartButtonClicked() {
        restartEvent.Invoke();
        // Hide();
    }
    
    private void Show() {
        pauseCanvas.SetActive(true);
        pauseUI.rootVisualElement.style.visibility = Visibility.Visible;
        Cursor.visible = true;
    }

    private void Hide() {
        pauseUI.rootVisualElement.style.visibility = Visibility.Hidden;
        pauseCanvas.SetActive(false);
        Cursor.visible = false;
    }
    
    private void OnPlayButtonClicked() {
        Debug.Log("Play button clicked");
        GameState.paused = false;
        Hide();
    }

    private void OnMainMenuButtonClicked() {
        SceneManager.LoadSceneAsync("Menu", LoadSceneMode.Single);
    }
    
    private void Start() {
        // wait for UI document to render
        // StartCoroutine(LaunchInitializer());
    }
    
    public void TogglePause(InputAction.CallbackContext context) {
        // Debug.Log("Play button clicked 2");
        if (GameState.dead) {
            // don't pause game if game over
            return;
        }

        GameState.paused = !GameState.paused;
        if (GameState.paused) {
            Show();
        } else {
            Hide();
        }
    }
}

