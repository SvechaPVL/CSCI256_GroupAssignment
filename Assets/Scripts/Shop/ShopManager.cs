using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// Main shop system manager - handles UI, purchases, and item effects
/// </summary>
public class ShopManager : MonoBehaviour
{
    [Header("Shop Items")]
    [SerializeField] private List<ShopItem> availableItems = new List<ShopItem>();

    [Header("UI References")]
    [SerializeField] private GameObject shopUI;
    [SerializeField] private GameObject promptUI;
    [SerializeField] private TextMeshProUGUI promptText;
    [SerializeField] private Transform itemCardsContainer;
    [SerializeField] private GameObject itemCardPrefab;

    [Header("Detail Panel")]
    [SerializeField] private GameObject detailPanel;
    [SerializeField] private Image detailIcon;
    [SerializeField] private TextMeshProUGUI detailName;
    [SerializeField] private TextMeshProUGUI detailDescription;
    [SerializeField] private TextMeshProUGUI detailPrice;
    [SerializeField] private TextMeshProUGUI detailStock;
    [SerializeField] private Button buyButton;
    [SerializeField] private TextMeshProUGUI buyButtonText;

    [Header("Currency Display")]
    [SerializeField] private TextMeshProUGUI currencyText;

    [Header("Player Reference")]
    [SerializeField] private GameObject player;

    private List<ShopItemCard> itemCards = new List<ShopItemCard>();
    private ShopItem selectedItem;

    private void Start()
    {
        // Hide UI at start
        if (shopUI != null) shopUI.SetActive(false);
        if (promptUI != null) promptUI.SetActive(false);
        if (detailPanel != null) detailPanel.SetActive(false);

        // Setup buy button
        if (buyButton != null)
        {
            buyButton.onClick.AddListener(OnBuyClicked);
        }

        // Setup currency listener
        if (PlayerCurrency.Instance != null)
        {
            PlayerCurrency.Instance.OnScrapChanged += UpdateCurrencyDisplay;
            UpdateCurrencyDisplay(PlayerCurrency.Instance.GetScrap());
        }

        // Find player if not assigned
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }

        // Create item cards
        CreateItemCards();
    }

    private void OnDestroy()
    {
        if (PlayerCurrency.Instance != null)
        {
            PlayerCurrency.Instance.OnScrapChanged -= UpdateCurrencyDisplay;
        }
    }

    private void CreateItemCards()
    {
        if (itemCardsContainer == null || itemCardPrefab == null) return;

        // Clear existing cards
        foreach (Transform child in itemCardsContainer)
        {
            Destroy(child.gameObject);
        }
        itemCards.Clear();

        // Create new cards
        foreach (ShopItem item in availableItems)
        {
            GameObject cardObj = Instantiate(itemCardPrefab, itemCardsContainer);
            ShopItemCard card = cardObj.GetComponent<ShopItemCard>();

            if (card != null)
            {
                card.Setup(item);
                card.OnCardClicked += OnItemCardClicked;
                itemCards.Add(card);
            }
        }
    }

    public void ShowPrompt(bool show)
    {
        if (promptUI != null)
        {
            promptUI.SetActive(show);
        }

        if (promptText != null && show)
        {
            promptText.text = "[E] Open Shop";
        }
    }

    public void OpenShop()
    {
        if (shopUI != null)
        {
            shopUI.SetActive(true);
        }

        if (promptUI != null)
        {
            promptUI.SetActive(false);
        }

        // Refresh all cards
        RefreshAllCards();

        // Hide detail panel initially
        if (detailPanel != null)
        {
            detailPanel.SetActive(false);
        }
    }

    public void CloseShop()
    {
        if (shopUI != null)
        {
            shopUI.SetActive(false);
        }

        selectedItem = null;

        if (detailPanel != null)
        {
            detailPanel.SetActive(false);
        }
    }

    private void OnItemCardClicked(ShopItem item)
    {
        selectedItem = item;
        ShowDetailPanel(item);
    }

    private void ShowDetailPanel(ShopItem item)
    {
        if (detailPanel == null) return;

        detailPanel.SetActive(true);

        if (detailIcon != null)
        {
            detailIcon.sprite = item.icon;
            detailIcon.color = item.icon != null ? Color.white : Color.clear;
        }

        if (detailName != null)
        {
            detailName.text = item.itemName;
        }

        if (detailDescription != null)
        {
            detailDescription.text = item.description + "\n\n" + item.GetEffectDescription();
        }

        if (detailPrice != null)
        {
            detailPrice.text = $"Price: {item.price} Scrap";
        }

        if (detailStock != null)
        {
            if (item.stock < 0)
            {
                detailStock.text = "Stock: Unlimited";
            }
            else
            {
                detailStock.text = $"Stock: {item.stock}";
            }
        }

        // Update buy button
        bool canAfford = PlayerCurrency.Instance.HasEnoughScrap(item.price);
        bool inStock = item.IsInStock();

        if (buyButton != null)
        {
            buyButton.interactable = canAfford && inStock;
        }

        if (buyButtonText != null)
        {
            if (!inStock)
            {
                buyButtonText.text = "OUT OF STOCK";
            }
            else if (!canAfford)
            {
                buyButtonText.text = "NOT ENOUGH SCRAP";
            }
            else
            {
                buyButtonText.text = "BUY";
            }
        }
    }

    private void OnBuyClicked()
    {
        if (selectedItem == null) return;

        if (PurchaseItem(selectedItem))
        {
            // Refresh UI
            RefreshAllCards();
            ShowDetailPanel(selectedItem);
        }
    }

    private bool PurchaseItem(ShopItem item)
    {
        // Check if can afford
        if (!PlayerCurrency.Instance.HasEnoughScrap(item.price))
        {
            Debug.Log("Not enough Scrap!");
            return false;
        }

        // Check stock
        if (!item.IsInStock())
        {
            Debug.Log("Item out of stock!");
            return false;
        }

        // Spend currency
        PlayerCurrency.Instance.SpendScrap(item.price);

        // Reduce stock
        item.Purchase();

        // Apply item effect
        ApplyItemEffect(item);

        Debug.Log($"Purchased: {item.itemName}");
        return true;
    }

    private void ApplyItemEffect(ShopItem item)
    {
        if (player == null) return;

        switch (item.itemType)
        {
            case ShopItem.ItemType.Consumable:
                ApplyConsumable(item);
                break;

            case ShopItem.ItemType.Upgrade:
                ApplyUpgrade(item);
                break;
        }
    }

    private void ApplyConsumable(ShopItem item)
    {
        switch (item.effectType)
        {
            case ShopItem.EffectType.HealthRestore:
                // Heal player
                PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.currentHealth = Mathf.Min(
                        playerHealth.currentHealth + item.effectValue,
                        playerHealth.maxHealth
                    );
                    Debug.Log($"Healed {item.effectValue} HP");
                }
                break;
        }

        // Add to inventory (optional for later use)
        if (PlayerInventory.Instance != null)
        {
            PlayerInventory.Instance.AddItem(item.itemId, item.itemName, item.icon);
        }
    }

    private void ApplyUpgrade(ShopItem item)
    {
        PlayerUpgrades playerUpgrades = player.GetComponent<PlayerUpgrades>();
        if (playerUpgrades == null) return;

        // Check if already purchased
        if (playerUpgrades.HasUpgrade(item.itemId))
        {
            Debug.Log("Upgrade already purchased!");
            return;
        }

        // Mark as purchased
        playerUpgrades.AddUpgrade(item.itemId);

        // Apply upgrade effect
        switch (item.effectType)
        {
            case ShopItem.EffectType.MaxHealthUpgrade:
                playerUpgrades.ApplyHealthUpgrade(item.effectValue);
                Debug.Log($"Max Health increased by {item.effectValue * 100}%");
                break;

            case ShopItem.EffectType.SpeedUpgrade:
                playerUpgrades.ApplySpeedUpgrade(item.effectValue);
                Debug.Log($"Move Speed increased by {item.effectValue * 100}%");
                break;

            case ShopItem.EffectType.DamageUpgrade:
                playerUpgrades.ApplyDamageUpgrade(item.effectValue);
                Debug.Log($"Damage increased by {item.effectValue * 100}%");
                break;
        }
    }

    private void UpdateCurrencyDisplay(int scrap)
    {
        if (currencyText != null)
        {
            currencyText.text = $"Scrap: {scrap}";
        }

        // Refresh card availability
        RefreshAllCards();
    }

    private void RefreshAllCards()
    {
        foreach (ShopItemCard card in itemCards)
        {
            card.UpdateAvailability();
        }
    }
}
