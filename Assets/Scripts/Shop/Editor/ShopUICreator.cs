#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

/// <summary>
/// Editor tool to automatically create complete Shop UI structure
/// Access via: Tools → Shop System → Create Shop UI
/// </summary>
public class ShopUICreator : EditorWindow
{
    [MenuItem("Tools/Shop System/Create Shop UI")]
    public static void ShowWindow()
    {
        GetWindow<ShopUICreator>("Shop UI Creator");
    }

    private void OnGUI()
    {
        GUILayout.Label("Shop System Creator", EditorStyles.boldLabel);
        GUILayout.Space(10);

        EditorGUILayout.HelpBox("Automatically create the entire shop system!\n\n" +
            "✨ ONE-CLICK COMPLETE SETUP:\n" +
            "- UI (Canvas, panels, prefabs)\n" +
            "- ShopManager (with all references)\n" +
            "- ShopTrigger (ready to use)\n\n" +
            "Or create components individually below.", MessageType.Info);

        GUILayout.Space(10);

        // MEGA BUTTON - Create everything!
        GUI.backgroundColor = new Color(0.2f, 0.8f, 0.2f);
        if (GUILayout.Button("🚀 CREATE COMPLETE SHOP SYSTEM 🚀", GUILayout.Height(50)))
        {
            CreateCompleteShopSystem();
        }
        GUI.backgroundColor = Color.white;

        GUILayout.Space(10);

        // DELETE & RECREATE BUTTON
        GUI.backgroundColor = new Color(1f, 0.6f, 0.2f);
        if (GUILayout.Button("🔄 DELETE & RECREATE UI (Fix Issues)", GUILayout.Height(40)))
        {
            if (EditorUtility.DisplayDialog("Delete and Recreate UI?",
                "This will DELETE existing UI and create fresh one!\n\n" +
                "Will delete:\n" +
                "- Canvas/ShopPanel (or entire Canvas if no other children)\n" +
                "- ItemCardPrefab\n\n" +
                "Then create everything from scratch.\n\n" +
                "Continue?",
                "Yes, Delete & Recreate",
                "Cancel"))
            {
                DeleteAndRecreateUI();
            }
        }
        GUI.backgroundColor = Color.white;

        GUILayout.Space(15);
        GUILayout.Label("Individual Components:", EditorStyles.boldLabel);
        GUILayout.Space(5);

        if (GUILayout.Button("Create Shop UI (Canvas + Panels)", GUILayout.Height(35)))
        {
            CreateCompleteShopUI();
        }

        GUILayout.Space(5);

        if (GUILayout.Button("Create Shop Manager (with auto-references)", GUILayout.Height(35)))
        {
            CreateShopManager();
        }

        GUILayout.Space(5);

        if (GUILayout.Button("Create Shop Trigger (in scene)", GUILayout.Height(35)))
        {
            CreateShopTrigger();
        }

        GUILayout.Space(10);

        if (GUILayout.Button("Create Only Item Card Prefab", GUILayout.Height(25)))
        {
            CreateItemCardPrefab();
        }

        GUILayout.Space(5);

        GUI.backgroundColor = new Color(1f, 0.5f, 0.5f);
        if (GUILayout.Button("🔄 Recreate Item Card Prefab", GUILayout.Height(25)))
        {
            if (EditorUtility.DisplayDialog("Recreate Item Card Prefab?",
                "This will DELETE existing ItemCardPrefab and create a NEW one with LayoutElement.\n\n" +
                "Continue?",
                "Yes, Recreate",
                "Cancel"))
            {
                RecreateItemCardPrefab();
            }
        }
        GUI.backgroundColor = Color.white;
    }

    private void CreateCompleteShopUI()
    {
        // Check if scene is open
        if (!UnityEngine.SceneManagement.SceneManager.GetActiveScene().IsValid())
        {
            EditorUtility.DisplayDialog("Error", "No scene is currently open!", "OK");
            return;
        }

        Debug.Log("[ShopUICreator] Creating complete Shop UI...");

        // 1. Find or create PlayerUI parent object
        GameObject playerUI = GameObject.Find("PlayerUI");
        if (playerUI == null)
        {
            playerUI = new GameObject("PlayerUI");
            Debug.Log("✓ Created PlayerUI parent object");
        }
        else
        {
            Debug.Log("✓ Found existing PlayerUI");
        }

        // 2. Create or find Canvas (inside PlayerUI or root)
        Canvas canvas = null;
        Canvas[] allCanvases = FindObjectsOfType<Canvas>();
        foreach (Canvas c in allCanvases)
        {
            // Find UI Canvas (not WorldSpace, not on NPCs)
            if (c.renderMode == RenderMode.ScreenSpaceOverlay ||
                (c.transform.parent != null && c.transform.parent.name == "PlayerUI") ||
                c.transform.parent == null)
            {
                canvas = c;
                break;
            }
        }

        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("Canvas");
            canvasObj.transform.SetParent(playerUI.transform, false);
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
            Debug.Log("✓ Created Canvas inside PlayerUI");

            // Create EventSystem if needed
            if (FindObjectOfType<EventSystem>() == null)
            {
                GameObject eventSystem = new GameObject("EventSystem");
                eventSystem.AddComponent<EventSystem>();
                eventSystem.AddComponent<StandaloneInputModule>();
                Debug.Log("✓ Created EventSystem");
            }
        }
        else
        {
            Debug.Log("✓ Found existing Canvas - FORCING ScreenSpaceOverlay mode");
            // CRITICAL: Force correct render mode
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            // Move Canvas to PlayerUI if not already
            if (canvas.transform.parent != playerUI.transform)
            {
                canvas.transform.SetParent(playerUI.transform, false);
                Debug.Log("✓ Moved Canvas into PlayerUI");
            }

            // Ensure has required components
            if (canvas.GetComponent<CanvasScaler>() == null)
            {
                canvas.gameObject.AddComponent<CanvasScaler>();
            }
            if (canvas.GetComponent<GraphicRaycaster>() == null)
            {
                canvas.gameObject.AddComponent<GraphicRaycaster>();
            }
        }

        // Setup Canvas Scaler
        CanvasScaler scaler = canvas.GetComponent<CanvasScaler>();
        if (scaler != null)
        {
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
        }

        // 2. Create ShopPanel
        GameObject shopPanel = CreateShopPanel(canvas.transform);

        // 3. Create PromptPanel
        GameObject promptPanel = CreatePromptPanel(canvas.transform);

        // 4. Create Item Card Prefab
        CreateItemCardPrefab();

        // Select ShopPanel
        Selection.activeGameObject = shopPanel;

        Debug.Log("✅ [ShopUICreator] Shop UI created successfully!");
        EditorUtility.DisplayDialog("Success!",
            "IMPROVED Shop UI has been created!\n\n" +
            "Created:\n" +
            "- Canvas with styled ShopPanel (gold header)\n" +
            "- PromptPanel\n" +
            "- DetailPanel (with gold outline)\n" +
            "- Item Card Prefab (improved visibility)\n\n" +
            "UI Improvements:\n" +
            "- Better colors and contrast\n" +
            "- Visible outlines on panels\n" +
            "- Gold theme for shop\n" +
            "- Proper text alignment\n" +
            "- Placeholder text for testing\n\n" +
            "Next: Create ShopManager and assign references!", "OK");
    }

    private GameObject CreateShopPanel(Transform parent)
    {
        // Main shop panel
        GameObject shopPanel = new GameObject("ShopPanel");
        shopPanel.transform.SetParent(parent, false);

        RectTransform shopRect = shopPanel.AddComponent<RectTransform>();
        shopRect.anchorMin = Vector2.zero;
        shopRect.anchorMax = Vector2.one;
        shopRect.sizeDelta = Vector2.zero;
        shopRect.anchoredPosition = Vector2.zero;

        Image shopImage = shopPanel.AddComponent<Image>();
        shopImage.color = new Color(0, 0, 0, 0.85f);

        // Deactivate by default
        shopPanel.SetActive(false);

        // Header panel (with background)
        GameObject header = new GameObject("Header");
        header.transform.SetParent(shopPanel.transform, false);
        RectTransform headerRect = header.AddComponent<RectTransform>();
        headerRect.anchorMin = new Vector2(0, 1);
        headerRect.anchorMax = new Vector2(1, 1);
        headerRect.pivot = new Vector2(0.5f, 1);
        headerRect.anchoredPosition = new Vector2(0, 0);
        headerRect.sizeDelta = new Vector2(0, 100);

        // Header background
        Image headerBg = header.AddComponent<Image>();
        headerBg.color = new Color(0.1f, 0.1f, 0.1f, 0.9f);

        // Title text
        GameObject titleText = new GameObject("TitleText");
        titleText.transform.SetParent(header.transform, false);
        TextMeshProUGUI titleTMP = titleText.AddComponent<TextMeshProUGUI>();
        titleTMP.text = "SHOP";
        titleTMP.fontSize = 48;
        titleTMP.fontStyle = FontStyles.Bold;
        titleTMP.alignment = TextAlignmentOptions.Center;
        titleTMP.color = new Color(1f, 0.85f, 0f); // Gold

        RectTransform titleRect = titleText.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0, 0);
        titleRect.anchorMax = new Vector2(0.5f, 1);
        titleRect.sizeDelta = Vector2.zero;
        titleRect.anchoredPosition = Vector2.zero;

        // Currency text
        GameObject currencyText = new GameObject("CurrencyText");
        currencyText.transform.SetParent(header.transform, false);
        TextMeshProUGUI currencyTMP = currencyText.AddComponent<TextMeshProUGUI>();
        currencyTMP.text = "Scrap: 100";
        currencyTMP.fontSize = 36;
        currencyTMP.fontStyle = FontStyles.Bold;
        currencyTMP.alignment = TextAlignmentOptions.Center;
        currencyTMP.color = new Color(1f, 0.85f, 0f); // Gold

        RectTransform currencyRect = currencyText.GetComponent<RectTransform>();
        currencyRect.anchorMin = new Vector2(0.5f, 0);
        currencyRect.anchorMax = new Vector2(1, 1);
        currencyRect.sizeDelta = Vector2.zero;
        currencyRect.anchoredPosition = Vector2.zero;

        // Scroll View for items
        GameObject scrollView = CreateScrollView(shopPanel.transform);

        // Detail Panel
        GameObject detailPanel = CreateDetailPanel(shopPanel.transform);

        Debug.Log("✓ Created ShopPanel with all elements");
        return shopPanel;
    }

    private GameObject CreateScrollView(Transform parent)
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
        scrollImage.color = new Color(0.15f, 0.15f, 0.15f, 0.9f);

        // Add outline for better visibility
        Outline scrollOutline = scrollView.AddComponent<Outline>();
        scrollOutline.effectColor = new Color(0.5f, 0.5f, 0.5f, 0.8f);
        scrollOutline.effectDistance = new Vector2(2, -2);

        ScrollRect scroll = scrollView.AddComponent<ScrollRect>();

        // Viewport
        GameObject viewport = new GameObject("Viewport");
        viewport.transform.SetParent(scrollView.transform, false);
        RectTransform viewportRect = viewport.AddComponent<RectTransform>();
        viewportRect.anchorMin = Vector2.zero;
        viewportRect.anchorMax = Vector2.one;
        viewportRect.sizeDelta = Vector2.zero;
        viewport.AddComponent<Image>().color = new Color(0, 0, 0, 0);
        viewport.AddComponent<Mask>().showMaskGraphic = false;

        // Content
        GameObject content = new GameObject("Content");
        content.transform.SetParent(viewport.transform, false);
        RectTransform contentRect = content.AddComponent<RectTransform>();
        contentRect.anchorMin = new Vector2(0, 1);
        contentRect.anchorMax = new Vector2(1, 1);
        contentRect.pivot = new Vector2(0.5f, 1);
        contentRect.anchoredPosition = Vector2.zero;
        contentRect.sizeDelta = new Vector2(0, 0);

        // Vertical Layout Group
        VerticalLayoutGroup layout = content.AddComponent<VerticalLayoutGroup>();
        layout.spacing = 10;
        layout.padding = new RectOffset(10, 10, 10, 10);
        layout.childControlHeight = true;  // CHANGED: Enable height control
        layout.childControlWidth = true;
        layout.childForceExpandHeight = false;
        layout.childForceExpandWidth = true;
        layout.childAlignment = TextAnchor.UpperCenter;

        ContentSizeFitter fitter = content.AddComponent<ContentSizeFitter>();
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        scroll.content = contentRect;
        scroll.viewport = viewportRect;
        scroll.horizontal = false;
        scroll.vertical = true;

        Debug.Log("✓ Created Scroll View");
        return scrollView;
    }

    private GameObject CreateDetailPanel(Transform parent)
    {
        GameObject detailPanel = new GameObject("DetailPanel");
        detailPanel.transform.SetParent(parent, false);

        RectTransform detailRect = detailPanel.AddComponent<RectTransform>();
        detailRect.anchorMin = new Vector2(0.6f, 0);
        detailRect.anchorMax = new Vector2(1, 1);
        detailRect.anchoredPosition = new Vector2(-50, -50);
        detailRect.sizeDelta = new Vector2(-100, -150);

        Image detailImage = detailPanel.AddComponent<Image>();
        detailImage.color = new Color(0.15f, 0.15f, 0.15f, 0.95f);

        // Add outline for better visibility
        Outline outline = detailPanel.AddComponent<Outline>();
        outline.effectColor = new Color(1f, 0.85f, 0f, 0.8f); // Gold outline
        outline.effectDistance = new Vector2(2, -2);

        // Deactivate by default
        detailPanel.SetActive(false);

        // Detail elements container
        GameObject container = new GameObject("Container");
        container.transform.SetParent(detailPanel.transform, false);
        RectTransform containerRect = container.AddComponent<RectTransform>();
        containerRect.anchorMin = Vector2.zero;
        containerRect.anchorMax = Vector2.one;
        containerRect.anchoredPosition = Vector2.zero;
        containerRect.sizeDelta = Vector2.zero;

        VerticalLayoutGroup layout = container.AddComponent<VerticalLayoutGroup>();
        layout.spacing = 15;
        layout.padding = new RectOffset(20, 20, 20, 20);
        layout.childControlHeight = false;
        layout.childControlWidth = true;
        layout.childForceExpandHeight = false;
        layout.childForceExpandWidth = true;
        layout.childAlignment = TextAnchor.UpperCenter;

        // Item Name
        GameObject nameObj = CreateDetailText(container.transform, "ItemName", "Item Name", 28, TextAlignmentOptions.Center);
        LayoutElement nameLayout = nameObj.AddComponent<LayoutElement>();
        nameLayout.preferredHeight = 40;

        // Item Description
        GameObject desc = CreateDetailText(container.transform, "ItemDescription", "Select an item to view its details...", 18, TextAlignmentOptions.TopLeft);
        TextMeshProUGUI descText = desc.GetComponent<TextMeshProUGUI>();
        descText.enableWordWrapping = true;
        LayoutElement descLayout = desc.AddComponent<LayoutElement>();
        descLayout.preferredHeight = 200;
        descLayout.flexibleHeight = 1;

        // Item Price
        GameObject priceObj = CreateDetailText(container.transform, "ItemPrice", "Price: 0 Scrap", 22, TextAlignmentOptions.Center);
        TextMeshProUGUI priceText = priceObj.GetComponent<TextMeshProUGUI>();
        priceText.color = new Color(1f, 0.85f, 0f); // Gold color
        LayoutElement priceLayout = priceObj.AddComponent<LayoutElement>();
        priceLayout.preferredHeight = 30;

        // Item Stock
        GameObject stockObj = CreateDetailText(container.transform, "ItemStock", "Stock: ∞", 20, TextAlignmentOptions.Center);
        LayoutElement stockLayout = stockObj.AddComponent<LayoutElement>();
        stockLayout.preferredHeight = 30;

        // Buy Button
        GameObject buyButton = new GameObject("BuyButton");
        buyButton.transform.SetParent(container.transform, false);
        RectTransform buyRect = buyButton.AddComponent<RectTransform>();
        buyRect.sizeDelta = new Vector2(0, 60);

        Image buyImage = buyButton.AddComponent<Image>();
        buyImage.color = new Color(0.2f, 0.8f, 0.2f, 1f);

        Button button = buyButton.AddComponent<Button>();

        // Button color transitions
        ColorBlock colors = button.colors;
        colors.normalColor = new Color(0.2f, 0.8f, 0.2f, 1f);
        colors.highlightedColor = new Color(0.3f, 0.9f, 0.3f, 1f);
        colors.pressedColor = new Color(0.15f, 0.6f, 0.15f, 1f);
        colors.disabledColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        button.colors = colors;

        GameObject buyText = new GameObject("Text");
        buyText.transform.SetParent(buyButton.transform, false);
        TextMeshProUGUI buyTMP = buyText.AddComponent<TextMeshProUGUI>();
        buyTMP.text = "BUY";
        buyTMP.fontSize = 28;
        buyTMP.fontStyle = FontStyles.Bold;
        buyTMP.alignment = TextAlignmentOptions.Center;
        buyTMP.color = Color.white;

        RectTransform buyTextRect = buyText.GetComponent<RectTransform>();
        buyTextRect.anchorMin = Vector2.zero;
        buyTextRect.anchorMax = Vector2.one;
        buyTextRect.sizeDelta = Vector2.zero;
        buyTextRect.anchoredPosition = Vector2.zero;

        LayoutElement buyLayout = buyButton.AddComponent<LayoutElement>();
        buyLayout.preferredHeight = 60;

        Debug.Log("✓ Created Detail Panel (IMPROVED)");
        return detailPanel;
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

        // Deactivate by default
        promptPanel.SetActive(false);

        // Prompt text
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

        Debug.Log("✓ Created Prompt Panel");
        return promptPanel;
    }

    private void CreateItemCardPrefab()
    {
        // Create item card
        GameObject itemCard = new GameObject("ItemCard");

        RectTransform cardRect = itemCard.AddComponent<RectTransform>();
        // CRITICAL: Set anchors for VerticalLayoutGroup compatibility
        cardRect.anchorMin = new Vector2(0, 1);
        cardRect.anchorMax = new Vector2(1, 1);
        cardRect.pivot = new Vector2(0.5f, 1f);
        cardRect.sizeDelta = new Vector2(0, 100); // Width will stretch, height fixed at 100

        Debug.Log($"[ShopUICreator] ItemCard RectTransform created - AnchorMin: {cardRect.anchorMin}, AnchorMax: {cardRect.anchorMax}, Pivot: {cardRect.pivot}, SizeDelta: {cardRect.sizeDelta}");

        // ЯРКИЙ ВИДИМЫЙ ФОН КАРТОЧКИ!
        Image cardImage = itemCard.AddComponent<Image>();
        cardImage.color = new Color(0.2f, 0.3f, 0.4f, 1f); // Темно-синий, полностью непрозрачный

        Button button = itemCard.AddComponent<Button>();

        // ЯРКИЕ цвета кнопки для отличной видимости
        ColorBlock buttonColors = button.colors;
        buttonColors.normalColor = new Color(0.25f, 0.35f, 0.45f, 1f); // Светлее синий
        buttonColors.highlightedColor = new Color(0.35f, 0.5f, 0.65f, 1f); // Ещё светлее при наведении
        buttonColors.pressedColor = new Color(0.15f, 0.25f, 0.35f, 1f); // Темнее при нажатии
        buttonColors.selectedColor = new Color(0.4f, 0.5f, 0.3f, 1f); // Жёлто-зелёный когда выбран
        button.colors = buttonColors;

        // ЯРКАЯ ОБВОДКА
        Outline outline = itemCard.AddComponent<Outline>();
        outline.effectColor = new Color(1f, 0.85f, 0f, 1f); // ЗОЛОТАЯ обводка!
        outline.effectDistance = new Vector2(3, -3);

        // Add ShopItemCard component
        itemCard.AddComponent<ShopItemCard>();

        // Background (visible stripe for accent)
        GameObject background = new GameObject("Background");
        background.transform.SetParent(itemCard.transform, false);
        RectTransform bgRect = background.AddComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.sizeDelta = Vector2.zero;
        bgRect.anchoredPosition = Vector2.zero;
        Image bgImage = background.AddComponent<Image>();
        bgImage.color = new Color(1f, 1f, 1f, 0.1f); // Немного видимый белый overlay

        // Icon (ЯРКИЙ placeholder!)
        GameObject icon = new GameObject("Icon");
        icon.transform.SetParent(itemCard.transform, false);
        RectTransform iconRect = icon.AddComponent<RectTransform>();
        iconRect.anchorMin = new Vector2(0, 0.5f);
        iconRect.anchorMax = new Vector2(0, 0.5f);
        iconRect.pivot = new Vector2(0, 0.5f);
        iconRect.anchoredPosition = new Vector2(10, 0);
        iconRect.sizeDelta = new Vector2(80, 80);
        Image iconImage = icon.AddComponent<Image>();
        iconImage.color = new Color(0.8f, 0.8f, 0.8f, 1f); // ЯРКИЙ серый placeholder - ПОЛНОСТЬЮ НЕПРОЗРАЧНЫЙ!

        // Name - ЯРКИЙ БЕЛЫЙ ТЕКСТ
        GameObject nameText = new GameObject("NameText");
        nameText.transform.SetParent(itemCard.transform, false);
        RectTransform nameRect = nameText.AddComponent<RectTransform>();
        nameRect.anchorMin = new Vector2(0, 0.5f);
        nameRect.anchorMax = new Vector2(1, 1);
        nameRect.pivot = new Vector2(0, 1);
        nameRect.anchoredPosition = new Vector2(100, -5);
        nameRect.sizeDelta = new Vector2(-210, -10);
        TextMeshProUGUI nameTMP = nameText.AddComponent<TextMeshProUGUI>();
        nameTMP.text = "ITEM NAME";
        nameTMP.fontSize = 24;
        nameTMP.fontStyle = FontStyles.Bold;
        nameTMP.color = new Color(1f, 1f, 1f, 1f); // ЯРКИЙ БЕЛЫЙ
        nameTMP.alignment = TextAlignmentOptions.TopLeft;
        nameTMP.enableWordWrapping = false;
        nameTMP.overflowMode = TextOverflowModes.Ellipsis;

        // Добавим тень для лучшей читаемости
        Shadow nameShadow = nameText.AddComponent<Shadow>();
        nameShadow.effectColor = new Color(0, 0, 0, 0.8f);
        nameShadow.effectDistance = new Vector2(2, -2);

        // Price - ЯРКИЙ ЗОЛОТОЙ
        GameObject priceText = new GameObject("PriceText");
        priceText.transform.SetParent(itemCard.transform, false);
        RectTransform priceRect = priceText.AddComponent<RectTransform>();
        priceRect.anchorMin = new Vector2(0, 0);
        priceRect.anchorMax = new Vector2(1, 0.5f);
        priceRect.pivot = new Vector2(0, 0);
        priceRect.anchoredPosition = new Vector2(100, 5);
        priceRect.sizeDelta = new Vector2(-210, -10);
        TextMeshProUGUI priceTMP = priceText.AddComponent<TextMeshProUGUI>();
        priceTMP.text = "50 Scrap";
        priceTMP.fontSize = 20;
        priceTMP.fontStyle = FontStyles.Bold;
        priceTMP.color = new Color(1f, 0.9f, 0.2f, 1f); // ЯРКО-ЗОЛОТОЙ
        priceTMP.alignment = TextAlignmentOptions.BottomLeft;

        // Тень для цены
        Shadow priceShadow = priceText.AddComponent<Shadow>();
        priceShadow.effectColor = new Color(0, 0, 0, 0.8f);
        priceShadow.effectDistance = new Vector2(2, -2);

        // Stock - ЯРКИЙ СВЕТЛЫЙ
        GameObject stockText = new GameObject("StockText");
        stockText.transform.SetParent(itemCard.transform, false);
        RectTransform stockRect = stockText.AddComponent<RectTransform>();
        stockRect.anchorMin = new Vector2(1, 0.5f);
        stockRect.anchorMax = new Vector2(1, 0.5f);
        stockRect.pivot = new Vector2(1, 0.5f);
        stockRect.anchoredPosition = new Vector2(-10, 0);
        stockRect.sizeDelta = new Vector2(80, 40);
        TextMeshProUGUI stockTMP = stockText.AddComponent<TextMeshProUGUI>();
        stockTMP.text = "x5";
        stockTMP.fontSize = 20;
        stockTMP.fontStyle = FontStyles.Bold;
        stockTMP.alignment = TextAlignmentOptions.Center;
        stockTMP.color = new Color(0.9f, 0.9f, 0.9f, 1f); // ЯРКИЙ светло-серый

        // Add Layout Element for proper sizing in ScrollView
        LayoutElement layoutElement = itemCard.AddComponent<LayoutElement>();
        layoutElement.preferredHeight = 100;
        layoutElement.minHeight = 100;
        layoutElement.preferredWidth = 400; // CRITICAL: Set preferred width for ContentSizeFitter
        layoutElement.flexibleWidth = 1; // Allow stretching to fill container

        // Assign references to ShopItemCard
        ShopItemCard cardScript = itemCard.GetComponent<ShopItemCard>();
        SerializedObject so = new SerializedObject(cardScript);
        so.FindProperty("iconImage").objectReferenceValue = iconImage;
        so.FindProperty("nameText").objectReferenceValue = nameTMP;
        so.FindProperty("priceText").objectReferenceValue = priceTMP;
        so.FindProperty("stockText").objectReferenceValue = stockTMP;
        so.FindProperty("cardButton").objectReferenceValue = button;
        so.FindProperty("backgroundImage").objectReferenceValue = bgImage;
        so.ApplyModifiedProperties();

        // Save as prefab
        string prefabPath = "Assets/Prefabs/Shop/ItemCardPrefab.prefab";
        string directory = "Assets/Prefabs/Shop";

        if (!AssetDatabase.IsValidFolder(directory))
        {
            AssetDatabase.CreateFolder("Assets/Prefabs", "Shop");
        }

        PrefabUtility.SaveAsPrefabAsset(itemCard, prefabPath);
        DestroyImmediate(itemCard);

        Debug.Log($"✓ Created IMPROVED Item Card Prefab at: {prefabPath}");

        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        EditorGUIUtility.PingObject(prefab);

        EditorUtility.DisplayDialog("Success!",
            "SUPER-VISIBLE Item Card Prefab created!\n\n" +
            $"Saved to: {prefabPath}\n\n" +
            "Improvements:\n" +
            "- Dark blue background (fully opaque!)\n" +
            "- GOLD outline around card\n" +
            "- BRIGHT white text with shadow\n" +
            "- BRIGHT gold price with shadow\n" +
            "- Visible gray placeholder for icon\n" +
            "- Hover effects on mouse over\n" +
            "- Text shadows for readability\n" +
            "- Layout Element for ScrollView (Height: 100)\n\n" +
            "Cards are NO LONGER transparent!\n" +
            "Cards will now display properly in ScrollView!", "OK");
    }

    // ============================================
    // NEW METHODS FOR COMPLETE AUTOMATION
    // ============================================

    private void CreateCompleteShopSystem()
    {
        Debug.Log("[ShopUICreator] 🚀 Creating COMPLETE Shop System...");

        // 1. Create UI
        CreateCompleteShopUI();

        // Small delay to ensure UI is created
        EditorApplication.delayCall += () =>
        {
            // 2. Create ShopManager
            CreateShopManager();

            // 3. Create ShopTrigger
            CreateShopTrigger();

            Debug.Log("✅ [ShopUICreator] COMPLETE Shop System created!");
            EditorUtility.DisplayDialog("Success! 🎉",
                "Complete Shop System has been created!\n\n" +
                "Created:\n" +
                "✅ Shop UI (Canvas, panels, prefab)\n" +
                "✅ ShopManager (with auto-references)\n" +
                "✅ ShopTrigger (in scene)\n\n" +
                "Next steps:\n" +
                "1. Assign shop items to ShopManager\n" +
                "2. Assign Player to ShopManager\n" +
                "3. Position ShopTrigger in scene\n" +
                "4. Test in Play mode!", "Awesome!");
        };
    }

    private void CreateShopManager()
    {
        Debug.Log("[ShopUICreator] Creating ShopManager...");

        // Check if ShopManager already exists
        ShopManager existingManager = FindObjectOfType<ShopManager>();
        if (existingManager != null)
        {
            bool replace = EditorUtility.DisplayDialog("ShopManager Exists",
                "ShopManager already exists in the scene. Replace it?",
                "Replace", "Cancel");

            if (!replace) return;

            DestroyImmediate(existingManager.gameObject);
        }

        // Create ShopManager GameObject
        GameObject managerObj = new GameObject("ShopManager");
        ShopManager manager = managerObj.AddComponent<ShopManager>();

        // Auto-assign all references
        Debug.Log("[ShopUICreator] Auto-assigning references to ShopManager...");
        AutoAssignShopManagerReferences(manager);

        Selection.activeGameObject = managerObj;
        Debug.Log("✓ ShopManager created successfully with all references!");
    }

    private void CreateShopTrigger()
    {
        Debug.Log("[ShopUICreator] Creating ShopTrigger...");

        // Create trigger cube
        GameObject trigger = GameObject.CreatePrimitive(PrimitiveType.Cube);
        trigger.name = "ShopTrigger";

        // Position near origin (user can move it)
        trigger.transform.position = new Vector3(5, 0, 0);
        trigger.transform.localScale = new Vector3(3, 3, 3);

        // Setup collider
        BoxCollider collider = trigger.GetComponent<BoxCollider>();
        if (collider != null)
        {
            collider.isTrigger = true;
        }

        // Add ShopTrigger component
        ShopTrigger triggerScript = trigger.AddComponent<ShopTrigger>();

        // Try to assign ShopManager
        ShopManager manager = FindObjectOfType<ShopManager>();
        if (manager != null)
        {
            SerializedObject so = new SerializedObject(triggerScript);
            so.FindProperty("shopManager").objectReferenceValue = manager;
            so.ApplyModifiedProperties();
            Debug.Log("✓ ShopManager reference assigned automatically!");
        }
        else
        {
            Debug.LogWarning("⚠ ShopManager not found. Create it first or assign manually.");
        }

        // Add a visual indicator material (optional)
        Renderer renderer = trigger.GetComponent<Renderer>();
        if (renderer != null)
        {
            Material mat = new Material(Shader.Find("Standard"));
            mat.color = new Color(0.2f, 0.8f, 0.2f, 0.5f);
            mat.SetFloat("_Mode", 3); // Transparent mode
            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            mat.SetInt("_ZWrite", 0);
            mat.DisableKeyword("_ALPHATEST_ON");
            mat.EnableKeyword("_ALPHABLEND_ON");
            mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            mat.renderQueue = 3000;
            renderer.material = mat;
        }

        Selection.activeGameObject = trigger;
        Debug.Log("✓ ShopTrigger created! Position it where you want the shop to be.");
        EditorGUIUtility.PingObject(trigger);
    }

    // Helper methods
    private GameObject FindChildByName(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name)
                return child.gameObject;
        }
        return null;
    }

    private GameObject FindDeepChild(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name)
                return child.gameObject;

            GameObject result = FindDeepChild(child, name);
            if (result != null)
                return result;
        }
        return null;
    }

    private void AssignDetailPanelReferences(SerializedObject so, Transform container)
    {
        foreach (Transform child in container)
        {
            switch (child.name)
            {
                case "ItemName":
                    so.FindProperty("detailName").objectReferenceValue = child.GetComponent<TextMeshProUGUI>();
                    break;
                case "ItemDescription":
                    so.FindProperty("detailDescription").objectReferenceValue = child.GetComponent<TextMeshProUGUI>();
                    break;
                case "ItemPrice":
                    so.FindProperty("detailPrice").objectReferenceValue = child.GetComponent<TextMeshProUGUI>();
                    break;
                case "ItemStock":
                    so.FindProperty("detailStock").objectReferenceValue = child.GetComponent<TextMeshProUGUI>();
                    break;
                case "BuyButton":
                    so.FindProperty("buyButton").objectReferenceValue = child.GetComponent<Button>();
                    Transform buttonText = child.Find("Text");
                    if (buttonText != null)
                    {
                        so.FindProperty("buyButtonText").objectReferenceValue = buttonText.GetComponent<TextMeshProUGUI>();
                    }
                    break;
            }
        }
    }

    // ============================================
    // RECREATE METHODS
    // ============================================

    private void RecreateItemCardPrefab()
    {
        Debug.Log("[ShopUICreator] ===== RECREATING ITEM CARD PREFAB =====");

        // Delete existing prefab
        string prefabPath = "Assets/Prefabs/Shop/ItemCardPrefab.prefab";
        if (AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath) != null)
        {
            Debug.Log("[ShopUICreator] Deleting existing ItemCardPrefab...");
            AssetDatabase.DeleteAsset(prefabPath);
            Debug.Log("✓ Deleted old ItemCardPrefab");
        }

        AssetDatabase.Refresh();

        // Create new prefab
        Debug.Log("[ShopUICreator] Creating new ItemCardPrefab with LayoutElement...");
        CreateItemCardPrefab();

        EditorUtility.DisplayDialog("Success!",
            "ItemCardPrefab recreated!\n\n" +
            "New prefab includes:\n" +
            "- LayoutElement (Height: 100)\n" +
            "- All visual improvements\n" +
            "- Proper sizing for ScrollView\n\n" +
            "Don't forget to reassign it in ShopManager!", "OK");

        Debug.Log("[ShopUICreator] ===== RECREATION COMPLETE =====");
    }

    private void DeleteAndRecreateUI()
    {
        Debug.Log("[ShopUICreator] ===== DELETING OLD UI =====");

        // 1. Delete shop UI elements from PlayerUI/Canvas (but keep PlayerUI and Canvas)
        GameObject playerUI = GameObject.Find("PlayerUI");
        Canvas canvas = null;

        if (playerUI != null)
        {
            canvas = playerUI.GetComponentInChildren<Canvas>();
        }

        if (canvas == null)
        {
            // Search for any ScreenSpaceOverlay Canvas
            Canvas[] allCanvases = FindObjectsOfType<Canvas>();
            foreach (Canvas c in allCanvases)
            {
                if (c.renderMode == RenderMode.ScreenSpaceOverlay)
                {
                    canvas = c;
                    break;
                }
            }
        }

        if (canvas != null)
        {
            // Delete only ShopPanel and PromptPanel, keep Canvas
            GameObject shopPanel = FindDeepChild(canvas.transform, "ShopPanel");
            if (shopPanel != null)
            {
                Debug.Log("[ShopUICreator] Found ShopPanel - deleting it...");
                DestroyImmediate(shopPanel);
                Debug.Log("✓ Deleted ShopPanel");
            }

            GameObject promptPanel = FindDeepChild(canvas.transform, "PromptPanel");
            if (promptPanel != null)
            {
                Debug.Log("[ShopUICreator] Found PromptPanel - deleting it...");
                DestroyImmediate(promptPanel);
                Debug.Log("✓ Deleted PromptPanel");
            }
        }
        else
        {
            Debug.Log("[ShopUICreator] No Canvas found in scene");
        }

        // 2. Delete ItemCardPrefab
        string prefabPath = "Assets/Prefabs/Shop/ItemCardPrefab.prefab";
        if (AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath) != null)
        {
            Debug.Log("[ShopUICreator] Found ItemCardPrefab - deleting it...");
            AssetDatabase.DeleteAsset(prefabPath);
            Debug.Log("✓ Deleted ItemCardPrefab");
        }
        else
        {
            Debug.Log("[ShopUICreator] ItemCardPrefab not found");
        }

        AssetDatabase.Refresh();

        Debug.Log("[ShopUICreator] ===== CREATING NEW UI =====");

        // 3. Create fresh UI
        CreateCompleteShopUI();

        // CRITICAL: Save and refresh assets BEFORE auto-assigning
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("[ShopUICreator] Assets saved and refreshed");

        // 4. Auto-assign references to ShopManager
        ShopManager shopManager = FindObjectOfType<ShopManager>();
        if (shopManager != null)
        {
            Debug.Log("[ShopUICreator] Found existing ShopManager - auto-assigning references...");
            AutoAssignShopManagerReferences(shopManager);
            Debug.Log("✓ Auto-assigned all UI references to ShopManager!");
        }
        else
        {
            Debug.Log("[ShopUICreator] No ShopManager found - you'll need to create one and assign references manually");
        }

        EditorUtility.DisplayDialog("Success!",
            "Old UI deleted and NEW UI created!\n\n" +
            "Created:\n" +
            "- Fresh Canvas with all panels\n" +
            "- New ItemCardPrefab with LayoutElement\n" +
            "- All components properly configured\n" +
            (shopManager != null ? "- Auto-assigned references to ShopManager!\n" : "") +
            "\n" +
            "Next: Add ShopItems to Available Items list!", "OK");

        Debug.Log("[ShopUICreator] ===== RECREATION COMPLETE =====");
    }

    // ============================================
    // AUTO-ASSIGN REFERENCES
    // ============================================

    private void AutoAssignShopManagerReferences(ShopManager shopManager)
    {
        SerializedObject so = new SerializedObject(shopManager);

        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            Debug.LogWarning("[ShopUICreator] No Canvas found!");
            return;
        }

        // Find all UI elements
        GameObject shopPanel = FindDeepChild(canvas.transform, "ShopPanel");
        GameObject promptPanel = FindDeepChild(canvas.transform, "PromptPanel");
        GameObject detailPanel = shopPanel != null ? FindDeepChild(shopPanel.transform, "DetailPanel") : null;
        GameObject itemScrollView = shopPanel != null ? FindDeepChild(shopPanel.transform, "ItemScrollView") : null;

        // Shop UI
        if (shopPanel != null)
        {
            so.FindProperty("shopUI").objectReferenceValue = shopPanel;
            Debug.Log("  ✓ Assigned Shop UI");
        }

        // Prompt UI
        if (promptPanel != null)
        {
            so.FindProperty("promptUI").objectReferenceValue = promptPanel;

            TextMeshProUGUI promptText = promptPanel.GetComponentInChildren<TextMeshProUGUI>();
            if (promptText != null)
            {
                so.FindProperty("promptText").objectReferenceValue = promptText;
            }
            Debug.Log("  ✓ Assigned Prompt UI");
        }

        // Item Cards Container
        if (itemScrollView != null)
        {
            GameObject viewport = FindDeepChild(itemScrollView.transform, "Viewport");
            if (viewport != null)
            {
                GameObject content = FindDeepChild(viewport.transform, "Content");
                if (content != null)
                {
                    so.FindProperty("itemCardsContainer").objectReferenceValue = content.GetComponent<RectTransform>();
                    Debug.Log("  ✓ Assigned Item Cards Container");
                }
            }
        }

        // Item Card Prefab
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Shop/ItemCardPrefab.prefab");
        if (prefab != null)
        {
            so.FindProperty("itemCardPrefab").objectReferenceValue = prefab;
            Debug.Log("  ✓ Assigned Item Card Prefab");
        }

        // Detail Panel
        if (detailPanel != null)
        {
            so.FindProperty("detailPanel").objectReferenceValue = detailPanel;

            Transform container = detailPanel.transform.Find("Container");
            if (container != null)
            {
                AssignDetailPanelReferences(so, container);
                Debug.Log("  ✓ Assigned Detail Panel references");
            }
        }

        // Currency Text
        if (shopPanel != null)
        {
            GameObject header = FindDeepChild(shopPanel.transform, "Header");
            if (header != null)
            {
                GameObject currencyText = FindDeepChild(header.transform, "CurrencyText");
                if (currencyText != null)
                {
                    TextMeshProUGUI currencyTMP = currencyText.GetComponent<TextMeshProUGUI>();
                    if (currencyTMP != null)
                    {
                        so.FindProperty("currencyText").objectReferenceValue = currencyTMP;
                        Debug.Log("  ✓ Assigned Currency Text");
                    }
                }
            }
        }

        // Player (try to find by tag)
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            so.FindProperty("player").objectReferenceValue = player;
            Debug.Log("  ✓ Assigned Player");
        }
        else
        {
            Debug.LogWarning("  ⚠ Player not found (tag 'Player'). Assign manually!");
        }

        so.ApplyModifiedProperties();
        EditorUtility.SetDirty(shopManager);
    }
}
#endif
