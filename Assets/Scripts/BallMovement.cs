using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallMovement : MonoBehaviour
{
    public Rigidbody2D rb;

    [SerializeField]
    private TrailRenderer trailRenderer;

    public float defaultSpeed = 5f;
    [HideInInspector]
    public float speed;

    public int deltaAngleVelocity = 60;

    private float angle = 0;
    private Vector2 velocity;

    public LayerMask platformLayer;

    public float maxWallHitDistance = 18.6f;

    [SerializeField]
    private float initRandomVelDelay = 0.1f;
    [SerializeField]
    private float spawnAppearDelay = 0.5f;

    private Vector2 initSpawnPos;
    public Vector2 InitSpawnPos
    {
        set => initSpawnPos = value;
        get => initSpawnPos;
    }

    [SerializeField]
    private SpriteRenderer ballSprite;

    void Start()
    {
        initSpawnPos = transform.position;
        SpawnBall();
    }

    public void SpawnBall(float delay)
    {
        StartCoroutine(SpawnWithDelay(delay));
    }

    public void SpawnBall()
    {
        speed = defaultSpeed;

        rb.velocity = Vector2.zero;
        transform.position = initSpawnPos;

        trailRenderer.Clear();

        DoVisibilityTransition(0f, 1f, spawnAppearDelay, AnimationCurve.EaseInOut(0f, 0f, 1f, 1f));
        RandomVelocityWithDelay(initRandomVelDelay);
    }

    public void IncreaseBallSpeed(float coef = 1f)  //при соприкосновении шарика с любой из платформ
    {
        rb.velocity *= coef;
        speed *= coef;
    }

    public void RandomVelocity()
    {
        angle = Random.Range(2f, deltaAngleVelocity / 2) * Mathf.PI / 180f;

        if (Random.Range(0, 2) == 1)    //рандом угла вниз/вверх от горизонта
        {
            angle *= -1;
        }

        if (Random.Range(0, 2) == 1)    //рандом направления мяча (влево/вправо)
        {
            angle += Mathf.PI;
        }

        velocity = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * speed;

        rb.velocity += velocity;

        //velocity = new Vector2(-1, 0) * speed; //delete it after testing and uncomment lines above!
        //rb.velocity += velocity;

        GameEvents.current.OnBallSpawn(gameObject);
    }

    public void RandomVelocityWithDelay(float delay)
    {
        StartCoroutine(RandomVelocityWithDelayCoroutine(delay));
    }

    IEnumerator RandomVelocityWithDelayCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        RandomVelocity();
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (((1 << collision.gameObject.layer) & platformLayer) != 0) //check if ball hits platforms -> recalculate ball speed if it's need
        {
            if (Mathf.Pow(Mathf.Abs(rb.velocity.x * rb.velocity.x) + Mathf.Abs(rb.velocity.y * rb.velocity.y), 0.5f) > speed + speed / 10)
            {
                print(speed);
                print("reculculated speed!");

                float vectorAttitude = Mathf.Pow(Mathf.Abs(rb.velocity.x * rb.velocity.x) + Mathf.Abs(rb.velocity.y * rb.velocity.y), 0.5f) / speed;
                rb.velocity /= vectorAttitude / 1.7f;

                //rb.velocity = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad) * speed * xSpeedCoef,
                //    Mathf.Sin(angle * Mathf.Deg2Rad) * speed * ySpeedCoef) * 1.7f;                  //add speed coef 1.7f
            }
        }


        if (collision.gameObject.CompareTag("Player"))
        {
            GameEvents.current.OnPlayerSave();
        }

        if (collision.gameObject.CompareTag("Bot"))
        {
            GameEvents.current.OnBotSave();
        }

        if (collision.gameObject.CompareTag("Wall"))
        {
            print("Wall pan: " + transform.position.x / maxWallHitDistance);
            AudioManager.instance.PlaySound(AudioManager.Sounds.WallHit, transform.position.x / maxWallHitDistance, Random.Range(0.9f, 1.05f));
        }
    }

    private void DoVisibilityTransition(float alphaStart, float alphaEnd, float time, AnimationCurve curve = null)
    {
        StartCoroutine(VisibilityTransition(alphaStart, alphaEnd, time, curve));
    }

    IEnumerator VisibilityTransition(float alphaStart, float alphaEnd, float time, AnimationCurve curve = null)
    {
        Color start = ballSprite.color;
        start.a = alphaStart;
        Color end = start;
        end.a = alphaEnd;

        if (curve != null)
        {
            for (float i = 0; i < 1f; i += Time.deltaTime / time)
            {
                ballSprite.color = Color.Lerp(start, end, curve.Evaluate(i));
                yield return new WaitForFixedUpdate();
            }
        }
        else
        {
            for (float i = 0; i < 1f; i += Time.deltaTime / time)
            {
                ballSprite.color = Color.Lerp(start, end, i);
                yield return new WaitForFixedUpdate();
            }
        }

        ballSprite.color = end;
    }

    IEnumerator SpawnWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SpawnBall();
    }
}