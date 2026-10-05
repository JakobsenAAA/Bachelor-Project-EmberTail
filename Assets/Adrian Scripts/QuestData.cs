using UnityEngine;

public enum QuestObjectiveType
{
    Collect,
    Deliver,
    Kill,
    KillSpecificEnemy,
    ReachArea,
    Interact
}

[CreateAssetMenu(
    fileName = "NewQuest",
    menuName = "EmberTail/Quests/Quest"
)]
public class QuestData : ScriptableObject
{
    [Header("Identity")]
    [SerializeField] private string questId;
    [SerializeField] private string questName;

    [TextArea(2, 5)]
    [SerializeField] private string questDescription;

    [Header("Objective")]
    [SerializeField] private QuestObjectiveType objectiveType;

    [SerializeField] private string targetId;

    [SerializeField] private int requiredAmount = 1;

    [Header("Item")]
    [SerializeField] private ItemData requiredItem;

    [Header("Quest Chain")]
    [SerializeField] private QuestData nextQuest;

    public string QuestId =>
        questId;

    public string QuestName =>
        questName;

    public string QuestDescription =>
        questDescription;

    public QuestObjectiveType ObjectiveType =>
        objectiveType;

    public string TargetId =>
        targetId;

    public int RequiredAmount =>
        requiredAmount;

    public ItemData RequiredItem =>
        requiredItem;

    public QuestData NextQuest =>
        nextQuest;
}