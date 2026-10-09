
using System.Collections;
using UnityEngine;

public class NPC : Entity
{
    [Header("Zombie Detection")]
    [SerializeField] private Transform detectionPoint;
    [SerializeField] private float detectionDistance = 8f;
    [SerializeField] private LayerMask zombieLayer;

    [Header("SafeZone Detection")]
    [SerializeField] private GameObject safeZone;
    [SerializeField] private float safeZoneDetectionDistance = 8f;
    [SerializeField] private LayerMask safeZoneLayer;

    private bool isRunningFromZombie;
    private bool isRunningToSafeZone;

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
            isRunningToSafeZone = false;
            StartCoroutine(RunAwayCoroutine());
        }

        if (isRunningFromZombie)
            return;

        if (SafeZoneDetected())
            isRunningToSafeZone = true;

        if (isRunningToSafeZone && safeZone != null)
        {
            float difference = safeZone.transform.position.x - transform.position.x;

            int direction = difference > 0.1f ? 1 :
                            difference < -0.1f ? -1 : 0;
            if (direction == 0)
            {
                rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
                isRunning = false;
                anim.SetBool("run", false);
                return;
            }

            facingDir = direction;
            rb.linearVelocity = new Vector2(direction * (moveSpeed + 1f), rb.linearVelocity.y);
            isRunning = true;
            anim.SetBool("run", true);
            return;
        }

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

    private bool SafeZoneDetected()
    {
        if (detectionPoint == null || safeZone == null)
            return false;

        Vector2 direction = safeZone.transform.position.x >= detectionPoint.position.x
            ? Vector2.right : Vector2.left;

        RaycastHit2D hit = Physics2D.Raycast(
            detectionPoint.position,
            direction,
            safeZoneDetectionDistance,
            safeZoneLayer
        );

        return hit.collider != null &&
            (hit.collider.gameObject == safeZone ||
             hit.collider.transform.IsChildOf(safeZone.transform));
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