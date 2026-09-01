using UnityEngine;

public class BetaCompletionWorldState : MonoBehaviour
{
    public enum CompletionBehaviour
    {
        HideAfterCompletion,
        ShowAfterCompletion
    }

    [Header("Behaviour")]
    [SerializeField] private CompletionBehaviour completionBehaviour;

    [Header("Target")]
    [SerializeField] private GameObject targetObject;

    private void Start()
    {
        ApplyState();
    }

    private void OnEnable()
    {
        if (GameProgressManager.Instance != null)
        {
            GameProgressManager.Instance
                .OnProgressChanged
                .AddListener(ApplyState);
        }
    }

    private void OnDisable()
    {
        if (GameProgressManager.Instance != null)
        {
            GameProgressManager.Instance
                .OnProgressChanged
                .RemoveListener(ApplyState);
        }
    }

    private void ApplyState()
    {
        if (targetObject == null)
        {
            targetObject = gameObject;
        }

        if (GameProgressManager.Instance == null)
        {
            return;
        }

        bool completed =
            GameProgressManager.Instance.BetaCompleted;

        switch (completionBehaviour)
        {
            case CompletionBehaviour.HideAfterCompletion:

                targetObject.SetActive(
                    !completed
                );

                break;

            case CompletionBehaviour.ShowAfterCompletion:

                targetObject.SetActive(
                    completed
                );

                break;
        }
    }
}