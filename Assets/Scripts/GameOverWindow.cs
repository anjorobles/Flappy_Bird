using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CodeMonkey.Utils;

public class GameOverWindow : MonoBehaviour
{
    private Text highScoreText;
    private Text scoreText;

    private void Awake()
    {
        scoreText = transform.Find("scoreText").GetComponent<Text>();
        highScoreText = transform.Find("highScoreText").GetComponent<Text>();

        transform.Find("retryButton").GetComponent<Button_UI>().ClickFunc = () => {Loader.Load(Loader.Scene.GameScene); };
        transform.Find("retryButton").GetComponent<Button_UI>().AddButtonSound();

        transform.Find("MainMenuButton").GetComponent<Button_UI>().ClickFunc = () => {Loader.Load(Loader.Scene.MainMenu); };
        transform.Find("MainMenuButton").GetComponent<Button_UI>().AddButtonSound();
        
    }

    private void Start()
    {
        Bird.GetInstance().OnDied += birdOnDied;
        Hide();
    }

    private void birdOnDied(object sender, System.EventArgs e)
    {
        scoreText.text = Level.GetInstance().GetpipePassedCount().ToString();

        int score = Level.GetInstance().GetpipePassedCount();
        int currentHghscore = Score.GetHighScore();

        if(score < currentHghscore)
        {
            highScoreText.text = "HIGHSCORE " + Score.GetHighScore();
            
        }else
        {
            highScoreText.text = "NEW HIGHSCORE!";
        }
        
        Show();
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
