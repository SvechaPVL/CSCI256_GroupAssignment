using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Manages permanent upgrades applied to the player
/// </summary>
public class PlayerUpgrades : MonoBehaviour
{
    public static PlayerUpgrades Instance { get; private set; }

    [Header("Player Stats")]
    [SerializeField] private float baseMaxHealth = 100f;
    [SerializeField] private float baseMoveSpeed = 5f;
    [SerializeField] private float baseDamage = 10f;

    [Header("Current Modifiers")]
    public float healthMultiplier = 1f;
    public float moveSpeedMultiplier = 1f;
    public float damageMultiplier = 1f;

    private HashSet<string> purchasedUpgrades = new HashSet<string>();
    private PlayerHealth playerHealth;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        playerHealth = GetComponent<PlayerHealth>();
        ApplyHealthUpgrade();
    }

    public bool HasUpgrade(string upgradeId)
    {
        return purchasedUpgrades.Contains(upgradeId);
    }

    public void AddUpgrade(string upgradeId)
    {
        purchasedUpgrades.Add(upgradeId);
    }

    public void ApplyHealthUpgrade(float multiplier = 0f)
    {
        if (multiplier > 0)
        {
            healthMultiplier += multiplier;
        }

        if (playerHealth != null)
        {
            float newMaxHealth = baseMaxHealth * healthMultiplier;
            playerHealth.maxHealth = newMaxHealth;

            // Heal player to match new max health percentage
            float healthPercentage = playerHealth.GetCurrentHealth() / playerHealth.maxHealth;
            playerHealth.currentHealth = newMaxHealth * healthPercentage;
        }
    }

    public void ApplySpeedUpgrade(float multiplier)
    {
        moveSpeedMultiplier += multiplier;

        // Apply to movement component if exists
        var fpsInput = GetComponent<FPSInput>();
        if (fpsInput != null)
        {
            fpsInput.walkSpeed = baseMoveSpeed * moveSpeedMultiplier;
            fpsInput.runSpeed = (baseMoveSpeed * 2f) * moveSpeedMultiplier;
        }
    }

    public void ApplyDamageUpgrade(float multiplier)
    {
        damageMultiplier += multiplier;

        // Apply to combat component if exists
        var combatSystem = GetComponent<PlayerMovement_MaleWarrior>();
        if (combatSystem != null)
        {
            combatSystem.normalDamage = (int)(baseDamage * damageMultiplier);
            combatSystem.spinDamage = (int)((baseDamage * 3f) * damageMultiplier); // Spin does 3x normal damage
        }
    }

    public float GetMaxHealth()
    {
        return baseMaxHealth * healthMultiplier;
    }

    public float GetMoveSpeed()
    {
        return baseMoveSpeed * moveSpeedMultiplier;
    }

    public float GetDamage()
    {
        return baseDamage * damageMultiplier;
    }
}
