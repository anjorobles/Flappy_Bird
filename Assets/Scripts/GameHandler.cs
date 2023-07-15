using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey;
using CodeMonkey.Utils;

public class GameHandler : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Start");
        Score.Start();
        Debug.Log("score start");

        //Score.TryNewGetHigscore(20);

        //GameObject gameobject = new GameObject("Pipe", typeof(SpriteRenderer));
        //gameobject.GetComponent<SpriteRenderer>().sprite = GameAssets.GetInstance().pipeHeadSprite;

       
    }

    
}
