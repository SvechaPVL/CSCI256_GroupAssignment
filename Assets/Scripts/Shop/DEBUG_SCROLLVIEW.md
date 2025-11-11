# 🔍 Диагностика: Карточки не видны в ScrollView

## Проблема
Префаб ItemCard выглядит нормально сам по себе, но карточки не отображаются в ScrollView (Content контейнере) магазина.

---

## ✅ Шаг 1: Запустите игру и проверьте консоль

### Что делать:
1. Откройте Unity Console (Window → General → Console)
2. Запустите игру (Play)
3. Откройте магазин (нажмите E возле триггера)

### Что искать в консоли:

Вы должны увидеть эти сообщения:
```
[ShopManager] CreateItemCards called
[ShopManager] Available items count: X
[ShopManager] Creating card for item: Health Potion
[ShopManager] Card created: ItemCardPrefab(Clone), Active: True, Parent: Content
[ShopManager] Card setup complete for: Health Potion
...
[ShopManager] Total cards created: X
```

### Диагностика по сообщениям:

#### ❌ Если видите: `itemCardsContainer is NULL!`
**Проблема:** ShopManager → Item Cards Container не назначен

**Решение:**
```
1. Hierarchy → ShopManager
2. Inspector → Item Cards Container
3. Перетащите: Canvas/ShopPanel/ItemScrollView/Viewport/Content
```

#### ❌ Если видите: `itemCardPrefab is NULL!`
**Проблема:** ShopManager → Item Card Prefab не назначен

**Решение:**
```
1. Hierarchy → ShopManager
2. Inspector → Item Card Prefab
3. Перетащите: Assets/Prefabs/Shop/ItemCardPrefab
```

#### ❌ Если видите: `Available items count: 0`
**Проблема:** Нет предметов в магазине

**Решение:**
```
1. Hierarchy → ShopManager
2. Inspector → Available Items
3. Size: установите 5-12
4. Перетащите ShopItem'ы из Assets/ScriptableObjects/ShopItems/
```

#### ✅ Если все сообщения есть, но карточек не видно
Переходите к Шагу 2!

---

## ✅ Шаг 2: Проверьте Hierarchy в Runtime

### Что делать:
1. Запустите игру (Play)
2. Откройте магазин (E)
3. В Hierarchy разверните:
   ```
   Canvas
   └── ShopPanel
       └── ItemScrollView
           └── Viewport
               └── Content
                   ├── ItemCardPrefab(Clone)  ← ДОЛЖНЫ БЫТЬ ЗДЕСЬ!
                   ├── ItemCardPrefab(Clone)
                   └── ItemCardPrefab(Clone)
   ```

### Диагностика:

#### ❌ Если клонов нет в Content
Вернитесь к Шагу 1 и проверьте консоль

#### ✅ Если клоны ЕСТЬ, но не видны
Выберите один клон в Hierarchy и проверьте Inspector:

**Проверка 1: GameObject активен?**
- Галочка слева от названия должна быть ✅
- Если нет - это баг, сообщите мне

**Проверка 2: RectTransform**
```
Width: 400 (или другая ширина)
Height: 100 (или другая высота)
Anchors: не должны быть все в одной точке
Scale: (1, 1, 1)
```

**Проверка 3: Image Component**
```
Color: RGB (51, 77, 102, 255) - тёмно-синий
Alpha: 255 (полностью непрозрачный!)
```

---

## ✅ Шаг 3: Проверьте Content контейнер

### В Hierarchy выберите: Canvas/ShopPanel/ItemScrollView/Viewport/Content

### Inspector должен показывать:

**RectTransform:**
```
Anchor Min: (0, 1)
Anchor Max: (1, 1)
Pivot: (0.5, 1)
Anchored Position: (0, 0)
Size Delta: (0, HEIGHT) ← HEIGHT должен расти при добавлении карточек!
```

**Vertical Layout Group:**
```
✅ Spacing: 10
✅ Padding: 10, 10, 10, 10
✅ Child Control Width: ✓
✅ Child Force Expand Width: ✓
✅ Child Control Height: ✗
✅ Child Force Expand Height: ✗
```

**Content Size Fitter:**
```
Horizontal Fit: Unconstrained
Vertical Fit: Preferred Size  ← ВАЖНО!
```

### Диагностика:

#### ❌ Если Height = 0
**Проблема:** Content Size Fitter не работает

**Решение:**
```
1. Выберите Content
2. Inspector → Content Size Fitter
3. Vertical Fit → Preferred Size
4. Если уже стоит, удалите компонент и добавьте заново:
   - Remove Component
   - Add Component → Layout → Content Size Fitter
   - Vertical Fit: Preferred Size
```

#### ❌ Если Child Control Height = ✓
**Проблема:** Layout Group управляет высотой детей

**Решение:**
```
1. Выберите Content
2. Vertical Layout Group
3. Child Control Height: снимите галочку
4. Child Force Expand Height: снимите галочку
```

---

## ✅ Шаг 4: Проверьте ItemCard Prefab

### Откройте префаб:
```
Assets/Prefabs/Shop/ItemCardPrefab.prefab
```

### Проверьте корневой объект:

**RectTransform:**
```
Width: 400
Height: 100
```

**НЕ должно быть Layout Element!**
Если есть компонент "Layout Element":
```
1. Если есть → Remove Component
2. Сохраните префаб (Ctrl+S)
```

**Image Component:**
```
Color: RGB (51, 77, 102, 255)
Alpha: 255
Raycast Target: ✓ (для кнопки)
```

---

## ✅ Шаг 5: Проверьте Viewport и Mask

### В Hierarchy выберите: Canvas/ShopPanel/ItemScrollView/Viewport

**RectTransform:**
```
Anchor Min: (0, 0)
Anchor Max: (1, 1)
Size Delta: (0, 0)
```

**Mask Component:**
```
Show Mask Graphic: ✗ (выключено)
```

**Image Component:**
```
Color Alpha: 0 (прозрачный, это нормально)
```

### Диагностика:

#### Попробуйте временно отключить Mask:
```
1. Выберите Viewport
2. Inspector → Mask Component
3. Снимите галочку с компонента (disable)
4. Проверьте - видны ли карточки теперь?
```

Если карточки появились - проблема в размере Viewport!

---

## ✅ Шаг 6: Проверьте ScrollView размеры

### В Hierarchy выберите: Canvas/ShopPanel/ItemScrollView

**RectTransform:**
```
Anchor Min: (0, 0)
Anchor Max: (0.6, 1)
Anchored Position: (50, -50)
Size Delta: (-100, -150)
```

Это должно дать ScrollView правильный размер слева от экрана.

### Визуальная проверка:

В Scene View (когда игра запущена):
1. Выберите ItemScrollView
2. Должен быть виден синий прямоугольник слева от экрана
3. Если прямоугольника нет или он крошечный - проблема в размерах!

---

## 🔧 Быстрое исправление (если ничего не помогло)

### Вариант 1: Пересоздайте UI полностью

```
1. Удалите Canvas в Hierarchy
2. Unity → Tools → Shop System → Create Shop UI
3. Нажмите: "CREATE COMPLETE SHOP UI"
4. Переназначьте ссылки в ShopManager
5. Попробуйте снова
```

### Вариант 2: Добавьте LayoutElement на ItemCard

Откройте `Assets/Prefabs/Shop/ItemCardPrefab.prefab`:

```
1. Выберите корневой объект (ItemCard)
2. Add Component → Layout → Layout Element
3. Настройте:
   - Preferred Height: 100
   - ✓ галочка слева от Preferred Height
4. Сохраните префаб
5. Перезапустите игру
```

### Вариант 3: Принудительно обновите Layout

Добавьте в `ShopManager.cs` после создания карточек:

```csharp
// В методе CreateItemCards() после цикла foreach
Canvas.ForceUpdateCanvases();
LayoutRebuilder.ForceRebuildLayoutImmediate(itemCardsContainer);
```

Полный код:
```csharp
private void CreateItemCards()
{
    // ... существующий код ...

    Debug.Log($"[ShopManager] Total cards created: {itemCards.Count}");

    // ДОБАВЬТЕ ЭТИ СТРОКИ:
    Canvas.ForceUpdateCanvases();
    UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(itemCardsContainer);
}
```

---

## 📊 Чеклист диагностики

Пройдитесь по этому списку:

- [ ] Console показывает "CreateItemCards called"
- [ ] Console показывает правильное количество Available items
- [ ] Console показывает "Card created" для каждого предмета
- [ ] Console НЕ показывает ошибок NULL
- [ ] В Hierarchy есть ItemCardPrefab(Clone) внутри Content
- [ ] Клоны активны (галочка в Hierarchy)
- [ ] Content имеет ContentSizeFitter с VerticalFit = PreferredSize
- [ ] Content имеет VerticalLayoutGroup
- [ ] Content Size Delta Height > 0
- [ ] ItemCard префаб имеет Width=400, Height=100
- [ ] ItemCard Image Color Alpha = 255
- [ ] Viewport имеет Mask компонент
- [ ] ScrollView имеет правильные anchors и size
- [ ] ShopManager → Available Items заполнен
- [ ] ShopManager → Item Cards Container назначен
- [ ] ShopManager → Item Card Prefab назначен

Если все пункты ✅ но карточки всё равно не видны - напишите мне какие именно сообщения видите в Console!

---

## 💡 Дополнительная диагностика

### Проверка через Scene View:

1. Запустите игру (Play)
2. Откройте магазин (E)
3. Переключитесь на Scene View
4. Выберите Content в Hierarchy
5. Нажмите F (Focus) - камера направится на Content
6. **Видите ли вы карточки в Scene View?**

**Если ДА** - проблема в Camera или Canvas настройках
**Если НЕТ** - карточки действительно не создаются или невидимы

### Проверка RectTransform позиций карточек:

В Runtime выберите ItemCardPrefab(Clone) в Hierarchy:
```
Anchored Position: (0, -Y)
где Y увеличивается для каждой карточки (0, -110, -220, и т.д.)
```

Если все карточки имеют одинаковую позицию (0, 0) - LayoutGroup не работает!

---

**После диагностики сообщите мне результаты и я помогу исправить проблему!** 🔧
