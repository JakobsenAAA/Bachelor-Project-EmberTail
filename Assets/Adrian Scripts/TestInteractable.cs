using UnityEngine;

public class TestInteractable :
    MonoBehaviour,
    IInteractable
{
    [Header("Interaction")]
    [SerializeField] private string interactionPrompt =
        "Interact";

    [SerializeField] private bool canInteract = true;

    public string InteractionPrompt =>
        interactionPrompt;

    public bool CanInteract =>
        canInteract;

    public void Interact()
    {
        Debug.Log(
            "Interacted with: " +
            gameObject.name
        );
    }
}