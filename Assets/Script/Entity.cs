using System.Collections;
using UnityEngine;

public class Entity : MonoBehaviour
{
    protected Animator anim;
    protected Rigidbody2D rb;
    protected Collider2D coll;
    protected SpriteRenderer sr;

    [Header("Health Details")]
    [SerializeField] public int maxHealth = 1;
    [SerializeField] public int currentHealth;
    [SerializeField] private Material damageMaterial;
    [SerializeField] private Material healMaterial;
    [SerializeField] private float MaterialDuration = 0.1f;
    private Material originalMaterial;
    private Coroutine damageFeedbackCoroutine;
    private Coroutine heathGainCoroutine;

    [Header("Attack Details")]
    [SerializeField] protected float attackRadius;
    [SerializeField] protected Transform attackPoint;
    [SerializeField] protected LayerMask whatIsTarget;
    protected bool isDead = false;
    protected bool targetDetected = false;

    [Header("collision details")]
    [SerializeField] private float groundDistanceCheck;
    [SerializeField] private LayerMask whatIsGround;
    protected bool isGrounded;

    // facing direction details
    protected int facingDir = 1;
    protected bool canMove = true;
    protected bool facingRight = true;
    protected bool isAttacking;
    protected bool isRunning = false;

    [Header("Audio")]
    [SerializeField] protected AudioClip[] healAudio;
    [SerializeField] protected AudioClip[] damageAudios;
    [SerializeField] protected AudioClip[] deathAudios;
    [SerializeField] protected AudioClip[] attackAudios;
    [SerializeField] protected AudioClip[] attackHitAudios;

    [Header("Free Movement details")]
    [SerializeField] protected float moveSpeed = 2f;
    private bool isWalking;
    private float walkCounter;
    public float walkTime = 3f;
    public float waitTime = 1f;
    private float waitCounter;



    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
        coll = GetComponent<Collider2D>();
        sr = GetComponentInChildren<SpriteRenderer>();
        originalMaterial = sr.material;
        currentHealth = maxHealth;
        isWalking = true;
        walkCounter = walkTime;
        waitCounter = waitTime;
    }

    protected virtual void Update()
    {
        HandleCollision();
        HandleMovement();
        HandleAnimations();
        HandleFlip();

    }

    public virtual void EndAttack()
    {
        isAttacking = false;
    }
    public void DamageTargets()
    {
        Collider2D[] enemyCollider = Physics2D.OverlapCircleAll(attackPoint.position, attackRadius, whatIsTarget);
        bool didHit = false;

       
        foreach (Collider2D enemy in enemyCollider)
        {
            Entity entityTarget = enemy.GetComponent<Entity>();
            if (entityTarget != null && !entityTarget.isDead && !entityTarget.isRunning)
            {

                entityTarget.TakeDamage();
                didHit = true;
            }
        }
        if (!didHit)
        {

            SoundFXManager.instance.PlayMultipleSoundFXClips(attackAudios, transform, 0.5f);
        }
        else
        {
            SoundFXManager.instance.PlayMultipleSoundFXClips(attackHitAudios, transform, 0.5f);
        }
    }

    public void TakeDamage()
    {
        SoundFXManager.instance.PlayMultipleSoundFXClips(damageAudios, transform, 0.5f);
        currentHealth--;
        PlayDamageFeedack();
        if (currentHealth <= 0)
            Die();
    }
    public void Heal(int amount)
    {
        SoundFXManager.instance.PlayMultipleSoundFXClips(healAudio, transform, 0.5f);
        currentHealth += amount;
        if (currentHealth > maxHealth)
            currentHealth = maxHealth;
        PlayHealthGainFeedback();
    }
    protected virtual void Die()
    {
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        anim.SetTrigger("die");
    }

    private void PlayDamageFeedack()
    {
        if (damageFeedbackCoroutine != null)
            StopCoroutine(damageFeedbackCoroutine);

        StartCoroutine(DamageFeedbackCo());
    }
    private void PlayHealthGainFeedback()
    {
        if (heathGainCoroutine != null)
            StopCoroutine(heathGainCoroutine);
        StartCoroutine(HeathGainCoroutine());
    }

    private IEnumerator HeathGainCoroutine()
    {
        sr.color = Color.green;

        yield return new WaitForSeconds(MaterialDuration);
        sr.color = Color.white;
    }

    private IEnumerator DamageFeedbackCo()
    {
        sr.material = damageMaterial;

        yield return new WaitForSeconds(MaterialDuration);
        sr.material = originalMaterial;
    }


    public virtual void EnableMovement(bool enable)
    {
        canMove = enable;
    }

    protected void HandleAnimations()
    {

        anim.SetBool("isGrounded", isGrounded);
        anim.SetFloat("xVelocity", rb.linearVelocity.x);
        anim.SetFloat("yVelocity", rb.linearVelocity.y);
    }


    protected virtual void HandleAttack()
    {
        if (isGrounded)
        {
            anim.SetTrigger("attack");
        }
    }

    protected virtual void HandleMovement()
    {
       
        if (!canMove)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }

        if (isWalking)
        {
            // Walking
            walkCounter -= Time.deltaTime;

            rb.linearVelocity = new Vector2(
                facingDir * moveSpeed,
                rb.linearVelocity.y
            );

            if (walkCounter <= 0)
            {
                isWalking = false;
                waitCounter = waitTime;
            }
        }
        else
        {
            // Waiting
            waitCounter -= Time.deltaTime;

            rb.linearVelocity = new Vector2(
                0,
                rb.linearVelocity.y
            );

            if (waitCounter <= 0)
            {
                isWalking = true;
                walkCounter = walkTime;

                Flip();
            }
        }
    }
    

    protected virtual void HandleCollision()
    {
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, groundDistanceCheck, whatIsGround);
    }

    protected virtual void HandleFlip()
    {
        if (rb.linearVelocityX < 0 && facingRight == true)
        {
            Flip();
        }
        else if (rb.linearVelocityX > 0 && facingRight == false)
        {
            Flip();
        }
    }
    public void Flip()
    {
        facingRight = !facingRight;
        transform.Rotate(0, 180, 0);
        facingDir *= -1;
    }



    protected virtual void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(0, -groundDistanceCheck));
        if (attackPoint != null)
            Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }


}
