
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

    [Header("Cinder Audio")]
    [SerializeField] private AudioClip cinderPickupSound;
    [SerializeField, Range(0f, 1f)] private float cinderPickupVolume = 0.5f;
    [SerializeField] private AudioClip healthRechargeSound;
    [SerializeField, Range(0f, 1f)] private float healthRechargeVolume = 0.8f;

    [Header("Damage Audio")]
    [SerializeField] private AudioClip damageSound;
    [SerializeField, Range(0f, 1f)] private float damageVolume = 0.8f;

    [Header("Knockback")]
    [SerializeField] private PlayerDamageKnockback damageKnockback;

    public UnityEvent OnHealthChanged;
    public UnityEvent OnCindersChanged;
    public UnityEvent OnCinderCollected;
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

        currentHitPoints = Mathf.Clamp(
            startingHitPoints,
            1,
            maxHitPoints
        );

        currentCinders = 0;
    }

    private void Start()
    {
        OnHealthChanged.Invoke();
        OnCindersChanged.Invoke();
    }

    public void TakeDamage(int damage)
    {
        TakeDamage(
            damage,
            transform.position
        );
    }

    public void TakeDamage(
        int damage,
        Vector3 damageSourcePosition
    )
    {
        if (Time.time < invulnerableUntilTime)
        {
            return;
        }

        if (Time.time - lastDamageTime < damageCooldown)
        {
            return;
        }

        if (currentHitPoints <= 0 || damage <= 0)
        {
            return;
        }

        lastDamageTime = Time.time;

        currentHitPoints -= damage;

        currentHitPoints = Mathf.Max(
            currentHitPoints,
            0
        );

        if (
            damageKnockback != null &&
            currentHitPoints > 0
        )
        {
            damageKnockback.KnockbackFrom(
                damageSourcePosition
            );
        }

        PlayDamageSound();

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

        OnCinderCollected.Invoke();

        PlayCinderPickupSound();

        currentCinders += amount;

        while (
            currentCinders >= cindersNeededForHitPoint &&
            currentHitPoints < maxHitPoints
        )
        {
            currentCinders -= cindersNeededForHitPoint;

            currentHitPoints++;

            OnHealthChanged.Invoke();

            PlayHealthRechargeSound();
        }

        if (currentHitPoints >= maxHitPoints)
        {
            currentCinders = Mathf.Min(
                currentCinders,
                cindersNeededForHitPoint
            );
        }

        OnCindersChanged.Invoke();
    }

    private void PlayCinderPickupSound()
    {
        if (
            cinderPickupSound == null ||
            UIAudioManager.Instance == null
        )
        {
            return;
        }

        UIAudioManager.Instance.PlaySound(
            cinderPickupSound,
            cinderPickupVolume
        );
    }

    private void PlayHealthRechargeSound()
    {
        if (
            healthRechargeSound == null ||
            UIAudioManager.Instance == null
        )
        {
            return;
        }

        UIAudioManager.Instance.PlaySound(
            healthRechargeSound,
            healthRechargeVolume
        );
    }

    private void PlayDamageSound()
    {
        if (
            damageSound == null ||
            UIAudioManager.Instance == null
        )
        {
            return;
        }

        UIAudioManager.Instance.PlaySound(
            damageSound,
            damageVolume
        );
    }

    public void RestoreFullHealth()
    {
        currentHitPoints = maxHitPoints;
        currentCinders = 0;

        lastDamageTime = Time.time;

        invulnerableUntilTime =
            Time.time +
            respawnInvulnerabilityTime;

        OnHealthChanged.Invoke();
        OnCindersChanged.Invoke();
    }
}
