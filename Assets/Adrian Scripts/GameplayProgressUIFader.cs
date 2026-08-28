using System.Collections;
using UnityEngine;

public class GameplayProgressUIFader : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private CollectibleManager collectibleManager;

    [Header("Timing")]
    [SerializeField] private float visibleDuration = 3f;
    [SerializeField] private float fadeInDuration = 0.2f;
    [SerializeField] private float fadeOutDuration = 0.5f;

    [Header("Start")]
    [SerializeField] private bool visibleOnStart;

    private Coroutine fadeCoroutine;

    private void Awake()
    {
        if (canvasGroup == null)
        {
            canvasGroup =
                GetComponent<CanvasGroup>();
        }

        if (playerHealth == null)
        {
            playerHealth =
                FindFirstObjectByType<PlayerHealth>();
        }

        if (collectibleManager == null)
        {
            collectibleManager =
                CollectibleManager.Instance;
        }

        if (canvasGroup != null)
        {
            canvasGroup.alpha =
                visibleOnStart ? 1f : 0f;

            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
    }

    private void OnEnable()
    {
        if (playerHealth != null)
        {
            playerHealth
                .OnCinderCollected
                .AddListener(ShowProgressUI);
        }

        if (collectibleManager != null)
        {
            collectibleManager
                .OnCollectiblesChanged
                .AddListener(ShowProgressUI);
        }
    }

    private void OnDisable()
    {
        if (playerHealth != null)
        {
            playerHealth
                .OnCinderCollected
                .RemoveListener(ShowProgressUI);
        }

        if (collectibleManager != null)
        {
            collectibleManager
                .OnCollectiblesChanged
                .RemoveListener(ShowProgressUI);
        }

        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
            fadeCoroutine = null;
        }
    }

    public void ShowProgressUI()
    {
        if (canvasGroup == null)
        {
            return;
        }

        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }

        fadeCoroutine =
            StartCoroutine(
                ShowRoutine()
            );
    }

    private IEnumerator ShowRoutine()
    {
        yield return FadeTo(
            1f,
            fadeInDuration
        );

        float timer = 0f;

        while (timer < visibleDuration)
        {
            timer += Time.unscaledDeltaTime;
            yield return null;
        }

        yield return FadeTo(
            0f,
            fadeOutDuration
        );

        fadeCoroutine = null;
    }

    private IEnumerator FadeTo(
        float targetAlpha,
        float duration
    )
    {
        float startingAlpha =
            canvasGroup.alpha;

        if (duration <= 0f)
        {
            canvasGroup.alpha =
                targetAlpha;

            yield break;
        }

        float timer = 0f;

        while (timer < duration)
        {
            timer +=
                Time.unscaledDeltaTime;

            float progress =
                Mathf.Clamp01(
                    timer / duration
                );

            canvasGroup.alpha =
                Mathf.Lerp(
                    startingAlpha,
                    targetAlpha,
                    progress
                );

            yield return null;
        }

        canvasGroup.alpha =
            targetAlpha;
    }
}