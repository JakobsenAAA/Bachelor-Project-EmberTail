using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestNotificationUI : MonoBehaviour
{
    public static QuestNotificationUI Instance { get; private set; }

    [Header("UI")]
    [SerializeField] private CanvasGroup notificationGroup;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text questNameText;
    [SerializeField] private Image notificationImage;

    [Header("Images")]
    [SerializeField] private Sprite objectiveCompleteImage;
    [SerializeField] private Sprite questCompleteImage;

    [Header("Timing")]
    [SerializeField] private float fadeInDuration = 0.25f;
    [SerializeField] private float visibleDuration = 2f;
    [SerializeField] private float fadeOutDuration = 0.5f;

    private Coroutine notificationCoroutine;

    private void Awake()
    {
        if (
            Instance != null &&
            Instance != this
        )
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        if (notificationGroup != null)
        {
            notificationGroup.alpha = 0f;
            notificationGroup.interactable = false;
            notificationGroup.blocksRaycasts = false;
        }
    }

    public void ShowObjectiveComplete(
        string questName
    )
    {
        ShowNotification(
            "OBJECTIVE COMPLETE",
            questName,
            objectiveCompleteImage
        );
    }

    public void ShowQuestComplete(
        string questName
    )
    {
        ShowNotification(
            "QUEST COMPLETE",
            questName,
            questCompleteImage
        );
    }

    private void ShowNotification(
        string title,
        string questName,
        Sprite image
    )
    {
        if (notificationCoroutine != null)
        {
            StopCoroutine(
                notificationCoroutine
            );
        }

        if (titleText != null)
        {
            titleText.text =
                title;
        }

        if (questNameText != null)
        {
            questNameText.text =
                questName;
        }

        if (notificationImage != null)
        {
            notificationImage.sprite =
                image;

            notificationImage.enabled =
                image != null;
        }

        notificationCoroutine =
            StartCoroutine(
                ShowNotificationRoutine()
            );
    }

    private IEnumerator ShowNotificationRoutine()
    {
        if (notificationGroup == null)
        {
            yield break;
        }

        notificationGroup.alpha = 0f;

        float timer = 0f;

        while (
            timer <
            fadeInDuration
        )
        {
            timer +=
                Time.unscaledDeltaTime;

            notificationGroup.alpha =
                fadeInDuration <= 0f
                    ? 1f
                    : Mathf.Clamp01(
                        timer /
                        fadeInDuration
                    );

            yield return null;
        }

        notificationGroup.alpha = 1f;

        yield return
            new WaitForSecondsRealtime(
                visibleDuration
            );

        timer = 0f;

        while (
            timer <
            fadeOutDuration
        )
        {
            timer +=
                Time.unscaledDeltaTime;

            notificationGroup.alpha =
                fadeOutDuration <= 0f
                    ? 0f
                    : 1f -
                    Mathf.Clamp01(
                        timer /
                        fadeOutDuration
                    );

            yield return null;
        }

        notificationGroup.alpha = 0f;
        notificationCoroutine = null;
    }
}