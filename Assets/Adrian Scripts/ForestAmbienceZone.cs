
using UnityEngine;

public class ForestAmbienceZone : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioSource ambienceSource;
    [SerializeField] private AudioClip forestAmbience;
    [SerializeField, Range(0f, 1f)] private float ambienceVolume = 0.5f;

    [Header("Fade")]
    [SerializeField, Min(0f)] private float fadeInDuration = 2f;
    [SerializeField, Min(0f)] private float fadeOutDuration = 2f;

    private bool playerInside;
    private float targetVolume;

    private void Start()
    {
        if (ambienceSource == null)
        {
            return;
        }

        ambienceSource.playOnAwake = false;
        ambienceSource.loop = true;
        ambienceSource.spatialBlend = 0f;
        ambienceSource.volume = 0f;
        ambienceSource.clip = forestAmbience;

        if (forestAmbience != null)
        {
            ambienceSource.Play();
        }
    }

    private void Update()
    {
        if (ambienceSource == null)
        {
            return;
        }

        targetVolume = playerInside ? ambienceVolume : 0f;

        float duration = playerInside
            ? fadeInDuration
            : fadeOutDuration;

        if (duration <= 0f)
        {
            ambienceSource.volume = targetVolume;
            return;
        }

        ambienceSource.volume = Mathf.MoveTowards(
            ambienceSource.volume,
            targetVolume,
            Time.deltaTime * Mathf.Max(ambienceVolume, 0.001f) / duration
        );
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsPlayer(other))
        {
            return;
        }

        playerInside = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!IsPlayer(other))
        {
            return;
        }

        playerInside = false;
    }

    private bool IsPlayer(Collider other)
    {
        PlayerHealth playerHealth =
            other.GetComponent<PlayerHealth>();

        if (playerHealth == null)
        {
            playerHealth =
                other.GetComponentInParent<PlayerHealth>();
        }

        return playerHealth != null;
    }
}
