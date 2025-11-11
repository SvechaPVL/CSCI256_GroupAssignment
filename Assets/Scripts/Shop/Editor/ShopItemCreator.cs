#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

/// <summary>
/// Editor window for quickly creating shop items
/// Access via: Tools → Shop System → Create Shop Items
/// </summary>
public class ShopItemCreator : EditorWindow
{
    private string folderPath = "Assets/ScriptableObjects/ShopItems";

    [MenuItem("Tools/Shop System/Create Shop Items")]
    public static void ShowWindow()
    {
        GetWindow<ShopItemCreator>("Shop Item Creator");
    }

    private void OnGUI()
    {
        GUILayout.Label("Quick Shop Item Creator", EditorStyles.boldLabel);
        GUILayout.Space(10);

        folderPath = EditorGUILayout.TextField("Output Folder:", folderPath);

        GUILayout.Space(10);
        EditorGUILayout.HelpBox("Click buttons below to create example shop items", MessageType.Info);
        GUILayout.Space(10);

        // Consumables Section
        GUILayout.Label("Consumables (Health Potions)", EditorStyles.boldLabel);

        if (GUILayout.Button("Create Small Health Potion (30 Scrap)"))
        {
            CreateHealthPotion("SmallHealthPotion", "Small Health Potion",
                "A small vial of healing liquid.", 30, 10, 20f, Color.white);
        }

        if (GUILayout.Button("Create Health Potion (50 Scrap)"))
        {
            CreateHealthPotion("HealthPotion", "Health Potion",
                "A standard healing potion. Restores moderate HP.", 50, 5, 50f, new Color(0.3f, 0.69f, 0.31f));
        }

        if (GUILayout.Button("Create Greater Health Potion (100 Scrap)"))
        {
            CreateHealthPotion("GreaterHealthPotion", "Greater Health Potion",
                "A powerful healing elixir. Fully restores your health.", 100, 2, 100f, new Color(0.13f, 0.59f, 0.95f));
        }

        GUILayout.Space(15);

        // Health Upgrades Section
        GUILayout.Label("Health Upgrades", EditorStyles.boldLabel);

        if (GUILayout.Button("Create Iron Heart I (100 Scrap)"))
        {
            CreateUpgrade("IronHeart", "Iron Heart",
                "Strengthens your body and increases maximum health.",
                100, ShopItem.EffectType.MaxHealthUpgrade, 0.2f, new Color(1f, 0.76f, 0.03f));
        }

        if (GUILayout.Button("Create Iron Heart II (200 Scrap)"))
        {
            CreateUpgrade("IronHeartII", "Iron Heart II",
                "Further strengthens your vitality. Maximum health increased significantly.",
                200, ShopItem.EffectType.MaxHealthUpgrade, 0.3f, new Color(1f, 0.6f, 0f));
        }

        if (GUILayout.Button("Create Dragon Heart (350 Scrap)"))
        {
            CreateUpgrade("DragonHeart", "Dragon Heart",
                "The heart of a legendary beast. Massively increases maximum health.",
                350, ShopItem.EffectType.MaxHealthUpgrade, 0.5f, new Color(0.96f, 0.26f, 0.21f));
        }

        GUILayout.Space(15);

        // Speed Upgrades Section
        GUILayout.Label("Speed Upgrades", EditorStyles.boldLabel);

        if (GUILayout.Button("Create Swift Boots I (80 Scrap)"))
        {
            CreateUpgrade("SwiftBoots", "Swift Boots",
                "Lightweight boots that enhance your movement speed.",
                80, ShopItem.EffectType.SpeedUpgrade, 0.15f, new Color(1f, 0.76f, 0.03f));
        }

        if (GUILayout.Button("Create Swift Boots II (150 Scrap)"))
        {
            CreateUpgrade("SwiftBootsII", "Swift Boots II",
                "Enchanted boots that greatly improve your agility.",
                150, ShopItem.EffectType.SpeedUpgrade, 0.25f, new Color(1f, 0.6f, 0f));
        }

        if (GUILayout.Button("Create Windwalker Boots (300 Scrap)"))
        {
            CreateUpgrade("WindwalkerBoots", "Windwalker Boots",
                "Legendary boots blessed by the wind spirits. Move like the wind itself.",
                300, ShopItem.EffectType.SpeedUpgrade, 0.4f, new Color(0.61f, 0.15f, 0.69f));
        }

        GUILayout.Space(15);

        // Damage Upgrades Section
        GUILayout.Label("Damage Upgrades", EditorStyles.boldLabel);

        if (GUILayout.Button("Create Warrior's Strength I (120 Scrap)"))
        {
            CreateUpgrade("WarriorStrength", "Warrior's Strength",
                "Increases your attack power through rigorous training.",
                120, ShopItem.EffectType.DamageUpgrade, 0.2f, new Color(1f, 0.76f, 0.03f));
        }

        if (GUILayout.Button("Create Warrior's Strength II (250 Scrap)"))
        {
            CreateUpgrade("WarriorStrengthII", "Warrior's Strength II",
                "Master warrior techniques. Significantly increases damage output.",
                250, ShopItem.EffectType.DamageUpgrade, 0.35f, new Color(1f, 0.6f, 0f));
        }

        if (GUILayout.Button("Create Titan's Fury (400 Scrap)"))
        {
            CreateUpgrade("TitansFury", "Titan's Fury",
                "Channel the rage of ancient titans. Devastating damage increase.",
                400, ShopItem.EffectType.DamageUpgrade, 0.5f, new Color(0.96f, 0.26f, 0.21f));
        }

        GUILayout.Space(15);

        // Create All Button
        if (GUILayout.Button("CREATE ALL ITEMS", GUILayout.Height(40)))
        {
            CreateAllItems();
        }
    }

    private void CreateHealthPotion(string id, string name, string description, int price, int stock, float healAmount, Color color)
    {
        ShopItem item = CreateShopItem(id, name, description, price, stock, color);
        item.itemType = ShopItem.ItemType.Consumable;
        item.effectType = ShopItem.EffectType.HealthRestore;
        item.effectValue = healAmount;
        SaveAsset(item, id);
    }

    private void CreateUpgrade(string id, string name, string description, int price, ShopItem.EffectType effectType, float effectValue, Color color)
    {
        ShopItem item = CreateShopItem(id, name, description, price, 1, color);
        item.itemType = ShopItem.ItemType.Upgrade;
        item.effectType = effectType;
        item.effectValue = effectValue;
        SaveAsset(item, id);
    }

    private ShopItem CreateShopItem(string id, string name, string description, int price, int stock, Color color)
    {
        ShopItem item = ScriptableObject.CreateInstance<ShopItem>();
        item.itemId = id.ToLower();
        item.itemName = name;
        item.description = description;
        item.price = price;
        item.stock = stock;
        item.rarityColor = color;
        return item;
    }

    private void SaveAsset(ShopItem item, string fileName)
    {
        // Ensure directory exists
        if (!AssetDatabase.IsValidFolder(folderPath))
        {
            string[] folders = folderPath.Split('/');
            string currentPath = folders[0];

            for (int i = 1; i < folders.Length; i++)
            {
                string newPath = currentPath + "/" + folders[i];
                if (!AssetDatabase.IsValidFolder(newPath))
                {
                    AssetDatabase.CreateFolder(currentPath, folders[i]);
                }
                currentPath = newPath;
            }
        }

        string assetPath = $"{folderPath}/{fileName}.asset";
        AssetDatabase.CreateAsset(item, assetPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"✓ Created shop item: {assetPath}");
        EditorGUIUtility.PingObject(item);
    }

    private void CreateAllItems()
    {
        Debug.Log("Creating all shop items...");

        // Consumables
        CreateHealthPotion("health_potion_small", "Small Health Potion", "A small vial of healing liquid.", 30, 10, 20f, Color.white);
        CreateHealthPotion("health_potion", "Health Potion", "A standard healing potion. Restores moderate HP.", 50, 5, 50f, new Color(0.3f, 0.69f, 0.31f));
        CreateHealthPotion("health_potion_greater", "Greater Health Potion", "A powerful healing elixir. Fully restores your health.", 100, 2, 100f, new Color(0.13f, 0.59f, 0.95f));

        // Health Upgrades
        CreateUpgrade("health_upgrade_tier1", "Iron Heart", "Strengthens your body and increases maximum health.", 100, ShopItem.EffectType.MaxHealthUpgrade, 0.2f, new Color(1f, 0.76f, 0.03f));
        CreateUpgrade("health_upgrade_tier2", "Iron Heart II", "Further strengthens your vitality. Maximum health increased significantly.", 200, ShopItem.EffectType.MaxHealthUpgrade, 0.3f, new Color(1f, 0.6f, 0f));
        CreateUpgrade("health_upgrade_tier3", "Dragon Heart", "The heart of a legendary beast. Massively increases maximum health.", 350, ShopItem.EffectType.MaxHealthUpgrade, 0.5f, new Color(0.96f, 0.26f, 0.21f));

        // Speed Upgrades
        CreateUpgrade("speed_upgrade_tier1", "Swift Boots", "Lightweight boots that enhance your movement speed.", 80, ShopItem.EffectType.SpeedUpgrade, 0.15f, new Color(1f, 0.76f, 0.03f));
        CreateUpgrade("speed_upgrade_tier2", "Swift Boots II", "Enchanted boots that greatly improve your agility.", 150, ShopItem.EffectType.SpeedUpgrade, 0.25f, new Color(1f, 0.6f, 0f));
        CreateUpgrade("speed_upgrade_tier3", "Windwalker Boots", "Legendary boots blessed by the wind spirits. Move like the wind itself.", 300, ShopItem.EffectType.SpeedUpgrade, 0.4f, new Color(0.61f, 0.15f, 0.69f));

        // Damage Upgrades
        CreateUpgrade("damage_upgrade_tier1", "Warrior's Strength", "Increases your attack power through rigorous training.", 120, ShopItem.EffectType.DamageUpgrade, 0.2f, new Color(1f, 0.76f, 0.03f));
        CreateUpgrade("damage_upgrade_tier2", "Warrior's Strength II", "Master warrior techniques. Significantly increases damage output.", 250, ShopItem.EffectType.DamageUpgrade, 0.35f, new Color(1f, 0.6f, 0f));
        CreateUpgrade("damage_upgrade_tier3", "Titan's Fury", "Channel the rage of ancient titans. Devastating damage increase.", 400, ShopItem.EffectType.DamageUpgrade, 0.5f, new Color(0.96f, 0.26f, 0.21f));

        Debug.Log("✅ All shop items created successfully!");
        EditorUtility.DisplayDialog("Success", "All 12 shop items have been created!", "OK");
    }
}
#endif
