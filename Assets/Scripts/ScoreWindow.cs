using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreWindow : MonoBehaviour
{
    private Text scoreText;
    private Text highscoreText;

    private void Awake()
    {
        scoreText = transform.Find("ScoreText").GetComponent<Text>();
        highscoreText = transform.Find("highscoreText").GetComponent<Text>();
    }

    private void Start()
    {
        Bird.GetInstance().OnStartingPlaying += birdStartedPlaying;
        Hide();
    }

    private void birdStartedPlaying(object sender, System.EventArgs e)
    {
        highscoreText.text = "HIGHSCORE: " + Score.GetHighScore();
        Show();
    }

    private void Update()
    {
        if (scoreText != null)
        {
            scoreText.text = Level.GetInstance().GetpipePassedCount().ToString();
        }else{
            Debug.Log("NULL");
        }

    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }
}
