using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashPowerUp : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Ghostface player = collision.GetComponent<Ghostface>();
            player.ActivateDash();
            Destroy(gameObject);
        }
    }
}
