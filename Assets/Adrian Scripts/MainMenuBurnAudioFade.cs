
using System.Collections;
using UnityEngine;

public class MainMenuBurnAudioFade : MonoBehaviour
{
    private AudioSource audioSource;
    private float targetVolume;
    private float duration;
    private float fadeIn;
    private float fadeOut;

    public void Initialize(
        AudioSource source,
        float volume,
        float soundDuration,
        float fadeInDuration,
        float fadeOutDuration
    )
    {
        audioSource = source;
        targetVolume = Mathf.Clamp01(volume);
        duration = Mathf.Max(0f, soundDuration);
        fadeIn = Mathf.Max(0f, fadeInDuration);
        fadeOut = Mathf.Max(0f, fadeOutDuration);

        StartCoroutine(PlayRoutine());
    }

    private IEnumerator PlayRoutine()
    {
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime;

            float fadeInMultiplier = fadeIn <= 0f
                ? 1f
                : Mathf.Clamp01(timer / fadeIn);

            float fadeOutMultiplier = fadeOut <= 0f
                ? 1f
                : Mathf.Clamp01((duration - timer) / fadeOut);

            if (audioSource != null)
            {
                audioSource.volume =
                    targetVolume *
                    Mathf.Min(
                        fadeInMultiplier,
                        fadeOutMultiplier
                    );
            }

            yield return null;
        }

        if (audioSource != null)
        {
            audioSource.Stop();
        }

        Destroy(gameObject);
    }
}
