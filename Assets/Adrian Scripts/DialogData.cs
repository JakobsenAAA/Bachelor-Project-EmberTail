using UnityEngine;

[CreateAssetMenu(
    fileName = "NewDialogue",
    menuName = "EmberTail/Dialogue/Dialogue"
)]
public class DialogueData : ScriptableObject
{
    [Header("Speaker")]
    [SerializeField] private string speakerName;

    [Header("Dialogue")]
    [TextArea(2, 5)]
    [SerializeField] private string[] dialogueLines;

    public string SpeakerName =>
        speakerName;

    public string[] DialogueLines =>
        dialogueLines;
}