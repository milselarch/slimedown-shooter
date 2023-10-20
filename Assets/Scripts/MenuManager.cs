using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuManager: MonoBehaviour {
    public GameObject highscoreText;
    public IntVariable gameScore;

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
        StartCoroutine(loadFade());
        Debug.Log("TEST");
    }

    IEnumerator loadFade() {
        int frames = 300;

        for (int k=0; k<frames; k++) {
            float alpha = ((float) k / frames);
            Debug.Log("ALPHA: " + alpha);

            for (int i=0; i<loadingUI.transform.childCount; i++) {
                GameObject child = loadingUI.transform.GetChild(i).gameObject;
                
                var childImage = child.GetComponent<Image>();
                if (childImage != null) {
                    Color newColor = childImage.color;  // Get the current color
                    newColor.a = alpha;  // Set the desired alpha value
                    childImage.color = newColor;  // Assign the new color with modified alpha
                }

                var uiText = child.GetComponent<TextMeshPro>();
                if (uiText != null) {
                    uiText.faceColor = new Color32(255, 255, 255, (byte) (alpha * 255));
                }
            }

            if (!this.loadGame) { break; }
            yield return null;
        }

        yield return null;
        if (this.loadGame) {
            SceneManager.LoadSceneAsync("SampleScene", LoadSceneMode.Single);
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
