using TMPro;
using UnityEngine;

public class PlayerCollectibleUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI collectible1Text;
    [SerializeField] private TextMeshProUGUI collectible2Text;
    [SerializeField] private TextMeshProUGUI collectible3Text;

    private CollectibleManager collectibleManager;

    private void Start()
    {
        FindCollectibleManager();
        RegisterEvents();
        UpdateUI();
    }

    private void OnEnable()
    {
        FindCollectibleManager();
        RegisterEvents();
        UpdateUI();
    }

    private void OnDisable()
    {
        UnregisterEvents();
    }

    private void FindCollectibleManager()
    {
        if (collectibleManager != null)
        {
            return;
        }

        collectibleManager = CollectibleManager.Instance;

        if (collectibleManager == null)
        {
            collectibleManager = FindFirstObjectByType<CollectibleManager>();
        }
    }

    private void RegisterEvents()
    {
        if (collectibleManager == null)
        {
            return;
        }

        collectibleManager.OnCollectiblesChanged.RemoveListener(UpdateUI);
        collectibleManager.OnCollectiblesChanged.AddListener(UpdateUI);
    }

    private void UnregisterEvents()
    {
        if (collectibleManager == null)
        {
            return;
        }

        collectibleManager.OnCollectiblesChanged.RemoveListener(UpdateUI);
    }

    private void UpdateUI()
    {
        if (collectibleManager == null)
        {
            return;
        }

        if (collectible1Text != null)
        {
            collectible1Text.text =
                collectibleManager.Collectible1Amount +
                " / " +
                collectibleManager.Collectible1Maximum;
        }

        if (collectible2Text != null)
        {
            collectible2Text.text =
                collectibleManager.Collectible2Amount +
                " / " +
                collectibleManager.Collectible2Maximum;
        }

        if (collectible3Text != null)
        {
            collectible3Text.text =
                collectibleManager.Collectible3Amount +
                " / " +
                collectibleManager.Collectible3Maximum;
        }
    }
}