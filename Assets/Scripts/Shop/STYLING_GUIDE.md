# 🎨 Гайд по стилизации магазина - Текстуры и Звуки

Полная инструкция по добавлению визуальных и звуковых эффектов в систему магазина.

---

## 📋 Содержание

1. [Иконки для предметов](#-1-иконки-для-предметов)
2. [UI текстуры и фоны](#-2-ui-текстуры-и-фоны)
3. [Звуковые эффекты](#-3-звуковые-эффекты)
4. [Рекомендуемые Asset Store паки](#-4-рекомендуемые-asset-store-паки)
5. [Пошаговая стилизация UI](#-5-пошаговая-стилизация-ui)

---

## 🖼️ 1. Иконки для предметов

### Где взять иконки?

**Вариант A: Unity Asset Store (РЕКОМЕНДУЕТСЯ)**

Бесплатные паки с иконками для RPG/Shop систем:

1. **[Game Icons](https://assetstore.unity.com/packages/2d/gui/icons/game-icons-207558)** - 4000+ бесплатных иконок
2. **[Fantasy Icon Pack](https://assetstore.unity.com/packages/2d/gui/icons/fantasy-icon-mega-pack-1-94835)** - иконки зелий, оружия, брони
3. **[RPG Inventory Icons](https://assetstore.unity.com/packages/2d/gui/icons/rpg-inventory-icons-56687)** - специально для инвентаря

**Вариант B: Бесплатные онлайн ресурсы:**

- **[game-icons.net](https://game-icons.net/)** - 4000+ SVG иконок (конвертируйте в PNG)
- **[Kenney Assets](https://kenney.nl/assets)** - бесплатные игровые ресурсы
- **[itch.io](https://itch.io/game-assets/free/tag-icons)** - бесплатные паки

### Подготовка иконок

1. **Формат**: PNG с прозрачностью (alpha channel)
2. **Размер**: 128x128 или 256x256 пикселей
3. **Стиль**: желательно одинаковый для всех иконок

### Установка иконок в Unity

**Шаг 1: Импорт иконок**

```
1. Создайте папку: Assets/Textures/ShopIcons/
2. Скопируйте туда PNG файлы
3. В Unity они импортируются автоматически
```

**Шаг 2: Настройка импорта**

```
1. Выберите иконку в Project
2. Inspector → Texture Type: Sprite (2D and UI)
3. Sprite Mode: Single
4. Pixels Per Unit: 100
5. Filter Mode: Bilinear
6. Compression: None или Normal Quality
7. Нажмите "Apply"
```

**Шаг 3: Назначение иконок предметам**

```
1. Откройте ScriptableObject предмета (например, health_potion.asset)
2. Найдите поле "Item Icon"
3. Перетащите спрайт из Assets/Textures/ShopIcons/
4. Повторите для всех предметов
```

**Рекомендуемые иконки для предметов:**

| Предмет | Рекомендуемая иконка |
|---------|---------------------|
| Health Potion | Красная бутылочка/флакон |
| Health Upgrade | Красное сердце |
| Speed Upgrade | Ботинки с крыльями/молнией |
| Damage Upgrade | Меч/кулак/огонь |

**Шаг 4: Отображение иконок в UI**

Если иконка не отображается в магазине:

```
1. Откройте префаб: Assets/Prefabs/Shop/ItemCardPrefab.prefab
2. Найдите объект "ItemIcon" (Image component)
3. В скрипте ShopItemCard.cs проверьте:
   - Есть ли public Image itemIcon;
   - В методе Setup() есть ли: itemIcon.sprite = shopItem.itemIcon;
```

Если нужно добавить иконку в карточку вручную:

```
1. Откройте ItemCardPrefab
2. Добавьте дочерний объект: UI → Image
3. Назовите "ItemIcon"
4. Настройте:
   - Width: 50, Height: 50
   - Anchor: Top-Left
5. В ShopItemCard компоненте назначьте ссылку
```

---

## 🎨 2. UI текстуры и фоны

### Фоны для панелей

**Вариант A: Unity UI Default**

Используйте встроенные спрайты Unity:
```
1. Выберите панель (например, ShopPanel)
2. Image (Script) → Source Image
3. Выберите: UI/Skin/UISprite или UI/Skin/Background
4. Color: настройте цвет и прозрачность
```

**Вариант B: Кастомные текстуры из Asset Store**

Рекомендуемые паки:

1. **[Fantasy Wooden GUI](https://assetstore.unity.com/packages/2d/gui/fantasy-wooden-gui-free-103811)** - БЕСПЛАТНО
2. **[Simple UI](https://assetstore.unity.com/packages/2d/gui/icons/simple-ui-166588)** - минималистичный стиль
3. **[Medieval UI Pack](https://assetstore.unity.com/packages/2d/gui/medieval-ui-pack-154113)** - средневековый стиль

### Установка кастомных фонов

**Импорт:**
```
1. Скачайте пак из Asset Store
2. Window → Package Manager → My Assets
3. Найдите пак → Download → Import
4. Выберите нужные текстуры → Import
```

**Применение:**
```
1. Выберите UI панель (ShopPanel, DetailPanel и т.д.)
2. Image Component → Source Image
3. Перетащите текстуру из импортированного пака
4. Image Type: Sliced (для растягивания без искажений)
5. Настройте Fill Center, если нужно
```

### Фон для карточек предметов

```
1. Откройте ItemCardPrefab
2. Выберите корневой объект (с Button компонентом)
3. Image → Source Image: назначьте текстуру
4. Настройте цвета для разных состояний:
   - Normal Color: базовый цвет
   - Highlighted Color: при наведении (светлее)
   - Pressed Color: при нажатии (темнее)
   - Selected Color: когда выбран
```

### Цветовая схема магазина

Рекомендуемые настройки для профессионального вида:

**ShopPanel (главная панель):**
```
Background Color: #2C2C2C (темно-серый)
Alpha: 0.95 (почти непрозрачный)
```

**Header (заголовок):**
```
Background Color: #1A1A1A (очень темный)
Title Text Color: #FFD700 (золотой)
Font Size: 36
```

**ItemCard (карточка предмета):**
```
Normal: #3A3A3A
Highlighted: #4A4A4A
Pressed: #2A2A2A
Selected: #5A5A2A (желтоватый оттенок)
```

**DetailPanel:**
```
Background Color: #252525
Border: можно добавить Outline компонент (золотой цвет)
```

**Buttons (кнопки):**
```
Normal: #4CAF50 (зеленый для Buy)
Highlighted: #66BB6A
Pressed: #388E3C
Text Color: #FFFFFF (белый)
Font Size: 20
```

---

## 🔊 3. Звуковые эффекты

### Где взять звуки?

**Unity Asset Store (БЕСПЛАТНЫЕ):**

1. **[Free Sound Effects Pack](https://assetstore.unity.com/packages/audio/sound-fx/free-sound-effects-pack-155776)**
2. **[Universal Sound FX](https://assetstore.unity.com/packages/audio/sound-fx/universal-sound-fx-17256)**
3. **[8-Bit Sound FX](https://assetstore.unity.com/packages/audio/sound-fx/8-bit-sfx-32831)** - ретро стиль

**Бесплатные онлайн ресурсы:**

- **[Freesound.org](https://freesound.org/)** - огромная библиотека (требуется регистрация)
- **[Zapsplat.com](https://www.zapsplat.com/)** - бесплатные SFX
- **[Mixkit.co](https://mixkit.co/free-sound-effects/)** - качественные звуки

### Необходимые звуки для магазина

| Действие | Тип звука | Пример |
|----------|-----------|--------|
| Открытие магазина | Мягкий "whoosh" или звон монет | shop_open.wav |
| Закрытие магазина | Короткий "close" звук | shop_close.wav |
| Выбор предмета | Клик / tap | item_select.wav |
| Успешная покупка | Звон монет / cash register | purchase_success.wav |
| Недостаточно денег | Негативный buzz | purchase_fail.wav |
| Наведение на предмет | Тихий hover звук | item_hover.wav |

### Подготовка звуков

**Формат:**
- WAV или MP3
- Mono или Stereo
- Короткие (0.2-2 секунды для UI звуков)

**Импорт в Unity:**
```
1. Создайте папку: Assets/Audio/Shop/
2. Скопируйте туда аудио файлы
3. Unity импортирует автоматически
```

**Настройка импорта:**
```
1. Выберите аудио файл в Project
2. Inspector настройки:
   - Force To Mono: ✓ (для UI звуков)
   - Load In Background: ✗
   - Preload Audio Data: ✓
   - Compression Format: Vorbis
   - Quality: 100%
   - Sample Rate Setting: Preserve Sample Rate
3. Apply
```

### Добавление звуков в ShopManager

**Шаг 1: Добавьте поля в ShopManager.cs**

Откройте `Assets/Scripts/Shop/ShopManager.cs` и добавьте после существующих полей:

```csharp
[Header("Audio Settings")]
[SerializeField] private AudioSource audioSource;
[SerializeField] private AudioClip shopOpenSound;
[SerializeField] private AudioClip shopCloseSound;
[SerializeField] private AudioClip itemSelectSound;
[SerializeField] private AudioClip purchaseSuccessSound;
[SerializeField] private AudioClip purchaseFailSound;
[SerializeField] private AudioClip itemHoverSound;
```

**Шаг 2: Добавьте метод для проигрывания звуков**

```csharp
private void PlaySound(AudioClip clip)
{
    if (audioSource != null && clip != null)
    {
        audioSource.PlayOneShot(clip);
    }
}
```

**Шаг 3: Добавьте вызовы звуков в существующие методы**

В методе `OpenShop()`:
```csharp
public void OpenShop()
{
    shopUI.SetActive(true);
    PlaySound(shopOpenSound); // ← ДОБАВИТЬ ЭТУ СТРОКУ
    PopulateShop();
    // ... остальной код
}
```

В методе `CloseShop()`:
```csharp
public void CloseShop()
{
    PlaySound(shopCloseSound); // ← ДОБАВИТЬ ЭТУ СТРОКУ
    shopUI.SetActive(false);
    // ... остальной код
}
```

В методе `SelectItem()`:
```csharp
public void SelectItem(ShopItem item)
{
    PlaySound(itemSelectSound); // ← ДОБАВИТЬ ЭТУ СТРОКУ
    selectedItem = item;
    // ... остальной код
}
```

В методе `BuySelectedItem()` после успешной покупки:
```csharp
public void BuySelectedItem()
{
    // ... проверки ...

    // После currency.RemoveScrap(selectedItem.price);
    PlaySound(purchaseSuccessSound); // ← ДОБАВИТЬ

    // ... остальной код
}
```

В методе `BuySelectedItem()` при недостаточных средствах:
```csharp
if (currency.currentScrap < selectedItem.price)
{
    PlaySound(purchaseFailSound); // ← ДОБАВИТЬ
    Debug.Log("Not enough Scrap!");
    return;
}
```

**Шаг 4: Добавьте AudioSource в сцену**

```
1. Выберите GameObject "ShopManager" в Hierarchy
2. Add Component → Audio → Audio Source
3. Настройки:
   - Play On Awake: ✗ (выключить)
   - Loop: ✗ (выключить)
   - Volume: 0.7
   - Spatial Blend: 0 (2D звук)
4. Назначьте AudioSource в поле "Audio Source" компонента ShopManager
```

**Шаг 5: Назначьте звуковые клипы**

```
1. Выберите ShopManager в Hierarchy
2. В Inspector найдите "Audio Settings"
3. Перетащите аудио клипы из Assets/Audio/Shop/:
   - Shop Open Sound: shop_open.wav
   - Shop Close Sound: shop_close.wav
   - Item Select Sound: item_select.wav
   - Purchase Success Sound: purchase_success.wav
   - Purchase Fail Sound: purchase_fail.wav
   - Item Hover Sound: item_hover.wav (опционально)
```

### Звук при наведении на предмет (БОНУС)

Для добавления hover звука нужно модифицировать `ShopItemCard.cs`:

```csharp
// В класс ShopItemCard добавить:
private ShopManager shopManager;

void Start()
{
    shopManager = FindObjectOfType<ShopManager>();
}

// Добавить в Button компонент через EventTrigger:
public void OnPointerEnter()
{
    // Вызвать метод PlayHoverSound() в ShopManager
}
```

Или проще - использовать Animation/Audio в Button → Navigation → Highlighted.

---

## 📦 4. Рекомендуемые Asset Store паки

### Комплексные UI паки (все в одном)

**БЕСПЛАТНЫЕ:**

1. **[Fantasy Wooden GUI: Free](https://assetstore.unity.com/packages/2d/gui/fantasy-wooden-gui-free-103811)**
   - Средневековый деревянный стиль
   - Панели, кнопки, фоны
   - + 100 иконок предметов

2. **[Clean & Minimalist GUI Pack](https://assetstore.unity.com/packages/2d/gui/clean-minimalist-gui-pack-75123)**
   - Современный минималистичный стиль
   - Чистый дизайн

3. **[Sci-Fi UI Pack](https://assetstore.unity.com/packages/2d/gui/sci-fi-ui-pack-112063)**
   - Футуристический стиль
   - Подходит для sci-fi игр

**ПЛАТНЫЕ (но качественные):**

4. **[Modern UI Pack](https://assetstore.unity.com/packages/2d/gui/modern-ui-pack-201717)** - $15
   - Профессиональный современный дизайн
   - Анимации включены

5. **[RPG Medieval UI](https://assetstore.unity.com/packages/2d/gui/rpg-medieval-ui-154113)** - $20
   - Полный набор для RPG
   - Иконки + UI элементы

### Только иконки

**БЕСПЛАТНЫЕ:**

1. **[700+ RPG Icons](https://assetstore.unity.com/packages/2d/gui/icons/700-rpg-icons-122417)**
2. **[100 Game Icons](https://assetstore.unity.com/packages/2d/gui/icons/100-game-icons-207792)**

### Только звуки

**БЕСПЛАТНЫЕ:**

1. **[Free Sound Effects Pack](https://assetstore.unity.com/packages/audio/sound-fx/free-sound-effects-pack-155776)** - 100+ звуков
2. **[8-Bit Sound Effects](https://assetstore.unity.com/packages/audio/sound-fx/8-bit-sfx-32831)** - ретро
3. **[Interface SFX](https://assetstore.unity.com/packages/audio/sound-fx/interface-sfx-48989)** - UI звуки

---

## 🎯 5. Пошаговая стилизация UI

### Полная стилизация магазина за 15 минут

**ШАГ 1: Импорт ресурсов (5 минут)**

```
1. Asset Store → Fantasy Wooden GUI: Free → Download → Import
2. Asset Store → 700+ RPG Icons → Download → Import
3. Asset Store → Free Sound Effects Pack → Download → Import
4. Готово! Ресурсы импортированы
```

**ШАГ 2: Стилизация ShopPanel (3 минуты)**

```
1. Выберите Canvas/ShopPanel
2. Image → Source Image → Fantasy Wooden GUI → panel_large
3. Image Type: Sliced
4. Color: #FFFFFF (белый, чтобы текстура была в оригинальном цвете)

5. Header/TitleText:
   - Font: Arial Bold или импортированный шрифт
   - Font Size: 40
   - Color: #FFD700 (золотой)
   - Alignment: Center

6. Header/CurrencyText:
   - Font Size: 24
   - Color: #FFE57F (светло-желтый)
   - Добавьте иконку монеты (UI → Image рядом с текстом)
```

**ШАГ 3: Стилизация ItemCard (3 минуты)**

```
1. Откройте Assets/Prefabs/Shop/ItemCardPrefab
2. Корневой объект (Button):
   - Image → Source Image → Fantasy Wooden GUI → button_normal
   - Image Type: Sliced
   - Button Colors:
     - Normal: #E8D4B0
     - Highlighted: #F5E6D3
     - Pressed: #D4C5A7
     - Selected: #FFE57F

3. Добавьте иконку:
   - Добавьте UI → Image как дочерний объект
   - Назовите "ItemIcon"
   - Width: 60, Height: 60
   - Позиция: слева в карточке
   - В ShopItemCard назначьте ссылку

4. Настройте текст:
   - ItemName: Font Size 18, Bold, Color #3E2723 (темно-коричневый)
   - ItemPrice: Font Size 16, Color #FFB300 (золотой)
```

**ШАГ 4: Стилизация DetailPanel (2 минуты)**

```
1. Выберите Canvas/ShopPanel/DetailPanel
2. Image → Source Image → Fantasy Wooden GUI → panel_medium
3. Добавьте Outline компонент:
   - Effect Color: #FFD700 (золотой)
   - Effect Distance: (2, -2)

4. BuyButton:
   - Source Image → Fantasy Wooden GUI → button_green
   - Text: "BUY" (крупными буквами)
   - Font Size: 24
   - Color: #FFFFFF
```

**ШАГ 5: Назначение иконок предметам (2 минуты)**

```
1. Откройте Assets/ScriptableObjects/ShopItems/health_potion.asset
2. Item Icon → перетащите иконку зелья из импортированного пака
3. Повторите для всех 12 предметов:
   - Health Potions → красные флаконы
   - Health Upgrades → сердца
   - Speed Upgrades → ботинки/крылья
   - Damage Upgrades → мечи/кулаки
```

**ШАГ 6: Добавление звуков (см. раздел 3)**

```
Следуйте инструкциям из раздела "Звуковые эффекты" выше
```

---

## ✅ Чеклист стилизации

- [ ] Импортированы UI текстуры из Asset Store
- [ ] Импортированы иконки предметов
- [ ] Импортированы звуковые эффекты
- [ ] ShopPanel имеет красивый фон
- [ ] Header стилизован (заголовок + валюта)
- [ ] ItemCard имеет фон и правильные цвета кнопки
- [ ] Иконки назначены всем предметам в ScriptableObjects
- [ ] DetailPanel стилизован
- [ ] BuyButton имеет привлекательный вид
- [ ] AudioSource добавлен на ShopManager
- [ ] Все звуковые клипы назначены
- [ ] Звуки работают (протестировано в игре)
- [ ] Шрифты изменены на более красивые (опционально)

---

## 🎨 Дополнительные улучшения

### Анимации UI (ADVANCED)

Добавьте плавные анимации открытия/закрытия:

```
1. Выберите ShopPanel
2. Add Component → Animator
3. Создайте Animation Clip:
   - Shop_Open (scale от 0 до 1 за 0.3 сек)
   - Shop_Close (scale от 1 до 0 за 0.2 сек)
4. В ShopManager.cs вызывайте animator.SetTrigger("Open/Close")
```

### Частицы при покупке

```
1. Создайте Particle System над ShopPanel
2. Настройте золотые искры
3. Play On Awake: ✗
4. В ShopManager.cs после покупки: particleSystem.Play();
```

### Кастомные шрифты

```
1. Скачайте шрифт (.ttf) например с Google Fonts
2. Assets/Fonts/ → скопируйте .ttf файл
3. Выберите Text компоненты
4. Font: перетащите импортированный шрифт
```

**Рекомендуемые шрифты:**
- **Cinzel** - средневековый/фэнтези стиль
- **Orbitron** - sci-fi стиль
- **Bebas Neue** - современный жирный стиль

---

## 💡 Советы по дизайну

1. **Цветовая согласованность**: используйте одну цветовую палитру для всего UI
2. **Контраст**: важный текст должен выделяться на фоне
3. **Иерархия**: размер шрифта показывает важность (заголовок > подзаголовок > текст)
4. **Spacing**: оставляйте пространство между элементами (padding/margin)
5. **Единообразие**: все кнопки/карточки должны выглядеть в одном стиле

### Цветовые схемы (примеры)

**Фэнтези/Средневековье:**
```
Primary: #8B4513 (коричневый)
Secondary: #FFD700 (золотой)
Background: #2C1810 (темно-коричневый)
Text: #F5DEB3 (пшеничный)
```

**Sci-Fi:**
```
Primary: #00BCD4 (cyan)
Secondary: #FF4081 (pink)
Background: #0A0E27 (темно-синий)
Text: #FFFFFF (белый)
```

**Минимализм:**
```
Primary: #2196F3 (синий)
Secondary: #4CAF50 (зеленый)
Background: #FAFAFA (светло-серый)
Text: #212121 (темно-серый)
```

---

## 📚 Дополнительные ресурсы

### Где учиться UI дизайну:

- **[Unity UI Best Practices](https://learn.unity.com/tutorial/ui-best-practices)** - официальный туториал Unity
- **[Game UI Database](https://www.gameuidatabase.com/)** - примеры UI из игр
- **[Brackeys UI Tutorial](https://www.youtube.com/watch?v=wbmjturGbAQ)** - YouTube туториал

### Инструменты для создания UI:

- **Figma** (бесплатно) - дизайн UI макетов
- **Photoshop** - создание текстур
- **Aseprite** - пиксель-арт иконки

---

**Готово! Теперь ваш магазин выглядит профессионально! 🎮✨**
