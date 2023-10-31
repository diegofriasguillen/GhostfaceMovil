using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeExtra : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.CompareTag("Player"))
        {
            Ghostface player = collision.GetComponent<Ghostface>();
            if (player != null)
            {
                if(player.lives < 5)
                {
                    player.AddLife();
                Destroy(gameObject);
                }
            }
        }
    }

}
