
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private Image[] healthBoxes;
    [SerializeField] private TextMeshProUGUI cinderText;

    [Header("Damage Animation")]
    [SerializeField] private Color damageFlashColor = Color.red;
    [SerializeField] private float damageDuration = 0.35f;
    [SerializeField] private float damageScale = 1.2f;

    [Header("Healing Animation")]
    [SerializeField] private Color healingFlashColor = new Color(1f, 0.8f, 0.2f, 1f);
    [SerializeField] private float healingDuration = 0.45f;
    [SerializeField] private float healingScale = 1.3f;

    private int previousHealth = -1;
    private Coroutine[] activeAnimations;
    private Vector3[] originalScales;
    private Color[] originalColors;

    private void Awake()
    {
        activeAnimations = new Coroutine[healthBoxes.Length];
        originalScales = new Vector3[healthBoxes.Length];
        originalColors = new Color[healthBoxes.Length];

        for (int i = 0; i < healthBoxes.Length; i++)
        {
            if (healthBoxes[i] == null)
            {
                continue;
            }

            originalScales[i] = healthBoxes[i].rectTransform.localScale;
            originalColors[i] = healthBoxes[i].color;
        }
    }

    private void OnEnable()
    {
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged.AddListener(UpdateHealthUI);
            playerHealth.OnCindersChanged.AddListener(UpdateCinderUI);
        }
    }

    private void OnDisable()
    {
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged.RemoveListener(UpdateHealthUI);
            playerHealth.OnCindersChanged.RemoveListener(UpdateCinderUI);
        }

        if (activeAnimations == null)
        {
            return;
        }

        for (int i = 0; i < activeAnimations.Length; i++)
        {
            if (activeAnimations[i] != null)
            {
                StopCoroutine(activeAnimations[i]);
                activeAnimations[i] = null;
            }
        }
    }

    private void Start()
    {
        UpdateHealthUI();
        UpdateCinderUI();
    }

    private void UpdateHealthUI()
    {
        if (playerHealth == null)
        {
            return;
        }

        int currentHealth = playerHealth.CurrentHitPoints;

        if (previousHealth < 0)
        {
            SetHealthImmediately(currentHealth);
            previousHealth = currentHealth;
            return;
        }

        if (currentHealth < previousHealth)
        {
            for (int i = currentHealth; i < previousHealth && i < healthBoxes.Length; i++)
            {
                StartHealthAnimation(i, false);
            }
        }
        else if (currentHealth > previousHealth)
        {
            for (int i = previousHealth; i < currentHealth && i < healthBoxes.Length; i++)
            {
                StartHealthAnimation(i, true);
            }
        }

        previousHealth = currentHealth;
    }

    private void StartHealthAnimation(int index, bool healing)
    {
        if (healthBoxes[index] == null)
        {
            return;
        }

        if (activeAnimations[index] != null)
        {
            StopCoroutine(activeAnimations[index]);
        }

        activeAnimations[index] = StartCoroutine(
            AnimateHealthSection(index, healing)
        );
    }

    private IEnumerator AnimateHealthSection(int index, bool healing)
    {
        Image image = healthBoxes[index];
        RectTransform rect = image.rectTransform;

        float duration = Mathf.Max(
            0.01f,
            healing ? healingDuration : damageDuration
        );

        float scaleMultiplier = healing ? healingScale : damageScale;
        Color flashColor = healing ? healingFlashColor : damageFlashColor;

        image.enabled = true;
        image.color = flashColor;
        rect.localScale = originalScales[index] * scaleMultiplier;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;

            float t = Mathf.Clamp01(elapsed / duration);

            rect.localScale = Vector3.Lerp(
                originalScales[index] * scaleMultiplier,
                originalScales[index],
                t
            );

            Color targetColor = originalColors[index];

            if (!healing)
            {
                targetColor.a = 0f;
            }

            image.color = Color.Lerp(
                flashColor,
                targetColor,
                t
            );

            yield return null;
        }

        rect.localScale = originalScales[index];
        image.color = originalColors[index];
        image.enabled = healing;

        activeAnimations[index] = null;
    }

    private void SetHealthImmediately(int health)
    {
        for (int i = 0; i < healthBoxes.Length; i++)
        {
            if (healthBoxes[i] == null)
            {
                continue;
            }

            if (activeAnimations[i] != null)
            {
                StopCoroutine(activeAnimations[i]);
                activeAnimations[i] = null;
            }

            healthBoxes[i].rectTransform.localScale = originalScales[i];
            healthBoxes[i].color = originalColors[i];
            healthBoxes[i].enabled = i < health;
        }
    }

    private void UpdateCinderUI()
    {
        if (playerHealth == null || cinderText == null)
        {
            return;
        }

        cinderText.text =
            playerHealth.CurrentCinders +
            " / " +
            playerHealth.CindersNeededForHitPoint;
    }
}
