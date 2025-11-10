using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    public int currentHealth;
    public bool isDead = false;

    private Animator animator;

    void Start()
    {
        currentHealth = maxHealth;
        // Animator Component
        animator = GetComponent<Animator>();

        if (animator == null)
        {
            Debug.LogError("PlayerHealth: Can't find Animator component.");
        }
    }

    //
    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth); // Set limit for lowest health

        Debug.Log($"Player got damaged : {damage} HP left: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            // Trigger for hit animation
            if (animator != null)
            {
                animator.SetTrigger("Hit");
            }
        }
    }

    void Die()
    {
        if (isDead) return;

        isDead = true;

        // Dead animation Trigger
        // animator.SetBool("IsDead", true); 

        Debug.Log("Player died!");
        // (add gammeover or respawn logic here)
    }
}