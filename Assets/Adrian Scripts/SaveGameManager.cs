using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveGameManager : MonoBehaviour
{
    public static SaveGameManager Instance { get; private set; }

    [Header("Save File")]
    [SerializeField] private string saveFileName = "savegame.json";

    private SaveGameData pendingLoadData;

    public string SavePath =>
        Path.Combine(
            Application.persistentDataPath,
            saveFileName
        );

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded +=
            OnSceneLoaded;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -=
                OnSceneLoaded;
        }
    }

    public bool HasSaveGame()
    {
        return File.Exists(
            SavePath
        );
    }

    public void SaveFromButton()
    {
        SaveCurrentGame();
    }

    public void LoadFromButton()
    {
        LoadGame();
    }

    public bool SaveCurrentGame()
    {
        PlayerRespawn playerRespawn =
            FindFirstObjectByType<PlayerRespawn>();

        CollectibleManager collectibleManager =
            CollectibleManager.Instance;

        if (
            playerRespawn == null ||
            collectibleManager == null
        )
        {
            Debug.LogWarning(
                "Save failed because required gameplay systems were not found."
            );

            return false;
        }

        SaveGameData data =
            new SaveGameData();

        data.sceneName =
            SceneManager.GetActiveScene().name;

        data.checkpointId =
            playerRespawn.CurrentCheckpointId;

        data.zoneId =
            playerRespawn.CurrentZoneId;

        data.zoneProgress =
            collectibleManager
                .CreateZoneSaveData();

        data.collectedPickupIds =
            collectibleManager
                .CreateCollectedPickupSaveData();

        if (GameProgressManager.Instance != null)
        {
            data.betaCompleted =
                GameProgressManager.Instance
                    .BetaCompleted;
        }

        string json =
            JsonUtility.ToJson(
                data,
                true
            );

        File.WriteAllText(
            SavePath,
            json
        );

        Debug.Log(
            "Game saved to: " +
            SavePath
        );

        return true;
    }

    public bool LoadGame()
    {
        if (!HasSaveGame())
        {
            Debug.LogWarning(
                "No save game exists."
            );

            return false;
        }

        string json =
            File.ReadAllText(
                SavePath
            );

        pendingLoadData =
            JsonUtility.FromJson<SaveGameData>(
                json
            );

        if (
            pendingLoadData == null ||
            string.IsNullOrWhiteSpace(
                pendingLoadData.sceneName
            )
        )
        {
            Debug.LogWarning(
                "Save file could not be loaded."
            );

            pendingLoadData = null;

            return false;
        }

        Time.timeScale = 1f;

        if (LoadingScreenManager.Instance != null)
        {
            LoadingScreenManager.Instance
                .LoadScene(
                    pendingLoadData.sceneName
                );
        }
        else
        {
            SceneManager.LoadScene(
                pendingLoadData.sceneName
            );
        }

        return true;
    }

    public void DeleteSave()
    {
        pendingLoadData = null;

        if (File.Exists(SavePath))
        {
            File.Delete(
                SavePath
            );
        }
    }

    private void OnSceneLoaded(
        Scene scene,
        LoadSceneMode mode
    )
    {
        if (pendingLoadData == null)
        {
            return;
        }

        if (
            scene.name !=
            pendingLoadData.sceneName
        )
        {
            return;
        }

        ApplyPendingLoad();
    }

    private void ApplyPendingLoad()
    {
        CollectibleManager collectibleManager =
            CollectibleManager.Instance;

        PlayerRespawn playerRespawn =
            FindFirstObjectByType<PlayerRespawn>();

        if (collectibleManager != null)
        {
            collectibleManager.RestoreSaveData(
                pendingLoadData.zoneProgress,
                pendingLoadData.collectedPickupIds
            );
        }

        if (GameProgressManager.Instance != null)
        {
            GameProgressManager.Instance
                .RestoreProgress(
                    pendingLoadData.betaCompleted
                );
        }

        if (playerRespawn != null)
        {
            playerRespawn.LoadCheckpoint(
                pendingLoadData.checkpointId
            );
        }

        pendingLoadData = null;
    }
}