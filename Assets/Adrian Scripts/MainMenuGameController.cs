using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuGameController : MonoBehaviour
{
    [Header("Scenes")]
    [SerializeField] private string gameplaySceneName = "Game";

    [Header("UI")]
    [SerializeField] private Button loadGameButton;

    private void Start()
    {
        RefreshLoadButton();
    }

    public void StartNewGame()
    {
        if (SaveGameManager.Instance != null)
        {
            SaveGameManager.Instance
                .DeleteSave();
        }

        if (CollectibleManager.Instance != null)
        {
            CollectibleManager.Instance
                .ResetProgress();
        }

        if (GameProgressManager.Instance != null)
        {
            GameProgressManager.Instance
                .ResetProgress();
        }

        Time.timeScale = 1f;

        if (LoadingScreenManager.Instance != null)
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

    public void LoadGame()
    {
        if (SaveGameManager.Instance == null)
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

        loadGameButton.interactable =
            hasSave;
    }
}