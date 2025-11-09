using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHealthUI : MonoBehaviour
{
    public Slider healthSlider;
    public TextMeshProUGUI healthText;
    public PlayerHealth playerHealth;

    void Start()
    {
        if (playerHealth != null)
        {
            healthSlider.maxValue = playerHealth.maxHealth;
            healthSlider.value = playerHealth.maxHealth;
            UpdateHealthText(playerHealth.maxHealth);
        }
    }

    void Update()
    {
        if (playerHealth != null)
        {
            healthSlider.value = Mathf.Lerp(healthSlider.value, playerHealth.GetCurrentHealth(), Time.deltaTime * 10f);
            UpdateHealthText(playerHealth.GetCurrentHealth());
        }
    }

    void UpdateHealthText(float current)
    {
        if (healthText != null)
        {
            healthText.text = $"HP: {Mathf.RoundToInt(current)}";
        }
    }
}
