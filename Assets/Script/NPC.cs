using System.Collections;
using UnityEngine;

public class NPC : Entity
{
    [Header("Zombie Detection")]
    [SerializeField] private Transform detectionPoint;
    [SerializeField] private float detectionDistance = 8f;
    [SerializeField] private LayerMask zombieLayer;

    [Header("Van / Safe Zone")]
    [SerializeField] private GameObject safeZone;

    [Header("Permission")]
    public bool permitted = false;

    [Header("Walking Sounds")]
    [SerializeField] private AudioClip[] walkingClips;
    [SerializeField] private float walkingVolume = 0.5f;
    [SerializeField] private float walkingInterval = 0.5f;

    [Header("Running Sounds")]
    [SerializeField] private AudioClip[] runningClips;
    [SerializeField] private float runningVolume = 0.7f;
    [SerializeField] private float runningInterval = 0.25f;

    [Header("Drinking Sound")]
    [SerializeField] private AudioClip drinkingClip;
    [SerializeField] private float drinkingVolume = 0.7f;

    private bool isRunningFromZombie;
    private bool wasMoving;
    private float footstepTimer;

    protected override void HandleMovement()
    {
        if (rb == null)
            return;

        if (!canMove)
        {
            rb.linearVelocity = new Vector2(
                0f,
                rb.linearVelocity.y
            );

            isRunning = false;

            if (anim != null)
                anim.SetBool("run", false);

            return;
        }

        // Run away from a detected zombie.
        if (ZombieDetected() && !isRunningFromZombie)
        {
            StartCoroutine(RunAwayCoroutine());
        }

        if (isRunningFromZombie)
            return;

        // Once permitted, continuously run right toward the van.
        // Teleport triggers handle movement between maps.
        if (permitted && safeZone != null)
        {
            facingDir = 1;

            rb.linearVelocity = new Vector2(
                moveSpeed + 1f,
                rb.linearVelocity.y
            );

            isRunning = true;

            if (anim != null)
                anim.SetBool("run", true);

            return;
        }

        // Normal movement before receiving permission.
        isRunning = false;

        if (anim != null)
            anim.SetBool("run", false);

        base.HandleMovement();
    }

    public void GivePermission()
    {
        permitted = true;
    }

    public void RevokePermission()
    {
        permitted = false;
    }

    private bool ZombieDetected()
    {
        if (detectionPoint == null)
            return false;

        RaycastHit2D hit = Physics2D.Raycast(
            detectionPoint.position,
            Vector2.right * facingDir,
            detectionDistance,
            zombieLayer
        );

        return hit.collider != null;
    }

    private IEnumerator RunAwayCoroutine()
    {
        isRunningFromZombie = true;
        isRunning = true;

        if (anim != null)
            anim.SetBool("run", true);

        rb.linearVelocity = new Vector2(
            -facingDir * (moveSpeed + 1f),
            rb.linearVelocity.y
        );

        yield return new WaitForSeconds(2f);

        isRunningFromZombie = false;
        isRunning = false;

        if (anim != null)
            anim.SetBool("run", false);
    }

    private void LateUpdate()
    {
        UpdateMovementSounds();
    }

    private void UpdateMovementSounds()
    {
        if (rb == null)
            return;

        bool moving = Mathf.Abs(rb.linearVelocity.x) > 0.1f;
        bool running = moving && isRunning;

        if (moving)
        {
            float interval = running
                ? runningInterval
                : walkingInterval;

            footstepTimer -= Time.deltaTime;

            if (footstepTimer <= 0f)
            {
                AudioClip[] clips = running
                    ? runningClips
                    : walkingClips;

                float volume = running
                    ? runningVolume
                    : walkingVolume;

                if (clips != null &&
                    clips.Length > 0 &&
                    SoundFXManager.instance != null)
                {
                    SoundFXManager.instance.PlayMultipleSoundFXClips(
                        clips,
                        transform,
                        volume
                    );
                }

                footstepTimer = interval;
            }
        }
        else
        {
            footstepTimer = 0f;

            if (wasMoving &&
                drinkingClip != null &&
                SoundFXManager.instance != null)
            {
                SoundFXManager.instance.PlaySingleSoundFXClip(
                    drinkingClip,
                    transform,
                    drinkingVolume
                );
            }
        }

        wasMoving = moving;
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        if (detectionPoint == null)
            return;

        Gizmos.DrawLine(
            detectionPoint.position,
            detectionPoint.position +
            new Vector3(
                facingDir * detectionDistance,
                0f,
                0f
            )
        );
    }
}
