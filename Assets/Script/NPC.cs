
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
    private bool isRunningToSafeZone;
    private bool wasMoving;
    private float footstepTimer;

    protected override void HandleMovement()
    {
        if (!canMove)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            isRunning = false;
            anim.SetBool("run", false);
            return;
        }

        if (ZombieDetected() && !isRunningFromZombie)
        {
            StartCoroutine(RunAwayCoroutine());
        }

        if (isRunningFromZombie)
            return;

        // Permission controls whether the NPC goes to the van.
        isRunningToSafeZone = permitted && safeZone != null;

        if (isRunningToSafeZone)
        {
            float difference =
                safeZone.transform.position.x - transform.position.x;

            int direction = difference > 0.1f ? 1 :
                            difference < -0.1f ? -1 : 0;

            if (direction == 0)
            {
                rb.linearVelocity = new Vector2(
                    0f, rb.linearVelocity.y);

                isRunning = false;
                anim.SetBool("run", false);
                return;
            }

            facingDir = direction;

            rb.linearVelocity = new Vector2(
                direction * (moveSpeed + 1f),
                rb.linearVelocity.y
            );

            isRunning = true;
            anim.SetBool("run", true);
            return;
        }

        // Without permission, use the Entity movement behaviour.
        isRunning = false;
        anim.SetBool("run", false);
        base.HandleMovement();
    }

    private bool ZombieDetected()
    {
        return detectionPoint != null &&
            Physics2D.Raycast(
                detectionPoint.position,
                Vector2.right * facingDir,
                detectionDistance,
                zombieLayer
            ).collider != null;
    }

    private IEnumerator RunAwayCoroutine()
    {
        isRunningFromZombie = true;
        isRunning = true;
        anim.SetBool("run", true);

        rb.linearVelocity = new Vector2(
            -facingDir * (moveSpeed + 1f),
            rb.linearVelocity.y
        );

        yield return new WaitForSeconds(2f);

        isRunningFromZombie = false;
        isRunning = false;
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

                if (clips != null && clips.Length > 0 &&
                    SoundFXManager.instance != null)
                {
                    SoundFXManager.instance.PlayMultipleSoundFXClips(
                        clips, transform, volume);
                }

                footstepTimer = interval;
            }
        }
        else
        {
            footstepTimer = 0f;

            // Play drinking sound once when movement stops.
            if (wasMoving && drinkingClip != null &&
                SoundFXManager.instance != null)
            {
                SoundFXManager.instance.PlaySingleSoundFXClip(
                    drinkingClip, transform, drinkingVolume);
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
            new Vector3(facingDir * detectionDistance, 0f)
        );
    }
}
