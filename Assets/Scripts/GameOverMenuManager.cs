using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;

public class GameOverMenuManager : MonoBehaviour {
    public UIDocument gameOverOverlay;
    public CanvasGroup gameOverCanvas;
    public GameObject gameOverScreen;
    public UnityEvent restartEvent;
    public IntVariable health;

    private Button _mainMenuButton;
    private Button _restartButton;
    private bool _exiting = false;
    
    private void Start() {
        GameRestart();
    }

    private void AttachHandlers() {
        Assert.IsNotNull(gameOverOverlay);
        var root = gameOverOverlay.rootVisualElement;
        this._mainMenuButton = root.Q<Button>("MainMenuButton");
        this._restartButton = root.Q<Button>("RestartButton");
        // Debug.Log("BUTTONED" + this._mainMenuButton);
        
        Assert.IsNotNull(this._restartButton);
        Assert.IsNotNull(this._mainMenuButton);
        _mainMenuButton.clicked += OnMainMenuButtonClicked;
        _restartButton.clicked += OnRestartButtonClicked;
    }
    
    private void OnEnable() {
        AttachHandlers();
        Hide();
    }

    private void Show() {
        gameOverScreen.SetActive(true);
        gameOverOverlay.rootVisualElement.style.visibility = Visibility.Visible;
        Cursor.visible = true;
        /* so it turns out that disabling the UI document will disconnect
         * all event handlers and the buttons will not work anymore, so we
         * need to reattach the handlers every time the UI is enabled
         */
    }

    public void OnPlayerHealthUpdate() {
        if (health.Value <= 0) {
            Show();
        }
    }

    private void Hide() {
        gameOverOverlay.rootVisualElement.style.visibility = Visibility.Hidden;
        gameOverScreen.SetActive(false);
        Cursor.visible = false;
    }

    public void GameRestart() {
        gameOverScreen.SetActive(false);
    }
    
    public void InitiateRestart() {
        Debug.Log("Initiating restart...");
        restartEvent.Invoke();
    }
    
    private void OnMainMenuButtonClicked() {
        Debug.Log("Main Menu button clicked");
        ReturnToMainMenu();
    }

    private void OnRestartButtonClicked() {
        Debug.Log("Restart button clicked");
    }
    
    IEnumerator FadeAndExit() {
        for (var alpha = 1f; alpha >= -0.05f; alpha -= 0.05f) {
            gameOverCanvas.alpha = alpha;
            yield return new WaitForSecondsRealtime(0.1f);
        }
        // once done, go to next scene
        SceneManager.LoadSceneAsync("Menu", LoadSceneMode.Single);
    }
    
    private void ReturnToMainMenu() {
        if (this._exiting) { return; }
        this._exiting = true;
        StartCoroutine(FadeAndExit());
    }
}
