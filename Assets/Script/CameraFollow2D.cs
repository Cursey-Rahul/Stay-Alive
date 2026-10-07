using UnityEngine;

public class CameraFollow2D : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float smoothSpeed = 5f;

    private void LateUpdate()
    {
        if (player == null)
            return;

        Vector3 targetPosition = new Vector3(
            player.position.x,
            transform.position.y,
            transform.position.z
        );
        if (targetPosition.x < 0)
        {
            targetPosition.x = 0;
        }
        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            smoothSpeed * Time.deltaTime
        );
       
    }
}