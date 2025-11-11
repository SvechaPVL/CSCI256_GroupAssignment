# ⚡ Quick Start - Полная настройка магазина за 3 минуты!

## 🎯 Ваша рабочая сцена: **Assets/Forest.unity**

---

## ✨ Автоматическая настройка (3 шага!)

### Шаг 1: Настройка игрока (30 секунд)

1. Откройте сцену **Forest.unity**
2. Найдите в Hierarchy: **MaleCharacterPBR** (ваш игрок)
3. Добавьте компонент **`AutoSetupPlayer`**
4. Нажмите **Play** (или ПКМ на компоненте → Setup Player (Force))
5. ✅ Готово! PlayerCurrency, PlayerInventory, PlayerUpgrades добавлены!

---

### Шаг 2: Создание предметов (30 секунд)

1. Unity меню: **Tools → Shop System → Create Shop Items**
2. Нажмите **"CREATE ALL ITEMS"**
3. ✅ Готово! Все 12 предметов созданы в `Assets/ScriptableObjects/ShopItems/`

---

### Шаг 3: Создание UI (30 секунд)

1. Unity меню: **Tools → Shop System → Create Shop UI**
2. Нажмите **"CREATE COMPLETE SHOP UI"**
3. ✅ Готово! Canvas, ShopPanel, PromptPanel, ItemCard созданы!

---

### Шаг 4: Настройка ShopManager (1 минута)

1. Создайте пустой GameObject: **ShopManager**
2. Добавьте компонент **`ShopManager`**
3. Назначьте ссылки:

**Available Items** (перетащите из Assets/ScriptableObjects/ShopItems/):
   - health_potion_small
   - health_potion
   - health_upgrade_tier1
   - speed_upgrade_tier1
   - damage_upgrade_tier1
   (или все 12 предметов)

**UI References:**
   - Shop UI: `Canvas/ShopPanel`
   - Prompt UI: `Canvas/PromptPanel`
   - Prompt Text: `Canvas/PromptPanel/PromptText`
   - Item Cards Container: `Canvas/ShopPanel/ItemScrollView/Viewport/Content`
   - Item Card Prefab: `Assets/Prefabs/Shop/ItemCardPrefab`

**Detail Panel:**
   - Detail Panel: `Canvas/ShopPanel/DetailPanel`
   - Detail Icon: (можете оставить пустым пока)
   - Detail Name: `Canvas/ShopPanel/DetailPanel/Container/ItemName`
   - Detail Description: `Canvas/ShopPanel/DetailPanel/Container/ItemDescription`
   - Detail Price: `Canvas/ShopPanel/DetailPanel/Container/ItemPrice`
   - Detail Stock: `Canvas/ShopPanel/DetailPanel/Container/ItemStock`
   - Buy Button: `Canvas/ShopPanel/DetailPanel/Container/BuyButton`
   - Buy Button Text: `Canvas/ShopPanel/DetailPanel/Container/BuyButton/Text`

**Currency Display:**
   - Currency Text: `Canvas/ShopPanel/Header/CurrencyText`

**Player Reference:**
   - Player: `MaleCharacterPBR`

---

### Шаг 5: Создание триггера магазина (30 секунд)

1. Создайте **3D Object → Cube**
2. Переименуйте в **ShopTrigger**
3. Position: поставьте рядом с игроком (например, X: 5, Y: 0, Z: 0)
4. Scale: увеличьте (например, 3, 3, 3)
5. Добавьте компонент **`ShopTrigger`**
6. В Inspector Box Collider → поставьте галочку **Is Trigger**
7. Назначьте **Shop Manager**: перетащите объект ShopManager
8. ✅ Готово!

---

### Шаг 6: БОНУС - Scrap за врагов (30 секунд)

1. Найдите врага в сцене (обычно "Enemy" или похожее имя)
2. Добавьте компонент **`ScrapDropper`**
3. Настройте награду:
   - Min Scrap: 5
   - Max Scrap: 15
4. ✅ Теперь враги будут давать Scrap при смерти!

---

## 🎮 Тестирование

1. Нажмите **Play**
2. Подойдите к кубу триггера
3. Увидите **"[E] Open Shop"**
4. Нажмите **E**
5. Магазин откроется!
6. Кликните на предмет
7. Нажмите **Buy**
8. Проверьте эффект!

---

## 🐛 Возможные проблемы

**Магазин не открывается:**
- Проверьте что у MaleCharacterPBR тег = "Player"
- Проверьте что Box Collider имеет Is Trigger = true

**Предметы не видны:**
- Проверьте что вы добавили ShopItems в Available Items
- Проверьте что Item Card Prefab назначен

**Покупка не работает:**
- Проверьте что AutoSetupPlayer отработал (должны быть компоненты на игроке)
- Проверьте что Player назначен в ShopManager

**UI не видно:**
- Проверьте что Canvas в режиме Screen Space - Overlay
- Проверьте что ShopPanel и PromptPanel изначально неактивны

---

## 📊 Что создано

### Игрок (MaleCharacterPBR):
- ✅ PlayerCurrency (Scrap: 100)
- ✅ PlayerInventory
- ✅ PlayerUpgrades
- ✅ PlayerHealth (уже был)
- ✅ FPSInput (уже был)
- ✅ PlayerMovement_MaleWarrior (уже был)

### Предметы (12 штук):
- ✅ 3 Health Potions
- ✅ 3 Health Upgrades
- ✅ 3 Speed Upgrades
- ✅ 3 Damage Upgrades

### UI:
- ✅ Canvas
- ✅ ShopPanel с header, scroll view, detail panel
- ✅ PromptPanel
- ✅ Item Card Prefab

### Менеджеры:
- ✅ ShopManager (настроить вручную)
- ✅ ShopTrigger (создать вручную)

---

## 🎯 Готовая система!

**Поздравляю!** Теперь у вас полностью рабочая система магазина!

Игрок может:
- 🛒 Подходить к магазину и открывать его нажатием E
- 👀 Просматривать доступные предметы
- 💰 Покупать расходники (Health Potions) и улучшения
- ⚡ Получать постоянные бонусы к здоровью, скорости и урону
- 🎮 Зарабатывать Scrap убивая врагов

---

## 📚 Дополнительно

### Добавить больше предметов:
- Tools → Shop System → Create Shop Items
- Создайте отдельные предметы кнопками

### Настроить внешний вид UI:
- Выберите элементы UI в Hierarchy
- Измените цвета, размеры, шрифты в Inspector

### Добавить иконки для предметов:
- Создайте Sprite иконки
- Назначьте их в ShopItem ScriptableObjects

### Изменить стартовый Scrap:
- AutoSetupPlayer → Starting Scrap: [ваше значение]

### Добавить звуки:
- Добавьте AudioSource на ShopManager
- Проигрывайте звуки в методах ShopManager

---

**Удачи с вашей игрой! 🎮**
