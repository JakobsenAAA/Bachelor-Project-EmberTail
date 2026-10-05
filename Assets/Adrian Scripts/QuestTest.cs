using UnityEngine;

public class QuestTest : MonoBehaviour
{
    [SerializeField] private QuestData testQuest;

    public void AcceptQuest()
    {
        if (QuestManager.Instance == null)
        {
            Debug.LogWarning(
                "QuestManager was not found."
            );

            return;
        }

        QuestManager.Instance.AcceptQuest(
            testQuest
        );
    }

    public void RefreshQuests()
    {
        if (QuestManager.Instance == null)
        {
            return;
        }

        QuestManager.Instance
            .RefreshAllQuests();
    }

    public void TurnInQuest()
    {
        if (QuestManager.Instance == null)
        {
            return;
        }

        QuestManager.Instance
            .TurnInQuest(
                testQuest
            );
    }
}