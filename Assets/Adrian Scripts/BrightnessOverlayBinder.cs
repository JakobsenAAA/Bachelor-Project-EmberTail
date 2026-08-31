using UnityEngine;
using UnityEngine.UI;

public class BrightnessOverlayBinder : MonoBehaviour
{
    [SerializeField] private Image brightnessOverlay;

    private void Start()
    {
        BindBrightnessOverlay();
    }

    private void BindBrightnessOverlay()
    {
        if (brightnessOverlay == null)
        {
            Debug.LogWarning(
                "BrightnessOverlayBinder has no Brightness Overlay assigned."
            );

            return;
        }

        if (GameSettingsManager.Instance == null)
        {
            Debug.LogWarning(
                "GameSettingsManager was not found."
            );

            return;
        }

        GameSettingsManager.Instance.SetBrightnessOverlay(
            brightnessOverlay
        );
    }
}