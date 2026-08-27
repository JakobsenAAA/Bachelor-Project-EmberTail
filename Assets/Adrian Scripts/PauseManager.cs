using UnityEngine;
using UnityEngine.InputSystem;

public class PauseManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject pauseMenu;

    [Header("Gameplay")]
    [SerializeField] private PlayerController playerController;
    [SerializeField] private ThirdPersonCamera thirdPersonCamera;

    private bool isPaused;

    public bool IsPaused => isPaused;

    private void Start()
    {
        ResumeGame();
    }

    private void Update()
    {
        if (Keyboard.current == null)
        {
            return;
        }

        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            TogglePause();
        }
    }

    private void LateUpdate()
    {
        ApplyCursorState();
    }

    public void TogglePause()
    {
        if (isPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    public void PauseGame()
    {
        isPaused = true;

        Time.timeScale = 0f;

        if (playerController != null)
        {
            playerController.SetGameplayInputEnabled(false);
        }

        if (thirdPersonCamera != null)
        {
            thirdPersonCamera.SetCameraInputEnabled(false);
        }

        if (pauseMenu != null)
        {
            pauseMenu.SetActive(true);
        }

        ApplyCursorState();
    }

    public void ResumeGame()
    {
        isPaused = false;

        Time.timeScale = 1f;

        if (playerController != null)
        {
            playerController.SetGameplayInputEnabled(true);
        }

        if (thirdPersonCamera != null)
        {
            thirdPersonCamera.SetCameraInputEnabled(true);
        }

        if (pauseMenu != null)
        {
            pauseMenu.SetActive(false);
        }

        ApplyCursorState();
    }

    private void ApplyCursorState()
    {
        if (isPaused)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private void OnDisable()
    {
        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}