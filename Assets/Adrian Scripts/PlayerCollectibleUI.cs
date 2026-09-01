using TMPro;
using UnityEngine;

public class PlayerCollectibleUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI collectible1Text;
    [SerializeField] private TextMeshProUGUI collectible2Text;
    [SerializeField] private TextMeshProUGUI collectible3Text;

    private CollectibleManager collectibleManager;
    private ZoneManager zoneManager;

    private bool collectibleSubscribed;
    private bool zoneSubscribed;

    private void Start()
    {
        ConnectToManagers();
        UpdateUI();
    }

    private void OnEnable()
    {
        ConnectToManagers();
    }

    private void OnDisable()
    {
        Unsubscribe();
    }

    private void ConnectToManagers()
    {
        if (collectibleManager == null)
        {
            collectibleManager =
                CollectibleManager.Instance;
        }

        if (collectibleManager == null)
        {
            collectibleManager =
                FindFirstObjectByType<CollectibleManager>();
        }

        if (zoneManager == null)
        {
            zoneManager =
                ZoneManager.Instance;
        }

        if (zoneManager == null)
        {
            zoneManager =
                FindFirstObjectByType<ZoneManager>();
        }

        if (
            collectibleManager != null &&
            !collectibleSubscribed
        )
        {
            collectibleManager
                .OnCollectiblesChanged
                .AddListener(UpdateUI);

            collectibleSubscribed = true;
        }

        if (
            zoneManager != null &&
            !zoneSubscribed
        )
        {
            zoneManager
                .OnZoneChanged
                .AddListener(UpdateUI);

            zoneSubscribed = true;
        }

        UpdateUI();
    }

    private void Unsubscribe()
    {
        if (
            collectibleManager != null &&
            collectibleSubscribed
        )
        {
            collectibleManager
                .OnCollectiblesChanged
                .RemoveListener(UpdateUI);

            collectibleSubscribed = false;
        }

        if (
            zoneManager != null &&
            zoneSubscribed
        )
        {
            zoneManager
                .OnZoneChanged
                .RemoveListener(UpdateUI);

            zoneSubscribed = false;
        }
    }

    private void UpdateUI()
    {
        if (
            collectibleManager == null ||
            zoneManager == null
        )
        {
            return;
        }

        UpdateCollectibleText(
            collectible1Text,
            CollectibleType.Collectible1
        );

        UpdateCollectibleText(
            collectible2Text,
            CollectibleType.Collectible2
        );

        UpdateCollectibleText(
            collectible3Text,
            CollectibleType.Collectible3
        );
    }

    private void UpdateCollectibleText(
        TextMeshProUGUI text,
        CollectibleType type
    )
    {
        if (text == null)
        {
            return;
        }

        string zoneId =
            zoneManager.CurrentZoneId;

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