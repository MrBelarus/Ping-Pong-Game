using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformMovement : MonoBehaviour
{
    public Rigidbody2D rb;

    public float defaultSpeed = 5f;
    [HideInInspector]
    public float speed = 5f;

    public float rotationSpeed = 5f;
    public float angleLimit = 45f;

    public float borderPosition = 8f;
    private float radAngleLimit = 0f;

    private Vector2 maxPositiveYPos;
    private Vector2 minPositiveYPos;

    private Vector2 futurePos;

    private void Awake()
    {
        GameEvents.current.onBallSpawn += ResetPlatform;
    }

    void Start()
    {
        speed = defaultSpeed;
        radAngleLimit = angleLimit * Mathf.PI / 360f;

        maxPositiveYPos = new Vector2(transform.position.x, borderPosition);
        minPositiveYPos = new Vector2(transform.position.x, -borderPosition);
    }


    public void Move(float verticalMove, float rotation)
    {
        if (verticalMove != 0)
        {
            futurePos = transform.position + new Vector3(0, speed * verticalMove * Time.deltaTime);

            if (Mathf.Abs(futurePos.y) > borderPosition)
            {
                if (futurePos.y > 0)
                {
                    futurePos = maxPositiveYPos;
                }
                else
                {
                    futurePos = minPositiveYPos;
                }
            }

            rb.MovePosition(futurePos);
        }

        if (rotation != 0)  //rotation = 1 значит что поворачиваем против часов. стрелки
        {
            transform.Rotate(Vector3.forward, rotation * rotationSpeed);

            if (Mathf.Abs(transform.rotation.z) > radAngleLimit)
            {
                if (transform.rotation.z < 0f)
                {
                    transform.rotation = new Quaternion(transform.rotation.x, transform.rotation.y, -radAngleLimit, transform.rotation.w);
                }
                else
                {
                    transform.rotation = new Quaternion(transform.rotation.x, transform.rotation.y, radAngleLimit, transform.rotation.w);
                }
            }
        }
    }

    public void ResetPlatform(GameObject ball)
    {
        speed = defaultSpeed;
    }
}
