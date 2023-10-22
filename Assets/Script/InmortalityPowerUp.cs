using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InmortalityPowerUp : MonoBehaviour
{
    public float duration = 10f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StartCoroutine(ActivatePowerUp(collision.GetComponent<Ghostface>()));
            collision.GetComponent<Ghostface>().BecomeImmortal();
            Destroy(gameObject);
        }
    }

    private IEnumerator ActivatePowerUp(Ghostface player)
    {
        player.isImmortal = true;
        yield return new WaitForSeconds(duration);
        player.isImmortal = false;
        Destroy(gameObject);
    }
}
