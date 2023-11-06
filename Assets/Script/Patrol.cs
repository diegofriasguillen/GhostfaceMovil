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

    void Start()
    {
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
        }
    }
}