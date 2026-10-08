
using System;
using System.Collections;
using UnityEngine;

public class NPC : Entity
{
    [Header("NPC Detection")]
    [SerializeField] private Transform detectionPoint;
    [SerializeField] private float detectionDistance = 8f;
    [SerializeField] private LayerMask zombieLayer;

    private bool isRunningFromZombie;

    protected override void HandleMovement()
    {
        if (!canMove)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            isRunning = false;
            anim.SetBool("run", false);
            return;
        }

        if (ZombieDetected() && !isRunningFromZombie)
        {
            StartCoroutine(RunAwayCoroutine());
        }

        if (!isRunningFromZombie)
        {
            isRunning = false;
            anim.SetBool("run", false);
            base.HandleMovement();
        }
    }

    private bool ZombieDetected()
    {
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
        anim.SetBool("run", true);

        float runDirection = -facingDir;

        rb.linearVelocity = new Vector2(
            runDirection * (moveSpeed + 1f),
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

        if (detectionPoint != null)
        {
            Gizmos.DrawLine(
                detectionPoint.position,
                detectionPoint.position +
                new Vector3(facingDir * detectionDistance, 0)
            );
        }
    }
}