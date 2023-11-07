using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
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

    public int maxJumps = 1;
    private int currentJumps;

    public LayerMask wallLayer;
    public float wallRayDistance = 0.6f;
    private bool touchingWall;
    //private bool wallJumping;
    public float wallJumpForce = 7f;

    public bool isImmortal = false;

    //Dash
    public float dashSpeed = 5;
    public float dashDuration = 0.1f;
    private bool isDashing = false;
    public bool hasDashPowerUp = false;
    public float dashPowerUpDuration = 10f;
    private float dashPowerUpTimeRemaining = 0f;

    //WallJump
    private bool canWallJump = false;
    //WallSlide
    private bool isWallSliding = false;
    public float wallSlideSpeed = 2.0f;

    //Jump si está la tecla presionada mucho time
    private bool isHoldingJump = false;
    private float jumpHoldTime = 0.0f;
    private float maxJumpHoldTime = 0.5f;
    public bool hasDoubleJump = false;

    //CoyoteTime
    private float coyoteTime = 0.1f;
    private float coyoteTimeCounter;

    //ChangingLvels
    public GameObject Level1;
    public GameObject Level2;
    public Transform RespawnPoint;
    public GameObject camerasObjectLevel1;
    public GameObject camerasObjectLevel2;

    //DoubleDamage
    private bool hasDoubleDamage = false;

        
    public GameObject loseCanvas;



    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        currentJumps = maxJumps;

        camerasObjectLevel1.SetActive(true);
        camerasObjectLevel2.SetActive(false);
        Level1.SetActive(true);
        Level2.SetActive(false);

    }

    private void Update()
    {

        float moveX = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(moveX * moveSpeed, rb.velocity.y);
        anim.SetFloat("Speed", Mathf.Abs(moveX));


        if (moveX > 0)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else if (moveX < 0)
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }


        if (Input.GetButtonDown("Jump") && !isJumping)
        {
            rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
            isJumping = true;
            anim.SetBool("Jumping", true);
            isHoldingJump = true;
            jumpHoldTime = Time.time;
        }

        if (Input.GetButton("Jump") && isHoldingJump && Time.time - jumpHoldTime < maxJumpHoldTime)
        {
            Jump();
        }

        if (Input.GetButtonDown("Fire1"))
        {
            Attack();
            anim.SetTrigger("Kill");
        }

        //Wall Jumping bug need to fix jeje

        touchingWall = Physics2D.Raycast(transform.position, Vector2.right * transform.localScale.x, wallRayDistance, wallLayer) ||
                        Physics2D.Raycast(transform.position, Vector2.up, wallRayDistance, wallLayer);



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

        if (Input.GetButtonDown("Jump"))
        {

            if (currentJumps > 0 && !isWallSliding)
            {
                Jump();
            }
            else if (isWallSliding)
            {
                WallSlideJump();
            }
            else if (hasDoubleJump && !isJumping && (currentJumps > 0 || touchingWall))
            {
                DoubleJump();
            }
        }

        //CoyoteTime
        if (!isJumping && coyoteTimeCounter > 0)
        {
            coyoteTimeCounter -= Time.deltaTime;
            if (Input.GetButtonDown("Jump"))
            {
                Jump();
            }
        }

        //Dash 
        if (Input.GetKeyDown(KeyCode.E) && !isDashing && hasDashPowerUp)
        {
            StartCoroutine(DoDash());
        }

        //WallSlide
        if (touchingWall && !isJumping && rb.velocity.y < 0)
        {
            isWallSliding = true;
            isJumping = false;
            anim.SetBool("Jumping", false);
        }
        else
        {
            isWallSliding = false;
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

        speedPowerUpSliderObject.SetActive(true);
        speedPowerUpSlider.value = speedPowerUpSlider.maxValue;

        while (elapsedTime < speedPowerUpSlider.maxValue)
        {
            elapsedTime += Time.deltaTime;
            speedPowerUpSlider.value = speedPowerUpSlider.maxValue - elapsedTime;
            yield return null;
        }

        moveSpeed /= 2;
        speedPowerUpSliderObject.SetActive(false);
    }

    //Here ends speed stuff

    //DoubleDamage
    public void ActivateDoubleDamage()
    {
        hasDoubleDamage = true;
        StartCoroutine(EndDoubleDamage());
    }

    private IEnumerator EndDoubleDamage()
    {
        yield return new WaitForSeconds(10f);
        hasDoubleDamage = false;
    }

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

    //TakeDamage

    public void TakeDamage(int damage)
    {
        if (isImmortal)
        {
            return;
        }

        lives -= damage;

        if (lives >= 0)
        {
            lifeIcons[lives].SetActive(false);
        }

        if (lives <= 0)
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
            currentJumps = maxJumps;
            coyoteTimeCounter = coyoteTime;


        }

        if (collision.gameObject.name == "meta")
        {

            camerasObjectLevel1.SetActive(false);
            camerasObjectLevel2.SetActive(true);

            Level1.SetActive(false);
            Level2.SetActive(true);

            transform.position = RespawnPoint.position;

        }



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
        while (powerUpSlider.value > 0)
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
        rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
        isJumping = true;
        anim.SetBool("Jumping", true);
        currentJumps--;
    }

    void DoubleJump()
    {
        rb.velocity = new Vector2(rb.velocity.x, 0f);
        rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
        isJumping = true;
        anim.SetBool("Jumping", true);
        hasDoubleJump = false;
    }

    public void EnableDoubleJump()
    {
        hasDoubleJump = true;
    }

    //WallJump
    public void EnableWallJump()
    {
        canWallJump = true;
    }

    void WallJump()
    {

        if (canWallJump && (currentJumps > 0 || touchingWall))
        {
            rb.velocity = new Vector2(0f, 0f);
            Vector2 jumpDirection = new Vector2(-transform.localScale.x, 1).normalized;
            rb.AddForce(jumpDirection * wallJumpForce, ForceMode2D.Impulse);
            //wallJumping = true;
        }
    }

    //Wallslide
    private void FixedUpdate()
    {
        if (isWallSliding)
        {
            rb.velocity = new Vector2(rb.velocity.x, -wallSlideSpeed);
        }
    }

    void WallSlideJump()
    {
        Vector2 jumpDirection = new Vector2(-transform.localScale.x, 1).normalized;
        rb.velocity = Vector2.zero;
        rb.AddForce(jumpDirection * wallJumpForce, ForceMode2D.Impulse);
        currentJumps--;
    }

    //AtaqueeeeeFuaaa
    void Attack()
    {
        Collider2D[] hitNPCs = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, npcLayers);
        foreach (Collider2D npc in hitNPCs)
        {
            npc.GetComponent<NPC>().TakeDamage(attackDamage);

            // DamagePowerUP for NPCs
            int damageNPC = hasDoubleDamage ? 2 * attackDamage : attackDamage;
            npc.GetComponent<NPC>().TakeDamage(damageNPC);
        }

        Collider2D[] hitPoliceOfficers = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, LayerMask.GetMask("Police"));
        foreach (Collider2D officer in hitPoliceOfficers)
        {
            PoliceOfficer policeOfficer = officer.GetComponent<PoliceOfficer>();
            if (policeOfficer != null)
            {
                policeOfficer.TakeDamage(attackDamage);

                // DamagePowerUP for PoliceOfficer
                int damage = hasDoubleDamage ? 2 * attackDamage : attackDamage;
                policeOfficer.TakeDamage(damage);
            }

            PoliceOfficer_HorizontalShooting horizontalShootingOfficer = officer.GetComponent<PoliceOfficer_HorizontalShooting>();
            if (horizontalShootingOfficer != null)
            {
                horizontalShootingOfficer.TakeDamage(attackDamage);

                // DamagePowerUP for PoliceOfficer_HorizontalShooting
                int damage = hasDoubleDamage ? 2 * attackDamage : attackDamage;
                horizontalShootingOfficer.TakeDamage(damage);
            }
        }

        Collider2D[] hitFootballPlayers = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, LayerMask.GetMask("FootballPlayer"));
        foreach (Collider2D player in hitFootballPlayers)
        {
            FootballPlayer footballPlayer = player.GetComponent<FootballPlayer>();
            if (footballPlayer != null)
            {
                footballPlayer.TakeDamage(attackDamage);

                // DamagePowerUP for FootballPlayer
                int damage = hasDoubleDamage ? 2 * attackDamage : attackDamage;
                footballPlayer.TakeDamage(damage);
            }
        }
    }

    //Extralife
    public void AddLife()
    {
        if (lives < 5)
        {
            lives++;
            if (lives <= lifeIcons.Length)
            {
                lifeIcons[lives - 1].SetActive(true);
            }
        }
    }

    void Die()
    {
        Time.timeScale = 0;
        loseCanvas.SetActive(true);
    }
}