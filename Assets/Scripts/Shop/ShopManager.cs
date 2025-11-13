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
    [SerializeField] private RectTransform itemCardsContainer;
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
        // CHECK CANVAS SETUP
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas != null)
        {
            Debug.Log($"[ShopManager] Canvas found - RenderMode: {canvas.renderMode}, PixelPerfect: {canvas.pixelPerfect}");

            if (canvas.renderMode == RenderMode.ScreenSpaceCamera || canvas.renderMode == RenderMode.WorldSpace)
            {
                Debug.Log($"[ShopManager] Canvas worldCamera: {(canvas.worldCamera != null ? canvas.worldCamera.name : "NULL")}");
            }

            CanvasScaler scaler = canvas.GetComponent<CanvasScaler>();
            if (scaler != null)
            {
                Debug.Log($"[ShopManager] CanvasScaler - UIScale: {scaler.uiScaleMode}, ReferenceResolution: {scaler.referenceResolution}");
            }
        }
        else
        {
            Debug.LogError("[ShopManager] NO CANVAS FOUND!");
        }

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
        Debug.Log($"[ShopManager] CreateItemCards called");

        if (itemCardsContainer == null)
        {
            Debug.LogError("[ShopManager] itemCardsContainer is NULL!");
            return;
        }

        if (itemCardPrefab == null)
        {
            Debug.LogError("[ShopManager] itemCardPrefab is NULL!");
            return;
        }

        Debug.Log($"[ShopManager] Available items count: {availableItems.Count}");

        // Clear existing cards
        foreach (Transform child in itemCardsContainer)
        {
            Destroy(child.gameObject);
        }
        itemCards.Clear();

        // Create new cards
        foreach (ShopItem item in availableItems)
        {
            Debug.Log($"[ShopManager] Creating card for item: {item.itemName}");

            GameObject cardObj = Instantiate(itemCardPrefab, itemCardsContainer);
            cardObj.SetActive(true); // Make sure it's active!

            RectTransform cardRect = cardObj.GetComponent<RectTransform>();
            Image cardImage = cardObj.GetComponent<Image>();

            Debug.Log($"[ShopManager] Card created: {cardObj.name}, Active: {cardObj.activeSelf}, Parent: {cardObj.transform.parent.name}");
            Debug.Log($"[ShopManager] Card RectTransform - Size: {cardRect.rect.size}, Position: {cardRect.anchoredPosition}, Scale: {cardRect.localScale}");

            if (cardImage != null)
            {
                Debug.Log($"[ShopManager] Card Image - Color: {cardImage.color}, Enabled: {cardImage.enabled}, Material: {(cardImage.material != null ? cardImage.material.name : "NULL")}, Sprite: {(cardImage.sprite != null ? cardImage.sprite.name : "NULL")}, Type: {cardImage.type}, Raycast: {cardImage.raycastTarget}");
            }
            else
            {
                Debug.LogWarning($"[ShopManager] Card has NO Image component!");
            }

            ShopItemCard card = cardObj.GetComponent<ShopItemCard>();

            if (card != null)
            {
                card.Setup(item);
                card.OnCardClicked += OnItemCardClicked;
                itemCards.Add(card);
                Debug.Log($"[ShopManager] Card setup complete for: {item.itemName}");
            }
            else
            {
                Debug.LogError($"[ShopManager] ShopItemCard component not found on instantiated prefab!");
            }
        }

        Debug.Log($"[ShopManager] Total cards created: {itemCards.Count}");

        // Force update layout to ensure cards are visible
        // Need to do this in next frame for layout to update properly
        StartCoroutine(ForceLayoutUpdate());
    }

    private System.Collections.IEnumerator ForceLayoutUpdate()
    {
        Debug.Log($"[ShopManager] Forcing layout update...");

        if (itemCardsContainer != null)
        {
            // CHECK VIEWPORT AND SCROLLVIEW SIZES
            Transform viewport = itemCardsContainer.parent;
            if (viewport != null)
            {
                RectTransform viewportRect = viewport.GetComponent<RectTransform>();
                Debug.Log($"[ShopManager] VIEWPORT - Size: {viewportRect.rect.size}, Position: {viewportRect.anchoredPosition}");

                Transform scrollView = viewport.parent;
                if (scrollView != null)
                {
                    RectTransform scrollRect = scrollView.GetComponent<RectTransform>();
                    Debug.Log($"[ShopManager] SCROLLVIEW - Size: {scrollRect.rect.size}, Position: {scrollRect.anchoredPosition}");
                }
            }

            Debug.Log($"[ShopManager] CONTENT - Size: {itemCardsContainer.rect.size}, Position: {itemCardsContainer.anchoredPosition}");

            VerticalLayoutGroup layoutGroup = itemCardsContainer.GetComponent<VerticalLayoutGroup>();
            ContentSizeFitter sizeFitter = itemCardsContainer.GetComponent<ContentSizeFitter>();

            if (layoutGroup != null)
            {
                Debug.Log($"[ShopManager] VerticalLayoutGroup found - Spacing: {layoutGroup.spacing}, ChildControlHeight: {layoutGroup.childControlHeight}, ChildControlWidth: {layoutGroup.childControlWidth}");
            }
            else
            {
                Debug.LogError("[ShopManager] NO VerticalLayoutGroup found on Content!");
                yield break;
            }

            if (sizeFitter != null)
            {
                Debug.Log($"[ShopManager] ContentSizeFitter found - VerticalFit: {sizeFitter.verticalFit}");
            }
            else
            {
                Debug.LogError("[ShopManager] NO ContentSizeFitter found on Content!");
                yield break;
            }

            // FORCE IMMEDIATE LAYOUT CALCULATION
            // Step 1: Mark layout as dirty
            UnityEngine.UI.LayoutRebuilder.MarkLayoutForRebuild(itemCardsContainer);

            // Step 2: Wait a frame
            yield return null;

            // Step 3: Force canvas update
            Canvas.ForceUpdateCanvases();

            // Step 4: Manually calculate layout
            layoutGroup.CalculateLayoutInputHorizontal();
            layoutGroup.CalculateLayoutInputVertical();
            layoutGroup.SetLayoutHorizontal();
            layoutGroup.SetLayoutVertical();

            Debug.Log($"[ShopManager] Manual layout calculation complete");

            // Step 5: Wait another frame
            yield return null;

            // Step 6: Force rebuild again
            Canvas.ForceUpdateCanvases();
            UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(itemCardsContainer);

            // Step 7: One more frame and rebuild
            yield return null;
            Canvas.ForceUpdateCanvases();
            UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(itemCardsContainer);

            Debug.Log($"[ShopManager] Layout rebuild complete. Content size: {itemCardsContainer.sizeDelta}");

            // 🔥 MANUAL POSITIONING AND SIZE FIX - force each card to correct position and size
            Debug.Log($"[ShopManager] 🔥 FORCING MANUAL CARD POSITIONING AND SIZE...");
            float currentY = -layoutGroup.padding.top;
            float contentWidth = itemCardsContainer.rect.width;
            float cardWidth = contentWidth - layoutGroup.padding.left - layoutGroup.padding.right;

            Debug.Log($"[ShopManager] Content width: {contentWidth}, Card width will be: {cardWidth}");

            for (int i = 0; i < itemCardsContainer.childCount; i++)
            {
                RectTransform childRect = itemCardsContainer.GetChild(i) as RectTransform;
                if (childRect != null && childRect.gameObject.activeSelf)
                {
                    LayoutElement layoutElement = childRect.GetComponent<LayoutElement>();
                    float cardHeight = layoutElement != null ? layoutElement.preferredHeight : 120f;

                    // FORCE the position AND size!
                    childRect.anchoredPosition = new Vector2(0, currentY);
                    childRect.sizeDelta = new Vector2(cardWidth, cardHeight);

                    Debug.Log($"[ShopManager] 🎯 FORCED Card {i} to Position Y={currentY}, Size=({cardWidth}, {cardHeight})");

                    currentY -= (cardHeight + layoutGroup.spacing);
                }
            }

            // 🔍 DETAILED VISIBILITY DIAGNOSTICS
            Debug.Log($"[ShopManager] 🔍 VISIBILITY DIAGNOSTICS:");
            Canvas canvas = FindObjectOfType<Canvas>();
            Debug.Log($"[ShopManager] Canvas active: {(canvas != null ? canvas.gameObject.activeSelf.ToString() : "NULL")}, Canvas.enabled: {(canvas != null ? canvas.enabled.ToString() : "NULL")}");
            Debug.Log($"[ShopManager] ShopUI active: {shopUI.activeSelf}");
            Debug.Log($"[ShopManager] Content active: {itemCardsContainer.gameObject.activeSelf}");

            // Check viewport mask
            UnityEngine.UI.Mask viewportMask = viewport.GetComponent<UnityEngine.UI.Mask>();
            if (viewportMask != null)
            {
                Debug.Log($"[ShopManager] Viewport has Mask component: enabled={viewportMask.enabled}, showMaskGraphic={viewportMask.showMaskGraphic}");
            }
            else
            {
                Debug.LogWarning($"[ShopManager] Viewport has NO Mask component!");
            }

            // Log positions after manual positioning
            Debug.Log($"[ShopManager] After manual positioning:");
            for (int i = 0; i < Mathf.Min(3, itemCardsContainer.childCount); i++)
            {
                RectTransform childRect = itemCardsContainer.GetChild(i) as RectTransform;
                if (childRect != null)
                {
                    Debug.Log($"[ShopManager] Card {i} - Position: {childRect.anchoredPosition}, AnchorMin: {childRect.anchorMin}, AnchorMax: {childRect.anchorMax}, Pivot: {childRect.pivot}, Size: {childRect.sizeDelta}");

                    Vector3[] corners = new Vector3[4];
                    childRect.GetWorldCorners(corners);
                    Debug.Log($"[ShopManager] Card {i} - Active: {childRect.gameObject.activeSelf}, WorldCorners: [{corners[0]}, {corners[1]}, {corners[2]}, {corners[3]}]");

                    Image cardImage = childRect.GetComponent<Image>();
                    if (cardImage != null)
                    {
                        Debug.Log($"[ShopManager] Card {i} Image - Enabled: {cardImage.enabled}, Color: {cardImage.color}, RaycastTarget: {cardImage.raycastTarget}");
                    }

                    CanvasGroup cg = childRect.GetComponent<CanvasGroup>();
                    if (cg != null)
                    {
                        Debug.Log($"[ShopManager] Card {i} CanvasGroup - Alpha: {cg.alpha}, Interactable: {cg.interactable}, BlocksRaycasts: {cg.blocksRaycasts}");
                    }

                    LayoutElement layoutElement = childRect.GetComponent<LayoutElement>();
                    if (layoutElement != null)
                    {
                        Debug.Log($"[ShopManager] Card {i} LayoutElement - PreferredHeight: {layoutElement.preferredHeight}, MinHeight: {layoutElement.minHeight}");
                    }
                    else
                    {
                        Debug.LogError($"[ShopManager] Card {i} has NO LayoutElement component!");
                    }
                }
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
