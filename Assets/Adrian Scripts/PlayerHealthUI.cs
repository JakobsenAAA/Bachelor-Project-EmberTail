using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private Image[] healthBoxes;
    [SerializeField] private TextMeshProUGUI cinderText;

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

        for (int i = 0; i < healthBoxes.Length; i++)
        {
            if (healthBoxes[i] != null)
            {
                healthBoxes[i].enabled = i < playerHealth.CurrentHitPoints;
            }
        }
    }

    private void UpdateCinderUI()
    {
        if (playerHealth == null || cinderText == null)
        {
            return;
        }

        cinderText.text = playerHealth.CurrentCinders + " / " + playerHealth.CindersNeededForHitPoint;
    }
}