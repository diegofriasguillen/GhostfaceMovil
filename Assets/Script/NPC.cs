using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    public int health = 1;
    public Animator anim;

    public GameObject powerUpPrefab;
    public float powerUpChance = 0.5f;

    public GameObject inmortalityPowerUpPrefab;

    public Vector3 powerUpOffset = new Vector3 (1f, 0, 0);

    private bool isDead = false;

    public void Start()
    {
        anim = GetComponent<Animator>();
    }
    public void TakeDamage(int damage)
    {
        health -= damage;
        
        if(health <= 0 && !isDead)
        {
            Die();
            isDead = true;
        }
    }

    void Die()
    {

        anim.SetBool("IsDead", true);

        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.enabled = false;
        }

        if (Random.value <= powerUpChance)
        {
            Vector3 spawnPosition = transform.position + powerUpOffset;
            Instantiate(powerUpPrefab, spawnPosition, Quaternion.identity);
        }

        if (Random.value <= 0f)
        {
            Vector3 spawnPosition = transform.position + powerUpOffset;
            Instantiate(inmortalityPowerUpPrefab, spawnPosition, Quaternion.identity);
        }
    }
}
