using TMPro;
using UnityEngine;

public class PauseProgressionUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PauseMapController pauseMapController;
    [SerializeField] private CollectibleManager collectibleManager;

    [Header("Zone Information")]
    [SerializeField] private TextMeshProUGUI discNameText;
    [SerializeField] private TextMeshProUGUI zoneNameText;
    [SerializeField] private TextMeshProUGUI zoneDescriptionText;

    [Header("Collectibles")]
    [SerializeField] private GameObject collectibleContainer;
    [SerializeField] private TextMeshProUGUI collectible1Text;
    [SerializeField] private TextMeshProUGUI collectible2Text;
    [SerializeField] private TextMeshProUGUI collectible3Text;

    [Header("Locked Content")]
    [SerializeField] private GameObject lockedContainer;
    [SerializeField] private TextMeshProUGUI lockedText;

    private void Awake()
    {
        if (collectibleManager == null)
        {
            collectibleManager =
                CollectibleManager.Instance;
        }
    }

    private void OnEnable()
    {
        if (pauseMapController != null)
        {
            pauseMapController
                .OnSelectionChanged
                .AddListener(UpdateUI);
        }

        if (collectibleManager != null)
        {
            collectibleManager
                .OnCollectiblesChanged
                .AddListener(UpdateUI);
        }

        UpdateUI();
    }

    private void OnDisable()
    {
        if (pauseMapController != null)
        {
            pauseMapController
                .OnSelectionChanged
                .RemoveListener(UpdateUI);
        }

        if (collectibleManager != null)
        {
            collectibleManager
                .OnCollectiblesChanged
                .RemoveListener(UpdateUI);
        }
    }

    public void UpdateUI()
    {
        if (
            pauseMapController == null ||
            collectibleManager == null
        )
        {
            return;
        }

        ProgressionDiscDefinition disc =
            pauseMapController
                .GetSelectedDiscDefinition();

        ZoneProgressDefinition zone =
            pauseMapController
                .GetSelectedZoneDefinition();

        if (discNameText != null)
        {
            discNameText.text =
                disc != null
                    ? disc.DisplayName
                    : "";
        }

        if (zone == null)
        {
            ShowLockedState("???");
            return;
        }

        bool locked =
            disc.Locked ||
            zone.Locked;

        if (locked)
        {
            ShowLockedState(
                string.IsNullOrWhiteSpace(zone.DisplayName)
                    ? "???"
                    : zone.DisplayName
            );

            return;
        }

        ShowUnlockedState(zone);
    }

    private void ShowUnlockedState(
        ZoneProgressDefinition zone
    )
    {
        if (zoneNameText != null)
        {
            zoneNameText.text =
                zone.DisplayName;
        }

        if (zoneDescriptionText != null)
        {
            zoneDescriptionText.text =
                zone.Description;
        }

        if (collectibleContainer != null)
        {
            collectibleContainer
                .SetActive(true);
        }

        if (lockedContainer != null)
        {
            lockedContainer
                .SetActive(false);
        }

        UpdateCollectibleText(
            collectible1Text,
            zone.ZoneId,
            CollectibleType.Collectible1
        );

        UpdateCollectibleText(
            collectible2Text,
            zone.ZoneId,
            CollectibleType.Collectible2
        );

        UpdateCollectibleText(
            collectible3Text,
            zone.ZoneId,
            CollectibleType.Collectible3
        );
    }

    private void ShowLockedState(
        string displayName
    )
    {
        if (zoneNameText != null)
        {
            zoneNameText.text =
                displayName;
        }

        if (zoneDescriptionText != null)
        {
            zoneDescriptionText.text = "";
        }

        if (collectibleContainer != null)
        {
            collectibleContainer
                .SetActive(false);
        }

        if (lockedContainer != null)
        {
            lockedContainer
                .SetActive(true);
        }

        if (lockedText != null)
        {
            lockedText.text =
                "COMING LATER";
        }
    }

    private void UpdateCollectibleText(
        TextMeshProUGUI text,
        string zoneId,
        CollectibleType type
    )
    {
        if (text == null)
        {
            return;
        }

        int collected =
            collectibleManager.GetCollected(
                zoneId,
                type
            );

        int total =
            collectibleManager.GetTotal(
                zoneId,
                type
            );

        text.text =
            collected +
            " / " +
            total;
    }
}