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

    private void AttachHandlers() {
        var root = pauseUI.rootVisualElement;
        this._playButton = root.Q<Button>("PlayButton");
        this._manuMenuButton = root.Q<Button>("MainMenuButton");
        this._restartButton = root.Q<Button>("RestartButton");
            
        Assert.IsNotNull(this._playButton);
        // Debug.Log("PLAY_BUTTON" + this._playButton);
        this._playButton.clickable.clicked += OnPlayButtonClicked;
        this._manuMenuButton.clickable.clicked += OnMainMenuButtonClicked;
        this._restartButton.clickable.clicked += OnRestartButtonClicked;
    }

    private void OnRestartButtonClicked() {
        restartEvent.Invoke();
        Hide();
    }
    
    private void OnEnable() {
        AttachHandlers();
        Hide();
    }
    
    private void Show() {
        pauseCanvas.SetActive(true);
        pauseUI.rootVisualElement.style.visibility = Visibility.Visible;
        Cursor.visible = true;
        /* so it turns out that disabling the UI document will disconnect 
         * all event handlers and the buttons will not work anymore, so we
         * need to reattach the handlers every time the UI is enabled
         */
        AttachHandlers();
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

