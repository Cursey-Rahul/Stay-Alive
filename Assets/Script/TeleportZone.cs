
using System.Collections;
using UnityEngine;

public class TeleportZone : MonoBehaviour
{
    [SerializeField] private Transform destination;
    [SerializeField] private CameraFollow2D cameraFollow;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (destination == null)
            return;

        Transform root = other.transform.root;

        if (root.CompareTag("Player") || root.GetComponent<NPC>() != null)
        {
            root.position = destination.position;

            if (root.CompareTag("Player") && cameraFollow != null)
            {
                StartCoroutine(UpdateCameraPosition(destination.position.y));
            }
        }
    }

    private IEnumerator UpdateCameraPosition(float destinationY)
    {
        yield return new WaitForSeconds(1f);

        cameraFollow.onTeleportValueChange =
            (destinationY + 2.1789f) -
            cameraFollow.transform.position.y;
    }
}
