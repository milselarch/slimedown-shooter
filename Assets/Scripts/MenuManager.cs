using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuManager: MonoBehaviour {
    public GameObject highscoreText;
    public IntVariable gameScore;

    public Slider progressBar;
    public GameObject menuUI;
    public GameObject loadingUI;
    public Image image;  // Assign the Image component from the Unity Editor

    private bool loadGame = false;

    // Start is called before the first frame update
    void Start() {
        setHighScore();
    }

    void Awake() {
        this.loadMenu();
    }

    public void loadMenu() {
        menuUI.SetActive(true);
        loadingUI.SetActive(false);
        this.loadGame = false;
    }

    public void loadLevel() {
        menuUI.SetActive(false);
        loadingUI.SetActive(true);
        
        this.loadGame = true;
        StartCoroutine(launchLoadSequence());
        Debug.Log("TEST");
    }

    IEnumerator launchLoadSequence()
    {
        if (!this.loadGame)
        {
            yield return null;
        }

        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(
            "SampleScene", LoadSceneMode.Single
        );

        while (!loadOperation.isDone)
        {
            float progress = Mathf.Clamp01(loadOperation.progress / .9f);
            progressBar.value = progress;
            yield return null;
        }
    }

    public void resetHighScore() {
        this.gameScore.ResetHighestValue();
        this.setHighScore();
    }

    public void setHighScore() {
        // set highscore
        highscoreText.GetComponent<TextMeshProUGUI>().text = (
            "TOP- " + gameScore.previousHighestValue.ToString("D6")
        );
    }

    // Update is called once per frame
    void Update() {
        
    }
}
