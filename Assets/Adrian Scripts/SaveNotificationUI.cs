using System.Collections;
using TMPro;
using UnityEngine;

public class SaveNotificationUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TextMeshProUGUI messageText;

    [Header("Timing")]
    [SerializeField] private float fadeInDuration = 0.2f;
    [SerializeField] private float visibleDuration = 1.5f;
    [SerializeField] private float fadeOutDuration = 0.5f;

    private Coroutine notificationRoutine;

    private void Awake()
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
    }

    public void ShowMessage(string message)
    {
        if (canvasGroup == null)
        {
            return;
        }

        if (messageText != null)
        {
            messageText.text = message;
        }

        if (notificationRoutine != null)
        {
            StopCoroutine(notificationRoutine);
        }

        notificationRoutine =
            StartCoroutine(
                ShowNotificationRoutine()
            );
    }

    private IEnumerator ShowNotificationRoutine()
    {
        yield return Fade(
            0f,
            1f,
            fadeInDuration
        );

        float timer = 0f;

        while (timer < visibleDuration)
        {
            timer += Time.unscaledDeltaTime;
            yield return null;
        }

        yield return Fade(
            1f,
            0f,
            fadeOutDuration
        );

        notificationRoutine = null;
    }

    private IEnumerator Fade(
        float startAlpha,
        float endAlpha,
        float duration
    )
    {
        if (duration <= 0f)
        {
            canvasGroup.alpha = endAlpha;
            yield break;
        }

        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime;

            float progress =
                Mathf.Clamp01(
                    timer / duration
                );

            canvasGroup.alpha =
                Mathf.Lerp(
                    startAlpha,
                    endAlpha,
                    progress
                );

            yield return null;
        }

        canvasGroup.alpha = endAlpha;
    }
}