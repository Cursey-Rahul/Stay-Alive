using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class Player : Entity
{
    private float xInput;
    [Header("Movement details")]
    [SerializeField] protected float moveSpeed = 3.5f;
    [SerializeField] private float jumpForce = 8;
    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip running;
    [SerializeField] private AudioClip standBy;
    [SerializeField] private AudioClip attack;
    [SerializeField] private AudioClip cry;
    private float specialBoost = 1f;

    private bool canJump = true;

  override protected void Awake()
    {
        base.Awake();
        //audioSource.pitch = UI.instance.timeSpeed;
    }
    override protected void Update()
    {
        base.Update();
        HandleInput();

        //UI.instance.AlinaHeath(currentHealth);
    }

    private void HandleInput()
    {
        xInput = Input.GetAxisRaw("Horizontal");

        if (Input.GetKeyDown(KeyCode.Space))
            TryToJump();
        if (Input.GetKeyDown(KeyCode.Mouse1))
            HandleAttack();
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            anim.SetTrigger("attack2");
        }
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            specialBoost = 1.3f;
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            specialBoost = 1f;
        }
        if (Input.GetKeyDown(KeyCode.LeftControl)&& canMove && xInput != 0 && !isSliding)
        {
            StartCoroutine(SlideCo());
        }
       

    }

    private IEnumerator SlideCo()
    {
        isSliding = true;
        specialBoost = 1.5f;
        anim.SetTrigger("slide");
        yield return new WaitForSeconds(0.5f);
        isSliding = false;
        specialBoost = 1f;
    }
    protected override void HandleMovement()
    {
        if (canMove)
        {
            if (isSliding)
            {
                rb.linearVelocity= new Vector2(facingDir * moveSpeed * specialBoost, rb.linearVelocity.y);
            }
            else {
                rb.linearVelocity = new Vector2(xInput * moveSpeed * specialBoost, rb.linearVelocity.y);
            }
                
            HandleAudio();
        }
        
        else
        {

            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }
    }

    private void HandleAudio()
    {
        if (xInput != 0)
        {
            if (audioSource.clip != running)
            {
                audioSource.Stop();
                audioSource.clip = running;
                audioSource.Play();
            }
        }
        else
        {
            if (audioSource.clip != standBy)
            {
                audioSource.Stop();
                audioSource.clip = standBy;
                audioSource.Play();
            }
        }
    }

    private void TryToJump()
    {
        if (isGrounded && canJump)
        {
            audioSource.Stop();
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }
    public override void EnableMovement(bool enable)
    {
        base.EnableMovement(enable);
        canJump = enable;
    }
    override protected void Die()
    {
        base.Die();
        isDead = true;
        ObjectToProtect mia = FindFirstObjectByType<ObjectToProtect>();
        if (mia != null)
            SoundFXManager.instance.PlaySingleSoundFXClip(cry, mia.transform, 1f);
        UI.instance.GameOver();
    }

}