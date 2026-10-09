
using System.Collections;
using UnityEngine;

public class TeleportZone : MonoBehaviour
{
    [SerializeField] private Transform destination;
    [SerializeField] private CameraFollow2D cameraFollow;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (destination == null || cameraFollow == null)
            return;

        other.transform.root.position = destination.position;

        StartCoroutine(UpdateCameraPosition(destination.position.y));
    }

    private IEnumerator UpdateCameraPosition(float destinationY)
    {
        yield return new WaitForSeconds(1f);

        cameraFollow.onTeleportValueChange =
            (destinationY+ 2.1789f) - cameraFollow.transform.position.y ;
    }
}