using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedPowerUp : MonoBehaviour
{
    public float speedMultiplier = 2f;
    public float duration = 10f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StartCoroutine(ActivatePowerUp(collision.GetComponent<Ghostface>()));
            Destroy(gameObject);
        }
    }

    private IEnumerator ActivatePowerUp(Ghostface player)
    {
        player.ActivateSpeedPowerUp();
        yield return new WaitForSeconds(duration);
        player.moveSpeed /= speedMultiplier;
        Destroy(gameObject);
    }
}
