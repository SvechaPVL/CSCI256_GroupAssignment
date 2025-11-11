# Example Shop Items

Эти примеры можно создать в Unity через: ПКМ → Create → Shop → Shop Item

## Consumables (Расходники)

### Small Health Potion
```
Item ID: health_potion_small
Item Name: Small Health Potion
Description: A small vial of healing liquid. Restores a small amount of HP.
Price: 30
Stock: 10
Item Type: Consumable
Effect Type: HealthRestore
Effect Value: 20
Rarity Color: #FFFFFF (белый)
```

### Medium Health Potion
```
Item ID: health_potion_medium
Item Name: Health Potion
Description: A standard healing potion. Restores moderate HP.
Price: 50
Stock: 5
Item Type: Consumable
Effect Type: HealthRestore
Effect Value: 50
Rarity Color: #4CAF50 (зеленый)
```

### Large Health Potion
```
Item ID: health_potion_large
Item Name: Greater Health Potion
Description: A powerful healing elixir. Fully restores your health.
Price: 100
Stock: 2
Item Type: Consumable
Effect Type: HealthRestore
Effect Value: 100
Rarity Color: #2196F3 (синий)
```

## Upgrades - Health (Улучшения здоровья)

### Iron Heart I
```
Item ID: health_upgrade_tier1
Item Name: Iron Heart
Description: Strengthens your body and increases maximum health.
Price: 100
Stock: 1
Item Type: Upgrade
Effect Type: MaxHealthUpgrade
Effect Value: 0.2 (increases max HP by 20%)
Rarity Color: #FFC107 (янтарный)
```

### Iron Heart II
```
Item ID: health_upgrade_tier2
Item Name: Iron Heart II
Description: Further strengthens your vitality. Maximum health increased significantly.
Price: 200
Stock: 1
Item Type: Upgrade
Effect Type: MaxHealthUpgrade
Effect Value: 0.3 (increases max HP by 30%)
Rarity Color: #FF9800 (оранжевый)
```

### Dragon Heart
```
Item ID: health_upgrade_tier3
Item Name: Dragon Heart
Description: The heart of a legendary beast. Massively increases maximum health.
Price: 350
Stock: 1
Item Type: Upgrade
Effect Type: MaxHealthUpgrade
Effect Value: 0.5 (increases max HP by 50%)
Rarity Color: #F44336 (красный)
```

## Upgrades - Speed (Улучшения скорости)

### Swift Boots I
```
Item ID: speed_upgrade_tier1
Item Name: Swift Boots
Description: Lightweight boots that enhance your movement speed.
Price: 80
Stock: 1
Item Type: Upgrade
Effect Type: SpeedUpgrade
Effect Value: 0.15 (increases speed by 15%)
Rarity Color: #FFC107 (янтарный)
```

### Swift Boots II
```
Item ID: speed_upgrade_tier2
Item Name: Swift Boots II
Description: Enchanted boots that greatly improve your agility.
Price: 150
Stock: 1
Item Type: Upgrade
Effect Type: SpeedUpgrade
Effect Value: 0.25 (increases speed by 25%)
Rarity Color: #FF9800 (оранжевый)
```

### Windwalker Boots
```
Item ID: speed_upgrade_tier3
Item Name: Windwalker Boots
Description: Legendary boots blessed by the wind spirits. Move like the wind itself.
Price: 300
Stock: 1
Item Type: Upgrade
Effect Type: SpeedUpgrade
Effect Value: 0.4 (increases speed by 40%)
Rarity Color: #9C27B0 (фиолетовый)
```

## Upgrades - Damage (Улучшения урона)

### Warrior's Strength I
```
Item ID: damage_upgrade_tier1
Item Name: Warrior's Strength
Description: Increases your attack power through rigorous training.
Price: 120
Stock: 1
Item Type: Upgrade
Effect Type: DamageUpgrade
Effect Value: 0.2 (increases damage by 20%)
Rarity Color: #FFC107 (янтарный)
```

### Warrior's Strength II
```
Item ID: damage_upgrade_tier2
Item Name: Warrior's Strength II
Description: Master warrior techniques. Significantly increases damage output.
Price: 250
Stock: 1
Item Type: Upgrade
Effect Type: DamageUpgrade
Effect Value: 0.35 (increases damage by 35%)
Rarity Color: #FF9800 (оранжевый)
```

### Titan's Fury
```
Item ID: damage_upgrade_tier3
Item Name: Titan's Fury
Description: Channel the rage of ancient titans. Devastating damage increase.
Price: 400
Stock: 1
Item Type: Upgrade
Effect Type: DamageUpgrade
Effect Value: 0.5 (increases damage by 50%)
Rarity Color: #F44336 (красный)
```

## Starter Bundle (Стартовый набор)

### Adventurer's Kit
```
Item ID: starter_bundle
Item Name: Adventurer's Kit
Description: A complete starter package for new adventurers. Contains basic supplies.
Price: 150
Stock: 1
Item Type: Consumable
Effect Type: HealthRestore
Effect Value: 30
Rarity Color: #4CAF50 (зеленый)
```

## Color Reference (Справочник цветов)

Используйте эти цвета для различных уровней редкости:

- **Common (Обычный)**: #FFFFFF (белый) - базовые предметы
- **Uncommon (Необычный)**: #4CAF50 (зеленый) - улучшенные предметы
- **Rare (Редкий)**: #2196F3 (синий) - редкие находки
- **Epic (Эпический)**: #9C27B0 (фиолетовый) - мощные предметы
- **Legendary (Легендарный)**: #FF9800 (оранжевый) - легендарные артефакты
- **Mythic (Мифический)**: #F44336 (красный) - мифические реликвии
- **Gold (Золотой)**: #FFC107 (янтарный) - ценные улучшения

## Notes (Примечания)

1. **Stock = -1** означает бесконечный запас
2. **Effect Value** для Upgrades указывается в долях (0.2 = 20%, 0.5 = 50%)
3. **Effect Value** для HealthRestore указывается в абсолютных значениях HP
4. Upgrades можно купить только один раз (проверяется автоматически)
5. Consumables добавляются в инвентарь и могут быть куплены несколько раз

## Recommended Starting Inventory (Рекомендуемый стартовый инвентарь)

Для тестирования рекомендуется добавить в магазин:
- 2-3 разных расходника (healing potions разных уровней)
- 1-2 улучшения каждого типа (health, speed, damage)
- Общее количество предметов: 6-9 для начала

Пример начального магазина:
1. Small Health Potion (30 Scrap)
2. Health Potion (50 Scrap)
3. Iron Heart (100 Scrap)
4. Swift Boots (80 Scrap)
5. Warrior's Strength (120 Scrap)

Игрок начинает с **100 Scrap**, что позволяет купить несколько базовых предметов.
