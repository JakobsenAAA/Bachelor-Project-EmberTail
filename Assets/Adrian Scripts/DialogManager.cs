using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [Header("UI")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TMP_Text speakerNameText;
    [SerializeField] private TMP_Text dialogueText;

    [Header("Typewriter")]
    [SerializeField] private float characterDelay = 0.03f;

    [Header("Player")]
    [SerializeField] private PlayerController playerController;
    [SerializeField] private PlayerInteraction playerInteraction;

    [Header("Camera")]
    [SerializeField] private ThirdPersonCamera thirdPersonCamera;

    private DialogueData currentDialogue;
    private int currentLineIndex;

    private Coroutine typewriterCoroutine;

    private bool dialogueActive;
    private bool lineTyping;

    private Action dialogueCompletedAction;

    public bool DialogueActive =>
        dialogueActive;

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
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }
    }

    public void StartDialogue(
        DialogueData dialogue
    )
    {
        StartDialogue(
            dialogue,
            null
        );
    }

    public void StartDialogue(
        DialogueData dialogue,
        Action completedAction
    )
    {
        if (dialogue == null)
        {
            return;
        }

        if (
            dialogue.DialogueLines == null ||
            dialogue.DialogueLines.Length == 0
        )
        {
            return;
        }

        currentDialogue = dialogue;
        currentLineIndex = 0;
        dialogueActive = true;

        dialogueCompletedAction =
            completedAction;

        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(true);
        }

        if (speakerNameText != null)
        {
            speakerNameText.text =
                currentDialogue.SpeakerName;
        }

        LockGameplay();

        ShowCurrentLine();
    }

    public void HandleInteract()
    {
        if (!dialogueActive)
        {
            return;
        }

        if (lineTyping)
        {
            FinishCurrentLine();
            return;
        }

        currentLineIndex++;

        if (
            currentLineIndex >=
            currentDialogue.DialogueLines.Length
        )
        {
            EndDialogue();
            return;
        }

        ShowCurrentLine();
    }

    private void ShowCurrentLine()
    {
        if (typewriterCoroutine != null)
        {
            StopCoroutine(
                typewriterCoroutine
            );
        }

        typewriterCoroutine =
            StartCoroutine(
                TypeLine(
                    currentDialogue
                        .DialogueLines[
                            currentLineIndex
                        ]
                )
            );
    }

    private IEnumerator TypeLine(
        string line
    )
    {
        lineTyping = true;

        if (dialogueText != null)
        {
            dialogueText.text =
                string.Empty;
        }

        for (int i = 0; i < line.Length; i++)
        {
            if (dialogueText != null)
            {
                dialogueText.text +=
                    line[i];
            }

            if (characterDelay > 0f)
            {
                yield return
                    new WaitForSecondsRealtime(
                        characterDelay
                    );
            }
            else
            {
                yield return null;
            }
        }

        lineTyping = false;
        typewriterCoroutine = null;
    }

    private void FinishCurrentLine()
    {
        if (typewriterCoroutine != null)
        {
            StopCoroutine(
                typewriterCoroutine
            );

            typewriterCoroutine = null;
        }

        if (
            dialogueText != null &&
            currentDialogue != null
        )
        {
            dialogueText.text =
                currentDialogue.DialogueLines[
                    currentLineIndex
                ];
        }

        lineTyping = false;
    }

    private void EndDialogue()
    {
        if (typewriterCoroutine != null)
        {
            StopCoroutine(
                typewriterCoroutine
            );

            typewriterCoroutine = null;
        }

        dialogueActive = false;
        lineTyping = false;

        currentDialogue = null;
        currentLineIndex = 0;

        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }

        Action completedAction =
            dialogueCompletedAction;

        dialogueCompletedAction = null;

        if (completedAction != null)
        {
            completedAction.Invoke();
        }
        else
        {
            UnlockGameplay();
        }
    }

    public void ReleaseGameplayLock()
    {
        UnlockGameplay();
    }

    private void LockGameplay()
    {
        if (playerController != null)
        {
            playerController
                .SetGameplayInputEnabled(false);
        }

        if (thirdPersonCamera != null)
        {
            thirdPersonCamera
                .SetCameraInputEnabled(false);
        }

        if (playerInteraction != null)
        {
            playerInteraction
                .SetInteractionEnabled(false);
        }
    }

    private void UnlockGameplay()
    {
        if (playerController != null)
        {
            playerController
                .SetGameplayInputEnabled(true);
        }

        if (thirdPersonCamera != null)
        {
            thirdPersonCamera
                .SetCameraInputEnabled(true);
        }

        if (playerInteraction != null)
        {
            playerInteraction
                .SetInteractionEnabled(true);
        }
    }
}