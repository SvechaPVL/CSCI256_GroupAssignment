#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Linq;

/// <summary>
/// ПОЛНОЕ ИСПРАВЛЕНИЕ СИСТЕМЫ МАГАЗИНА
/// Удаляет всё старое, создаёт новое, автоматически назначает все shop items
/// </summary>
public class FixShopSystem : EditorWindow
{
    [MenuItem("Tools/Shop System/🔥 FIX EVERYTHING 🔥")]
    public static void ShowWindow()
    {
        GetWindow<FixShopSystem>("Fix Shop System");
    }

    private void OnGUI()
    {
        GUILayout.Label("ПОЛНОЕ ИСПРАВЛЕНИЕ МАГАЗИНА", EditorStyles.boldLabel);
        GUILayout.Space(10);

        EditorGUILayout.HelpBox(
            "Этот скрипт:\n\n" +
            "✅ УДАЛИТ весь старый UI\n" +
            "✅ ПЕРЕСОЗДАСТ всё с нуля\n" +
            "✅ НАСТРОИТ игрока (Currency, Inventory, Upgrades)\n" +
            "✅ АВТОНАЗНАЧИТ все shop items\n" +
            "✅ ИСПРАВИТ проблему с карточками\n" +
            "✅ НАСТРОИТ все ссылки автоматически\n\n" +
            "НАЖМИТЕ КНОПКУ НИЖЕ!", MessageType.Warning);

        GUILayout.Space(20);

        GUI.backgroundColor = new Color(1f, 0.2f, 0.2f);
        if (GUILayout.Button("🔥 УДАЛИТЬ И ПЕРЕСОЗДАТЬ ВСЁ 🔥", GUILayout.Height(60)))
        {
            if (EditorUtility.DisplayDialog("Вы уверены?",
                "Это удалит весь UI магазина и пересоздаст всё заново!\n\n" +
                "Продолжить?",
                "ДА, ИСПРАВИТЬ ВСЁ",
                "Отмена"))
            {
                FixEverything();
            }
        }
        GUI.backgroundColor = Color.white;
    }

    private void FixEverything()
    {
        Debug.Log("========================================");
        Debug.Log("🔥 НАЧИНАЕМ ПОЛНОЕ ИСПРАВЛЕНИЕ МАГАЗИНА");
        Debug.Log("========================================");

        // ШАГ 1: Удаляем всё старое
        DeleteOldUI();

        // ШАГ 2: Создаём новый UI с правильными настройками
        CreateNewUI();

        // ШАГ 3: Настраиваем компоненты на игрока
        SetupPlayerComponents();

        // ШАГ 4: Находим или создаём ShopManager
        ShopManager manager = SetupShopManager();

        // ШАГ 5: Автоматически находим и назначаем все shop items
        AssignAllShopItems(manager);

        // ШАГ 6: Настраиваем ShopTrigger
        SetupShopTrigger(manager);

        // ШАГ 7: Сохраняем всё
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("========================================");
        Debug.Log("✅ ИСПРАВЛЕНИЕ ЗАВЕРШЕНО!");
        Debug.Log("========================================");

        EditorUtility.DisplayDialog("Успех! 🎉",
            "Магазин полностью пересоздан!\n\n" +
            "✅ UI создан\n" +
            "✅ Игрок настроен (Currency, Inventory, Upgrades)\n" +
            "✅ ShopManager настроен\n" +
            "✅ Shop Items назначены\n" +
            "✅ ShopTrigger готов\n\n" +
            "Запустите игру и проверьте магазин!",
            "Отлично!");

        // Выбираем ShopManager в иерархии
        Selection.activeGameObject = manager.gameObject;
    }

    private void DeleteOldUI()
    {
        Debug.Log("🗑️ Удаляем старый UI...");

        // Находим Canvas
        Canvas[] canvases = FindObjectsOfType<Canvas>();
        foreach (Canvas canvas in canvases)
        {
            // Удаляем ShopPanel
            Transform shopPanel = canvas.transform.Find("ShopPanel");
            if (shopPanel != null)
            {
                DestroyImmediate(shopPanel.gameObject);
                Debug.Log("  ✓ Удалён ShopPanel");
            }

            // Удаляем PromptPanel
            Transform promptPanel = canvas.transform.Find("PromptPanel");
            if (promptPanel != null)
            {
                DestroyImmediate(promptPanel.gameObject);
                Debug.Log("  ✓ Удалён PromptPanel");
            }
        }

        // Удаляем prefab
        string prefabPath = "Assets/Prefabs/Shop/ItemCardPrefab.prefab";
        if (AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath) != null)
        {
            AssetDatabase.DeleteAsset(prefabPath);
            Debug.Log("  ✓ Удалён ItemCardPrefab");
        }

        AssetDatabase.Refresh();
    }

    private void CreateNewUI()
    {
        Debug.Log("🎨 Создаём новый UI...");

        // Находим или создаём Canvas
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("Canvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
            Debug.Log("  ✓ Создан Canvas");

            if (FindObjectOfType<EventSystem>() == null)
            {
                GameObject eventSystem = new GameObject("EventSystem");
                eventSystem.AddComponent<EventSystem>();
                eventSystem.AddComponent<StandaloneInputModule>();
                Debug.Log("  ✓ Создан EventSystem");
            }
        }

        // Настраиваем Canvas
        CanvasScaler scaler = canvas.GetComponent<CanvasScaler>();
        if (scaler != null)
        {
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
        }

        // Создаём ShopPanel
        CreateShopPanel(canvas.transform);

        // Создаём PromptPanel
        CreatePromptPanel(canvas.transform);

        // Создаём ItemCardPrefab с ПРАВИЛЬНЫМИ настройками
        CreateFixedItemCardPrefab();

        Debug.Log("  ✅ UI создан успешно!");
    }

    private GameObject CreateShopPanel(Transform parent)
    {
        GameObject shopPanel = new GameObject("ShopPanel");
        shopPanel.transform.SetParent(parent, false);

        RectTransform shopRect = shopPanel.AddComponent<RectTransform>();
        shopRect.anchorMin = Vector2.zero;
        shopRect.anchorMax = Vector2.one;
        shopRect.sizeDelta = Vector2.zero;

        Image shopImage = shopPanel.AddComponent<Image>();
        shopImage.color = new Color(0, 0, 0, 0.8f);
        shopPanel.SetActive(false);

        // Header
        GameObject header = new GameObject("Header");
        header.transform.SetParent(shopPanel.transform, false);
        RectTransform headerRect = header.AddComponent<RectTransform>();
        headerRect.anchorMin = new Vector2(0, 1);
        headerRect.anchorMax = new Vector2(1, 1);
        headerRect.pivot = new Vector2(0.5f, 1);
        headerRect.anchoredPosition = new Vector2(0, 0);
        headerRect.sizeDelta = new Vector2(0, 100);

        // Title
        GameObject titleText = new GameObject("TitleText");
        titleText.transform.SetParent(header.transform, false);
        TextMeshProUGUI titleTMP = titleText.AddComponent<TextMeshProUGUI>();
        titleTMP.text = "SHOP";
        titleTMP.fontSize = 48;
        titleTMP.alignment = TextAlignmentOptions.Center;
        titleTMP.color = Color.white;

        RectTransform titleRect = titleText.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0, 0);
        titleRect.anchorMax = new Vector2(0.5f, 1);
        titleRect.sizeDelta = Vector2.zero;

        // Currency
        GameObject currencyText = new GameObject("CurrencyText");
        currencyText.transform.SetParent(header.transform, false);
        TextMeshProUGUI currencyTMP = currencyText.AddComponent<TextMeshProUGUI>();
        currencyTMP.text = "Scrap: 0";
        currencyTMP.fontSize = 36;
        currencyTMP.alignment = TextAlignmentOptions.Center;
        currencyTMP.color = Color.yellow;

        RectTransform currencyRect = currencyText.GetComponent<RectTransform>();
        currencyRect.anchorMin = new Vector2(0.5f, 0);
        currencyRect.anchorMax = new Vector2(1, 1);
        currencyRect.sizeDelta = Vector2.zero;

        // ScrollView с ИСПРАВЛЕННЫМИ настройками
        CreateFixedScrollView(shopPanel.transform);

        // DetailPanel
        CreateDetailPanel(shopPanel.transform);

        Debug.Log("  ✓ Создан ShopPanel");
        return shopPanel;
    }

    private void CreateFixedScrollView(Transform parent)
    {
        GameObject scrollView = new GameObject("ItemScrollView");
        scrollView.transform.SetParent(parent, false);

        RectTransform scrollRect = scrollView.AddComponent<RectTransform>();
        scrollRect.anchorMin = new Vector2(0, 0);
        scrollRect.anchorMax = new Vector2(0.6f, 1);
        scrollRect.pivot = new Vector2(0, 0.5f);
        scrollRect.anchoredPosition = new Vector2(50, -50);
        scrollRect.sizeDelta = new Vector2(-100, -150);

        Image scrollImage = scrollView.AddComponent<Image>();
        scrollImage.color = new Color(0.1f, 0.1f, 0.1f, 0.8f);
        scrollImage.raycastTarget = true;

        ScrollRect scroll = scrollView.AddComponent<ScrollRect>();

        // Viewport
        GameObject viewport = new GameObject("Viewport");
        viewport.transform.SetParent(scrollView.transform, false);
        RectTransform viewportRect = viewport.AddComponent<RectTransform>();
        viewportRect.anchorMin = Vector2.zero;
        viewportRect.anchorMax = Vector2.one;
        viewportRect.sizeDelta = Vector2.zero;

        Image viewportImage = viewport.AddComponent<Image>();
        viewportImage.color = new Color(0, 0, 0, 0);
        viewportImage.raycastTarget = false;

        Mask mask = viewport.AddComponent<Mask>();
        mask.showMaskGraphic = false;

        // Content - КРИТИЧЕСКИ ВАЖНЫЕ НАСТРОЙКИ!
        GameObject content = new GameObject("Content");
        content.transform.SetParent(viewport.transform, false);
        RectTransform contentRect = content.AddComponent<RectTransform>();
        contentRect.anchorMin = new Vector2(0, 1);
        contentRect.anchorMax = new Vector2(1, 1);
        contentRect.pivot = new Vector2(0.5f, 1);
        contentRect.anchoredPosition = Vector2.zero;
        contentRect.sizeDelta = new Vector2(0, 500); // ВАЖНО: Начальная высота!

        // VerticalLayoutGroup с ПРАВИЛЬНЫМИ настройками
        VerticalLayoutGroup layout = content.AddComponent<VerticalLayoutGroup>();
        layout.spacing = 10;
        layout.padding = new RectOffset(10, 10, 10, 10);
        layout.childControlHeight = false;  // FALSE - карточки контролируют свою высоту!
        layout.childControlWidth = true;    // TRUE - заполняют ширину
        layout.childForceExpandHeight = false;
        layout.childForceExpandWidth = true;
        layout.childAlignment = TextAnchor.UpperCenter;

        // ContentSizeFitter
        ContentSizeFitter fitter = content.AddComponent<ContentSizeFitter>();
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;

        scroll.content = contentRect;
        scroll.viewport = viewportRect;
        scroll.horizontal = false;
        scroll.vertical = true;
        scroll.movementType = ScrollRect.MovementType.Clamped;
        scroll.scrollSensitivity = 20;

        Debug.Log("  ✓ Создан ScrollView с исправленными настройками");
    }

    private void CreateDetailPanel(Transform parent)
    {
        GameObject detailPanel = new GameObject("DetailPanel");
        detailPanel.transform.SetParent(parent, false);

        RectTransform detailRect = detailPanel.AddComponent<RectTransform>();
        detailRect.anchorMin = new Vector2(0.6f, 0);
        detailRect.anchorMax = new Vector2(1, 1);
        detailRect.pivot = new Vector2(0.5f, 0.5f);
        detailRect.anchoredPosition = new Vector2(-50, -50);
        detailRect.sizeDelta = new Vector2(-100, -150);

        Image detailImage = detailPanel.AddComponent<Image>();
        detailImage.color = new Color(0.15f, 0.15f, 0.15f, 0.9f);
        detailPanel.SetActive(false);

        // Container
        GameObject container = new GameObject("Container");
        container.transform.SetParent(detailPanel.transform, false);
        RectTransform containerRect = container.AddComponent<RectTransform>();
        containerRect.anchorMin = Vector2.zero;
        containerRect.anchorMax = Vector2.one;
        containerRect.pivot = new Vector2(0.5f, 0.5f);
        containerRect.anchoredPosition = Vector2.zero;
        containerRect.sizeDelta = new Vector2(-40, -40);

        VerticalLayoutGroup layout = container.AddComponent<VerticalLayoutGroup>();
        layout.spacing = 20;
        layout.padding = new RectOffset(20, 20, 20, 20);
        layout.childControlHeight = false;
        layout.childForceExpandHeight = false;

        // ItemName
        CreateDetailText(container.transform, "ItemName", "Item Name", 32, TextAlignmentOptions.Center);

        // ItemDescription
        GameObject desc = CreateDetailText(container.transform, "ItemDescription", "Description", 20, TextAlignmentOptions.TopLeft);
        LayoutElement descLayout = desc.AddComponent<LayoutElement>();
        descLayout.preferredHeight = 200;

        // ItemPrice
        CreateDetailText(container.transform, "ItemPrice", "Price: 0 Scrap", 24, TextAlignmentOptions.Center);

        // ItemStock
        CreateDetailText(container.transform, "ItemStock", "Stock: 0", 24, TextAlignmentOptions.Center);

        // BuyButton
        GameObject buyButton = new GameObject("BuyButton");
        buyButton.transform.SetParent(container.transform, false);
        RectTransform buyRect = buyButton.AddComponent<RectTransform>();
        buyRect.sizeDelta = new Vector2(0, 60);

        Image buyImage = buyButton.AddComponent<Image>();
        buyImage.color = new Color(0.2f, 0.8f, 0.2f, 1f);
        buyButton.AddComponent<Button>();

        GameObject buyText = new GameObject("Text");
        buyText.transform.SetParent(buyButton.transform, false);
        TextMeshProUGUI buyTMP = buyText.AddComponent<TextMeshProUGUI>();
        buyTMP.text = "BUY";
        buyTMP.fontSize = 28;
        buyTMP.alignment = TextAlignmentOptions.Center;
        buyTMP.color = Color.white;

        RectTransform buyTextRect = buyText.GetComponent<RectTransform>();
        buyTextRect.anchorMin = Vector2.zero;
        buyTextRect.anchorMax = Vector2.one;
        buyTextRect.sizeDelta = Vector2.zero;

        LayoutElement buyLayout = buyButton.AddComponent<LayoutElement>();
        buyLayout.preferredHeight = 60;

        Debug.Log("  ✓ Создан DetailPanel");
    }

    private GameObject CreateDetailText(Transform parent, string name, string text, float fontSize, TextAlignmentOptions alignment)
    {
        GameObject textObj = new GameObject(name);
        textObj.transform.SetParent(parent, false);

        TextMeshProUGUI tmp = textObj.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.alignment = alignment;
        tmp.color = Color.white;

        RectTransform rect = textObj.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(0, fontSize * 1.5f);

        return textObj;
    }

    private GameObject CreatePromptPanel(Transform parent)
    {
        GameObject promptPanel = new GameObject("PromptPanel");
        promptPanel.transform.SetParent(parent, false);

        RectTransform promptRect = promptPanel.AddComponent<RectTransform>();
        promptRect.anchorMin = new Vector2(0.5f, 0);
        promptRect.anchorMax = new Vector2(0.5f, 0);
        promptRect.pivot = new Vector2(0.5f, 0);
        promptRect.anchoredPosition = new Vector2(0, 100);
        promptRect.sizeDelta = new Vector2(300, 80);

        Image promptImage = promptPanel.AddComponent<Image>();
        promptImage.color = new Color(0, 0, 0, 0.7f);
        promptPanel.SetActive(false);

        GameObject promptText = new GameObject("PromptText");
        promptText.transform.SetParent(promptPanel.transform, false);

        TextMeshProUGUI promptTMP = promptText.AddComponent<TextMeshProUGUI>();
        promptTMP.text = "[E] Open Shop";
        promptTMP.fontSize = 24;
        promptTMP.alignment = TextAlignmentOptions.Center;
        promptTMP.color = Color.white;

        RectTransform promptTextRect = promptText.GetComponent<RectTransform>();
        promptTextRect.anchorMin = Vector2.zero;
        promptTextRect.anchorMax = Vector2.one;
        promptTextRect.sizeDelta = Vector2.zero;

        Debug.Log("  ✓ Создан PromptPanel");
        return promptPanel;
    }

    private void CreateFixedItemCardPrefab()
    {
        Debug.Log("🎴 Создаём ИСПРАВЛЕННЫЙ ItemCardPrefab...");

        GameObject itemCard = new GameObject("ItemCard");

        // КРИТИЧЕСКИ ВАЖНЫЕ НАСТРОЙКИ RectTransform!
        RectTransform cardRect = itemCard.AddComponent<RectTransform>();
        cardRect.anchorMin = new Vector2(0, 1);      // Привязка к верху
        cardRect.anchorMax = new Vector2(1, 1);      // Растягивается по ширине
        cardRect.pivot = new Vector2(0.5f, 1f);      // Pivot сверху
        cardRect.sizeDelta = new Vector2(1000, 120);    // Начальная ширина 1000, высота 120

        // Background Image - ЯРКИЙ ЦВЕТ ДЛЯ ВИДИМОСТИ!
        Image cardImage = itemCard.AddComponent<Image>();
        cardImage.color = new Color(0.3f, 0.5f, 0.7f, 1f);  // Ярко-синий!
        cardImage.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Background.psd");
        cardImage.type = Image.Type.Sliced;
        cardImage.raycastTarget = true;  // ВАЖНО для кликов!

        Button button = itemCard.AddComponent<Button>();
        itemCard.AddComponent<ShopItemCard>();

        // Background (inner)
        GameObject background = new GameObject("Background");
        background.transform.SetParent(itemCard.transform, false);
        RectTransform bgRect = background.AddComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.sizeDelta = Vector2.zero;
        bgRect.offsetMin = new Vector2(2, 2);
        bgRect.offsetMax = new Vector2(-2, -2);
        Image bgImage = background.AddComponent<Image>();
        bgImage.color = new Color(0.15f, 0.15f, 0.15f, 1f);
        bgImage.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Background.psd");
        bgImage.type = Image.Type.Sliced;

        // Icon
        GameObject icon = new GameObject("Icon");
        icon.transform.SetParent(itemCard.transform, false);
        RectTransform iconRect = icon.AddComponent<RectTransform>();
        iconRect.anchorMin = new Vector2(0, 0.5f);
        iconRect.anchorMax = new Vector2(0, 0.5f);
        iconRect.pivot = new Vector2(0, 0.5f);
        iconRect.anchoredPosition = new Vector2(10, 0);
        iconRect.sizeDelta = new Vector2(100, 100);
        Image iconImage = icon.AddComponent<Image>();
        iconImage.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
        iconImage.color = new Color(1f, 0.8f, 0.2f, 1f);  // ЯРКИЙ ОРАНЖЕВЫЙ!

        // Name
        GameObject nameText = new GameObject("NameText");
        nameText.transform.SetParent(itemCard.transform, false);
        RectTransform nameRect = nameText.AddComponent<RectTransform>();
        nameRect.anchorMin = new Vector2(0, 0.5f);
        nameRect.anchorMax = new Vector2(1, 1);
        nameRect.anchoredPosition = new Vector2(120, -10);
        nameRect.sizeDelta = new Vector2(-240, -20);
        TextMeshProUGUI nameTMP = nameText.AddComponent<TextMeshProUGUI>();
        nameTMP.text = "Item Name";
        nameTMP.fontSize = 24;
        nameTMP.fontStyle = FontStyles.Bold;
        nameTMP.color = Color.white;
        nameTMP.alignment = TextAlignmentOptions.Left;

        // Price
        GameObject priceText = new GameObject("PriceText");
        priceText.transform.SetParent(itemCard.transform, false);
        RectTransform priceRect = priceText.AddComponent<RectTransform>();
        priceRect.anchorMin = new Vector2(0, 0);
        priceRect.anchorMax = new Vector2(1, 0.5f);
        priceRect.anchoredPosition = new Vector2(120, 10);
        priceRect.sizeDelta = new Vector2(-240, -20);
        TextMeshProUGUI priceTMP = priceText.AddComponent<TextMeshProUGUI>();
        priceTMP.text = "0 Scrap";
        priceTMP.fontSize = 20;
        priceTMP.color = Color.yellow;
        priceTMP.alignment = TextAlignmentOptions.Left;

        // Stock
        GameObject stockText = new GameObject("StockText");
        stockText.transform.SetParent(itemCard.transform, false);
        RectTransform stockRect = stockText.AddComponent<RectTransform>();
        stockRect.anchorMin = new Vector2(1, 0.5f);
        stockRect.anchorMax = new Vector2(1, 0.5f);
        stockRect.pivot = new Vector2(1, 0.5f);
        stockRect.anchoredPosition = new Vector2(-10, 0);
        stockRect.sizeDelta = new Vector2(80, 40);
        TextMeshProUGUI stockTMP = stockText.AddComponent<TextMeshProUGUI>();
        stockTMP.text = "x0";
        stockTMP.fontSize = 18;
        stockTMP.alignment = TextAlignmentOptions.Center;
        stockTMP.color = Color.gray;

        // КРИТИЧЕСКИ ВАЖНЫЙ LayoutElement!
        LayoutElement layoutElement = itemCard.AddComponent<LayoutElement>();
        layoutElement.minHeight = 120;
        layoutElement.preferredHeight = 120;
        layoutElement.minWidth = 400;  // Минимальная ширина
        layoutElement.preferredWidth = 1000;  // Предпочитаемая ширина (будет растягиваться)
        layoutElement.flexibleHeight = 0;
        layoutElement.flexibleWidth = 1;  // Растягивается чтобы заполнить доступную ширину

        // Назначаем ссылки
        ShopItemCard cardScript = itemCard.GetComponent<ShopItemCard>();
        SerializedObject so = new SerializedObject(cardScript);
        so.FindProperty("iconImage").objectReferenceValue = iconImage;
        so.FindProperty("nameText").objectReferenceValue = nameTMP;
        so.FindProperty("priceText").objectReferenceValue = priceTMP;
        so.FindProperty("stockText").objectReferenceValue = stockTMP;
        so.FindProperty("cardButton").objectReferenceValue = button;
        so.FindProperty("backgroundImage").objectReferenceValue = bgImage;
        so.ApplyModifiedProperties();

        // Сохраняем prefab
        string prefabPath = "Assets/Prefabs/Shop/ItemCardPrefab.prefab";
        string directory = "Assets/Prefabs/Shop";

        if (!AssetDatabase.IsValidFolder(directory))
        {
            AssetDatabase.CreateFolder("Assets/Prefabs", "Shop");
        }

        PrefabUtility.SaveAsPrefabAsset(itemCard, prefabPath);
        DestroyImmediate(itemCard);

        Debug.Log("  ✅ ИСПРАВЛЕННЫЙ ItemCardPrefab создан!");
    }

    private ShopManager SetupShopManager()
    {
        Debug.Log("⚙️ Настраиваем ShopManager...");

        // Удаляем старый ShopManager если есть
        ShopManager existingManager = FindObjectOfType<ShopManager>();
        if (existingManager != null)
        {
            DestroyImmediate(existingManager.gameObject);
            Debug.Log("  ✓ Удалён старый ShopManager");
        }

        // Создаём новый ShopManager
        GameObject managerObj = new GameObject("ShopManager");
        ShopManager manager = managerObj.AddComponent<ShopManager>();

        // Автоназначаем все ссылки
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas != null)
        {
            SerializedObject so = new SerializedObject(manager);

            // Shop UI
            Transform shopPanel = canvas.transform.Find("ShopPanel");
            if (shopPanel != null)
            {
                so.FindProperty("shopUI").objectReferenceValue = shopPanel.gameObject;

                // Item Cards Container
                Transform content = shopPanel.Find("ItemScrollView/Viewport/Content");
                if (content != null)
                {
                    so.FindProperty("itemCardsContainer").objectReferenceValue = content.GetComponent<RectTransform>();
                }

                // Detail Panel
                Transform detailPanel = shopPanel.Find("DetailPanel");
                if (detailPanel != null)
                {
                    so.FindProperty("detailPanel").objectReferenceValue = detailPanel.gameObject;

                    Transform container = detailPanel.Find("Container");
                    if (container != null)
                    {
                        so.FindProperty("detailName").objectReferenceValue = container.Find("ItemName")?.GetComponent<TextMeshProUGUI>();
                        so.FindProperty("detailDescription").objectReferenceValue = container.Find("ItemDescription")?.GetComponent<TextMeshProUGUI>();
                        so.FindProperty("detailPrice").objectReferenceValue = container.Find("ItemPrice")?.GetComponent<TextMeshProUGUI>();
                        so.FindProperty("detailStock").objectReferenceValue = container.Find("ItemStock")?.GetComponent<TextMeshProUGUI>();

                        Transform buyButton = container.Find("BuyButton");
                        if (buyButton != null)
                        {
                            so.FindProperty("buyButton").objectReferenceValue = buyButton.GetComponent<Button>();
                            so.FindProperty("buyButtonText").objectReferenceValue = buyButton.Find("Text")?.GetComponent<TextMeshProUGUI>();
                        }
                    }
                }

                // Currency Text
                Transform currencyText = shopPanel.Find("Header/CurrencyText");
                if (currencyText != null)
                {
                    so.FindProperty("currencyText").objectReferenceValue = currencyText.GetComponent<TextMeshProUGUI>();
                }
            }

            // Prompt UI
            Transform promptPanel = canvas.transform.Find("PromptPanel");
            if (promptPanel != null)
            {
                so.FindProperty("promptUI").objectReferenceValue = promptPanel.gameObject;
                Transform promptText = promptPanel.Find("PromptText");
                if (promptText != null)
                {
                    so.FindProperty("promptText").objectReferenceValue = promptText.GetComponent<TextMeshProUGUI>();
                }
            }

            // Item Card Prefab
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Shop/ItemCardPrefab.prefab");
            if (prefab != null)
            {
                so.FindProperty("itemCardPrefab").objectReferenceValue = prefab;
            }

            // Player
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                so.FindProperty("player").objectReferenceValue = player;
            }

            so.ApplyModifiedProperties();
        }

        Debug.Log("  ✅ ShopManager настроен!");
        return manager;
    }

    private void AssignAllShopItems(ShopManager manager)
    {
        Debug.Log("📦 Ищем все Shop Items...");

        // Находим все ShopItem ScriptableObjects в проекте
        string[] guids = AssetDatabase.FindAssets("t:ShopItem");
        ShopItem[] allItems = new ShopItem[guids.Length];

        for (int i = 0; i < guids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);
            allItems[i] = AssetDatabase.LoadAssetAtPath<ShopItem>(path);
        }

        if (allItems.Length == 0)
        {
            Debug.LogWarning("  ⚠️ Shop Items не найдены!");
            return;
        }

        // Назначаем на ShopManager
        SerializedObject so = new SerializedObject(manager);
        SerializedProperty availableItems = so.FindProperty("availableItems");

        availableItems.ClearArray();
        availableItems.arraySize = allItems.Length;

        for (int i = 0; i < allItems.Length; i++)
        {
            availableItems.GetArrayElementAtIndex(i).objectReferenceValue = allItems[i];
            Debug.Log($"  ✓ Назначен: {allItems[i].itemName}");
        }

        so.ApplyModifiedProperties();

        Debug.Log($"  ✅ Назначено {allItems.Length} shop items!");
    }

    private void SetupShopTrigger(ShopManager manager)
    {
        Debug.Log("🎯 Настраиваем ShopTrigger...");

        // Удаляем старый триггер если есть
        ShopTrigger existingTrigger = FindObjectOfType<ShopTrigger>();
        if (existingTrigger != null)
        {
            DestroyImmediate(existingTrigger.gameObject);
        }

        // Создаём новый триггер
        GameObject trigger = GameObject.CreatePrimitive(PrimitiveType.Cube);
        trigger.name = "ShopTrigger";
        trigger.transform.position = new Vector3(5, 0, 0);
        trigger.transform.localScale = new Vector3(3, 3, 3);

        BoxCollider collider = trigger.GetComponent<BoxCollider>();
        if (collider != null)
        {
            collider.isTrigger = true;
        }

        ShopTrigger triggerScript = trigger.AddComponent<ShopTrigger>();

        // Назначаем ShopManager
        SerializedObject so = new SerializedObject(triggerScript);
        so.FindProperty("shopManager").objectReferenceValue = manager;
        so.ApplyModifiedProperties();

        // Материал
        Renderer renderer = trigger.GetComponent<Renderer>();
        if (renderer != null)
        {
            Material mat = new Material(Shader.Find("Standard"));
            mat.color = new Color(0.2f, 0.8f, 0.2f, 0.5f);
            mat.SetFloat("_Mode", 3);
            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            mat.SetInt("_ZWrite", 0);
            mat.DisableKeyword("_ALPHATEST_ON");
            mat.EnableKeyword("_ALPHABLEND_ON");
            mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            mat.renderQueue = 3000;
            renderer.material = mat;
        }

        Debug.Log("  ✅ ShopTrigger настроен!");
    }

    private void SetupPlayerComponents()
    {
        Debug.Log("👤 Настраиваем компоненты на игрока...");

        // Находим игрока по тегу
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogWarning("  ⚠️ Игрок с тегом 'Player' не найден!");
            EditorUtility.DisplayDialog("Внимание",
                "Игрок не найден!\n\n" +
                "Убедитесь что у игрока есть тег 'Player'.",
                "OK");
            return;
        }

        Debug.Log($"  ✓ Найден игрок: {player.name}");

        // Добавляем или проверяем PlayerCurrency
        PlayerCurrency currency = player.GetComponent<PlayerCurrency>();
        if (currency == null)
        {
            currency = player.AddComponent<PlayerCurrency>();
            Debug.Log("  ✓ Добавлен PlayerCurrency");
        }
        else
        {
            Debug.Log("  ✓ PlayerCurrency уже есть");
        }

        // Настраиваем начальное количество скрапа
        SerializedObject currencySO = new SerializedObject(currency);
        SerializedProperty currentScrapProp = currencySO.FindProperty("currentScrap");
        if (currentScrapProp != null)
        {
            currentScrapProp.intValue = 100;
            currencySO.ApplyModifiedProperties();
            Debug.Log("  ✓ Начальный скрап установлен: 100");
        }

        // Добавляем или проверяем PlayerInventory
        PlayerInventory inventory = player.GetComponent<PlayerInventory>();
        if (inventory == null)
        {
            inventory = player.AddComponent<PlayerInventory>();
            Debug.Log("  ✓ Добавлен PlayerInventory");
        }
        else
        {
            Debug.Log("  ✓ PlayerInventory уже есть");
        }

        // Добавляем или проверяем PlayerUpgrades
        PlayerUpgrades upgrades = player.GetComponent<PlayerUpgrades>();
        if (upgrades == null)
        {
            upgrades = player.AddComponent<PlayerUpgrades>();
            Debug.Log("  ✓ Добавлен PlayerUpgrades");
        }
        else
        {
            Debug.Log("  ✓ PlayerUpgrades уже есть");
        }

        // Проверяем PlayerHealth - должен быть публичным
        PlayerHealth health = player.GetComponent<PlayerHealth>();
        if (health != null)
        {
            Debug.Log("  ✓ PlayerHealth найден");

            // Проверяем доступность поля currentHealth через рефлексию
            System.Reflection.FieldInfo healthField = typeof(PlayerHealth).GetField("currentHealth",
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

            if (healthField != null)
            {
                Debug.Log("  ✓ PlayerHealth.currentHealth публичен - всё готово для shop системы");
            }
            else
            {
                Debug.LogWarning("  ⚠️ PlayerHealth.currentHealth должен быть public для работы shop системы");
            }
        }
        else
        {
            Debug.LogWarning("  ⚠️ PlayerHealth не найден на игроке");
        }

        // Помечаем игрока как изменённого
        EditorUtility.SetDirty(player);

        Debug.Log("  ✅ Компоненты игрока настроены!");
    }
}
#endif
