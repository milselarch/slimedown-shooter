using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using TMPro;
using UnityEngine.UIElements;
using Slider = UnityEngine.UI.Slider;

public class StartMenuManager: MonoBehaviour {
    public UIDocument startMenu;
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
    }

    public void LoadLevel() {
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
