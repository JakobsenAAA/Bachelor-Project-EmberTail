using System;

[Serializable]
public class QuestRuntimeData
{
    private QuestData questData;
    private QuestState state;
    private int currentProgress;

    public QuestData QuestData =>
        questData;

    public QuestState State =>
        state;

    public int CurrentProgress =>
        currentProgress;

    public QuestRuntimeData(
        QuestData questData,
        QuestState startingState
    )
    {
        this.questData =
            questData;

        state =
            startingState;

        currentProgress = 0;
    }

    public void SetState(
        QuestState newState
    )
    {
        state =
            newState;
    }

    public void SetProgress(
        int progress
    )
    {
        if (questData == null)
        {
            currentProgress = 0;
            return;
        }

        currentProgress =
            Math.Max(
                0,
                Math.Min(
                    progress,
                    questData.RequiredAmount
                )
            );
    }

    public void AddProgress(
        int amount
    )
    {
        SetProgress(
            currentProgress +
            amount
        );
    }
}