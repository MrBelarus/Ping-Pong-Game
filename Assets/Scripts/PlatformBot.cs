using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformBot : MonoBehaviour
{
    public float ballSpeedIncrease = 1.2f;
    public float ballSpeedLimit = 15f;

    public float platformSpeedIncrease = 2f;
    public float platformSpeedLimit = 15f;

    float verticalMove = 0f;
    float rotation = 0f;
    public float maxYPlatformPosition = 7.8f; //если мячь попадет в угол, то мы говорим идти в эту точку (platformPos, +-maxYPlatformPosition)

    public float rayMaxDistance = 32f;
    public LayerMask layer;

    PlatformMovement platform;

    [SerializeField]
    private GameObject ball;
    private Vector2 ballVelocity;
    private float minVelocity = 0.3f;
    private float ballRadius;

    private bool goToPoint;
    private float platformXPos;
    private Vector2 platformPointToSave;
    private Vector2 futureBallPoint;

    public float reactTime = 0.45f;
    public int difficulty = 3;
    private float pan = 1;

    void Awake()
    {
        platform = gameObject.GetComponent<PlatformMovement>();
        platformXPos = transform.position.x;

        if (transform.position.x < 0)
        {
            pan = -1f;
        }

        if (ball == null)
        {
            Debug.LogError("Укажи первый мяч для бота!");
        }

        difficulty = GameData.instance.difficulty;

        switch (difficulty)
        {
            case 1:
                reactTime = 0.6f;
                break;
            case 2:
                reactTime = 0.40f;
                break;
            case 3:
                reactTime = 0.45f;
                break;
            default:
                Debug.LogError("difficulty did set up!");
                break;
        }

        ballVelocity = ball.GetComponent<Rigidbody2D>().velocity;
        ballRadius = ball.GetComponent<CircleCollider2D>().radius / 2;

        GameEvents.current.onBallDestroy += OnBallDestroy;
        GameEvents.current.onBallSpawn += OnBallSpawn;
        GameEvents.current.onPlayerSave += OnPlayerSave;

        if (difficulty == 3)
        {
            GameEvents.current.onBotSave += OnBotSave;
        }

        print(difficulty);
    }

    private void Update()
    {
        if (goToPoint)
        {
            if (verticalMove > 0 && platformPointToSave.y < transform.position.y)
            {
                goToPoint = false;
                verticalMove = 0f;
            }
            else if (verticalMove < 0 && platformPointToSave.y > transform.position.y)
            {
                goToPoint = false;
                verticalMove = 0f;
            }
        }
    }

    void FixedUpdate()
    {
        platform.Move(verticalMove, rotation);
    }
    
    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("ball"))
        {
            BallMovement ballScript = col.gameObject.GetComponent<BallMovement>();

            AudioManager.instance.PlaySound(AudioManager.Sounds.Save, pan, Random.Range(0.9f, 1.05f));

            if (ballScript.speed < ballSpeedLimit)
            {
                col.gameObject.GetComponent<BallMovement>().IncreaseBallSpeed(ballSpeedIncrease);
            }

            if (platform.speed < platformSpeedLimit)
            {
                platform.speed += 2f;
            }

            print(platform.speed);
        }
    }

    private void OnBallSpawn(GameObject ball)
    {
        verticalMove = 0f;

        this.ball = ball;
        ballVelocity = ball.GetComponent<Rigidbody2D>().velocity;
        platform.speed = platform.defaultSpeed;

        StopAllCoroutines(); //to avoid bad motion
        futureBallPoint = GetFutureBallPoint();
        changeBehaviour();
    }

    private void OnBallDestroy(GameObject ball)
    {
        StopAllCoroutines(); //to avoid bad motion

        ball = null;
        ballVelocity = Vector2.zero;

        platformPointToSave = new Vector2(platformXPos, 0f);
        if (platformPointToSave.y > transform.position.y)
        {
            verticalMove = 1f;
        }
        else
        {
            verticalMove = -1f;
        }

        goToPoint = true;
    }

    private void OnPlayerSave()
    {
        StopAllCoroutines(); //to avoid bad motion
        futureBallPoint = GetFutureBallPoint();
        changeBehaviour();
    }

    Vector2 GetFutureBallPoint()
    {
        ballVelocity = ball.GetComponent<Rigidbody2D>().velocity;

        if (ball == null)
        {
            Debug.LogError("There is no ball to calculate raycast!");
            return Vector2.zero;
        }

        if (Mathf.Abs(ballVelocity.x) < minVelocity)
        {
            return Vector2.zero;
        }

        Vector2 rayStartPos = ball.transform.position;
        float yVelocityCoef = 1;


        RaycastHit2D hit = Physics2D.Raycast(rayStartPos, ballVelocity, rayMaxDistance);
        rayStartPos = CalculateRayStartPoint(hit, rayStartPos, yVelocityCoef);

        Debug.DrawLine(ball.transform.position, rayStartPos, Color.red, 20f);

        while (hit && ((1 << hit.collider.gameObject.layer) & layer) == 0)
        {
            yVelocityCoef *= -1;
            hit = Physics2D.Raycast(rayStartPos, new Vector2(ballVelocity.x, ballVelocity.y * yVelocityCoef));
            Debug.DrawLine(rayStartPos, hit.point, Color.red, 20f);
            rayStartPos = CalculateRayStartPoint(hit, rayStartPos, yVelocityCoef);
        }

        return hit.point;
    }

    Vector2 CalculateRayStartPoint(RaycastHit2D hit, Vector2 rayStartPos, float xCoef)
    {
        if (hit.point.y > 0)
        {
            rayStartPos = hit.point - new Vector2(Mathf.Tan(Vector2.Angle(Vector2.up, ballVelocity) * Mathf.Deg2Rad * xCoef) * ballRadius, ballRadius);
        }
        else
        {
            rayStartPos = hit.point - new Vector2(Mathf.Tan(Vector2.Angle(Vector2.down, ballVelocity) * Mathf.Deg2Rad * xCoef) * ballRadius, -ballRadius);
        }

        return rayStartPos;
    }

    void OnBotSave()
    {
        platformPointToSave = new Vector2(platformXPos, 0f);
        goToPoint = true;

        if (platformPointToSave.y > transform.position.y)
        {
            verticalMove = 1f;
        }
        else
        {
            verticalMove = -1f;
        }
    }

    void changeBehaviour()
    {
        if (futureBallPoint != Vector2.zero)
        {
            if (Mathf.Abs(futureBallPoint.y) > maxYPlatformPosition)
            {
                if (futureBallPoint.y > 0)
                {
                    platformPointToSave = new Vector2(platformXPos, maxYPlatformPosition);
                }
                else
                {
                    platformPointToSave = new Vector2(platformXPos, -maxYPlatformPosition);
                }
            }
            else
            {
                platformPointToSave = new Vector2(platformXPos, futureBallPoint.y);
            }
        }
        else
        {
            platformPointToSave = new Vector2(platformXPos, 0f);
        }

        if (Mathf.Abs(transform.position.y - platformPointToSave.y) > 0.25f)
        {
            StartCoroutine(StartMotion(reactTime));
            goToPoint = true;
        }
        else
        {
            goToPoint = false;
        }

        futureBallPoint = Vector2.zero;
    }

    IEnumerator StartMotion(float delay)
    {
        yield return new WaitForSeconds(reactTime);

        if (platformPointToSave.y > transform.position.y)
        {
            verticalMove = 1f;
        }
        else
        {
            verticalMove = -1f;
        }
    }
}
