using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallBorder : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("ball"))
        {
            BallMovement ballMovement = collision.gameObject.GetComponent<BallMovement>();

            GameEvents.current.OnBallDestroy(collision.gameObject);
            ballMovement.SpawnBall();
        }
    }
}
