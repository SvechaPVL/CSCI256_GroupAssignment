# Shop System Setup Guide

## Обзор системы

Система магазина включает следующие компоненты:
- **PlayerCurrency** - управление валютой (Scrap)
- **PlayerInventory** - инвентарь для расходников
- **PlayerUpgrades** - постоянные улучшения
- **ShopItem** - ScriptableObject для предметов
- **ShopManager** - основной менеджер магазина
- **ShopTrigger** - триггер для открытия магазина
- **ShopItemCard** - UI карточка предмета

## Быстрый старт

### Шаг 1: Настройка игрока

1. Добавьте на объект игрока следующие компоненты:
   - `PlayerCurrency` (управление деньгами)
   - `PlayerInventory` (инвентарь)
   - `PlayerUpgrades` (улучшения)

2. В компоненте `PlayerCurrency`:
   - Установите `Current Scrap` (например, 100 для старта)

3. В компоненте `PlayerUpgrades`:
   - Установите базовые параметры:
     - Base Max Health: 100
     - Base Move Speed: 5
     - Base Damage: 10

### Шаг 2: Создание предметов магазина

1. В Unity: ПКМ → Create → Shop → Shop Item

2. Настройте параметры предмета:

**Пример расходника (Health Potion):**
```
Item ID: health_potion
Item Name: Health Potion
Description: Restores health instantly
Price: 50
Stock: 5 (или -1 для бесконечного)
Item Type: Consumable
Effect Type: HealthRestore
Effect Value: 30 (восстанавливает 30 HP)
```

**Пример улучшения (Speed Boost):**
```
Item ID: speed_upgrade_1
Item Name: Swift Legs
Description: Permanently increases movement speed
Price: 100
Stock: 1
Item Type: Upgrade
Effect Type: SpeedUpgrade
Effect Value: 0.2 (увеличивает скорость на 20%)
```

### Шаг 3: Создание UI магазина

#### 3.1 Основной Canvas

1. Создайте Canvas (UI → Canvas)
2. Установите Canvas Scaler → UI Scale Mode → Scale With Screen Size
3. Reference Resolution: 1920x1080

#### 3.2 Shop UI структура

```
Canvas
└── ShopPanel (панель магазина)
    ├── Background (затемнение)
    ├── MainPanel
    │   ├── Header
    │   │   ├── TitleText "SHOP"
    │   │   └── CurrencyText "Scrap: 100"
    │   ├── ItemScrollView
    │   │   └── Content (контейнер для карточек)
    │   └── DetailPanel
    │       ├── ItemIcon
    │       ├── ItemName
    │       ├── ItemDescription
    │       ├── ItemPrice
    │       ├── ItemStock
    │       └── BuyButton
    └── PromptPanel (подсказка [E] Open Shop)
```

#### 3.3 Создание Item Card Prefab

1. Создайте GameObject → UI → Button
2. Переименуйте в "ItemCardPrefab"
3. Добавьте компонент `ShopItemCard`
4. Структура карточки:

```
ItemCardPrefab (Button + ShopItemCard)
├── Background (Image - цвет редкости)
├── Icon (Image - иконка предмета)
├── NameText (TextMeshPro - название)
├── PriceText (TextMeshPro - цена)
└── StockText (TextMeshPro - остаток)
```

5. Назначьте ссылки в компоненте `ShopItemCard`:
   - Icon Image → Icon
   - Name Text → NameText
   - Price Text → PriceText
   - Stock Text → StockText
   - Card Button → сам Button компонент
   - Background Image → Background

6. Сохраните как Prefab

### Шаг 4: Настройка ShopManager

1. Создайте пустой GameObject, назовите "ShopManager"
2. Добавьте компонент `ShopManager`
3. Назначьте ссылки:

**Shop Items:**
- Перетащите созданные ShopItem ScriptableObjects в список

**UI References:**
- Shop UI → ShopPanel
- Prompt UI → PromptPanel
- Prompt Text → текст подсказки
- Item Cards Container → Content (из ScrollView)
- Item Card Prefab → созданный префаб карточки

**Detail Panel:**
- Detail Panel → панель с деталями
- Detail Icon → иконка в панели
- Detail Name → название
- Detail Description → описание
- Detail Price → цена
- Detail Stock → остаток
- Buy Button → кнопка покупки
- Buy Button Text → текст на кнопке

**Currency Display:**
- Currency Text → текст валюты в header

**Player Reference:**
- Player → объект игрока

### Шаг 5: Создание триггера магазина

1. Создайте 3D объект (например, Cube)
2. Назовите "ShopTrigger"
3. Добавьте компонент `ShopTrigger`
4. Настройте Collider:
   - Установите `Is Trigger` = true
   - Настройте размер зоны взаимодействия

5. В компоненте ShopTrigger:
   - Shop Manager → ссылка на ShopManager
   - Pause Game When Open → true/false (на ваш выбор)

6. Убедитесь, что у игрока есть тег "Player"

### Шаг 6: Визуальное оформление триггера (опционально)

Добавьте 3D модель киоска/лавки как child объект ShopTrigger:
```
ShopTrigger (с Collider)
└── ShopModel (визуальная модель)
```

## Типы предметов и эффектов

### Consumable (Расходники)
- **HealthRestore** - восстанавливает HP
  - Effect Value: количество HP для восстановления

### Upgrade (Улучшения)
- **MaxHealthUpgrade** - увеличивает максимальное здоровье
  - Effect Value: множитель (0.2 = +20%)

- **SpeedUpgrade** - увеличивает скорость передвижения
  - Effect Value: множитель (0.15 = +15%)

- **DamageUpgrade** - увеличивает урон
  - Effect Value: множитель (0.25 = +25%)

## Управление

- **E** - открыть магазин (в зоне триггера)
- **E / ESC** - закрыть магазин
- **ЛКМ** - выбрать предмет
- **Buy Button** - купить предмет

## Интеграция с существующими системами

### PlayerHealth
Убедитесь, что PlayerHealth имеет публичное поле `currentHealth`:
```csharp
public float currentHealth;
public float maxHealth = 100f;
```

### FPSInput (для Speed Upgrade)
Убедитесь, что скорости публичные:
```csharp
public float walkSpeed = 5f;
public float runSpeed = 10f;
```

### PlayerMovement_MaleWarrior (для Damage Upgrade)
Убедитесь, что урон публичный:
```csharp
public float damageAmount = 10f;
```

## Примеры предметов

### Health Potion (Малая)
```
ID: health_potion_small
Name: Small Health Potion
Price: 30
Stock: 10
Type: Consumable
Effect: HealthRestore (20 HP)
```

### Health Potion (Средняя)
```
ID: health_potion_medium
Name: Health Potion
Price: 50
Stock: 5
Type: Consumable
Effect: HealthRestore (50 HP)
```

### Speed Upgrade
```
ID: speed_upgrade_tier1
Name: Swift Boots
Price: 100
Stock: 1
Type: Upgrade
Effect: SpeedUpgrade (0.2 = +20%)
```

### Health Upgrade
```
ID: health_upgrade_tier1
Name: Iron Heart
Price: 150
Stock: 1
Type: Upgrade
Effect: MaxHealthUpgrade (0.3 = +30%)
```

### Damage Upgrade
```
ID: damage_upgrade_tier1
Name: Warrior's Strength
Price: 120
Stock: 1
Type: Upgrade
Effect: DamageUpgrade (0.25 = +25%)
```

## Тестирование

1. Создайте новую сцену
2. Добавьте игрока с всеми необходимыми компонентами
3. Создайте ShopManager
4. Создайте ShopTrigger
5. Создайте несколько ShopItem ScriptableObjects
6. Настройте UI
7. Запустите игру и проверьте:
   - Вход в триггер показывает подсказку
   - Нажатие E открывает магазин
   - Карточки отображаются корректно
   - Клик по карточке показывает детали
   - Покупка работает и применяет эффекты
   - Валюта обновляется
   - Закрытие магазина работает

## Решение проблем

**Магазин не открывается:**
- Проверьте, что у игрока есть тег "Player"
- Проверьте, что Collider триггера имеет Is Trigger = true
- Проверьте ссылку на ShopManager в ShopTrigger

**Предметы не отображаются:**
- Проверьте, что ShopItems добавлены в список в ShopManager
- Проверьте, что Item Card Prefab назначен
- Проверьте, что Container назначен правильно

**Покупка не работает:**
- Проверьте, что PlayerCurrency добавлен на игрока
- Проверьте, что достаточно Scrap
- Проверьте консоль на ошибки

**Эффекты не применяются:**
- Проверьте, что PlayerUpgrades добавлен на игрока
- Проверьте, что ссылка на Player в ShopManager установлена
- Проверьте, что соответствующие компоненты (FPSInput, etc.) есть на игроке

## Дополнительные возможности

### Сохранение между сценами
Добавьте DontDestroyOnLoad для сохранения валюты и улучшений:
```csharp
void Awake()
{
    if (Instance == null)
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    else
    {
        Destroy(gameObject);
    }
}
```

### Звуковые эффекты
Добавьте AudioSource и проигрывайте звуки при:
- Открытии магазина
- Покупке предмета
- Недостатке денег

### Анимации
Добавьте Animator для анимаций:
- Появление/исчезновение магазина
- Hover эффект на карточках
- Анимация покупки
