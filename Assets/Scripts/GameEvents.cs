using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEvents : MonoBehaviour
{
    public static GameEvents current;

    void Awake()
    {
        current = this;
    }

    public event Action<GameObject> onBallSpawn;
    public void OnBallSpawn(GameObject obj)
    {
        if (onBallSpawn != null)
        {
            onBallSpawn(obj);
        }
    }

    public event Action<GameObject> onBallDestroy;
    public void OnBallDestroy(GameObject obj)
    {
        if (onBallDestroy != null)
        {
            onBallDestroy(obj);
        }
    }

    public event Action onPlayerSave;
    public void OnPlayerSave()
    {
        if (onPlayerSave != null)
        {
            onPlayerSave();
        }
    }

    public event Action onBotSave;
    public void OnBotSave()
    {
        if (onBotSave != null)
        {
            onBotSave();
        }
    }
}
