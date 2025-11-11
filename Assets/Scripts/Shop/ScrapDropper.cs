using UnityEngine;

/// <summary>
/// Add this component to enemies to drop Scrap when they die
/// Attach to any enemy with EnemyHealth component
/// </summary>
[RequireComponent(typeof(EnemyHealth))]
public class ScrapDropper : MonoBehaviour
{
    [Header("Scrap Reward")]
    [SerializeField] private int minScrap = 5;
    [SerializeField] private int maxScrap = 15;

    [Header("Optional: Visual Feedback")]
    [SerializeField] private GameObject scrapParticlePrefab;

    private EnemyHealth enemyHealth;
    private bool hasDropped = false;

    private void Start()
    {
        enemyHealth = GetComponent<EnemyHealth>();
    }

    private void Update()
    {
        // Check if enemy is dead and hasn't dropped scrap yet
        if (!hasDropped && enemyHealth != null && enemyHealth.currentHealth <= 0)
        {
            DropScrap();
            hasDropped = true;
        }
    }

    private void DropScrap()
    {
        if (PlayerCurrency.Instance == null)
        {
            Debug.LogWarning("[ScrapDropper] PlayerCurrency instance not found!");
            return;
        }

        // Calculate random scrap amount
        int scrapAmount = Random.Range(minScrap, maxScrap + 1);

        // Add scrap to player
        PlayerCurrency.Instance.AddScrap(scrapAmount);

        Debug.Log($"[ScrapDropper] {gameObject.name} dropped {scrapAmount} Scrap!");

        // Spawn visual effect if assigned
        if (scrapParticlePrefab != null)
        {
            Instantiate(scrapParticlePrefab, transform.position, Quaternion.identity);
        }
    }

    // Optional: Manual trigger from other scripts
    public void ForceDropScrap()
    {
        if (!hasDropped)
        {
            DropScrap();
            hasDropped = true;
        }
    }
}
