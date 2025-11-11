using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Triggers shop interaction when player enters the zone
/// </summary>
[RequireComponent(typeof(Collider))]
public class ShopTrigger : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ShopManager shopManager;

    [Header("Settings")]
    [SerializeField] private bool pauseGameWhenOpen = true;

    private bool playerInZone = false;
    private bool shopOpen = false;

    private void Start()
    {
        // Ensure trigger is set
        GetComponent<Collider>().isTrigger = true;

        if (shopManager == null)
        {
            shopManager = FindObjectOfType<ShopManager>();
        }
    }

    private void Update()
    {
        if (playerInZone && !shopOpen)
        {
            // Check for E key press (or Input System Interact action)
            if (Input.GetKeyDown(KeyCode.E) || Keyboard.current?.eKey.wasPressedThisFrame == true)
            {
                OpenShop();
            }
        }
        else if (shopOpen)
        {
            // Check for Esc or E to close
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.E) ||
                Keyboard.current?.escapeKey.wasPressedThisFrame == true ||
                Keyboard.current?.eKey.wasPressedThisFrame == true)
            {
                CloseShop();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInZone = true;

            if (shopManager != null)
            {
                shopManager.ShowPrompt(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInZone = false;

            if (shopManager != null)
            {
                shopManager.ShowPrompt(false);
            }

            // Auto-close shop if player leaves
            if (shopOpen)
            {
                CloseShop();
            }
        }
    }

    private void OpenShop()
    {
        shopOpen = true;

        if (shopManager != null)
        {
            shopManager.OpenShop();
        }

        if (pauseGameWhenOpen)
        {
            Time.timeScale = 0f;
        }

        // Unlock cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void CloseShop()
    {
        shopOpen = false;

        if (shopManager != null)
        {
            shopManager.CloseShop();
        }

        if (pauseGameWhenOpen)
        {
            Time.timeScale = 1f;
        }

        // Lock cursor back
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, GetComponent<Collider>().bounds.size);
    }
}
