using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage = 1;
    public float speed = 5f;
    private Vector2 direction;

    private void Update()
    {
        transform.Translate(direction*speed*Time.deltaTime);
    }

    public void SetDirection(Vector2 newDirection)
    {
        direction = newDirection;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            Ghostface player = collision.GetComponent<Ghostface>();
            player.TakeDamage(damage);

            Destroy(gameObject);
        }    
    }
}
