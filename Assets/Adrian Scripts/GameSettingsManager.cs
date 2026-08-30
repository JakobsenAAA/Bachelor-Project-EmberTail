using UnityEngine;
using UnityEngine.UI;

public class GameSettingsManager : MonoBehaviour
{
    public static GameSettingsManager Instance { get; private set; }

    [Header("Defaults")]
    [SerializeField] private float defaultMasterVolume = 1f;
    [SerializeField] private float defaultBrightness = 1f;

    [Header("Brightness")]
    [SerializeField] private float minimumBrightness = 0.5f;
    [SerializeField] private float maximumBrightness = 1.5f;
    [SerializeField] private Image brightnessOverlay;
    [SerializeField] private float maximumDarkOverlayAlpha = 0.5f;
    [SerializeField] private float maximumBrightOverlayAlpha = 0.15f;

    private const string MasterVolumeKey = "MasterVolume";
    private const string BrightnessKey = "Brightness";

    private float masterVolume;
    private float brightness;

    public float MasterVolume => masterVolume;
    public float Brightness => brightness;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);

        LoadSettings();
        ApplySettings();
    }

    public void SetMasterVolume(float value)
    {
        masterVolume = Mathf.Clamp01(value);

        ApplyMasterVolume();

        PlayerPrefs.SetFloat(
            MasterVolumeKey,
            masterVolume
        );

        PlayerPrefs.Save();
    }

    public void SetBrightness(float value)
    {
        brightness = Mathf.Clamp(
            value,
            minimumBrightness,
            maximumBrightness
        );

        ApplyBrightness();

        PlayerPrefs.SetFloat(
            BrightnessKey,
            brightness
        );

        PlayerPrefs.Save();
    }

    public void SetBrightnessOverlay(Image overlay)
    {
        brightnessOverlay = overlay;

        ApplyBrightness();
    }

    private void LoadSettings()
    {
        masterVolume = PlayerPrefs.GetFloat(
            MasterVolumeKey,
            defaultMasterVolume
        );

        brightness = PlayerPrefs.GetFloat(
            BrightnessKey,
            defaultBrightness
        );
    }

    private void ApplySettings()
    {
        ApplyMasterVolume();
        ApplyBrightness();
    }

    private void ApplyMasterVolume()
    {
        AudioListener.volume = masterVolume;
    }

    private void ApplyBrightness()
    {
        if (brightnessOverlay == null)
        {
            return;
        }

        Color color;

        if (brightness < 1f)
        {
            float darkness = Mathf.InverseLerp(
                1f,
                minimumBrightness,
                brightness
            );

            color = new Color(
                0f,
                0f,
                0f,
                darkness * maximumDarkOverlayAlpha
            );
        }
        else
        {
            float brightnessAmount = Mathf.InverseLerp(
                1f,
                maximumBrightness,
                brightness
            );

            color = new Color(
                1f,
                1f,
                1f,
                brightnessAmount * maximumBrightOverlayAlpha
            );
        }

        brightnessOverlay.color = color;
    }

    public void ResetToDefaults()
    {
        SetMasterVolume(
            defaultMasterVolume
        );

        SetBrightness(
            defaultBrightness
        );
    }
}