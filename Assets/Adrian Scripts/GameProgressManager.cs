using UnityEngine;
using UnityEngine.Events;

public class GameProgressManager : MonoBehaviour
{
    public static GameProgressManager Instance { get; private set; }

    public UnityEvent OnProgressChanged;

    private bool betaCompleted;

    public bool BetaCompleted => betaCompleted;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    public void SetBetaCompleted(bool completed)
    {
        if (betaCompleted == completed)
        {
            return;
        }

        betaCompleted = completed;

        OnProgressChanged.Invoke();
    }

    public void CompleteBeta()
    {
        SetBetaCompleted(true);
    }

    public void ResetProgress()
    {
        betaCompleted = false;

        OnProgressChanged.Invoke();
    }

    public void RestoreProgress(bool savedBetaCompleted)
    {
        betaCompleted = savedBetaCompleted;

        OnProgressChanged.Invoke();
    }
}