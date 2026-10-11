using UnityEngine;

public class Distance_based_audio : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float maxDistance = 10f;
    [SerializeField] private AudioSource audioSource;

    private void Start()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        if (player == null)
        {
            GameObject playerObject =
                GameObject.FindGameObjectWithTag("Player");

            if (playerObject != null)
                player = playerObject.transform;
        }
    }

    private void Update()
    {
        if (player == null || audioSource == null)
            return;

        float distance =
            Vector3.Distance(player.position, transform.position);

        float volume = Mathf.Clamp01(1f - distance / maxDistance);

        audioSource.volume = volume;
    }

    public void SetPlayer(Transform playerTransform)
    {
        player = playerTransform;
    }
}