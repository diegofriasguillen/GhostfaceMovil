using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Patrol : MonoBehaviour
{
    public Transform target;
    public float speed = 2.0f;
    private bool reachedTarget = false;
    private bool canMove = false;
    private Animator animator;
    private bool hasChangedAnimation = false;

    void Start()
    {
        animator = GetComponent<Animator>();
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
                animator.SetBool("HasReachedTarget", true);
                hasChangedAnimation = true;
            }
        }
    }

    IEnumerator StartMovementDelay()
    {
        yield return new WaitForSeconds(6f);
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
}
