using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

/// <summary>
/// UI card for displaying shop item
/// </summary>
public class ShopItemCard : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private TextMeshProUGUI stockText;
    [SerializeField] private Button cardButton;
    [SerializeField] private Image backgroundImage;

    private ShopItem shopItem;
    public event Action<ShopItem> OnCardClicked;

    private void Awake()
    {
        if (cardButton != null)
        {
            cardButton.onClick.AddListener(OnClick);
        }
    }

    public void Setup(ShopItem item)
    {
        shopItem = item;

        if (iconImage != null)
        {
            iconImage.sprite = item.icon;
            iconImage.color = item.icon != null ? Color.white : Color.clear;
        }

        if (nameText != null)
        {
            nameText.text = item.itemName;
        }

        if (priceText != null)
        {
            priceText.text = $"{item.price} Scrap";
        }

        if (stockText != null)
        {
            if (item.stock < 0)
            {
                stockText.text = "∞";
            }
            else
            {
                stockText.text = $"x{item.stock}";
            }
        }

        if (backgroundImage != null)
        {
            backgroundImage.color = item.rarityColor;
        }

        UpdateAvailability();
    }

    public void UpdateAvailability()
    {
        if (shopItem == null) return;

        bool canAfford = PlayerCurrency.Instance != null &&
                         PlayerCurrency.Instance.HasEnoughScrap(shopItem.price);
        bool inStock = shopItem.IsInStock();

        if (cardButton != null)
        {
            cardButton.interactable = canAfford && inStock;
        }

        // Visual feedback for unavailable items
        Color cardColor = (canAfford && inStock) ? Color.white : new Color(0.6f, 0.6f, 0.6f, 0.8f);

        if (iconImage != null && shopItem.icon != null)
        {
            iconImage.color = cardColor;
        }

        if (nameText != null)
        {
            nameText.color = cardColor;
        }
    }

    private void OnClick()
    {
        OnCardClicked?.Invoke(shopItem);
    }

    public ShopItem GetShopItem()
    {
        return shopItem;
    }
}
