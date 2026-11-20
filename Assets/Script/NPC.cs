using System.Collections;
using UnityEngine;

public class NPC : Entity
{
    [Header("Entity Detection")]
    [SerializeField] private LayerMask whoAreEntities;
    [SerializeField] private float sightRadius = 2f;
    private bool SpottedAnEntity = false;
    [SerializeField] private Transform detectionPoint;
    private Transform currentTarget;
    private bool runningAway;


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

    private bool wasMoving;
    private float footstepTimer;

    override protected void Update()
    {
        base.Update();
        DetectOtherEntity();
    }
    protected override void HandleMovement()
    {
        if (rb == null)
            return;

        if (!canMove)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            isRunning = false;

            if (anim != null)
                anim.SetBool("run", false);

            return;
        }

        if (permitted && safeZone != null)
        {
            rb.linearVelocity = new Vector2(
                moveSpeed + 1f,
                rb.linearVelocity.y
            );

            isRunning = true;

            if (anim != null)
                anim.SetBool("run", true);

            return;
        }

        if (specialMovement && currentTarget != null)
        {
            float difference =
    currentTarget.position.x - transform.position.x;

            if (Mathf.Abs(difference) < 0.1f)
                return;

            float direction = Mathf.Sign(difference);

            if (runningAway)
                direction *= -1f;
            rb.linearVelocity = new Vector2(
                direction * moveSpeed,
                rb.linearVelocity.y
            );

            isRunning = true;

            if (anim != null)
                anim.SetBool("run", true);

            return;
        }

        isRunning = false;

        if (anim != null)
            anim.SetBool("run", false);

        base.HandleMovement();
    }
    public void DetectOtherEntity()
    {
        if (detectionPoint == null)
            return;

        // Keep chasing or fleeing while the target remains nearby.
        if (currentTarget != null)
        {
            float distance = Vector2.Distance(
                transform.position,
                currentTarget.position
            );

            if (distance <= sightRadius)
            {
                specialMovement = true;
                return;
            }

            currentTarget = null;
            specialMovement = false;
            SpottedAnEntity = false;
        }

        Vector2 origin = detectionPoint.position;
        Vector2 direction = Vector2.right * facingDir;

        RaycastHit2D hit = Physics2D.Raycast(
            origin,
            direction,
            sightRadius,
            whoAreEntities
        );

        Debug.DrawRay(
            origin,
            direction * sightRadius,
            hit.collider != null ? Color.green : Color.red
        );

        if (hit.collider == null)
            return;

        if (hit.collider.CompareTag("Player") ||
            hit.collider.CompareTag("NPC"))
        {
            currentTarget = hit.collider.transform;
            runningAway = false;
        }
        else if (hit.collider.CompareTag("Zombie"))
        {
            currentTarget = hit.collider.transform;
            runningAway = true;
        }
        else
        {
            return;
        }

        specialMovement = true;
        SpottedAnEntity = true;
    }
    protected virtual void RunTowards(Transform target)
    {
        if (!canMove)
            return;
        Debug.Log("Running towards: " + target.name);
        Vector2 direction = (target.position - transform.position).normalized;
        rb.linearVelocity = new Vector2(direction.x * moveSpeed, rb.linearVelocity.y);
        anim.SetBool("run", true);
    }
    protected virtual void RunAwayFrom(Transform target)
    {
        if (!canMove)
            return;
        Debug.Log("Running away from: " + target.name);
        Vector2 direction = (transform.position - target.position).normalized;
        rb.linearVelocity = new Vector2(direction.x * moveSpeed, rb.linearVelocity.y);
        anim.SetBool("run", true);
    }

    public void GivePermission()
    {
        permitted = true;
    }

    public void RevokePermission()
    {
        permitted = false;
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
                facingDir * sightRadius,
                0f,
                0f
            )
        );
    }
}
