using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScreenManager : MonoBehaviour
{
    public static LoadingScreenManager Instance { get; private set; }

    [Header("Fade")]
    [SerializeField] private CanvasGroup fadeOverlay;
    [SerializeField] private float sceneFadeOutDuration = 0.5f;
    [SerializeField] private float sceneFadeInDuration = 0.5f;

    [Header("Loading Screen")]
    [SerializeField] private GameObject loadingPanel;
    [SerializeField] private Slider progressBar;
    [SerializeField] private TextMeshProUGUI progressText;
    [SerializeField] private float minimumLoadingScreenTime = 2f;

    [Header("Rat Animation")]
    [SerializeField] private Image ratImage;
    [SerializeField] private Sprite[] ratFrames;
    [SerializeField] private float frameInterval = 0.33f;

    private bool isTransitioning;
    private float frameTimer;
    private int frameIndex;

    public bool IsTransitioning => isTransitioning;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);

        if (fadeOverlay != null)
        {
            fadeOverlay.alpha = 0f;
            fadeOverlay.interactable = false;
            fadeOverlay.blocksRaycasts = false;
        }

        if (loadingPanel != null)
        {
            loadingPanel.SetActive(false);
        }

        SetProgress(0f);
    }

    private void Update()
    {
        UpdateRatAnimation();
    }

    private void UpdateRatAnimation()
    {
        if (
            loadingPanel == null ||
            !loadingPanel.activeSelf ||
            ratImage == null ||
            ratFrames == null ||
            ratFrames.Length == 0
        )
        {
            return;
        }

        frameTimer += Time.unscaledDeltaTime;

        if (frameTimer < frameInterval)
        {
            return;
        }

        frameTimer -= frameInterval;

        frameIndex++;

        if (frameIndex >= ratFrames.Length)
        {
            frameIndex = 0;
        }

        ratImage.sprite = ratFrames[frameIndex];
    }

    public void LoadScene(string sceneName)
    {
        if (
            isTransitioning ||
            string.IsNullOrWhiteSpace(sceneName)
        )
        {
            return;
        }

        StartCoroutine(
            LoadSceneRoutine(sceneName)
        );
    }

    public void StartRespawnTransition(
        Action respawnAction,
        float fadeOutDuration,
        float loadingDuration,
        float fadeInDuration
    )
    {
        if (isTransitioning)
        {
            return;
        }

        StartCoroutine(
            RespawnTransitionRoutine(
                respawnAction,
                fadeOutDuration,
                loadingDuration,
                fadeInDuration
            )
        );
    }

    private IEnumerator LoadSceneRoutine(
        string sceneName
    )
    {
        isTransitioning = true;

        Time.timeScale = 1f;

        yield return FadeTo(
            1f,
            sceneFadeOutDuration
        );

        ShowLoadingScreen();

        AsyncOperation operation =
            SceneManager.LoadSceneAsync(
                sceneName
            );

        operation.allowSceneActivation = false;

        float timer = 0f;
        float displayedProgress = 0f;

        while (true)
        {
            timer += Time.unscaledDeltaTime;

            float actualProgress =
                Mathf.Clamp01(
                    operation.progress / 0.9f
                );

            float timedProgress;

            if (minimumLoadingScreenTime <= 0f)
            {
                timedProgress = 1f;
            }
            else
            {
                timedProgress =
                    Mathf.Clamp01(
                        timer /
                        minimumLoadingScreenTime
                    );
            }

            float targetProgress =
                Mathf.Min(
                    actualProgress,
                    timedProgress
                );

            displayedProgress =
                Mathf.MoveTowards(
                    displayedProgress,
                    targetProgress,
                    Time.unscaledDeltaTime
                );

            SetProgress(
                displayedProgress
            );

            bool sceneReady =
                operation.progress >= 0.9f;

            bool minimumTimeFinished =
                timer >= minimumLoadingScreenTime;

            bool visualFinished =
                displayedProgress >= 0.999f;

            if (
                sceneReady &&
                minimumTimeFinished &&
                visualFinished
            )
            {
                break;
            }

            yield return null;
        }

        SetProgress(1f);

        yield return new WaitForSecondsRealtime(
            0.1f
        );

        operation.allowSceneActivation = true;

        while (!operation.isDone)
        {
            yield return null;
        }

        HideLoadingScreen();

        yield return null;

        yield return FadeTo(
            0f,
            sceneFadeInDuration
        );

        isTransitioning = false;
    }

    private IEnumerator RespawnTransitionRoutine(
        Action respawnAction,
        float fadeOutDuration,
        float loadingDuration,
        float fadeInDuration
    )
    {
        isTransitioning = true;

        Time.timeScale = 0f;

        yield return FadeTo(
            1f,
            fadeOutDuration
        );

        ShowLoadingScreen();

        float timer = 0f;

        if (loadingDuration <= 0f)
        {
            SetProgress(1f);
        }
        else
        {
            while (timer < loadingDuration)
            {
                timer +=
                    Time.unscaledDeltaTime;

                float progress =
                    Mathf.Clamp01(
                        timer /
                        loadingDuration
                    );

                SetProgress(progress);

                yield return null;
            }
        }

        SetProgress(1f);

        if (respawnAction != null)
        {
            respawnAction.Invoke();
        }

        yield return null;

        HideLoadingScreen();

        yield return FadeTo(
            0f,
            fadeInDuration
        );

        Time.timeScale = 1f;

        isTransitioning = false;
    }

    private IEnumerator FadeTo(
        float targetAlpha,
        float duration
    )
    {
        if (fadeOverlay == null)
        {
            yield break;
        }

        fadeOverlay.blocksRaycasts = true;

        float startingAlpha =
            fadeOverlay.alpha;

        if (duration <= 0f)
        {
            fadeOverlay.alpha =
                targetAlpha;

            if (targetAlpha <= 0f)
            {
                fadeOverlay.blocksRaycasts =
                    false;
            }

            yield break;
        }

        float timer = 0f;

        while (timer < duration)
        {
            timer +=
                Time.unscaledDeltaTime;

            float progress =
                Mathf.Clamp01(
                    timer /
                    duration
                );

            fadeOverlay.alpha =
                Mathf.Lerp(
                    startingAlpha,
                    targetAlpha,
                    progress
                );

            yield return null;
        }

        fadeOverlay.alpha =
            targetAlpha;

        if (targetAlpha <= 0f)
        {
            fadeOverlay.blocksRaycasts =
                false;
        }
    }

    private void ShowLoadingScreen()
    {
        SetProgress(0f);

        frameTimer = 0f;
        frameIndex = 0;

        if (
            ratImage != null &&
            ratFrames != null &&
            ratFrames.Length > 0
        )
        {
            ratImage.sprite =
                ratFrames[0];
        }

        if (loadingPanel != null)
        {
            loadingPanel.SetActive(true);
        }
    }

    private void HideLoadingScreen()
    {
        if (loadingPanel != null)
        {
            loadingPanel.SetActive(false);
        }
    }

    private void SetProgress(float value)
    {
        float progress =
            Mathf.Clamp01(value);

        if (progressBar != null)
        {
            progressBar.value =
                progress;
        }

        if (progressText != null)
        {
            progressText.text =
                Mathf.RoundToInt(
                    progress * 100f
                ) +
                "%";
        }
    }
}