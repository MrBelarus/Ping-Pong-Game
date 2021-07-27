using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformController : MonoBehaviour
{
    public bool leftPlayer;

    public float ballSpeedIncrease = 1.2f;
    public float ballSpeedLimit = 15f;

    public float platformSpeedIncrease = 2f;
    public float platformSpeedLimit = 20f;

    float verticalMove = 0f;
    float rotation = 0f;

    PlatformMovement platform;
    private float pan = 0f;

    [SerializeField]
    private ParticleSystem ballHitFX;

    void Awake()
    {
        platform = gameObject.GetComponent<PlatformMovement>();
        
        if (transform.position.x < 0)
        {
            pan = -1f;
        }
    }

    void Update()
    {
        if (leftPlayer)
        {
            if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.S))
            {
                return;
            }
            
            if (Input.GetKey(KeyCode.W))
            {
                verticalMove = 1f;
            }
            else if (Input.GetKey(KeyCode.S))
            {
                verticalMove = -1f;
            }
            else
            {
                verticalMove = 0f;
            }
            
            if(Input.GetKey(KeyCode.A))
            {
                rotation = 1f;
            }
            else if (Input.GetKey(KeyCode.D))
            {
                rotation = -1f;
            }
            else
            {
                rotation = 0f;
            }
        }
        else
        {
            if (Input.GetKey(KeyCode.UpArrow) && Input.GetKey(KeyCode.DownArrow))
            {
                return;
            }

            if (Input.GetKey(KeyCode.UpArrow))
            {
                verticalMove = 1f;
            }
            else if (Input.GetKey(KeyCode.DownArrow))
            {
                verticalMove = -1f;
            }
            else
            {
                verticalMove = 0f;
            }

            if (Input.GetKey(KeyCode.LeftArrow))
            {
                rotation = 1f;
            }
            else if (Input.GetKey(KeyCode.RightArrow))
            {
                rotation = -1f;
            }
            else
            {
                rotation = 0f;
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

            FXPool.instance.SpawnHitFX(col.contacts[0].point, transform.rotation);
        }
    }
}
