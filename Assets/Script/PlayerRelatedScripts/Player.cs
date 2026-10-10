
using UnityEngine;

public class Player : Entity
{
    private float xInput;

    [Header("Inventory")]
    [SerializeField] private InventoryManager inventoryManager;
    [SerializeField] private InventoryData inventoryData;

    [Header("Movement Details")]
    [SerializeField] private float jumpForce = 8f;

    [Header("Audio")]
    [SerializeField] private AudioClip[] walkingClips;
    [SerializeField] private AudioClip[] runningClips;
    [SerializeField] private AudioClip standBy;
    [SerializeField] private AudioClip attack;
    [SerializeField] private AudioClip gunshot;
    [SerializeField] private float gunshotVolume = 1f;

    [SerializeField] private float walkingInterval = 0.5f;
    [SerializeField] private float runningInterval = 0.25f;
    [SerializeField] private float footstepVolume = 1f;
    [SerializeField] private float standByVolume = 1f;

    [Header("Projectile")]
    [SerializeField] private ProjectileBehaviour projectilePrefab;
    [SerializeField] private Transform firePoint;

    private float specialBoost = 1f;
    private float footstepTimer;
    private bool canJump = true;
    private bool standbyPlayed;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Update()
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
        if (!canMove || Time.timeScale == 0f)
        {
            xInput = 0f;
            return;
        }

        xInput = Input.GetAxisRaw("Horizontal");

        if (Input.GetKeyDown(KeyCode.Space))
            TryToJump();

        if (Input.GetKeyDown(KeyCode.Mouse1))
            HandleAttack();

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (isGrounded)
            {
                Instantiate(
                    projectilePrefab,
                    firePoint.position,
                    firePoint.rotation
                );

                anim.SetTrigger("attack2");

                // Play gunshot sound when firing.
                if (gunshot != null && SoundFXManager.instance != null)
                {
                    SoundFXManager.instance.PlaySingleSoundFXClip(
                        gunshot,
                        firePoint != null ? firePoint : transform,
                        gunshotVolume
                    );
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
            specialBoost = 1.3f;

        if (Input.GetKeyUp(KeyCode.LeftShift))
            specialBoost = 1f;
    }


    protected override void HandleMovement()
    {
        if (canMove)
        {
            rb.linearVelocity = new Vector2(
                xInput * moveSpeed * specialBoost,
                rb.linearVelocity.y
            );
        }
        else
        {
            rb.linearVelocity = new Vector2(
                0f,
                rb.linearVelocity.y
            );
        }

        bool moving = canMove
            && isGrounded
            && Mathf.Abs(rb.linearVelocity.x) > 0.1f;

        bool runningNow = moving && specialBoost > 1f;

        
        anim.SetBool("run", runningNow);

        HandleAudio();
    }
    public InventoryData InventoryData => inventoryData;

    public void RefreshInventoryUI()
    {
        if (inventoryManager != null && inventoryData != null)
        {
            inventoryManager.UpdateInventoryUI(inventoryData);
        }
        else
        {
            Debug.LogError(
                "InventoryManager or InventoryData is not assigned on Player!"
            );
        }
    }

    private void HandleAudio()
    {
        bool moving = canMove
            && isGrounded
            && Mathf.Abs(rb.linearVelocity.x) > 0.1f;

        bool runningNow = moving && specialBoost > 1f;

        if (moving)
        {
            // Stop standby from playing while moving.
            standbyPlayed = false;

            footstepTimer -= Time.deltaTime;

            if (footstepTimer <= 0f)
            {
                AudioClip[] clips = runningNow
                    ? runningClips
                    : walkingClips;

                if (SoundFXManager.instance != null
                    && clips != null
                    && clips.Length > 0)
                {
                    SoundFXManager.instance.PlayMultipleSoundFXClips(
                        clips,
                        transform,
                        footstepVolume
                    );
                }

                footstepTimer = runningNow
                    ? runningInterval
                    : walkingInterval;
            }
        }
        else
        {
            footstepTimer = 0f;

            // Play standby once when the player becomes idle.
            if (!standbyPlayed
                && standBy != null
                && SoundFXManager.instance != null)
            {
                SoundFXManager.instance.PlaySingleSoundFXClip(
                    standBy,
                    transform,
                    standByVolume
                );

                standbyPlayed = true;
            }
        }
    }

    private void TryToJump()
    {
        if (isGrounded && canJump)
        {
            rb.linearVelocity = new Vector2(
                rb.linearVelocity.x,
                jumpForce
            );
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
        UI.instance.GameOver();
    }

}