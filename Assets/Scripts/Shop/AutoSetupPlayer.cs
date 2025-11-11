using UnityEngine;

/// <summary>
/// Auto-setup script that adds all required shop components to the player
/// Add this component to your player object and it will automatically configure everything
/// </summary>
public class AutoSetupPlayer : MonoBehaviour
{
    [Header("Auto-Setup Configuration")]
    [SerializeField] private bool autoSetupOnStart = true;
    [SerializeField] private int startingScrap = 100;

    [Header("Base Stats")]
    [SerializeField] private float baseMaxHealth = 100f;
    [SerializeField] private float baseMoveSpeed = 5f;
    [SerializeField] private float baseDamage = 10f;

    [Header("Status")]
    [SerializeField] private bool isSetupComplete = false;

    private void Start()
    {
        if (autoSetupOnStart && !isSetupComplete)
        {
            SetupPlayer();
        }
    }

    [ContextMenu("Setup Player (Force)")]
    public void SetupPlayer()
    {
        Debug.Log("[AutoSetupPlayer] Starting automatic player setup...");

        // 1. Setup PlayerCurrency
        PlayerCurrency currency = GetComponent<PlayerCurrency>();
        if (currency == null)
        {
            currency = gameObject.AddComponent<PlayerCurrency>();
            Debug.Log("✓ Added PlayerCurrency component");
        }
        else
        {
            Debug.Log("✓ PlayerCurrency already exists");
        }

        // Use reflection to set private field for starting scrap
        var currencyType = typeof(PlayerCurrency);
        var currentScrapField = currencyType.GetField("currentScrap",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (currentScrapField != null)
        {
            currentScrapField.SetValue(currency, startingScrap);
            Debug.Log($"✓ Set starting scrap to {startingScrap}");
        }

        // 2. Setup PlayerInventory
        PlayerInventory inventory = GetComponent<PlayerInventory>();
        if (inventory == null)
        {
            inventory = gameObject.AddComponent<PlayerInventory>();
            Debug.Log("✓ Added PlayerInventory component");
        }
        else
        {
            Debug.Log("✓ PlayerInventory already exists");
        }

        // 3. Setup PlayerUpgrades
        PlayerUpgrades upgrades = GetComponent<PlayerUpgrades>();
        if (upgrades == null)
        {
            upgrades = gameObject.AddComponent<PlayerUpgrades>();
            Debug.Log("✓ Added PlayerUpgrades component");
        }
        else
        {
            Debug.Log("✓ PlayerUpgrades already exists");
        }

        // Configure PlayerUpgrades base stats using reflection
        var upgradesType = typeof(PlayerUpgrades);

        var healthField = upgradesType.GetField("baseMaxHealth",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (healthField != null)
        {
            healthField.SetValue(upgrades, baseMaxHealth);
            Debug.Log($"✓ Set base max health to {baseMaxHealth}");
        }

        var speedField = upgradesType.GetField("baseMoveSpeed",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (speedField != null)
        {
            speedField.SetValue(upgrades, baseMoveSpeed);
            Debug.Log($"✓ Set base move speed to {baseMoveSpeed}");
        }

        var damageField = upgradesType.GetField("baseDamage",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (damageField != null)
        {
            damageField.SetValue(upgrades, baseDamage);
            Debug.Log($"✓ Set base damage to {baseDamage}");
        }

        // 4. Verify Player tag
        if (!gameObject.CompareTag("Player"))
        {
            Debug.LogWarning("⚠ GameObject does not have 'Player' tag! Please set it manually.");
        }
        else
        {
            Debug.Log("✓ Player tag is set correctly");
        }

        // 5. Check for required components
        CheckRequiredComponents();

        isSetupComplete = true;
        Debug.Log("✅ [AutoSetupPlayer] Setup complete! Player is ready for shop system.");
    }

    private void CheckRequiredComponents()
    {
        // Check for PlayerHealth
        PlayerHealth health = GetComponent<PlayerHealth>();
        if (health == null)
        {
            Debug.LogWarning("⚠ PlayerHealth component not found! Please add it manually.");
        }
        else
        {
            Debug.Log("✓ PlayerHealth found");
        }

        // Check for movement component
        FPSInput fpsInput = GetComponent<FPSInput>();
        PlayerMovement_MaleWarrior combatMovement = GetComponent<PlayerMovement_MaleWarrior>();

        if (fpsInput == null && combatMovement == null)
        {
            Debug.LogWarning("⚠ No movement component found (FPSInput or PlayerMovement_MaleWarrior)");
        }
        else
        {
            if (fpsInput != null) Debug.Log("✓ FPSInput found");
            if (combatMovement != null) Debug.Log("✓ PlayerMovement_MaleWarrior found");
        }
    }

    [ContextMenu("Remove Shop Components")]
    public void RemoveShopComponents()
    {
        if (GetComponent<PlayerCurrency>()) DestroyImmediate(GetComponent<PlayerCurrency>());
        if (GetComponent<PlayerInventory>()) DestroyImmediate(GetComponent<PlayerInventory>());
        if (GetComponent<PlayerUpgrades>()) DestroyImmediate(GetComponent<PlayerUpgrades>());

        isSetupComplete = false;
        Debug.Log("[AutoSetupPlayer] Removed all shop components");
    }
}
