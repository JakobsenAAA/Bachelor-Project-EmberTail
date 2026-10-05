using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuGameController : MonoBehaviour
{
    [Header("Scenes")]
    [SerializeField] private string gameplaySceneName = "Game";

    [Header("UI")]
    [SerializeField] private Button loadGameButton;

    [Header("Transition")]
    [SerializeField] private MainMenuTransitionController transitionController;

    private void Start()
    {
        RefreshLoadButton();
    }

    private void OnEnable()
    {
        RefreshLoadButton();
    }

    private void LateUpdate()
    {
        RefreshLoadButton();
    }

    public void StartNewGame()
    {
        if (transitionController != null)
        {
            transitionController
                .StartTransition(
                    CompleteNewGame
                );

            return;
        }

        CompleteNewGame();
    }

    public void LoadGame()
    {
        if (
            SaveGameManager.Instance == null ||
            !SaveGameManager.Instance.HasSaveGame()
        )
        {
            RefreshLoadButton();
            return;
        }

        if (transitionController != null)
        {
            transitionController
                .StartTransition(
                    CompleteLoadGame
                );

            return;
        }

        CompleteLoadGame();
    }

    private void CompleteNewGame()
    {
        if (
            SaveGameManager.Instance != null
        )
        {
            SaveGameManager.Instance
                .DeleteSave();
        }

        if (
            CollectibleManager.Instance != null
        )
        {
            CollectibleManager.Instance
                .ResetProgress();
        }

        if (
            GameProgressManager.Instance != null
        )
        {
            GameProgressManager.Instance
                .ResetProgress();
        }

        Time.timeScale = 1f;

        if (
            LoadingScreenManager.Instance != null
        )
        {
            LoadingScreenManager.Instance
                .LoadScene(
                    gameplaySceneName
                );
        }
        else
        {
            SceneManager.LoadScene(
                gameplaySceneName
            );
        }
    }

    private void CompleteLoadGame()
    {
        if (
            SaveGameManager.Instance == null
        )
        {
            return;
        }

        SaveGameManager.Instance
            .LoadGame();
    }

    public void ExitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        Debug.Log(
            "Exit Game pressed. Application.Quit does not close Play Mode in the Unity Editor."
        );
#endif
    }

    public void RefreshLoadButton()
    {
        if (loadGameButton == null)
        {
            return;
        }

        bool hasSave =
            SaveGameManager.Instance != null &&
            SaveGameManager.Instance
                .HasSaveGame();

        if (
            loadGameButton.interactable !=
            hasSave
        )
        {
            loadGameButton.interactable =
                hasSave;
        }
    }
}