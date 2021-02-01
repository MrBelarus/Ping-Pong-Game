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

            GameEvents.current.OnBallDestroy(col.gameObject);

            AudioManager.instance.PlaySound(AudioManager.Sounds.Goal, pan, Random.Range(0.8f, 1f));

            Destroy(col.gameObject);

            GameObject newBall = Instantiate(col.gameObject, new Vector3(0, 0, 0), Quaternion.identity);

            StartCoroutine(StartMotion(newBall, goalDelay));
        }
    }

    IEnumerator StartMotion(GameObject ball, float delay)
    {
        yield return new WaitForSeconds(delay);

        leftPlatform.speed = leftPlatform.defaultSpeed;     //меняем скорость
        rightPlatform.speed = rightPlatform.defaultSpeed;   //каждой платформы к обычной

        ball.GetComponent<BallMovement>().enabled = true;
        ball.GetComponent<Collider2D>().enabled = true;
    }
}
