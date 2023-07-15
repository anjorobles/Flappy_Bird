using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAssets : MonoBehaviour
{
    private static GameAssets instance;

    public static GameAssets GetInstance()
    {
        return instance;
    }

    private void Awake()
    {
        instance = this;
    }

    public Sprite pipeHeadSprite;
    public Sprite pipeBodySprite;
    public Transform pipeHead;
    public Transform pipeBody;
    public Transform ground;

    public Transform cloud1;
    public Transform cloud2;
    public Transform cloud3;

    public SoundAudioClip[] soundAudioClipArray;

    [Serializable]
    public class SoundAudioClip
    {
        public SoundManager.Sound sound;
        public AudioClip audioClip;
    }

    
}
