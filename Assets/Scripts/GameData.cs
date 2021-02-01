using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData : MonoBehaviour
{
    public static GameData instance;

    public string leftPlayerName = "LEFT PLAYER";
    public string rightPlayerName = "RIGHT PLAYER";
    public int maxGoals = 5;
    public int difficulty = -1;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }
}
