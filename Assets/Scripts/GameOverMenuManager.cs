using System.Collections;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;

public class GameOverMenuManager : MonoBehaviour {
    private const float FADE_DURATION = 1.0f;

    public UIDocument gameOverOverlay;
    public GameObject gameOverScreen;
    public UnityEvent restartEvent;
    public IntVariable health;
    public GameConstants gameConstants;

    private float _initialBackgroundAlpha = -1.0f;

    private Button _mainMenuButton;
    private Button _restartButton;
    private VisualElement _background;
    private GroupBox _contentBox;
    private bool _exiting = false;
    
    private void Start() {
        GameRestart();
    }

    private void AttachHandlers() {
        Assert.IsNotNull(gameOverOverlay);
        var root = gameOverOverlay.rootVisualElement;
        this._background = root.Q<VisualElement>("Background");
        this._contentBox = root.Q<GroupBox>("Content");
        this._mainMenuButton = _contentBox.Q<Button>("MainMenuButton");
        this._restartButton = _contentBox.Q<Button>("RestartButton");

        if (_initialBackgroundAlpha < 0.0f) {
            // TODO: for some reason reading the initial opacity doesn't work
            this._initialBackgroundAlpha = gameConstants.uiScreenOpacity;
            this._background.style.opacity = this._initialBackgroundAlpha;
        }
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
        AttachHandlers();
    }

    public void OnPlayerHealthUpdate() {
        if (health.value <= 0) {
            Show();
        }
    }

    private void Hide() {
        gameOverOverlay.rootVisualElement.style.visibility = Visibility.Hidden;
        gameOverScreen.SetActive(false);
        Cursor.visible = false;
    }

    public void GameRestart() {
        Debug.Log("Initiating restart...");
        gameOverScreen.SetActive(false);
    }
    
    private void OnMainMenuButtonClicked() {
        // Debug.Log("Main Menu button clicked");
        ReturnToMainMenu();
    }

    private void OnRestartButtonClicked() {
        Debug.Log("Restart button clicked");
        gameOverScreen.SetActive(false);
        restartEvent.Invoke();
    }
    
    private IEnumerator FadeAndExit() {
        var startTime = Time.time;
        // Debug.Log("INITIAL_ALPHA: " + _initialBackgroundAlpha);
        var targetElement = gameOverOverlay.rootVisualElement;

        while (true) {
            var progress = (Time.time - startTime) / FADE_DURATION;
            if (progress >= 1.0f) { break; }
            
            var alpha = Mathf.Lerp(1.0f, 0.0f, progress);
            // Debug.Log("Progress: " + progress);
            // Debug.Log("Alpha: " + alpha);
            targetElement.style.opacity = alpha;
            yield return 0;
        }
        
        targetElement.style.opacity = 0.0f;
        // once done, go to next scene
        SceneManager.LoadSceneAsync("Menu", LoadSceneMode.Single);
    }
    
    private void ReturnToMainMenu() {
        if (this._exiting) { return; }
        this._exiting = true;
        StartCoroutine(FadeAndExit());
    }
}
