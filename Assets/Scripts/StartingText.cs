using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartingText : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Bird.GetInstance().OnStartingPlaying += birdStartedPlaying;
        Show();
    }

    private void birdStartedPlaying(object sender, System.EventArgs e)
    {
        Hide();
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
