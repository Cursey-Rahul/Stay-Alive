
using UnityEngine;

public class Distance_based_audio : MonoBehaviour
{
    public Transform player;
    public float maxDistance = 10f;
    public AudioSource audioSource;

    void Start()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        // Find the Player automatically
        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

            if (playerObject != null)
            {
                player = playerObject.transform;
            }
            else
            {
                Debug.LogError("Player not found! Make sure the Player has the Player tag.");
            }
        }
    }

    void Update()
    {
        if (player == null || audioSource == null)
            return;

        float distance = Vector3.Distance(player.position, transform.position);
        float volume = Mathf.Clamp01(1 - (distance / maxDistance));

        audioSource.volume = volume;
    }
}
