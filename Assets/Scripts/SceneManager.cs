using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SceneManager : MonoBehaviour
{
    // camera
    [SerializeField]
    CameraZoom CameraZoomScript;

    // the player
    [SerializeField]
    GameObject Player;

    // the ground
    [SerializeField]
    GameObject[] Ground;

    // the pipes
    [SerializeField]
    GameObject[] TopPipes;
    [SerializeField]
    GameObject[] BottomPipes;
    bool[] pipeIsScorable = new bool[3];

    // titles and messages
    [SerializeField]
    GameObject HUDTitle;
    [SerializeField]
    GameObject HUDStartButton;
    [SerializeField]
    GameObject HUDGetReady;
    [SerializeField]
    GameObject HUDTutorial;
    [SerializeField]
    GameObject HUDWrecked;
    [SerializeField]
    GameObject HUDGameover;
    [SerializeField]
    GameObject HUDScoreCard;
    [SerializeField]
    GameObject HUDFinalScore;
    [SerializeField]
    GameObject HUDTopScore;
    [SerializeField]
    GameObject HUDScore;

    // scrolling scene movement
    Vector2 sceneMovement;
    Vector2 sceneScrollSpeed = new Vector2(3f, 3f);
    Vector2 sceneScrollDirection = new Vector2(-1f, 0f);

    // timers user to decide how long to display messages
    float wreckTimer = 0;
    float scoreTimer = 0;
    float wreckTimerMax = 1.5f;
    float scoreTimerMax = 2.0f;

    AudioSource audioSource;
    [SerializeField]
    AudioClip BlipSound;

    void Awake()
    {
        Application.targetFrameRate = 60;

        CameraZoomScript.SetOrthographicSize();

        Globals.BestScore = Globals.LoadFromPlayerPrefs(Globals.BestScorePlayerPrefsKey);

        wreckTimer = wreckTimerMax;
        scoreTimer = scoreTimerMax;

        audioSource = this.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Globals.CurrentGameState == Globals.GameState.TitleScreen)
        {
            UpdateTitleScreenState();
        }
        else if (Globals.CurrentGameState == Globals.GameState.GetReady)
        {
            UpdateGetReadyState();
        }
        else if (Globals.CurrentGameState == Globals.GameState.Playing)
        {
            UpdatePlaying();
        }
        else if (Globals.CurrentGameState == Globals.GameState.Dead)
        {
            UpdateDead();
        }
        else if (Globals.CurrentGameState == Globals.GameState.Score)
        {
            UpdateScore();
        }
        else if (Globals.CurrentGameState == Globals.GameState.ScoreRestart)
        {
            UpdateScoreRestart();
        }
    }

    void FixedUpdate()
    {
        for (int i = 0; i < TopPipes.Length; i++)
        {
            TopPipes[i].GetComponent<Rigidbody2D>().velocity = sceneMovement;
            // update score
            if (TopPipes[i].transform.localPosition.x < -3.0 && pipeIsScorable[i])
            {
                pipeIsScorable[i] = false;
                Globals.CurrentScore++;
                HUDScore.GetComponent<TextMeshPro>().text = Globals.CurrentScore.ToString();
            }
        }
        for (int i = 0; i < BottomPipes.Length; i++)
        {
            BottomPipes[i].GetComponent<Rigidbody2D>().velocity = sceneMovement;
        }
        for (int i = 0; i < Ground.Length; i++)
        {
            Ground[i].GetComponent<Rigidbody2D>().velocity = sceneMovement;
        }
    }

    void UpdateTitleScreenState()
    {
        if (Input.GetKeyDown ("space") || Input.GetButtonDown ("Fire1") || Input.GetButtonDown ("Fire2"))
        {
            audioSource.PlayOneShot(BlipSound, 1f);
            InitGetReady();
            Globals.CurrentGameState = Globals.GameState.GetReady;
        }
    }

    void InitGetReady()
    {
        Globals.CurrentScore = 0;
        HUDScore.SetActive(false);
        HUDTitle.SetActive(false);
        HUDStartButton.SetActive(false);
        HUDTutorial.SetActive(true);
        HUDGetReady.SetActive(true);
        HUDGameover.SetActive(false);
        HUDScoreCard.SetActive(false);
        HUDFinalScore.SetActive(false);
        HUDTopScore.SetActive(false);
        Player.transform.localPosition = new Vector2(-1.85f, 2f);
        Player.SetActive(true);
        InitPipes();
    }

    void UpdateGetReadyState()
    {
        if (Input.GetKeyDown ("space") || Input.GetButtonDown ("Fire1") || Input.GetButtonDown ("Fire2"))
        {
            audioSource.PlayOneShot(BlipSound, 1f);
            HUDScore.GetComponent<TextMeshPro>().text = Globals.CurrentScore.ToString();

            HUDTutorial.SetActive(false);
            HUDGetReady.SetActive(false);
            HUDScore.SetActive(true);
            Globals.CurrentGameState = Globals.GameState.Playing;
        }
    }

    void UpdatePlaying()
    {
        sceneMovement = new Vector2 (sceneScrollSpeed.x * sceneScrollDirection.x, sceneScrollSpeed.y * sceneScrollDirection.y);
        float groundMinX = -7.5f;
        for (int i = 0; i < Ground.Length; i++)
        {
            if (Ground[i].transform.localPosition.x < groundMinX)
            {
                int abutIndex = i == Ground.Length - 1 ? 0 : i + 1;
                Ground[i].transform.localPosition = new Vector2(
                        Ground[abutIndex].transform.localPosition.x + Ground[abutIndex].GetComponent<Renderer>().bounds.size.x,
                        Ground[i].transform.localPosition.y
                    );
            }
        }

        float pipeMinX = -6f;
        for (int i = 0; i < TopPipes.Length; i++)
        {
            if (TopPipes[i].transform.localPosition.x < pipeMinX)
            {
                // randomize the y position of pipes
                float startY = Random.Range (0.0f, 4f);
                Vector2 newTopPos = new Vector2 (9f, 10f - startY);
                Vector2 newBottomPos = new Vector2 (9f, -2.85f - startY);
                TopPipes[i].transform.localPosition = newTopPos;
                BottomPipes[i].transform.localPosition = newBottomPos;

                pipeIsScorable[i] = true;
            }
        }
    }

    void UpdateDead()
    {
        HUDWrecked.transform.localPosition = new Vector2(HUDWrecked.transform.localPosition.x, Globals.WreckedYpos);
        HUDWrecked.SetActive(true);

        sceneMovement = new Vector2(0, 0);

        wreckTimer -= Time.deltaTime;
        if (wreckTimer <= 0)
        {
            wreckTimer = wreckTimerMax;
            InitScore();
            Globals.CurrentGameState = Globals.GameState.Score;
        }
    }

    void InitScore()
    {
        if (Globals.CurrentScore > Globals.BestScore)
        {
            Globals.BestScore = Globals.CurrentScore;
            Globals.SaveToPlayerPrefs(Globals.BestScorePlayerPrefsKey, Globals.BestScore);
        }

        HUDFinalScore.GetComponent<TextMeshPro>().text = Globals.CurrentScore.ToString();
        HUDTopScore.GetComponent<TextMeshPro>().text = Globals.BestScore.ToString();

        HUDScore.SetActive(false);
        HUDWrecked.SetActive(false);
        HUDGameover.SetActive(true);
        HUDScoreCard.SetActive(true);
        HUDFinalScore.SetActive(true);
        HUDTopScore.SetActive(true);
    }

    void UpdateScore()
    {
        scoreTimer-=Time.deltaTime;
        if (scoreTimer <= 0)
        {
            scoreTimer = scoreTimerMax;
            InitScoreRestart();
            Globals.CurrentGameState = Globals.GameState.ScoreRestart;
        }
    }

    void InitScoreRestart()
    {
        sceneMovement = new Vector2(0, 0);
        HUDStartButton.SetActive(true);
    }

    void UpdateScoreRestart()
    {
        if (Input.GetKeyDown ("space") || Input.GetButtonDown ("Fire1") || Input.GetButtonDown ("Fire2"))
        {
            audioSource.PlayOneShot(BlipSound, 1f);
            InitGetReady();
            Globals.CurrentGameState = Globals.GameState.GetReady;
        }
    }

    void InitPipes()
    {
        for (int i = 0; i < TopPipes.Length; i++)
        {
            // randomize the y position of pipes
            float startY = Random.Range (0.0f, 3.5f);
            Vector2 newTopPos = new Vector2 (9f + i * 5f, 8.9f - startY);
            Vector2 newBottomPos = new Vector2 (9f + i * 5f, -3.85f - startY);
            TopPipes[i].transform.localPosition = newTopPos;
            BottomPipes[i].transform.localPosition = newBottomPos;
        }
        for (int i = 0; i < pipeIsScorable.Length; i++)
        {
            pipeIsScorable[i] = true;
        }
    }

}
