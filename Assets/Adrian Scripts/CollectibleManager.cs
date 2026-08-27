using UnityEngine;
using UnityEngine.Events;

public class CollectibleManager : MonoBehaviour
{
    public static CollectibleManager Instance { get; private set; }

    [Header("Collectible 1")]
    [SerializeField] private int collectible1Maximum = 10;

    [Header("Collectible 2")]
    [SerializeField] private int collectible2Maximum = 10;

    [Header("Collectible 3")]
    [SerializeField] private int collectible3Maximum = 10;

    public UnityEvent OnCollectiblesChanged;

    private int collectible1Amount;
    private int collectible2Amount;
    private int collectible3Amount;

    public int Collectible1Amount => collectible1Amount;
    public int Collectible2Amount => collectible2Amount;
    public int Collectible3Amount => collectible3Amount;

    public int Collectible1Maximum => collectible1Maximum;
    public int Collectible2Maximum => collectible2Maximum;
    public int Collectible3Maximum => collectible3Maximum;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        OnCollectiblesChanged.Invoke();
    }

    public void AddCollectible(CollectibleType type, int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        switch (type)
        {
            case CollectibleType.Collectible1:
                collectible1Amount = Mathf.Clamp(
                    collectible1Amount + amount,
                    0,
                    collectible1Maximum
                );
                break;

            case CollectibleType.Collectible2:
                collectible2Amount = Mathf.Clamp(
                    collectible2Amount + amount,
                    0,
                    collectible2Maximum
                );
                break;

            case CollectibleType.Collectible3:
                collectible3Amount = Mathf.Clamp(
                    collectible3Amount + amount,
                    0,
                    collectible3Maximum
                );
                break;
        }

        OnCollectiblesChanged.Invoke();
    }

    public int GetAmount(CollectibleType type)
    {
        switch (type)
        {
            case CollectibleType.Collectible1:
                return collectible1Amount;

            case CollectibleType.Collectible2:
                return collectible2Amount;

            case CollectibleType.Collectible3:
                return collectible3Amount;

            default:
                return 0;
        }
    }

    public int GetMaximum(CollectibleType type)
    {
        switch (type)
        {
            case CollectibleType.Collectible1:
                return collectible1Maximum;

            case CollectibleType.Collectible2:
                return collectible2Maximum;

            case CollectibleType.Collectible3:
                return collectible3Maximum;

            default:
                return 0;
        }
    }

    public void ResetProgress()
    {
        collectible1Amount = 0;
        collectible2Amount = 0;
        collectible3Amount = 0;

        OnCollectiblesChanged.Invoke();
    }
}