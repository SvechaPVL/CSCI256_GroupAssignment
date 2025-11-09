using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthUI : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 50f;

    [Header("UI")]
    public Slider healthSlider; // assign in Inspector

    float currentHealth;
    bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;

        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = maxHealth;
        }
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;

        if (healthSlider != null)
            healthSlider.value = currentHealth;

        if (currentHealth <= 0f)
            Die();
    }

    void Die()
    {
        isDead = true;
        // TODO: death anim / sound
        Destroy(gameObject, 0.1f);
    }
}
