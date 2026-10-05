using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class QuestOfferUI : MonoBehaviour
{
    public static QuestOfferUI Instance { get; private set; }

    [Header("UI")]
    [SerializeField] private GameObject questOfferPanel;
    [SerializeField] private TMP_Text questNameText;
    [SerializeField] private TMP_Text questDescriptionText;

    [Header("Options")]
    [SerializeField] private Image acceptHighlight;
    [SerializeField] private Image declineHighlight;

    [Header("Input")]
    [SerializeField] private float inputDelay = 0.15f;

    private QuestData currentQuest;

    private bool offerActive;
    private bool inputEnabled;
    private int selectedOption;

    private Coroutine inputDelayCoroutine;

    public bool OfferActive =>
        offerActive;

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
        if (questOfferPanel != null)
        {
            questOfferPanel.SetActive(false);
        }

        UpdateSelectionVisuals();
    }

    private void Update()
    {
        if (
            !offerActive ||
            !inputEnabled
        )
        {
            return;
        }

        Keyboard keyboard =
            Keyboard.current;

        if (keyboard == null)
        {
            return;
        }

        if (keyboard.aKey.wasPressedThisFrame)
        {
            selectedOption = 0;

            UpdateSelectionVisuals();
        }

        if (keyboard.dKey.wasPressedThisFrame)
        {
            selectedOption = 1;

            UpdateSelectionVisuals();
        }

        if (keyboard.fKey.wasPressedThisFrame)
        {
            ConfirmSelection();
        }
    }

    public void ShowQuest(
        QuestData quest
    )
    {
        if (quest == null)
        {
            return;
        }

        currentQuest = quest;

        selectedOption = 0;
        offerActive = true;
        inputEnabled = false;

        if (questNameText != null)
        {
            questNameText.text =
                quest.QuestName;
        }

        if (questDescriptionText != null)
        {
            questDescriptionText.text =
                quest.QuestDescription;
        }

        if (questOfferPanel != null)
        {
            questOfferPanel.SetActive(true);
        }

        Cursor.lockState =
            CursorLockMode.Locked;

        Cursor.visible = false;

        UpdateSelectionVisuals();

        if (inputDelayCoroutine != null)
        {
            StopCoroutine(
                inputDelayCoroutine
            );
        }

        inputDelayCoroutine =
            StartCoroutine(
                EnableInputAfterDelay()
            );
    }

    private IEnumerator EnableInputAfterDelay()
    {
        yield return
            new WaitForSecondsRealtime(
                inputDelay
            );

        inputEnabled = true;
        inputDelayCoroutine = null;
    }

    private void ConfirmSelection()
    {
        if (
            !offerActive ||
            !inputEnabled
        )
        {
            return;
        }

        inputEnabled = false;

        if (selectedOption == 0)
        {
            AcceptQuest();
        }
        else
        {
            DeclineQuest();
        }
    }

    private void AcceptQuest()
    {
        if (
            currentQuest != null &&
            QuestManager.Instance != null
        )
        {
            QuestManager.Instance
                .AcceptQuest(
                    currentQuest
                );
        }

        CloseQuestOffer();
    }

    private void DeclineQuest()
    {
        CloseQuestOffer();
    }

    private void CloseQuestOffer()
    {
        offerActive = false;
        inputEnabled = false;
        currentQuest = null;

        if (inputDelayCoroutine != null)
        {
            StopCoroutine(
                inputDelayCoroutine
            );

            inputDelayCoroutine = null;
        }

        if (questOfferPanel != null)
        {
            questOfferPanel.SetActive(false);
        }

        UpdateSelectionVisuals();

        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance
                .ReleaseGameplayLock();
        }
    }

    private void UpdateSelectionVisuals()
    {
        if (acceptHighlight != null)
        {
            acceptHighlight.enabled =
                offerActive &&
                selectedOption == 0;
        }

        if (declineHighlight != null)
        {
            declineHighlight.enabled =
                offerActive &&
                selectedOption == 1;
        }
    }
}