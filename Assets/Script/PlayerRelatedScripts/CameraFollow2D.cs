
using UnityEngine;

public class CameraFollow2D : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float smoothSpeed = 5f;

    public float onTeleportValueChange = 0f;

    private void LateUpdate()
    {
        if (player == null)
            return;

        if (onTeleportValueChange != 0f)
        {
            transform.position += new Vector3(
                0f,
                onTeleportValueChange,
                0f
            );

            onTeleportValueChange = 0f;
        }

        Vector3 targetPosition = new Vector3(
            player.position.x,
            transform.position.y,
            transform.position.z
        );

        if (targetPosition.x < 0f)
            targetPosition.x = 0f;

        if (targetPosition.x > 55f)
            targetPosition.x = 55f;

        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            smoothSpeed * Time.deltaTime
        );
    }
}