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
        // CRITICAL: Set anchors for VerticalLayoutGroup compatibility
        cardRect.anchorMin = new Vector2(0, 1);
        cardRect.anchorMax = new Vector2(1, 1);
        cardRect.pivot = new Vector2(0.5f, 1f);
        cardRect.sizeDelta = new Vector2(0, 100); // Width will stretch, height fixed at 100

        Image cardImage = itemCard.AddComponent<Image>();
        cardImage.color = new Color(0.2f, 0.2f, 0.2f, 1f);
        // Use built-in sprite for background
        cardImage.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Background.psd");
        cardImage.type = Image.Type.Sliced;

        Button button = itemCard.AddComponent<Button>();

        // Add ShopItemCard component
        itemCard.AddComponent<ShopItemCard>();

        // Background (border/outline effect)
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
        iconRect.sizeDelta = new Vector2(80, 80);
        Image iconImage = icon.AddComponent<Image>();
        iconImage.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
        iconImage.color = new Color(0.5f, 0.5f, 0.5f, 1f); // Gray placeholder

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

        // Add LayoutElement for proper sizing in VerticalLayoutGroup
        LayoutElement layoutElement = itemCard.AddComponent<LayoutElement>();
        layoutElement.minHeight = 100;
        layoutElement.preferredHeight = 100;
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

        Debug.Log($"✓ Created Item Card Prefab at: {prefabPath}");

        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        EditorGUIUtility.PingObject(prefab);
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

        // Find UI elements
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas != null)
        {
            // Try to find and assign UI references
            GameObject shopPanel = FindChildByName(canvas.transform, "ShopPanel");
            GameObject promptPanel = FindChildByName(canvas.transform, "PromptPanel");

            if (shopPanel != null && promptPanel != null)
            {
                SerializedObject so = new SerializedObject(manager);

                // Shop UI
                so.FindProperty("shopUI").objectReferenceValue = shopPanel;

                // Prompt UI
                so.FindProperty("promptUI").objectReferenceValue = promptPanel;
                GameObject promptText = FindChildByName(promptPanel.transform, "PromptText");
                if (promptText != null)
                {
                    so.FindProperty("promptText").objectReferenceValue = promptText.GetComponent<TextMeshProUGUI>();
                }

                // Item Cards Container
                GameObject content = FindDeepChild(shopPanel.transform, "Content");
                if (content != null)
                {
                    so.FindProperty("itemCardsContainer").objectReferenceValue = content.transform;
                }

                // Item Card Prefab
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Shop/ItemCardPrefab.prefab");
                if (prefab != null)
                {
                    so.FindProperty("itemCardPrefab").objectReferenceValue = prefab;
                }

                // Detail Panel
                GameObject detailPanel = FindChildByName(shopPanel.transform, "DetailPanel");
                if (detailPanel != null)
                {
                    so.FindProperty("detailPanel").objectReferenceValue = detailPanel;

                    GameObject container = FindChildByName(detailPanel.transform, "Container");
                    if (container != null)
                    {
                        AssignDetailPanelReferences(so, container.transform);
                    }
                }

                // Currency Text
                GameObject currencyText = FindDeepChild(shopPanel.transform, "CurrencyText");
                if (currencyText != null)
                {
                    so.FindProperty("currencyText").objectReferenceValue = currencyText.GetComponent<TextMeshProUGUI>();
                }

                so.ApplyModifiedProperties();

                Debug.Log("✓ ShopManager created and UI references assigned automatically!");
            }
            else
            {
                Debug.LogWarning("⚠ UI elements not found. Create UI first or assign references manually.");
            }
        }
        else
        {
            Debug.LogWarning("⚠ Canvas not found. Create UI first.");
        }

        // Try to find and assign player
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            SerializedObject so = new SerializedObject(manager);
            so.FindProperty("player").objectReferenceValue = player;
            so.ApplyModifiedProperties();
            Debug.Log($"✓ Player assigned automatically: {player.name}");
        }
        else
        {
            Debug.LogWarning("⚠ Player not found (missing 'Player' tag). Assign manually.");
        }

        Selection.activeGameObject = managerObj;
        Debug.Log("✓ ShopManager created successfully!");
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
}
#endif
