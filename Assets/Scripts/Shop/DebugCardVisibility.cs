using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Debug tool to check card visibility in ScrollView
/// Add this to the Content object to see real-time debug info
/// </summary>
public class DebugCardVisibility : MonoBehaviour
{
    [Header("Debug Settings")]
    [SerializeField] private bool showDebugInfo = true;
    [SerializeField] private Color debugBorderColor = Color.red;

    private void OnEnable()
    {
        if (showDebugInfo)
        {
            Debug.Log($"[DebugCardVisibility] Content enabled. Checking children...");
            CheckChildren();
        }
    }

    private void Update()
    {
        if (showDebugInfo && Input.GetKeyDown(KeyCode.F1))
        {
            Debug.Log($"[DebugCardVisibility] ===== MANUAL CHECK (F1) =====");
            CheckChildren();
            CheckContentSize();
        }
    }

    private void CheckChildren()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();

        Debug.Log($"[DebugCardVisibility] Content RectTransform:");
        Debug.Log($"  - Size: {rectTransform.rect.size}");
        Debug.Log($"  - Position: {rectTransform.anchoredPosition}");
        Debug.Log($"  - Child count: {transform.childCount}");

        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            RectTransform childRect = child.GetComponent<RectTransform>();
            Image childImage = child.GetComponent<Image>();
            CanvasGroup childGroup = child.GetComponent<CanvasGroup>();

            Debug.Log($"[DebugCardVisibility] Child {i}: {child.name}");
            Debug.Log($"  - Active: {child.gameObject.activeSelf}");
            Debug.Log($"  - Size: {childRect.rect.size}");
            Debug.Log($"  - Position: {childRect.anchoredPosition}");
            Debug.Log($"  - Scale: {childRect.localScale}");

            if (childImage != null)
            {
                Debug.Log($"  - Image Color: {childImage.color}");
                Debug.Log($"  - Image Enabled: {childImage.enabled}");
                Debug.Log($"  - Image Sprite: {(childImage.sprite != null ? childImage.sprite.name : "NULL")}");
            }
            else
            {
                Debug.LogWarning($"  - NO IMAGE COMPONENT!");
            }

            if (childGroup != null)
            {
                Debug.Log($"  - CanvasGroup Alpha: {childGroup.alpha}");
            }
        }
    }

    private void CheckContentSize()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        VerticalLayoutGroup layoutGroup = GetComponent<VerticalLayoutGroup>();
        ContentSizeFitter sizeFitter = GetComponent<ContentSizeFitter>();

        Debug.Log($"[DebugCardVisibility] Layout Components:");

        if (layoutGroup != null)
        {
            Debug.Log($"  - VerticalLayoutGroup:");
            Debug.Log($"    - Spacing: {layoutGroup.spacing}");
            Debug.Log($"    - Padding: {layoutGroup.padding.top}, {layoutGroup.padding.right}, {layoutGroup.padding.bottom}, {layoutGroup.padding.left}");
            Debug.Log($"    - Child Control Width: {layoutGroup.childControlWidth}");
            Debug.Log($"    - Child Control Height: {layoutGroup.childControlHeight}");
        }
        else
        {
            Debug.LogWarning("  - NO VerticalLayoutGroup!");
        }

        if (sizeFitter != null)
        {
            Debug.Log($"  - ContentSizeFitter:");
            Debug.Log($"    - Horizontal Fit: {sizeFitter.horizontalFit}");
            Debug.Log($"    - Vertical Fit: {sizeFitter.verticalFit}");
        }
        else
        {
            Debug.LogWarning("  - NO ContentSizeFitter!");
        }
    }

    private void OnDrawGizmos()
    {
        if (!showDebugInfo) return;

        // Draw border around Content
        RectTransform rectTransform = GetComponent<RectTransform>();
        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);

        Gizmos.color = debugBorderColor;
        for (int i = 0; i < 4; i++)
        {
            Gizmos.DrawLine(corners[i], corners[(i + 1) % 4]);
        }

        // Draw borders around children
        Gizmos.color = Color.yellow;
        for (int i = 0; i < transform.childCount; i++)
        {
            RectTransform childRect = transform.GetChild(i).GetComponent<RectTransform>();
            if (childRect != null)
            {
                Vector3[] childCorners = new Vector3[4];
                childRect.GetWorldCorners(childCorners);

                for (int j = 0; j < 4; j++)
                {
                    Gizmos.DrawLine(childCorners[j], childCorners[(j + 1) % 4]);
                }
            }
        }
    }
}
