using UnityEngine;

/// <summary>
/// Defines a shop item (consumable or upgrade)
/// </summary>
[CreateAssetMenu(fileName = "New Shop Item", menuName = "Shop/Shop Item")]
public class ShopItem : ScriptableObject
{
    public enum ItemType
    {
        Consumable,
        Upgrade
    }

    public enum EffectType
    {
        HealthRestore,      // Consumable: restores HP
        MaxHealthUpgrade,   // Upgrade: increases max health
        SpeedUpgrade,       // Upgrade: increases move speed
        DamageUpgrade       // Upgrade: increases damage
    }

    [Header("Basic Info")]
    public string itemId;
    public string itemName;
    public Sprite icon;
    [TextArea(3, 5)]
    public string description;

    [Header("Shop Properties")]
    public int price;
    public int stock = -1; // -1 = infinite stock
    public ItemType itemType;

    [Header("Effects")]
    public EffectType effectType;
    public float effectValue; // Amount to heal, or multiplier for upgrades

    [Header("Visual")]
    public Color rarityColor = Color.white;

    public bool IsInStock()
    {
        return stock != 0;
    }

    public void Purchase()
    {
        if (stock > 0)
        {
            stock--;
        }
    }

    public string GetEffectDescription()
    {
        switch (effectType)
        {
            case EffectType.HealthRestore:
                return $"Restores {effectValue} HP";
            case EffectType.MaxHealthUpgrade:
                return $"Increases Max Health by {effectValue * 100}%";
            case EffectType.SpeedUpgrade:
                return $"Increases Move Speed by {effectValue * 100}%";
            case EffectType.DamageUpgrade:
                return $"Increases Damage by {effectValue * 100}%";
            default:
                return "Unknown effect";
        }
    }
}
