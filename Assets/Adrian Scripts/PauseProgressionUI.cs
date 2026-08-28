using System.Collections;
using TMPro;
using UnityEngine;

public class PauseProgressionUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PauseMapController pauseMapController;
    [SerializeField] private CollectibleManager collectibleManager;
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("Zone Information")]
    [SerializeField] private TextMeshProUGUI discNameText;
    [SerializeField] private TextMeshProUGUI zoneNameText;
    [SerializeField] private TextMeshProUGUI zoneDescriptionText;

    [Header("Collectibles")]
    [SerializeField] private GameObject collectibleContainer;
    [SerializeField] private TextMeshProUGUI collectible1Text;
    [SerializeField] private TextMeshProUGUI collectible2Text;
    [SerializeField] private TextMeshProUGUI collectible3Text;

    [Header("Locked Content")]
    [SerializeField] private GameObject lockedContainer;
    [SerializeField] private TextMeshProUGUI lockedText;

    [Header("Transition")]
    [SerializeField] private float fadeOutDuration = 0.15f;
    [SerializeField] private float fadeInDuration = 0.25f;

    private Coroutine transitionCoroutine;

    private void Awake()
    {
        if (canvasGroup == null)
        {
            canvasGroup =
                GetComponent<CanvasGroup>();
        }

        if (collectibleManager == null)
        {
            collectibleManager =
                CollectibleManager.Instance;
        }
    }

    private void OnEnable()
    {
        if (pauseMapController != null)
        {
            pauseMapController
                .OnSelectionChanged
                .AddListener(
                    BeginTransition
                );
        }

        if (collectibleManager != null)
        {
            collectibleManager
                .OnCollectiblesChanged
                .AddListener(
                    UpdateUIImmediately
                );
        }

        UpdateUIImmediately();
    }

    private void OnDisable()
    {
        if (pauseMapController != null)
        {
            pauseMapController
                .OnSelectionChanged
                .RemoveListener(
                    BeginTransition
                );
        }

        if (collectibleManager != null)
        {
            collectibleManager
                .OnCollectiblesChanged
                .RemoveListener(
                    UpdateUIImmediately
                );
        }

        if (transitionCoroutine != null)
        {
            StopCoroutine(
                transitionCoroutine
            );

            transitionCoroutine = null;
        }
    }

    private void BeginTransition()
    {
        if (transitionCoroutine != null)
        {
            StopCoroutine(
                transitionCoroutine
            );
        }

        transitionCoroutine =
            StartCoroutine(
                TransitionRoutine()
            );
    }

    private IEnumerator TransitionRoutine()
    {
        yield return FadeTo(
            0f,
            fadeOutDuration
        );

        while (
            pauseMapController != null &&
            pauseMapController
                .IsSelectedDiscRotating()
        )
        {
            yield return null;
        }

        UpdateUIContent();

        yield return FadeTo(
            1f,
            fadeInDuration
        );

        transitionCoroutine = null;
    }

    private void UpdateUIImmediately()
    {
        UpdateUIContent();

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
        }
    }

    private void UpdateUIContent()
    {
        if (
            pauseMapController == null ||
            collectibleManager == null
        )
        {
            return;
        }

        ProgressionDiscDefinition disc =
            pauseMapController
                .GetSelectedDiscDefinition();

        ZoneProgressDefinition zone =
            pauseMapController
                .GetSelectedZoneDefinition();

        if (discNameText != null)
        {
            discNameText.text =
                disc != null
                    ? disc.DisplayName
                    : "";
        }

        if (zone == null)
        {
            ShowLockedState("???");
            return;
        }

        bool locked =
            disc.Locked ||
            zone.Locked;

        if (locked)
        {
            ShowLockedState(
                string.IsNullOrWhiteSpace(
                    zone.DisplayName
                )
                    ? "???"
                    : zone.DisplayName
            );

            return;
        }

        ShowUnlockedState(zone);
    }

    private void ShowUnlockedState(
        ZoneProgressDefinition zone
    )
    {
        if (zoneNameText != null)
        {
            zoneNameText.text =
                zone.DisplayName;
        }

        if (zoneDescriptionText != null)
        {
            zoneDescriptionText.text =
                zone.Description;
        }

        if (collectibleContainer != null)
        {
            collectibleContainer
                .SetActive(true);
        }

        if (lockedContainer != null)
        {
            lockedContainer
                .SetActive(false);
        }

        UpdateCollectibleText(
            collectible1Text,
            zone.ZoneId,
            CollectibleType.Collectible1
        );

        UpdateCollectibleText(
            collectible2Text,
            zone.ZoneId,
            CollectibleType.Collectible2
        );

        UpdateCollectibleText(
            collectible3Text,
            zone.ZoneId,
            CollectibleType.Collectible3
        );
    }

    private void ShowLockedState(
        string displayName
    )
    {
        if (zoneNameText != null)
        {
            zoneNameText.text =
                displayName;
        }

        if (zoneDescriptionText != null)
        {
            zoneDescriptionText.text = "";
        }

        if (collectibleContainer != null)
        {
            collectibleContainer
                .SetActive(false);
        }

        if (lockedContainer != null)
        {
            lockedContainer
                .SetActive(true);
        }

        if (lockedText != null)
        {
            lockedText.text =
                "COMING LATER";
        }
    }

    private void UpdateCollectibleText(
        TextMeshProUGUI text,
        string zoneId,
        CollectibleType type
    )
    {
        if (text == null)
        {
            return;
        }

        int collected =
            collectibleManager.GetCollected(
                zoneId,
                type
            );

        int total =
            collectibleManager.GetTotal(
                zoneId,
                type
            );

        text.text =
            collected +
            " / " +
            total;
    }

    private IEnumerator FadeTo(
        float targetAlpha,
        float duration
    )
    {
        if (canvasGroup == null)
        {
            yield break;
        }

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