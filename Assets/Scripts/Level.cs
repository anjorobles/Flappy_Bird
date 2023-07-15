using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey;
using CodeMonkey.Utils;

public class Level : MonoBehaviour
{
    private const float cameraOrthoSize = 50f;
    private const float pipeWidth = 7.8f;
    private const float pipeHeadHeight = 3.75f;

    private const float pipeMovementSpeed = 24.75f;

    private const float birdXPosition = 0f;

    private static Level instance;

    public static Level GetInstance()
    {
        return instance;
    }

    private const float destroyXPosition = -100f;
    private const float spawnPipeXPosition = +100f;
    private const float groundDestroyXPosition = -200f;
    private const float spawnCloudXPosition = 160f;
    private const float spawnCloudYPosition = 30f;
    private const float CloudDestroyXPosition = -160f;

    private int pipesSpawned;
    private float spawnTimer;
    private float spawnTimerMax;
    private float gapSize;
    private State state;

    private int pipePassedCount;

    private List<Pipe> pipeList;
    private List<Transform>groundList;
    private List<Transform> cloudList;
    private float cloudSpawnTimer;

    private enum State
    {
        WaitingToPlay,
        Playing,
        BirdDead,
    }
    private enum Difficulty
    {
        Easy,
        Medium,
        Hard,
        Extreme,
    }

    private void Awake()
    {
        instance = this;
        pipeList = new List<Pipe>();
        cloudList = new List<Transform>();
        SpawnInitialGround();
        SpawnInitialCloud();
        spawnTimerMax = 2.0f;
        gapSize = 50f;
        Bird.GetInstance().OnWaiting += birdOnWaiting;
        
    }
    void Start()
    {
        Bird.GetInstance().OnDied += birdOnDied;
        Bird.GetInstance().OnStartingPlaying += birdStartedPlaying;
    }

    private void birdOnWaiting(object sender, System.EventArgs e)
    {
        state = State.WaitingToPlay;
    }

    private void birdStartedPlaying(object sender, System.EventArgs e)
    {
       state = State.Playing;
    }


    private void birdOnDied(object sender, System.EventArgs e)
    {
        CMDebug.TextPopupMouse("Dead");
        state = State.BirdDead;
    }

    private void Update()
    {
        if(state == State.Playing)
        {
            HandlePipeMovement();
            HandleSpawnPipe();
            HandleGround();
            HandleClouds();
        }
        
    }

    private void SpawnInitialCloud()
    {
        Transform cloudTransform;
        cloudTransform = Instantiate(GetCloudPrefabTransform(), new Vector3(0, spawnCloudYPosition, 0), Quaternion.identity);
        cloudList.Add(cloudTransform);
    }

    private Transform GetCloudPrefabTransform() {
        switch (Random.Range(0, 3)) {
        default:
        case 0: return GameAssets.GetInstance().cloud1;
        case 1: return GameAssets.GetInstance().cloud2;
        case 2: return GameAssets.GetInstance().cloud3;
        }
    }

    private void HandleClouds() {
        // Handle Cloud Spawning
        cloudSpawnTimer -= Time.deltaTime;
        if (cloudSpawnTimer < 0) {
            // Time to spawn another cloud
            float cloudSpawnTimerMax = 8f;
            cloudSpawnTimer = cloudSpawnTimerMax;
            Transform cloudTransform = Instantiate(GetCloudPrefabTransform(), new Vector3(spawnCloudXPosition, spawnCloudYPosition, 0), Quaternion.identity);
            cloudList.Add(cloudTransform);
        }

        // Handle Cloud Moving
        for (int i=0; i<cloudList.Count; i++) {
            Transform cloudTransform = cloudList[i];
            // Move cloud by less speed than pipes for Parallax
            cloudTransform.position += new Vector3(-1, 0, 0) * pipeMovementSpeed * Time.deltaTime * .7f;

            if (cloudTransform.position.x < CloudDestroyXPosition) {
                // Cloud past destroy point, destroy self
                Destroy(cloudTransform.gameObject);
                cloudList.RemoveAt(i);
                i--;
            }
        }
    }

    private void SpawnInitialGround()
    {
        groundList = new List<Transform>();
        Transform groundTransform;
        float groundY = -48.5f;
        float groundWidth = 183f;
        groundTransform  = Instantiate(GameAssets.GetInstance().ground, new Vector3 (0, groundY, 0), Quaternion.identity);
        groundList.Add(groundTransform);
        groundTransform  = Instantiate(GameAssets.GetInstance().ground, new Vector3 (groundWidth, groundY, 0), Quaternion.identity);
        groundList.Add(groundTransform);
       // groundTransform  = Instantiate(GameAssets.GetInstance().ground, new Vector3 (groundWidth * 2f, groundY, 0), Quaternion.identity);
        //groundList.Add(groundTransform);
    }

    private void HandleGround()
    {
        foreach(Transform groundTransform in groundList)
        {
            groundTransform.position += new Vector3 (-1, 0, 0) * pipeMovementSpeed * Time.deltaTime;

            if(groundTransform.position.x < groundDestroyXPosition)
            {
                //ground passed the left side, relocate on right side
                //find right most x position
                float rightMostXPosition = -100f;
                for(int i = 0; i < groundList.Count; i++)
                {
                    if(groundList[i].position.x > rightMostXPosition)
                    {
                        rightMostXPosition = groundList[i].position.x;
                    }
                }
                //place ground on the right most position
                float groundWidth = 183f;
                groundTransform.position = new Vector3 (rightMostXPosition + groundWidth, groundTransform.position.y, groundTransform.position.z);
            }
        }
    }

    private void HandleSpawnPipe(){
        spawnTimer -= Time.deltaTime;
        if(spawnTimer < 0)
        {
            float heightEdgeLimit = 10f;
            float minHeight = gapSize * 0.5f + heightEdgeLimit;
            float totalHeight = cameraOrthoSize * 2f;
            float maxHeight  = totalHeight - gapSize * 0.5f - heightEdgeLimit;
            float height = Random.Range (minHeight, maxHeight);
            spawnTimer += spawnTimerMax;
            CreateGapPipes(height, gapSize, spawnPipeXPosition);
        }
    }

    private void HandlePipeMovement()
    {
        for(int i =0; i<pipeList.Count; i++)
        {
            Pipe pipe = pipeList[i];
            bool isToTheRightofBird = pipe.GetXposition() > birdXPosition;
            pipe.Move();
            if(isToTheRightofBird && pipe.GetXposition() <= birdXPosition && pipe.IsBottom())
            {
                pipePassedCount++;
                SoundManager.PlaySound(SoundManager.Sound.Score);
            }
            // for destroying pipe
            if(pipe.GetXposition() < destroyXPosition)
            {
                pipe.destroyPipe();
                pipeList.Remove(pipe);
                i--;
            }
        }
    }

    private void SetDifficulty(Difficulty difficulty){
        switch(difficulty){

            case Difficulty.Easy:
                gapSize = 50f;
                spawnTimerMax = 2.0f;
                break;
            case Difficulty.Medium:
                gapSize = 40f;
                spawnTimerMax = 1.8f;
                break;
            case Difficulty.Hard:
                gapSize = 35f;
                spawnTimerMax = 1.5f;
                break;
            case Difficulty.Extreme:
                gapSize = 25f;
                spawnTimerMax = 1.3f;
                break;
        }
        
    }

    private Difficulty GetDifficulty()
    {
        if(pipesSpawned >= 30) return Difficulty.Extreme;
        if(pipesSpawned >= 20) return Difficulty.Hard;
        if(pipesSpawned >= 10) return Difficulty.Medium;
        return Difficulty.Easy;
    }

    public int GetPipesSpawned()
    {
        return pipesSpawned;
    }

    public int GetpipePassedCount()
    {
        return pipePassedCount;
    }

    private void CreateGapPipes(float gapY, float gapSize, float xPosition)
    {
        CreatePipe(gapY - gapSize * 0.5f, xPosition, true);
        CreatePipe(cameraOrthoSize * 2f - gapY -gapSize *0.5f, xPosition, false);
        pipesSpawned ++;
        SetDifficulty(GetDifficulty());
    }

    private void CreatePipe(float height, float xPosition, bool createBottom)
    {
        //set up for pipe head
        float pipeHeadYPosition;
        Transform pipeHead = Instantiate(GameAssets.GetInstance().pipeHead);
        if(createBottom)
        {
           pipeHeadYPosition = -cameraOrthoSize + height  - pipeHeadHeight * 0.5f;
        }else{

            pipeHeadYPosition = +cameraOrthoSize - height  + pipeHeadHeight * 0.5f;
        }
        pipeHead.position = new Vector3 (xPosition, pipeHeadYPosition);
        
        // set up for pipe body
        Transform pipeBody = Instantiate(GameAssets.GetInstance().pipeBody);

        if(createBottom)
        {
            pipeHeadYPosition = -cameraOrthoSize;
        }else{
            pipeHeadYPosition = +cameraOrthoSize;
            pipeBody.localScale = new Vector3 (1, -1, 1);
        }
        pipeBody.position = new Vector3 (xPosition, pipeHeadYPosition);

        SpriteRenderer pipeBodySpriteRenderer = pipeBody.GetComponent<SpriteRenderer>();
        pipeBodySpriteRenderer.size = new Vector2 (pipeWidth, height);

        BoxCollider2D pipeBodyBoxCollider  = pipeBody.GetComponent<BoxCollider2D>();
        pipeBodyBoxCollider.size = new Vector2 (pipeWidth, height);
        pipeBodyBoxCollider.offset = new Vector2 (0f, height * 0.5f);

        Pipe pipe = new Pipe(pipeBody, pipeHead, createBottom);
        pipeList.Add(pipe);
    }

    private class Pipe 
    {
        private Transform pipeHeadTransform;
        private Transform pipeBodyTransform;

        private bool isBottom;

        public Pipe (Transform pipeHeadTransform, Transform pipeBodyTransform, bool isBottom)
        {
            this.pipeHeadTransform = pipeHeadTransform;
            this.pipeBodyTransform = pipeBodyTransform;
            this.isBottom = isBottom;
        }

        public void Move()
        {
            pipeHeadTransform.position += new Vector3 (-1, 0, 0) * pipeMovementSpeed * Time.deltaTime;
            pipeBodyTransform.position += new Vector3 (-1, 0, 0) * pipeMovementSpeed * Time.deltaTime;
        }

        public float GetXposition()
        {
            return pipeHeadTransform.position.x;
        }

        public bool IsBottom()
        {
            return isBottom;
        }

        public void destroyPipe()
        {
            Destroy(pipeHeadTransform.gameObject);
            Destroy(pipeBodyTransform.gameObject);
        }
    }
}
