using System.Collections;
using System.Collections.Generic;
using ScriptableObjects;
using UnityEngine.SceneManagement;
using UnityEngine;
using TMPro;
using UnityEngine.Assertions;
using UnityEngine.UIElements;
using Slider = UnityEngine.UI.Slider;

public class StartMenuManager: MonoBehaviour {
    public UIDocument startMenu;
    private Button _startButton;
    private Label _highscoreLabel;
    
    // TODO: remove deprecated high score text
    public GameObject highscoreText;
    public IntVariable gameScore;
    public BoolVariable resetGame;

    public Slider progressBar;
    // public GameObject menuUI;
    public GameObject loadingUI;
    public GameObject bouncingSlime;

    private bool _loadingGame = false;
    
    void OnEnable() {
        this.LoadMenu();
        resetGame.LoadFromPreviousValue();
        if (resetGame.value) {
            Debug.Log("RESET_HIGHSCORE");
            ResetHighScore();
            resetGame.SetValue(false);
        }
        resetGame.LoadFromPreviousValue();
        SetHighScore();
    }

    private void LoadMenu() {
        // menuUI.SetActive(true);
        startMenu.rootVisualElement.style.visibility = Visibility.Visible;
        loadingUI.SetActive(false);
        this._loadingGame = false;
        
        this._startButton = startMenu.rootVisualElement.Q<Button>("start_game");
        Assert.IsNotNull(this._startButton);
        this._startButton.RegisterCallback<ClickEvent>(evt => {
            if (evt.button == (int) MouseButton.LeftMouse) {
                LoadLevel();
            }
        });
        this._highscoreLabel = startMenu.rootVisualElement.Q<Label>("highscore");
        this.SetHighScore();
        // this._startButton.clickable.clicked += LoadLevel;
        Debug.Log("START_BUTTON_ATTACHED " + this._startButton);
    }

    public void LoadLevel() {
        Debug.Log("START_BUTTON_PRESSED");
        if (this._loadingGame) { return; }
        // menuUI.SetActive(false);
        startMenu.rootVisualElement.style.visibility = Visibility.Hidden;
        loadingUI.SetActive(true);
        
        this._loadingGame = true;
        StartCoroutine(LaunchLoadSequence());
        Debug.Log("TEST");
    }

    IEnumerator LaunchLoadSequence() {
        if (!this._loadingGame) {
            yield return null;
        }
        
        bouncingSlime.GetComponent<UISpriteAnimation>().StartAnimation();
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(
            "SampleScene", LoadSceneMode.Single
        );

        while (!loadOperation.isDone) {
            var progress = Mathf.Clamp01(loadOperation.progress / .9f);
            progressBar.value = progress;
            yield return null;
        }
    }

    public void ResetHighScore() {
        this.gameScore.ResetHighestValue();
        this.SetHighScore();
    }

    private void SetHighScore() {
        // set highscore
        var scoreText = "HIGHSCORE: " + gameScore.previousHighestValue.ToString("D6");
        highscoreText.GetComponent<TextMeshProUGUI>().text = scoreText;
        _highscoreLabel.text = scoreText;
    }
}
