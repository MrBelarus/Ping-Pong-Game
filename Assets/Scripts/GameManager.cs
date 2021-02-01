using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    //Score
    public Text playerOneScore, playerTwoScore;
    public string leftPlayerName = "LEFT PLAYER", rightPlayerName = "RIGHT PLAYER";

    //WinScreen
    public GameObject aiPlatform;
    public GameObject rightPlatform;

    //WinScreen
    public GameObject winScreen;
    public Text winner;

    //Variables
    public int maxGoals = 10;
    [HideInInspector]
    public int goals = 0;
    int leftPlayerGoals = 0, rightPlayerGoals = 0;
    bool gameOver = false;

    private int _totalAmountOfSaves = 0;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        if (GameData.instance.difficulty != -1)
        {
            rightPlatform.SetActive(false);
            aiPlatform.SetActive(true);
        }
        else
        {
            rightPlatform.SetActive(true);
            aiPlatform.SetActive(false);
        }

        playerOneScore.text = "0";
        playerTwoScore.text = "0";
    }

    void Start()
    {
        GameData gameData = GameData.instance;

        GameEvents.current.onPlayerSave += BallWasSaved;

        leftPlayerName = gameData.leftPlayerName;
        rightPlayerName = gameData.rightPlayerName;
        maxGoals = gameData.maxGoals;

        print(leftPlayerName + " " + rightPlayerName + " " + maxGoals);
    }

    void BallWasSaved() //количество отбитий подсчет
    {
        _totalAmountOfSaves++;

        print("мяч был отбит");
    }


    public void UpdateScore(bool leftPlayer)
    {
        goals++;

        if (leftPlayer)
        {
            leftPlayerGoals++;
            playerOneScore.text = leftPlayerGoals.ToString();
        }
        else
        {
            rightPlayerGoals++;
            playerTwoScore.text = rightPlayerGoals.ToString();
        }        

        if (leftPlayerGoals >= maxGoals)
        {
            Win(leftPlayerName);
        }
        else if (rightPlayerGoals >= maxGoals)
        {
            Win(rightPlayerName);
        }
    }

    public void Win(string playerName)
    {
        if (playerName == leftPlayerName)
        {
            winner.text = leftPlayerName + " " + winner.text;
        }
        else
        {
            winner.text = rightPlayerName + " " + winner.text;
        }

        if (GameData.instance.difficulty > 0 && playerName == leftPlayerName) //we play against the bot
        {
            AudioManager.instance.PlaySound(AudioManager.Sounds.Win, 0);
        }
        else if (GameData.instance.difficulty > 0)
        {
            AudioManager.instance.PlaySound(AudioManager.Sounds.Defeat, 0);
        }

        Time.timeScale = 0f;
        gameOver = true;
        winner.gameObject.SetActive(true);
        winScreen.SetActive(true);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !gameOver)
        {
            if (!winScreen.activeSelf)
            {
                Time.timeScale = 0f;
            }
            else
            {
                Time.timeScale = 1f;
            }

            winScreen.SetActive(!winScreen.activeSelf);
        }
    }
}
