using UnityEngine;

public class NPCDialogue :
    MonoBehaviour,
    IInteractable
{
    [Header("Dialogue")]
    [SerializeField] private DialogueData dialogue;

    [Header("Interaction")]
    [SerializeField] private string interactionPrompt =
        "Talk";

    public string InteractionPrompt =>
        interactionPrompt;

    public bool CanInteract =>
        dialogue != null &&
        (
            DialogueManager.Instance == null ||
            !DialogueManager.Instance.DialogueActive
        );

    public void Interact()
    {
        if (!CanInteract)
        {
            return;
        }

        if (DialogueManager.Instance == null)
        {
            Debug.LogWarning(
                "No DialogueManager was found."
            );

            return;
        }

        DialogueManager.Instance
            .StartDialogue(
                dialogue
            );
    }
}