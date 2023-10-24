using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ghostface : MonoBehaviour
{
    public int lives = 3;
    public GameObject[] lifeIcons;

    //ImmortalUI
    public Slider powerUpSlider;
    public GameObject powerUpSliderObject;

    //SpeedUI
    public Slider speedPowerUpSlider;
    public GameObject speedPowerUpSliderObject;

    //DashUI
    public Slider dashPowerUpSlider;
    public GameObject dashPowerUpSliderObject;

    public float moveSpeed = 5f;
    public float jumpForce = 7f;
    private bool isJumping = false;
    private Rigidbody2D rb;
    private Animator anim;

    public int attackDamage = 1;
    public LayerMask npcLayers;
    public Transform attackPoint;
    public float attackRange = 1f;

    public int maxJumps = 2;
    private int currentJumps;

    public LayerMask wallLayer;
    public float wallRayDistance = 0.6f;
    private bool touchingWall;
    private bool wallJumping;
    public float wallJumpForce = 7f;

    public bool isImmortal = false;

    //Dash
    public float dashSpeed = 5;
    public float dashDuration = 0.1f;
    private bool isDashing = false;
    public bool hasDashPowerUp = false;
    public float dashPowerUpDuration = 10f;
    private float dashPowerUpTimeRemaining = 0f;

    public GameObject loseCanvas;


    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        currentJumps = maxJumps;
    }

    private void Update()
    {
        float moveX = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2 (moveX * moveSpeed, rb.velocity.y);
        anim.SetFloat("Speed", Mathf.Abs(moveX));

        if(moveX > 0)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else if (moveX < 0)
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }

        if(Input.GetButtonDown("Jump") && !isJumping)
        {
            rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
            isJumping = true;
            anim.SetBool("Jumping", true);
        }

        if(Input.GetButtonDown("Fire1"))
        {
            Attack();
            anim.SetTrigger("Kill");
        }

        //Wall Jumping bug need to fix jeje

        touchingWall = Physics2D.Raycast(transform.position, Vector2.right * transform.localScale.x, wallRayDistance, wallLayer);

        if (Input.GetButtonDown("Jump") && (currentJumps > 0 || touchingWall))
        {
            if (touchingWall && !isJumping)
            {
                WallJump();
            }
            else
            {
                Jump();
            }
        }

        //Dash 
        if (Input.GetKeyDown(KeyCode.E) && !isDashing && hasDashPowerUp)
        {
            StartCoroutine(DoDash());
        }
    }

    //SpeedStuff
    public void ActivateSpeedPowerUp()
    {
        moveSpeed *= 2;
        StartCoroutine(HandleSpeedPowerUp());
    }

    private IEnumerator HandleSpeedPowerUp()
    {
        float elapsedTime = 0f;

        speedPowerUpSliderObject.SetActive(true);  // Mostrar el slider
        speedPowerUpSlider.value = speedPowerUpSlider.maxValue;

        while (elapsedTime < speedPowerUpSlider.maxValue)
        {
            elapsedTime += Time.deltaTime;
            speedPowerUpSlider.value = speedPowerUpSlider.maxValue - elapsedTime;  // Actualizar el slider
            yield return null;
        }

        moveSpeed /= 2;  // Restaurar la velocidad original
        speedPowerUpSliderObject.SetActive(false);  // Ocultar el slider
    }

    //Here ends speed stuff

    //Dash 
    public void ActivateDash()
    {
        dashPowerUpTimeRemaining = 10f;
        dashPowerUpSliderObject.SetActive(true);
        dashPowerUpSlider.maxValue = dashPowerUpTimeRemaining;
        dashPowerUpSlider.value = dashPowerUpTimeRemaining;

        if (!hasDashPowerUp)
        {
            hasDashPowerUp = true;
            StartCoroutine(DashPowerUpCountdown());
        }

        StartCoroutine(UpdateDashPowerUp());
    }
    private IEnumerator UpdateDashPowerUp()
    {
        while (dashPowerUpTimeRemaining > 0)
        {
            dashPowerUpTimeRemaining -= Time.deltaTime;
            dashPowerUpSlider.value = dashPowerUpTimeRemaining;
            yield return null;
        }
    }
    private IEnumerator DashPowerUpCountdown()
    {
        yield return new WaitForSeconds(dashPowerUpDuration);
        hasDashPowerUp = false;
        dashPowerUpSliderObject.SetActive(false);
    }
    private IEnumerator DoDash()
    {
        float dashEndTime = Time.time + dashDuration;
        isDashing = true;


        while (Time.time < dashEndTime)
        {
            rb.velocity = new Vector2(dashSpeed * transform.localScale.x, rb.velocity.y);
            yield return null;
        }

        dashPowerUpTimeRemaining -= dashDuration;
        isDashing = false;
    }

    public void TakeDamage(int damage)
    {
        if(isImmortal)
        {
            return;
        }

        lives -= damage;

        if(lives >=0)
        {
            lifeIcons[lives].SetActive(false);
        }

        if(lives <=0)
        {
            Die();
        }

    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isJumping = false;
            anim.SetBool("Jumping", false);
        }

        currentJumps = maxJumps;
    }

    //ImmortalStuff
    public void BecomeImmortal()
    {
        isImmortal = true;
        StartCoroutine(EndImmortality());
        StartPowerUpCountdown();
    }


    private void StartPowerUpCountdown()
    {
        powerUpSliderObject.SetActive(true);
        powerUpSlider.value = powerUpSlider.maxValue;
        StartCoroutine(UpdatePowerUpCountdown());
    }

    private IEnumerator UpdatePowerUpCountdown()
    {
        while(powerUpSlider.value > 0)
        {
            powerUpSlider.value -= Time.deltaTime;
            yield return null;
        }
        powerUpSliderObject.SetActive(false);
    }

    private IEnumerator EndImmortality()
    {
        yield return new WaitForSeconds(10f);
        isImmortal = false;
    }

    //Here ends immortal stuff
    void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, 0f);
        rb.AddForce(new Vector2(0f,jumpForce ), ForceMode2D.Impulse);
        isJumping=true;
        anim.SetBool("Jumping", true);
        currentJumps--;
    }

    void WallJump()
    {
        rb.velocity = new Vector2(0f, 0f);
        Vector2 jumpDirection = new Vector2(-transform.localScale.x, 1).normalized;
        rb.AddForce(jumpDirection * wallJumpForce, ForceMode2D.Impulse);
        wallJumping = true;
    }

    void Attack()
    {
        Collider2D[] hitNPCs = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, npcLayers);
        foreach (Collider2D npc in hitNPCs)
        {
            npc.GetComponent<NPC>().TakeDamage(attackDamage);
        }
    }

    void Die()
    {
        Time.timeScale = 0;
        loseCanvas.SetActive(true);
    }
}
