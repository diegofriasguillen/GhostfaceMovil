using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallCollision : MonoBehaviour
{
    public float detectionRadius = 5f;
    public LayerMask detectionLayer;
    public int damage = 1;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player")) // Cambiado de "Player" a "Ghostface"
        {
            Ghostface player = collision.gameObject.GetComponent<Ghostface>();
            if (player != null)
            {
                player.TakeDamage(damage);
            }

            Destroy(gameObject); // Destruye la pelota después de colisionar
        }
    }
}