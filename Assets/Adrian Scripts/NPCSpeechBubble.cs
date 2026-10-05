using System.Collections;
using TMPro;
using UnityEngine;

public class NPCSpeechBubble : MonoBehaviour
{
    [Header("Player Detection")]
    [SerializeField] private Transform player;

    [Header("Distance")]
    [SerializeField] private float fullyVisibleDistance = 2.5f;
    [SerializeField] private float fadeOutDistance = 4f;

    [Header("UI")]
    [SerializeField] private GameObject speechBubble;
    [SerializeField] private CanvasGroup speechBubbleCanvasGroup;
    [SerializeField] private TMP_Text npcNameText;
    [SerializeField] private TMP_Text speechText;

    [Header("NPC")]
    [SerializeField] private string npcName = "NPC";

    [Header("Dialogue")]
    [TextArea(2, 4)]
    [SerializeField] private string message;

    [Header("Typewriter")]
    [SerializeField] private float characterDelay = 0.03f;

    [Header("Fade")]
    [SerializeField] private float fadeSpeed = 5f;

    [Header("Camera")]
    [SerializeField] private Camera targetCamera;

    private Coroutine typewriterCoroutine;

    private bool playerInRange;
    private float targetAlpha;

    private void Start()
    {
        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }

        if (speechBubble != null)
        {
            speechBubble.SetActive(true);
        }

        if (speechBubbleCanvasGroup != null)
        {
            speechBubbleCanvasGroup.alpha = 0f;
        }

        if (npcNameText != null)
        {
            npcNameText.text =
                npcName;
        }

        if (speechText != null)
        {
            speechText.text =
                string.Empty;
        }
    }

    private void Update()
    {
        UpdateBubbleRotation();
        UpdatePlayerDistance();
        UpdateFade();
    }

    private void UpdatePlayerDistance()
    {
        if (player == null)
        {
            targetAlpha = 0f;
            return;
        }

        bool dialogueActive =
            DialogueManager.Instance != null &&
            DialogueManager.Instance.DialogueActive;

        if (dialogueActive)
        {
            targetAlpha = 0f;

            if (playerInRange)
            {
                playerInRange = false;
                StopTyping();
            }

            return;
        }

        float distance =
            Vector3.Distance(
                player.position,
                transform.position
            );

        bool isInRange =
            distance <
            fadeOutDistance;

        if (
            isInRange &&
            !playerInRange
        )
        {
            playerInRange = true;
            StartTyping();
        }
        else if (
            !isInRange &&
            playerInRange
        )
        {
            playerInRange = false;
            StopTyping();
        }

        if (!isInRange)
        {
            targetAlpha = 0f;
            return;
        }

        if (distance <= fullyVisibleDistance)
        {
            targetAlpha = 1f;
            return;
        }

        targetAlpha =
            Mathf.InverseLerp(
                fadeOutDistance,
                fullyVisibleDistance,
                distance
            );
    }

    private void UpdateFade()
    {
        if (speechBubbleCanvasGroup == null)
        {
            return;
        }

        speechBubbleCanvasGroup.alpha =
            Mathf.MoveTowards(
                speechBubbleCanvasGroup.alpha,
                targetAlpha,
                fadeSpeed *
                Time.deltaTime
            );
    }

    private void StartTyping()
    {
        StopTyping();

        if (speechText != null)
        {
            speechText.text =
                string.Empty;
        }

        typewriterCoroutine =
            StartCoroutine(
                TypeMessage()
            );
    }

    private IEnumerator TypeMessage()
    {
        if (speechText == null)
        {
            typewriterCoroutine = null;
            yield break;
        }

        for (int i = 0; i < message.Length; i++)
        {
            speechText.text +=
                message[i];

            if (characterDelay > 0f)
            {
                yield return
                    new WaitForSeconds(
                        characterDelay
                    );
            }
            else
            {
                yield return null;
            }
        }

        typewriterCoroutine = null;
    }

    private void StopTyping()
    {
        if (typewriterCoroutine != null)
        {
            StopCoroutine(
                typewriterCoroutine
            );

            typewriterCoroutine = null;
        }

        if (speechText != null)
        {
            speechText.text =
                string.Empty;
        }
    }

    private void UpdateBubbleRotation()
    {
        if (
            speechBubble == null ||
            targetCamera == null
        )
        {
            return;
        }

        Transform bubbleTransform =
            speechBubble.transform;

        Vector3 direction =
            bubbleTransform.position -
            targetCamera.transform.position;

        if (direction.sqrMagnitude <= 0.001f)
        {
            return;
        }

        bubbleTransform.rotation =
            Quaternion.LookRotation(
                direction,
                Vector3.up
            );
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(
            transform.position,
            fullyVisibleDistance
        );

        Gizmos.DrawWireSphere(
            transform.position,
            fadeOutDistance
        );
    }
}