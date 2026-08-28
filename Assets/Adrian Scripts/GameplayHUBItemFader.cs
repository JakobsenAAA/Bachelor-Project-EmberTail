using System.Collections;
using UnityEngine;

public class GameplayHUDItemFader : MonoBehaviour
{
    public enum HUDItemType
    {
        Cinder,
        Collectible1,
        Collectible2,
        Collectible3
    }

    [Header("Item")]
    [SerializeField] private HUDItemType itemType;

    [Header("Zone")]
    [SerializeField] private string zoneId = "zone1";

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
    private int previousCollectibleAmount;
    private bool initialized;

    private void Awake()
    {
        if (canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        if (playerHealth == null)
        {
            playerHealth = FindFirstObjectByType<PlayerHealth>();
        }

        if (collectibleManager == null)
        {
            collectibleManager = CollectibleManager.Instance;
        }

        if (canvasGroup != null)
        {
            canvasGroup.alpha = visibleOnStart ? 1f : 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
    }

    private void Start()
    {
        StoreCurrentAmount();
        initialized = true;
    }

    private void OnEnable()
    {
        if (playerHealth == null)
        {
            playerHealth = FindFirstObjectByType<PlayerHealth>();
        }

        if (collectibleManager == null)
        {
            collectibleManager = CollectibleManager.Instance;
        }

        if (itemType == HUDItemType.Cinder)
        {
            if (playerHealth != null)
            {
                playerHealth.OnCinderCollected.AddListener(ShowItem);
            }
        }
        else
        {
            if (collectibleManager != null)
            {
                collectibleManager.OnCollectiblesChanged.AddListener(CheckCollectibleChange);
            }
        }
    }

    private void OnDisable()
    {
        if (playerHealth != null)
        {
            playerHealth.OnCinderCollected.RemoveListener(ShowItem);
        }

        if (collectibleManager != null)
        {
            collectibleManager.OnCollectiblesChanged.RemoveListener(CheckCollectibleChange);
        }

        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
            fadeCoroutine = null;
        }
    }

    private void CheckCollectibleChange()
    {
        if (!initialized || collectibleManager == null)
        {
            StoreCurrentAmount();
            initialized = true;
            return;
        }

        int currentAmount = GetCurrentCollectibleAmount();

        if (currentAmount > previousCollectibleAmount)
        {
            ShowItem();
        }

        previousCollectibleAmount = currentAmount;
    }

    private void StoreCurrentAmount()
    {
        if (itemType == HUDItemType.Cinder)
        {
            return;
        }

        if (collectibleManager == null)
        {
            return;
        }

        previousCollectibleAmount = GetCurrentCollectibleAmount();
    }

    private int GetCurrentCollectibleAmount()
    {
        if (collectibleManager == null)
        {
            return 0;
        }

        switch (itemType)
        {
            case HUDItemType.Collectible1:
                return collectibleManager.GetCollected(
                    zoneId,
                    CollectibleType.Collectible1
                );

            case HUDItemType.Collectible2:
                return collectibleManager.GetCollected(
                    zoneId,
                    CollectibleType.Collectible2
                );

            case HUDItemType.Collectible3:
                return collectibleManager.GetCollected(
                    zoneId,
                    CollectibleType.Collectible3
                );

            default:
                return 0;
        }
    }

    public void ShowItem()
    {
        if (canvasGroup == null)
        {
            return;
        }

        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }

        fadeCoroutine = StartCoroutine(ShowRoutine());
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
        float startingAlpha = canvasGroup.alpha;

        if (duration <= 0f)
        {
            canvasGroup.alpha = targetAlpha;
            yield break;
        }

        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime;

            float progress = Mathf.Clamp01(
                timer / duration
            );

            canvasGroup.alpha = Mathf.Lerp(
                startingAlpha,
                targetAlpha,
                progress
            );

            yield return null;
        }

        canvasGroup.alpha = targetAlpha;
    }
}