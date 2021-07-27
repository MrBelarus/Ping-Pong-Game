using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class goalZoneTrigger : MonoBehaviour
{
    public bool rightGoalZone;
    public float goalDelay = 2f;

    private float pan = 1f;

    public PlatformMovement leftPlatform, rightPlatform;

    private void Start()
    {
        if (!rightGoalZone)
        {
            pan = -pan;
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("ball"))
        {
            GameManager.instance.UpdateScore(rightGoalZone);

            if (GameManager.instance.GameOver)
            {
                col.GetComponent<BallMovement>().IncreaseBallSpeed(0f);
                return;
            }

            GameEvents.current.OnBallDestroy(col.gameObject);
            AudioManager.instance.PlaySound(AudioManager.Sounds.Goal, pan, Random.Range(0.8f, 1f));

            col.GetComponent<BallMovement>().SpawnBall(1f);
        }
    }
}
