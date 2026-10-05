using UnityEngine;

public class QuestGiver :
    MonoBehaviour,
    IInteractable
{
    [Header("Quest")]
    [SerializeField] private QuestData quest;

    [Header("Dialogue")]
    [SerializeField] private DialogueData offerDialogue;
    [SerializeField] private DialogueData activeDialogue;
    [SerializeField] private DialogueData completionDialogue;
    [SerializeField] private DialogueData completedDialogue;

    [Header("Interaction Prompts")]
    [SerializeField] private string talkPrompt =
        "Talk";

    [SerializeField] private string turnInPrompt =
        "Turn In Quest";

    [Header("Reward")]
    [SerializeField] private Transform rewardSpawnPoint;

    private GameObject spawnedReward;

    public string InteractionPrompt
    {
        get
        {
            if (
                QuestManager.Instance == null ||
                quest == null
            )
            {
                return talkPrompt;
            }

            QuestState state =
                QuestManager.Instance
                    .GetQuestState(
                        quest
                    );

            if (
                state ==
                QuestState.ReadyToTurnIn
            )
            {
                if (
                    quest.ObjectiveType ==
                    QuestObjectiveType.Deliver &&
                    quest.RequiredItem != null
                )
                {
                    return
                        "Give " +
                        quest.RequiredAmount +
                        " " +
                        quest.RequiredItem.DisplayName;
                }

                return turnInPrompt;
            }

            return talkPrompt;
        }
    }

    public bool CanInteract =>
        quest != null &&
        (
            DialogueManager.Instance == null ||
            !DialogueManager.Instance.DialogueActive
        );

    public void Interact()
    {
        if (
            quest == null ||
            QuestManager.Instance == null ||
            DialogueManager.Instance == null
        )
        {
            return;
        }

        QuestState state =
            QuestManager.Instance
                .GetQuestState(
                    quest
                );

        switch (state)
        {
            case QuestState.Available:
                StartOfferDialogue();
                break;

            case QuestState.Active:
                StartActiveDialogue();
                break;

            case QuestState.ReadyToTurnIn:
                StartCompletionDialogue();
                break;

            case QuestState.Completed:
                StartCompletedDialogue();
                break;
        }
    }

    private void StartOfferDialogue()
    {
        if (offerDialogue == null)
        {
            ShowQuestOffer();
            return;
        }

        DialogueManager.Instance
            .StartDialogue(
                offerDialogue,
                ShowQuestOffer
            );
    }

    private void ShowQuestOffer()
    {
        if (QuestOfferUI.Instance == null)
        {
            DialogueManager.Instance
                .ReleaseGameplayLock();

            return;
        }

        QuestOfferUI.Instance
            .ShowQuest(
                quest
            );
    }

    private void StartActiveDialogue()
    {
        if (activeDialogue == null)
        {
            return;
        }

        DialogueManager.Instance
            .StartDialogue(
                activeDialogue
            );
    }

    private void StartCompletionDialogue()
    {
        if (completionDialogue == null)
        {
            CompleteQuest();
            return;
        }

        DialogueManager.Instance
            .StartDialogue(
                completionDialogue,
                CompleteQuest
            );
    }

    private void CompleteQuest()
    {
        if (QuestManager.Instance == null)
        {
            if (DialogueManager.Instance != null)
            {
                DialogueManager.Instance
                    .ReleaseGameplayLock();
            }

            return;
        }

        bool completed =
            QuestManager.Instance
                .TurnInQuest(
                    quest
                );

        if (completed)
        {
            if (
                QuestNotificationUI.Instance != null
            )
            {
                QuestNotificationUI.Instance
                    .ShowQuestComplete(
                        quest.QuestName
                    );
            }

            SpawnReward();
        }

        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance
                .ReleaseGameplayLock();
        }
    }

    private void SpawnReward()
    {
        if (quest == null)
        {
            return;
        }

        if (
            quest.CollectibleRewardPrefab == null
        )
        {
            Debug.LogWarning(
                quest.QuestName +
                " has no Collectible Reward Prefab."
            );

            return;
        }

        if (rewardSpawnPoint == null)
        {
            Debug.LogWarning(
                gameObject.name +
                " has no Reward Spawn Point."
            );

            return;
        }

        if (
            string.IsNullOrWhiteSpace(
                quest.RewardPickupId
            )
        )
        {
            Debug.LogWarning(
                quest.QuestName +
                " has no Reward Pickup ID."
            );

            return;
        }

        if (
            string.IsNullOrWhiteSpace(
                quest.RewardZoneId
            )
        )
        {
            Debug.LogWarning(
                quest.QuestName +
                " has no Reward Zone ID."
            );

            return;
        }

        if (
            CollectibleManager.Instance != null &&
            CollectibleManager.Instance
                .IsPickupCollected(
                    quest.RewardPickupId
                )
        )
        {
            return;
        }

        if (spawnedReward != null)
        {
            return;
        }

        CollectiblePickup reward =
            Instantiate(
                quest.CollectibleRewardPrefab,
                rewardSpawnPoint.position,
                rewardSpawnPoint.rotation
            );

        reward.Configure(
            quest.RewardPickupId,
            quest.RewardZoneId,
            quest.RewardCollectibleType,
            quest.RewardAmount
        );

        spawnedReward =
            reward.gameObject;
    }

    private void StartCompletedDialogue()
    {
        if (completedDialogue == null)
        {
            return;
        }

        DialogueManager.Instance
            .StartDialogue(
                completedDialogue
            );
    }
}