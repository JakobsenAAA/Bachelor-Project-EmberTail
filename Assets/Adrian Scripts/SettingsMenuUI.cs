using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenuUI : MonoBehaviour
{
    [Header("Volume")]
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private TextMeshProUGUI masterVolumeValueText;

    [Header("Brightness")]
    [SerializeField] private Slider brightnessSlider;
    [SerializeField] private TextMeshProUGUI brightnessValueText;

    private GameSettingsManager settingsManager;

    private void OnEnable()
    {
        settingsManager =
            GameSettingsManager.Instance;

        if (settingsManager == null)
        {
            return;
        }

        if (masterVolumeSlider != null)
        {
            masterVolumeSlider.SetValueWithoutNotify(
                settingsManager.MasterVolume
            );

            masterVolumeSlider.onValueChanged.RemoveListener(
                OnMasterVolumeChanged
            );

            masterVolumeSlider.onValueChanged.AddListener(
                OnMasterVolumeChanged
            );
        }

        if (brightnessSlider != null)
        {
            brightnessSlider.SetValueWithoutNotify(
                settingsManager.Brightness
            );

            brightnessSlider.onValueChanged.RemoveListener(
                OnBrightnessChanged
            );

            brightnessSlider.onValueChanged.AddListener(
                OnBrightnessChanged
            );
        }

        UpdateVolumeText();
        UpdateBrightnessText();
    }

    private void OnDisable()
    {
        if (masterVolumeSlider != null)
        {
            masterVolumeSlider.onValueChanged.RemoveListener(
                OnMasterVolumeChanged
            );
        }

        if (brightnessSlider != null)
        {
            brightnessSlider.onValueChanged.RemoveListener(
                OnBrightnessChanged
            );
        }
    }

    private void OnMasterVolumeChanged(float value)
    {
        if (settingsManager == null)
        {
            return;
        }

        settingsManager.SetMasterVolume(
            value
        );

        UpdateVolumeText();
    }

    private void OnBrightnessChanged(float value)
    {
        if (settingsManager == null)
        {
            return;
        }

        settingsManager.SetBrightness(
            value
        );

        UpdateBrightnessText();
    }

    private void UpdateVolumeText()
    {
        if (
            masterVolumeValueText == null ||
            settingsManager == null
        )
        {
            return;
        }

        int percentage =
            Mathf.RoundToInt(
                settingsManager.MasterVolume *
                100f
            );

        masterVolumeValueText.text =
            percentage + "%";
    }

    private void UpdateBrightnessText()
    {
        if (
            brightnessValueText == null ||
            settingsManager == null
        )
        {
            return;
        }

        int percentage =
            Mathf.RoundToInt(
                settingsManager.Brightness *
                100f
            );

        brightnessValueText.text =
            percentage + "%";
    }
}