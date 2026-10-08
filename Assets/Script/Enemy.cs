using UnityEngine;


public class Enemy : Entity
{
    protected bool playerDetected;
    [Header("Movement details")]
    [SerializeField] protected AudioSource audioSource;
    [SerializeField] protected AudioClip walk;
    [Header("Attack details")]
    [SerializeField] protected Transform lineOfSightPoint;
    [SerializeField] protected float lineOfSightDistance = 10f;
    [SerializeField] protected Transform rangeOfHearing;
    [SerializeField] protected float hearingRange = 5f;

    override protected void Awake()
    {
        base.Awake();
        
    }
    private void Start()
    {
       
        //audioSource.pitch = UI.instance.timeSpeed;
    }
    protected override void Update()
    {
        base.Update();
        HandleAttack();
    }
    protected override void HandleAttack()
    {
        if (playerDetected && !isAttacking)
        {
            isAttacking = true;
            anim.SetTrigger("attack");
        }
    }

    protected override void HandleMovement()
    {
        if (!targetDetected) {
            base.HandleMovement();
        }else if (targetDetected && canMove)
        {
            rb.linearVelocity = new Vector2(facingDir * moveSpeed * 2, rb.linearVelocity.y);
        }


        //audioSource.clip = walk;
        //if (!audioSource.isPlaying && !UI.instance.GamePausedState)
        //{
        //    audioSource.Play();
        //}
        //else if (audioSource.isPlaying && UI.instance.GamePausedState) 
        //{
        //    audioSource.Stop();
        //}
        // rb.linearVelocity = new Vector2(facingDir * moveSpeed, rb.linearVelocity.y);
        
    } 
  
    protected override void HandleCollision()
    {
        base.HandleCollision();
        playerDetected = Physics2D.OverlapCircle(attackPoint.position, attackRadius, whatIsTarget);
        targetDetected = Physics2D.Raycast(lineOfSightPoint.position, Vector2.right * facingDir, lineOfSightDistance, whatIsTarget);
    }
   
    override protected void Die()
    {
        if (isDead)
            return;
        isDead = true;
        //GameManagerScript.instance.EnemyDied();
        base.Die();
        Destroy(gameObject, 2.5f);
        UI.instance.UpdateKillCount();
        UI.instance.UpdateSoulsCount(1);

    }
    override protected void HandleFlip()
    {
        if (playerDetected && !isDead)
        {
            Transform player = FindFirstObjectByType<Player>().transform;
            if (player == null)
                return;
            if (player.transform.position.x < transform.position.x && facingRight == true)
                Flip();
            else if (player.transform.position.x > transform.position.x && facingRight == false)
                Flip();
        }
    }
    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(facingDir * lineOfSightDistance, 0));
        Gizmos.DrawWireSphere(rangeOfHearing.position, hearingRange);
    }
}
