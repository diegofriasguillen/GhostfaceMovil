using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolOnFire : MonoBehaviour
{
    public int damageAmount = 1;
    public float damageInterval = 1.5f;
    private float lastDamageTime = 0f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Ghostface player = other.GetComponent<Ghostface>();
            if (player != null)
            {
                InflictDamage(player);
            }
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Ghostface player = other.GetComponent<Ghostface>();
            if (player != null && Time.time - lastDamageTime >= damageInterval)
            {
                InflictDamage(player);
            }
        }
    }

    private void InflictDamage(Ghostface player)
    {
        player.TakeDamage(damageAmount);
        lastDamageTime = Time.time;
    }
}
