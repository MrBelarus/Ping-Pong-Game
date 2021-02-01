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

    private float backSpeed = 0.1f;   //если угол > angleLimit, то возвращаем обратно

    public float borderPosition = 8f;
    private float radAngleLimit = 0f;

    private Vector2 maxPositiveYPos;
    private Vector2 minPositiveYPos;

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
            if (Mathf.Abs(transform.position.y) < borderPosition)
            {
                rb.MovePosition(transform.position + new Vector3(0, speed * verticalMove * Time.deltaTime));
            }
            else
            {
                if (transform.position.y > 0)
                {
                    if (verticalMove > 0f)
                    {
                        rb.MovePosition(maxPositiveYPos);
                    }
                    else
                    {
                        rb.MovePosition(transform.position + new Vector3(0, speed * verticalMove * Time.deltaTime));
                    }
                }
                else if (transform.position.y < 0)
                {
                    if (verticalMove < 0f)
                    {
                        rb.MovePosition(minPositiveYPos);
                    }
                    else
                    {
                        rb.MovePosition(transform.position + new Vector3(0, speed * verticalMove * Time.deltaTime));
                    }
                }
            }
        }

        if (rotation != 0)  //rotation = 1 значит что поворачиваем против часов. стрелки
        {
            if (Mathf.Abs(transform.rotation.z) < radAngleLimit)
            {
                transform.eulerAngles += new Vector3(0, 0, rotation * rotationSpeed);
            }
            else
            {
                if (transform.rotation.z < 0f)
                {
                    if (rotation < 0f)
                    {
                        transform.eulerAngles += new Vector3(0, 0, 0.01f);
                    }
                    else
                    {
                        transform.eulerAngles += new Vector3(0, 0, rotation * rotationSpeed);
                    }
                }
                else
                {
                    if (rotation > 0f)
                    {
                        transform.eulerAngles += new Vector3(0, 0, 0.01f);
                    }
                    else
                    {
                        transform.eulerAngles += new Vector3(0, 0, rotation * rotationSpeed);
                    }

                }
            }
        }
    }
}
