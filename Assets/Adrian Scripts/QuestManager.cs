using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get; private set; }

    [Header("Available Quests")]
    [SerializeField] private QuestData[] startingQuests;

    [Header("Events")]
    public UnityEvent OnQuestChanged;

    private readonly Dictionary<string, QuestRuntimeData>
        quests =
            new Dictionary<string, QuestRuntimeData>();

    private PlayerInventory playerInventory;

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
        playerInventory =
            FindFirstObjectByType<PlayerInventory>();

        RegisterStartingQuests();
    }

    public void AcceptQuest(
        QuestData quest
    )
    {
        if (quest == null)
        {
            return;
        }

        QuestRuntimeData runtimeQuest =
            GetOrCreateQuest(
                quest
            );

        if (
            runtimeQuest.State !=
            QuestState.Available
        )
        {
            return;
        }

        runtimeQuest.SetState(
            QuestState.Active
        );

        RefreshQuestProgress(
            runtimeQuest
        );

        Debug.Log(
            "Quest accepted: " +
            quest.QuestName
        );

        OnQuestChanged.Invoke();
    }

    public void RefreshAllQuests()
    {
        foreach (
            KeyValuePair<string, QuestRuntimeData>
            pair in quests
        )
        {
            RefreshQuestProgress(
                pair.Value
            );
        }

        OnQuestChanged.Invoke();
    }

    public void ReportKill(
        string enemyId
    )
    {
        if (string.IsNullOrWhiteSpace(enemyId))
        {
            return;
        }

        foreach (
            KeyValuePair<string, QuestRuntimeData>
            pair in quests
        )
        {
            QuestRuntimeData runtimeQuest =
                pair.Value;

            if (
                runtimeQuest.State !=
                QuestState.Active
            )
            {
                continue;
            }

            QuestData quest =
                runtimeQuest.QuestData;

            if (
                quest.ObjectiveType ==
                QuestObjectiveType.Kill
            )
            {
                if (
                    quest.TargetId !=
                    enemyId
                )
                {
                    continue;
                }

                runtimeQuest.AddProgress(1);

                CheckObjectiveComplete(
                    runtimeQuest
                );
            }
            else if (
                quest.ObjectiveType ==
                QuestObjectiveType.KillSpecificEnemy
            )
            {
                if (
                    quest.TargetId !=
                    enemyId
                )
                {
                    continue;
                }

                runtimeQuest.SetProgress(
                    quest.RequiredAmount
                );

                CheckObjectiveComplete(
                    runtimeQuest
                );
            }
        }

        OnQuestChanged.Invoke();
    }

    public void ReportAreaReached(
        string areaId
    )
    {
        if (string.IsNullOrWhiteSpace(areaId))
        {
            return;
        }

        foreach (
            KeyValuePair<string, QuestRuntimeData>
            pair in quests
        )
        {
            QuestRuntimeData runtimeQuest =
                pair.Value;

            if (
                runtimeQuest.State !=
                QuestState.Active
            )
            {
                continue;
            }

            QuestData quest =
                runtimeQuest.QuestData;

            if (
                quest.ObjectiveType !=
                QuestObjectiveType.ReachArea
            )
            {
                continue;
            }

            if (
                quest.TargetId !=
                areaId
            )
            {
                continue;
            }

            runtimeQuest.SetProgress(
                quest.RequiredAmount
            );

            CheckObjectiveComplete(
                runtimeQuest
            );
        }

        OnQuestChanged.Invoke();
    }

    public void ReportInteraction(
        string interactionId
    )
    {
        if (
            string.IsNullOrWhiteSpace(
                interactionId
            )
        )
        {
            return;
        }

        foreach (
            KeyValuePair<string, QuestRuntimeData>
            pair in quests
        )
        {
            QuestRuntimeData runtimeQuest =
                pair.Value;

            if (
                runtimeQuest.State !=
                QuestState.Active
            )
            {
                continue;
            }

            QuestData quest =
                runtimeQuest.QuestData;

            if (
                quest.ObjectiveType !=
                QuestObjectiveType.Interact
            )
            {
                continue;
            }

            if (
                quest.TargetId !=
                interactionId
            )
            {
                continue;
            }

            runtimeQuest.SetProgress(
                quest.RequiredAmount
            );

            CheckObjectiveComplete(
                runtimeQuest
            );
        }

        OnQuestChanged.Invoke();
    }

    public bool TurnInQuest(
        QuestData quest
    )
    {
        if (quest == null)
        {
            return false;
        }

        QuestRuntimeData runtimeQuest =
            GetQuest(
                quest.QuestId
            );

        if (runtimeQuest == null)
        {
            return false;
        }

        if (
            runtimeQuest.State !=
            QuestState.ReadyToTurnIn
        )
        {
            return false;
        }

        if (
            quest.ObjectiveType ==
            QuestObjectiveType.Deliver
        )
        {
            if (
                playerInventory == null ||
                quest.RequiredItem == null
            )
            {
                return false;
            }

            if (
                !playerInventory.HasItem(
                    quest.RequiredItem,
                    quest.RequiredAmount
                )
            )
            {
                RefreshQuestProgress(
                    runtimeQuest
                );

                OnQuestChanged.Invoke();

                return false;
            }

            playerInventory.RemoveItem(
                quest.RequiredItem,
                quest.RequiredAmount
            );
        }

        runtimeQuest.SetState(
            QuestState.Completed
        );

        Debug.Log(
            "Quest completed: " +
            quest.QuestName
        );

        UnlockNextQuest(
            quest
        );

        OnQuestChanged.Invoke();

        return true;
    }

    public QuestRuntimeData GetQuest(
        string questId
    )
    {
        if (
            string.IsNullOrWhiteSpace(
                questId
            )
        )
        {
            return null;
        }

        if (
            quests.TryGetValue(
                questId,
                out QuestRuntimeData quest
            )
        )
        {
            return quest;
        }

        return null;
    }

    public QuestState GetQuestState(
        QuestData quest
    )
    {
        if (quest == null)
        {
            return QuestState.Unavailable;
        }

        QuestRuntimeData runtimeQuest =
            GetQuest(
                quest.QuestId
            );

        if (runtimeQuest == null)
        {
            return QuestState.Unavailable;
        }

        return runtimeQuest.State;
    }

    private void RegisterStartingQuests()
    {
        if (startingQuests == null)
        {
            return;
        }

        for (
            int i = 0;
            i < startingQuests.Length;
            i++
        )
        {
            QuestData quest =
                startingQuests[i];

            if (quest == null)
            {
                continue;
            }

            QuestRuntimeData runtimeQuest =
                GetOrCreateQuest(
                    quest
                );

            runtimeQuest.SetState(
                QuestState.Available
            );
        }

        OnQuestChanged.Invoke();
    }

    private QuestRuntimeData GetOrCreateQuest(
        QuestData quest
    )
    {
        if (
            quests.TryGetValue(
                quest.QuestId,
                out QuestRuntimeData existingQuest
            )
        )
        {
            return existingQuest;
        }

        QuestRuntimeData newQuest =
            new QuestRuntimeData(
                quest,
                QuestState.Unavailable
            );

        quests.Add(
            quest.QuestId,
            newQuest
        );

        return newQuest;
    }

    private void RefreshQuestProgress(
        QuestRuntimeData runtimeQuest
    )
    {
        if (runtimeQuest == null)
        {
            return;
        }

        if (
            runtimeQuest.State !=
            QuestState.Active &&
            runtimeQuest.State !=
            QuestState.ReadyToTurnIn
        )
        {
            return;
        }

        QuestData quest =
            runtimeQuest.QuestData;

        switch (quest.ObjectiveType)
        {
            case QuestObjectiveType.Collect:
            case QuestObjectiveType.Deliver:

                if (
                    playerInventory == null ||
                    quest.RequiredItem == null
                )
                {
                    return;
                }

                runtimeQuest.SetProgress(
                    playerInventory.GetItemAmount(
                        quest.RequiredItem
                    )
                );

                break;
        }

        CheckObjectiveComplete(
            runtimeQuest
        );
    }

    private void CheckObjectiveComplete(
        QuestRuntimeData runtimeQuest
    )
    {
        if (
            runtimeQuest.State !=
            QuestState.Active &&
            runtimeQuest.State !=
            QuestState.ReadyToTurnIn
        )
        {
            return;
        }

        if (
            runtimeQuest.CurrentProgress >=
            runtimeQuest.QuestData.RequiredAmount
        )
        {
            runtimeQuest.SetState(
                QuestState.ReadyToTurnIn
            );

            Debug.Log(
                "Quest objective complete: " +
                runtimeQuest
                    .QuestData
                    .QuestName
            );
        }
        else if (
            runtimeQuest.State ==
            QuestState.ReadyToTurnIn
        )
        {
            runtimeQuest.SetState(
                QuestState.Active
            );
        }
    }

    private void UnlockNextQuest(
        QuestData completedQuest
    )
    {
        QuestData nextQuest =
            completedQuest.NextQuest;

        if (nextQuest == null)
        {
            return;
        }

        QuestRuntimeData nextRuntimeQuest =
            GetOrCreateQuest(
                nextQuest
            );

        if (
            nextRuntimeQuest.State ==
            QuestState.Unavailable
        )
        {
            nextRuntimeQuest.SetState(
                QuestState.Available
            );

            Debug.Log(
                "New quest available: " +
                nextQuest.QuestName
            );
        }
    }
}