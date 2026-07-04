using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int maxHitPoints = 3;
    [SerializeField] private int startingHitPoints = 3;
    [SerializeField] private float damageCooldown = 0.75f;
    [SerializeField] private float respawnInvulnerabilityTime = 1f;

    [Header("Cinders")]
    [SerializeField] private int cindersNeededForHitPoint = 50;

    [Header("Knockback")]
    [SerializeField] private PlayerDamageKnockback damageKnockback;

    public UnityEvent OnHealthChanged;
    public UnityEvent OnCindersChanged;
    public UnityEvent OnPlayerDied;

    private int currentHitPoints;
    private int currentCinders;
    private float lastDamageTime = -999f;
    private float invulnerableUntilTime;

    public int CurrentHitPoints => currentHitPoints;
    public int MaxHitPoints => maxHitPoints;
    public int CurrentCinders => currentCinders;
    public int CindersNeededForHitPoint => cindersNeededForHitPoint;

    private void Awake()
    {
        if (damageKnockback == null)
        {
            damageKnockback = GetComponent<PlayerDamageKnockback>();
        }

        currentHitPoints = Mathf.Clamp(startingHitPoints, 1, maxHitPoints);
        currentCinders = 0;
    }

    private void Start()
    {
        OnHealthChanged.Invoke();
        OnCindersChanged.Invoke();
    }

    public void TakeDamage(int damage)
    {
        TakeDamage(damage, transform.position);
    }

    public void TakeDamage(int damage, Vector3 damageSourcePosition)
    {
        if (Time.time < invulnerableUntilTime)
        {
            return;
        }

        if (Time.time - lastDamageTime < damageCooldown)
        {
            return;
        }

        if (currentHitPoints <= 0)
        {
            return;
        }

        lastDamageTime = Time.time;
        currentHitPoints -= damage;
        currentHitPoints = Mathf.Max(currentHitPoints, 0);

        if (damageKnockback != null && currentHitPoints > 0)
        {
            damageKnockback.KnockbackFrom(damageSourcePosition);
        }

        OnHealthChanged.Invoke();

        if (currentHitPoints <= 0)
        {
            OnPlayerDied.Invoke();
        }
    }

    public void AddCinders(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        currentCinders += amount;

        while (currentCinders >= cindersNeededForHitPoint && currentHitPoints < maxHitPoints)
        {
            currentCinders -= cindersNeededForHitPoint;
            currentHitPoints++;
            OnHealthChanged.Invoke();
        }

        if (currentHitPoints >= maxHitPoints)
        {
            currentCinders = Mathf.Min(currentCinders, cindersNeededForHitPoint);
        }

        OnCindersChanged.Invoke();
    }

    public void RestoreFullHealth()
    {
        currentHitPoints = maxHitPoints;
        currentCinders = 0;
        lastDamageTime = Time.time;
        invulnerableUntilTime = Time.time + respawnInvulnerabilityTime;

        OnHealthChanged.Invoke();
        OnCindersChanged.Invoke();
    }
}