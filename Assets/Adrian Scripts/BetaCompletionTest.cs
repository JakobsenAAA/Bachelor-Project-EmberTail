using UnityEngine;

public class BetaCompletionTest : MonoBehaviour
{
    public void CompleteBeta()
    {
        if (GameProgressManager.Instance == null)
        {
            Debug.LogWarning(
                "GameProgressManager was not found."
            );

            return;
        }

        GameProgressManager.Instance
            .CompleteBeta();
    }

    public void ResetBeta()
    {
        if (GameProgressManager.Instance == null)
        {
            Debug.LogWarning(
                "GameProgressManager was not found."
            );

            return;
        }

        GameProgressManager.Instance
            .ResetProgress();
    }
}