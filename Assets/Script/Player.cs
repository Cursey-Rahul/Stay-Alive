using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class Player : Entity
{
    private float xInput;
    [SerializeField] private InventoryManager inventoryManager;
    [SerializeField] private InventoryData inventoryData;

    [Header("Movement details")]
    [SerializeField] private float jumpForce = 8;
    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip running;
    [SerializeField] private AudioClip standBy;
    [SerializeField] private AudioClip attack;
    [SerializeField] private AudioClip cry;
    [SerializeField] private ProjectileBehaviour projectilePrefab;
    [SerializeField] private Transform firePoint;
    private float specialBoost = 1f;

    private bool canJump = true;

    override protected void Awake()
    {
        base.Awake();
       audioSource.pitch = UI.instance.timeSpeed;
    }
    override protected void Update()
    {
        base.Update();
        HandleInput();
        HandleInventory();

    }

    private void HandleInventory()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            UI.instance.ToggleInventory();
            inventoryManager.UpdateInventoryUI(inventoryData);
        }
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
            if (isGrounded) {
                Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
                anim.SetTrigger("attack2");
            }
            
        }
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            specialBoost = 1.3f;
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            specialBoost = 1f;
        }
      


    }

    protected override void HandleMovement()
    {
        if (canMove)
        {
            if (isRunning)
            {
                rb.linearVelocity = new Vector2(facingDir * moveSpeed * specialBoost, rb.linearVelocity.y);
            }
            else
            {
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
            //audioSource.Stop();
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }
    public override void EnableMovement(bool enable)
    {
        base.EnableMovement(enable);
        canJump = enable;
    }
    //override protected void Die()
    //{
    //    base.Die();
    //    isDead = true;
    //    ObjectToProtect mia = FindFirstObjectByType<ObjectToProtect>();
    //    if (mia != null)
    //        SoundFXManager.instance.PlaySingleSoundFXClip(cry, mia.transform, 1f);
    //    UI.instance.GameOver();
    //}

}