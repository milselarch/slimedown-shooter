using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using TMPro;
using UnityEngine.Assertions;
using UnityEngine.UIElements;
using Slider = UnityEngine.UI.Slider;

public class StartMenuManager: MonoBehaviour {
    public UIDocument startMenu;
    private Button _startButton;
    
    public GameObject highscoreText;
    public IntVariable gameScore;

    public Slider progressBar;
    // public GameObject menuUI;
    public GameObject loadingUI;
    public GameObject bouncingSlime;

    private bool _loadGame = false;

    // Start is called before the first frame update
    void Start() {
        SetHighScore();
    }

    void Awake() {
        this.LoadMenu();
    }

    private void LoadMenu() {
        // menuUI.SetActive(true);
        startMenu.rootVisualElement.style.visibility = Visibility.Visible;
        loadingUI.SetActive(false);
        this._loadGame = false;
        
        this._startButton = startMenu.rootVisualElement.Q<Button>("start_game");
        Assert.IsNotNull(this._startButton);
        this._startButton.RegisterCallback<ClickEvent>(evt => {
            if (evt.button == (int) MouseButton.LeftMouse) {
                LoadLevel();
            }
        });
        // this._startButton.clickable.clicked += LoadLevel;
        Debug.Log("START_BUTTON_ATTACHED " + this._startButton);
    }

    public void LoadLevel() {
        Debug.Log("START_BUTTON_PRESSED");
        if (this._loadGame) { return; }
        // menuUI.SetActive(false);
        startMenu.rootVisualElement.style.visibility = Visibility.Hidden;
        loadingUI.SetActive(true);
        
        this._loadGame = true;
        StartCoroutine(LaunchLoadSequence());
        Debug.Log("TEST");
    }

    IEnumerator LaunchLoadSequence() {
        if (!this._loadGame) {
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
        highscoreText.GetComponent<TextMeshProUGUI>().text = (
            "HIGHSCORE: " + gameScore.previousHighestValue.ToString("D6")
        );
    }
}
