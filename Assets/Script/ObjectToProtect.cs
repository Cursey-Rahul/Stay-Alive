using UnityEngine;

public class ObjectToProtect : Entity

{
    private Transform player;
    [SerializeField] private float moveSpeed;
    private bool closeToAlina = false;
    private bool isFollowing = false;
    [SerializeField] private AudioClip cry;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip walk;

    override protected void Awake()
    {
        base.Awake();
        player = FindFirstObjectByType<Player>().transform;
       
    }
    override protected void Update()
    {
        HandleFlip();
        HandleDistanceWithPlayer();
        FollowAlina();
        UI.instance.MiaHeath(currentHealth);
    }

    private void HandleDistanceWithPlayer()
    {
        closeToAlina = Vector2.Distance(attackPoint.position, player.position) <= attackRadius;

    }

    override protected void HandleFlip()
    {
        if (player == null)
            return;
        if (player.transform.position.x < transform.position.x && facingRight == true)
            Flip();
        else if (player.transform.position.x > transform.position.x && facingRight == false)
            Flip();
    }
    protected override void Die()
    {
        base.Die();
        isDead = true;
        Player alina = FindFirstObjectByType<Player>();

        if (alina != null)
            SoundFXManager.instance.PlaySingleSoundFXClip(cry, alina.transform, 1f);

        UI.instance.GameOver();
    }

    private void FollowAlina()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            isFollowing = !isFollowing;
        }

        if (isFollowing && closeToAlina == false)
        {
            audioSource.clip = walk;
            audioSource.Play();
            if (facingRight == true)
                rb.linearVelocity = new Vector2(
                    1 * moveSpeed,
                    rb.linearVelocity.y
                );
            else
                rb.linearVelocity = new Vector2(
                    -1 * moveSpeed,
                    rb.linearVelocity.y
                );
        }
        else
        {
            audioSource.Stop();
            rb.linearVelocity = new Vector2(
                0,
                rb.linearVelocity.y
            );
        }
    }


}
