using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FXPool : MonoBehaviour
{
    public static FXPool instance;

    [SerializeField]
    private int hitFXPrewarmAmount = 20;

    [SerializeField]
    private ParticleSystem hitFX;
    private Queue<ParticleSystem> hitQueue;

    private void Awake()
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

        SetupHitFXsQueue();
    }

    private void SetupHitFXsQueue()
    {
        hitQueue = new Queue<ParticleSystem>();
        for (int i = 0; i < hitFXPrewarmAmount; i++)
        {
            hitQueue.Enqueue(Instantiate(hitFX.gameObject, transform).
                GetComponent<ParticleSystem>());
        }
    }

    public void SpawnHitFX(ContactPoint2D contactInfo)
    {
        if (hitQueue.Count > 0)
        {
            ParticleSystem particle = hitQueue.Dequeue();
            particle.transform.parent = null;
            particle.transform.position = contactInfo.point;
            particle.transform.rotation = Quaternion.FromToRotation(Vector2.up, -contactInfo.normal);
            Debug.DrawLine(contactInfo.point, contactInfo.point + -contactInfo.normal * 10f, Color.green, 10f);
            particle.Play();
            StartCoroutine(ReturnObjToPool(particle, hitQueue));
        }
    }

    IEnumerator ReturnObjToPool(ParticleSystem particle, Queue<ParticleSystem> pool)
    {
        yield return new WaitForSeconds(particle.main.duration);

        particle.Stop();
        particle.transform.parent = transform;
        particle.transform.localPosition = Vector3.zero;
        pool.Enqueue(particle);
    }
}
