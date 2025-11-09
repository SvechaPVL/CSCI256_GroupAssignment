using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    float currentHealth;

    void Start() { currentHealth = maxHealth; }

    public void TakeDamage(float dmg)
    {
        currentHealth -= dmg;
        Debug.Log("[PlayerHealth] Took damage: " + dmg + " | HP now: " + currentHealth);
    }

    public float GetCurrentHealth() => currentHealth;


    void Die()
    {
        Debug.Log("Player died!");
        // TODO: respawn, game over screen, etc.
    }

   
}
