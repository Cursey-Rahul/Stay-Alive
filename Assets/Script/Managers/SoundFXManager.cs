using UnityEngine;

public class SoundFXManager : MonoBehaviour
{
    public static SoundFXManager instance;
    [SerializeField] private AudioSource audioSourcePrefab;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

    }

    public void PlayMultipleSoundFXClips(AudioClip[] clips, Transform spawnPosition, float volume)
    {
        // spawn in gameobject
        AudioSource audioSource = Instantiate(audioSourcePrefab, spawnPosition.position, Quaternion.identity);

        // assign the clip to the audio source
        audioSource.clip = clips[Random.Range(0, clips.Length)];

        // assign volume to the audio source
        audioSource.volume = volume;

        // play the clip
        audioSource.Play();

        // get the length of the clip
        float clipLength = audioSource.clip.length;

        // destroy the gameobject after the clip is done playing
        Destroy(audioSource.gameObject, clipLength);
    }
    public void PlaySingleSoundFXClip(AudioClip clips, Transform spawnPosition, float volume)
    {
        // spawn in gameobject
        AudioSource audioSource = Instantiate(audioSourcePrefab, spawnPosition.position, Quaternion.identity);

        // assign the clip to the audio source
        audioSource.clip = clips;

        // assign volume to the audio source
        audioSource.volume = volume;

        // play the clip
        audioSource.Play();

        // get the length of the clip
        float clipLength = audioSource.clip.length;

        // destroy the gameobject after the clip is done playing
        Destroy(audioSource.gameObject, clipLength);
    }
}
