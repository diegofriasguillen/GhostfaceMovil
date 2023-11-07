using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleDamagePowerUp : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Ghostface player = collision.GetComponent<Ghostface>();
            player.ActivateDoubleDamage();
            Destroy(gameObject);
        }
    }
}
