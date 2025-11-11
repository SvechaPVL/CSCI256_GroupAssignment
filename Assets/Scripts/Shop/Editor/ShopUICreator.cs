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
        GUILayout.Label("Shop UI Creator", EditorStyles.boldLabel);
        GUILayout.Space(10);

        EditorGUILayout.HelpBox("This will create complete Shop UI structure in your scene:\n" +
            "- Canvas\n" +
            "- ShopPanel with all elements\n" +
            "- PromptPanel\n" +
            "- Item Card Prefab\n" +
            "- Detail Panel", MessageType.Info);

        GUILayout.Space(10);

        if (GUILayout.Button("CREATE COMPLETE SHOP UI", GUILayout.Height(40)))
        {
            CreateCompleteShopUI();
        }

        GUILayout.Space(10);

        if (GUILayout.Button("Create Only Item Card Prefab", GUILayout.Height(30)))
        {
            CreateItemCardPrefab();
        }
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

        // 1. Create or find Canvas
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("Canvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
            Debug.Log("✓ Created Canvas");

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
            Debug.Log("✓ Using existing Canvas");
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
        EditorUtility.DisplayDialog("Success",
            "Shop UI has been created!\n\n" +
            "Created:\n" +
            "- Canvas with ShopPanel\n" +
            "- PromptPanel\n" +
            "- DetailPanel\n" +
            "- Item Card Prefab\n\n" +
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

        Image shopImage = shopPanel.AddComponent<Image>();
        shopImage.color = new Color(0, 0, 0, 0.8f);

        // Deactivate by default
        shopPanel.SetActive(false);

        // Header panel
        GameObject header = new GameObject("Header");
        header.transform.SetParent(shopPanel.transform, false);
        RectTransform headerRect = header.AddComponent<RectTransform>();
        headerRect.anchorMin = new Vector2(0, 1);
        headerRect.anchorMax = new Vector2(1, 1);
        headerRect.pivot = new Vector2(0.5f, 1);
        headerRect.anchoredPosition = new Vector2(0, 0);
        headerRect.sizeDelta = new Vector2(0, 100);

        // Title text
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

        // Currency text
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
        scrollImage.color = new Color(0.1f, 0.1f, 0.1f, 0.8f);

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
        layout.childControlHeight = false;
        layout.childControlWidth = true;
        layout.childForceExpandHeight = false;
        layout.childForceExpandWidth = true;

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
        detailImage.color = new Color(0.15f, 0.15f, 0.15f, 0.9f);

        // Deactivate by default
        detailPanel.SetActive(false);

        // Detail elements container
        GameObject container = new GameObject("Container");
        container.transform.SetParent(detailPanel.transform, false);
        RectTransform containerRect = container.AddComponent<RectTransform>();
        containerRect.anchorMin = Vector2.zero;
        containerRect.anchorMax = Vector2.one;
        containerRect.sizeDelta = new Vector2(-40, -40);

        VerticalLayoutGroup layout = container.AddComponent<VerticalLayoutGroup>();
        layout.spacing = 20;
        layout.padding = new RectOffset(20, 20, 20, 20);
        layout.childControlHeight = false;
        layout.childForceExpandHeight = false;

        // Item Name
        CreateDetailText(container.transform, "ItemName", "Item Name", 32, TextAlignmentOptions.Center);

        // Item Description
        GameObject desc = CreateDetailText(container.transform, "ItemDescription", "Description text here...", 20, TextAlignmentOptions.TopLeft);
        LayoutElement descLayout = desc.AddComponent<LayoutElement>();
        descLayout.preferredHeight = 200;

        // Item Price
        CreateDetailText(container.transform, "ItemPrice", "Price: 0 Scrap", 24, TextAlignmentOptions.Center);

        // Item Stock
        CreateDetailText(container.transform, "ItemStock", "Stock: 0", 24, TextAlignmentOptions.Center);

        // Buy Button
        GameObject buyButton = new GameObject("BuyButton");
        buyButton.transform.SetParent(container.transform, false);
        RectTransform buyRect = buyButton.AddComponent<RectTransform>();
        buyRect.sizeDelta = new Vector2(0, 60);

        Image buyImage = buyButton.AddComponent<Image>();
        buyImage.color = new Color(0.2f, 0.8f, 0.2f, 1f);

        Button button = buyButton.AddComponent<Button>();

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

        Debug.Log("✓ Created Detail Panel");
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
        cardRect.sizeDelta = new Vector2(400, 100);

        Image cardImage = itemCard.AddComponent<Image>();
        cardImage.color = new Color(0.2f, 0.2f, 0.2f, 1f);

        Button button = itemCard.AddComponent<Button>();

        // Add ShopItemCard component
        itemCard.AddComponent<ShopItemCard>();

        // Background
        GameObject background = new GameObject("Background");
        background.transform.SetParent(itemCard.transform, false);
        RectTransform bgRect = background.AddComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.sizeDelta = Vector2.zero;
        Image bgImage = background.AddComponent<Image>();
        bgImage.color = new Color(1, 1, 1, 0.1f);

        // Icon
        GameObject icon = new GameObject("Icon");
        icon.transform.SetParent(itemCard.transform, false);
        RectTransform iconRect = icon.AddComponent<RectTransform>();
        iconRect.anchorMin = new Vector2(0, 0.5f);
        iconRect.anchorMax = new Vector2(0, 0.5f);
        iconRect.pivot = new Vector2(0, 0.5f);
        iconRect.anchoredPosition = new Vector2(10, 0);
        iconRect.sizeDelta = new Vector2(80, 80);
        Image iconImage = icon.AddComponent<Image>();
        iconImage.color = Color.white;

        // Name
        GameObject nameText = new GameObject("NameText");
        nameText.transform.SetParent(itemCard.transform, false);
        RectTransform nameRect = nameText.AddComponent<RectTransform>();
        nameRect.anchorMin = new Vector2(0, 0.5f);
        nameRect.anchorMax = new Vector2(1, 1);
        nameRect.anchoredPosition = new Vector2(100, -10);
        nameRect.sizeDelta = new Vector2(-220, -20);
        TextMeshProUGUI nameTMP = nameText.AddComponent<TextMeshProUGUI>();
        nameTMP.text = "Item Name";
        nameTMP.fontSize = 20;
        nameTMP.color = Color.white;

        // Price
        GameObject priceText = new GameObject("PriceText");
        priceText.transform.SetParent(itemCard.transform, false);
        RectTransform priceRect = priceText.AddComponent<RectTransform>();
        priceRect.anchorMin = new Vector2(0, 0);
        priceRect.anchorMax = new Vector2(1, 0.5f);
        priceRect.anchoredPosition = new Vector2(100, 10);
        priceRect.sizeDelta = new Vector2(-220, -20);
        TextMeshProUGUI priceTMP = priceText.AddComponent<TextMeshProUGUI>();
        priceTMP.text = "0 Scrap";
        priceTMP.fontSize = 18;
        priceTMP.color = Color.yellow;

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
        stockTMP.fontSize = 16;
        stockTMP.alignment = TextAlignmentOptions.Center;
        stockTMP.color = Color.gray;

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

        Debug.Log($"✓ Created Item Card Prefab at: {prefabPath}");

        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        EditorGUIUtility.PingObject(prefab);
    }
}
#endif
