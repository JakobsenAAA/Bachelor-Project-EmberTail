using UnityEngine;

public class PauseMenuUI : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private GameObject settingsPanel;

    private void OnEnable()
    {
        HideSettings();
    }

    public void ToggleSettings()
    {
        if (settingsPanel == null)
        {
            return;
        }

        settingsPanel.SetActive(
            !settingsPanel.activeSelf
        );
    }

    public void ShowSettings()
    {
        if (settingsPanel == null)
        {
            return;
        }

        settingsPanel.SetActive(true);
    }

    public void HideSettings()
    {
        if (settingsPanel == null)
        {
            return;
        }

        settingsPanel.SetActive(false);
    }
}