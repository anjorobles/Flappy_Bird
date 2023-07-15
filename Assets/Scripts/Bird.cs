
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey;


public class Bird : MonoBehaviour
{
    private const float JumpAmount = 114f;

    private static Bird instance;

    public static Bird GetInstance()
    {
        return instance;
    }

    public event EventHandler OnDied;
    public event EventHandler OnStartingPlaying;
    public event EventHandler OnWaiting;
    private Rigidbody2D birdRigidBody;
    private State state;

    private enum State 
    {
        WaitingToPlay,
        Playing,
        Dead,
    }
    private void Awake()
    {
        instance = this;
        birdRigidBody = GetComponent<Rigidbody2D>();
        birdRigidBody.bodyType = RigidbodyType2D.Static;
    }
    void Update()
    {
        switch(state)
        {
            default:
            case State.WaitingToPlay:
                if(Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
                    {
                        state = State.Playing;
                        birdRigidBody.bodyType = RigidbodyType2D.Dynamic;
                        Jump();
                        if(OnStartingPlaying != null) OnStartingPlaying(this, EventArgs.Empty);
                    }
            break;
            case State.Playing:
                if(Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
                    {
                        Jump();
                    }

                    transform.eulerAngles = new Vector3 (0, 0, birdRigidBody.velocity.y * 0.4f);
            break;
            case State.Dead:
            break;
        }
        
    }

    private void Jump()
    {
        birdRigidBody.velocity = Vector2.up * JumpAmount;
        SoundManager.PlaySound(SoundManager.Sound.BirdJump);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        birdRigidBody.bodyType = RigidbodyType2D.Static;
        if(OnDied != null) OnDied(this, EventArgs.Empty);
        SoundManager.PlaySound(SoundManager.Sound.Lose);
    }
}
