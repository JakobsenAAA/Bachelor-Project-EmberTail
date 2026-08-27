using TMPro;
using UnityEngine;

public class PlayerCollectibleUI : MonoBehaviour
{
    [Header("Zone")]
    [SerializeField] private string zoneId = "zone1";

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI collectible1Text;
    [SerializeField] private TextMeshProUGUI collectible2Text;
    [SerializeField] private TextMeshProUGUI collectible3Text;

    private CollectibleManager collectibleManager;
    private bool subscribed;

    private void Start()
    {
        ConnectToManager();
        UpdateUI();
    }

    private void OnEnable()
    {
        ConnectToManager();
    }

    private void OnDisable()
    {
        Unsubscribe();
    }

    private void ConnectToManager()
    {
        if (collectibleManager == null)
        {
            collectibleManager = CollectibleManager.Instance;
        }

        if (collectibleManager == null)
        {
            collectibleManager = FindFirstObjectByType<CollectibleManager>();
        }

        if (collectibleManager == null)
        {
            return;
        }

        if (!subscribed)
        {
            collectibleManager.OnCollectiblesChanged.AddListener(UpdateUI);
            subscribed = true;
        }

        UpdateUI();
    }

    private void Unsubscribe()
    {
        if (collectibleManager == null || !subscribed)
        {
            return;
        }

        collectibleManager.OnCollectiblesChanged.RemoveListener(UpdateUI);
        subscribed = false;
    }

    private void UpdateUI()
    {
        if (collectibleManager == null)
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

        int collected = collectibleManager.GetCollected(zoneId, type);
        int total = collectibleManager.GetTotal(zoneId, type);

        text.text = collected + " / " + total;
    }
}