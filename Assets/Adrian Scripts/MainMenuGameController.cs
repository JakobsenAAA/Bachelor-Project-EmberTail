using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuGameController : MonoBehaviour
{
    [Header("Scenes")]
    [SerializeField] private string gameplaySceneName = "Game";

    [Header("New Game")]
    [SerializeField] private bool clearPlayerPrefsSaveData = true;

    private const string SaveExistsKey = "SaveExists";

    public void StartNewGame()
    {
        ResetGameProgress();

        SceneManager.LoadScene(
            gameplaySceneName
        );
    }

    public void LoadGame()
    {
        if (!HasSaveGame())
        {
            return;
        }

        SceneManager.LoadScene(
            gameplaySceneName
        );
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

    public bool HasSaveGame()
    {
        return PlayerPrefs.GetInt(
            SaveExistsKey,
            0
        ) == 1;
    }

    private void ResetGameProgress()
    {
        if (CollectibleManager.Instance != null)
        {
            CollectibleManager.Instance
                .ResetProgress();
        }

        if (clearPlayerPrefsSaveData)
        {
            PlayerPrefs.DeleteKey(
                SaveExistsKey
            );

            PlayerPrefs.DeleteKey(
                "SavedScene"
            );

            PlayerPrefs.DeleteKey(
                "SavedCheckpoint"
            );

            PlayerPrefs.Save();
        }
    }
}