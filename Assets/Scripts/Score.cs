using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Score
{

    public static void Start()
    {
        //ResetHighscore();
        Bird.GetInstance().OnDied += birdOnDied;
    }

    private static void birdOnDied(object sender, System.EventArgs e)
    {
        TryNewGetHigscore(Level.GetInstance().GetpipePassedCount());
    }

    public static int GetHighScore()
    {
        return PlayerPrefs.GetInt("highscore");
    }

    public static bool TryNewGetHigscore(int score)
    {
        int currentHghscore = Score.GetHighScore();
        if(score > currentHghscore )
        {
            //save new highscore
            PlayerPrefs.SetInt("highscore", score);
            PlayerPrefs.Save();
            return true;

        }else{
            //return current highscore
            return false;
        }
    }

    public static void ResetHighscore()
    {
        PlayerPrefs.SetInt("highscore", 0);
        PlayerPrefs.Save();
    }
}
