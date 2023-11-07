using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Patrol : MonoBehaviour
{
    public Transform target;
    public float speed = 2.0f;
    private bool reachedTarget = false;
    private bool canMove = false;
    public float startDelay = 6f;

    //sounnnd
    public AudioSource audioSource;
    public float maxVolume = 0.006f;
    public float minVolume = 0.001f;

    void Start()
    {
        audioSource = GetComponent<AudioSource>(); 
        audioSource.volume = minVolume; 

        StartCoroutine(StartMovementDelay());
    }

    void Update()
    {
        if (canMove)
        {
            transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

            if (transform.position == target.position)
            {
                reachedTarget = true;
            }

            float distance = Vector2.Distance(transform.position, target.position);
            float newVolume = Mathf.Lerp(minVolume, maxVolume, 1 - (distance / 10));
            audioSource.volume = newVolume;
        }
    }

    IEnumerator StartMovementDelay()
    {
        yield return new WaitForSeconds(startDelay);
        canMove = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Ghostface player = other.gameObject.GetComponent<Ghostface>();
            if (player != null)
            {
                player.TakeDamage(2);
            }
        }
    }

    void LateUpdate()
    {
        if (reachedTarget)
        {
            Destroy(gameObject);
            audioSource.enabled = false;
        }
    }

    //private void OnDisable()
    //{
    //    audioSource.Stop();
    //    audioSource.enabled = false;
    //}
}