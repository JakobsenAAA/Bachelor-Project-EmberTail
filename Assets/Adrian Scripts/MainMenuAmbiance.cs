
using UnityEngine;

public class MainMenuAmbience : MonoBehaviour
{
    [Header("Forest Ambience")]
    [SerializeField] private AudioSource forestAudioSource;
    [SerializeField] private AudioClip forestAmbience;
    [SerializeField, Range(0f, 1f)] private float forestVolume = 0.5f;

    [Header("Bonfire Ambience")]
    [SerializeField] private AudioSource bonfireAudioSource;
    [SerializeField] private AudioClip bonfireAmbience;
    [SerializeField, Range(0f, 1f)] private float bonfireVolume = 0.7f;

    private void Start()
    {
        StartAmbience();
    }

    private void OnEnable()
    {
        if (Application.isPlaying)
        {
            StartAmbience();
        }
    }

    private void OnDisable()
    {
        StopAmbience();
    }

    public void StartAmbience()
    {
        SetupSource(
            forestAudioSource,
            forestAmbience,
            forestVolume
        );

        SetupSource(
            bonfireAudioSource,
            bonfireAmbience,
            bonfireVolume
        );
    }

    public void StopAmbience()
    {
        if (forestAudioSource != null)
        {
            forestAudioSource.Stop();
        }

        if (bonfireAudioSource != null)
        {
            bonfireAudioSource.Stop();
        }
    }

    private void SetupSource(
        AudioSource source,
        AudioClip clip,
        float volume
    )
    {
        if (source == null || clip == null)
        {
            return;
        }

        source.clip = clip;
        source.volume = volume;
        source.loop = true;
        source.playOnAwake = false;
        source.spatialBlend = 0f;

        if (!source.isPlaying)
        {
            source.Play();
        }
    }
}
