using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public IntVariable gameScore;
    public IntVariable health;

    public TextMeshProUGUI gameScoreText;
    public TextMeshProUGUI healthText;
    
    // Start is called before the first frame update
    void Start()
    {
        gameScore.SetValue(0);
        health.SetValue(10);
        UpdateUI();
    }

    public void UpdateUI()
    {
        Debug.Log("HEALTH_UPDATE");
        gameScoreText.SetText("SCORE: " + gameScore.Value);
        healthText.SetText("HEALTH: " + health.Value);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
