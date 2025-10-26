using UnityEngine;
using TMPro;
using Managers;
using Systems;
using System.Collections;

public class ShopUI : MonoBehaviour
{
    public static ShopUI Instance;

    [Header("References")]
    [SerializeField] private GameObject shopPrompt;
    [SerializeField] private GameObject shopScreen;
    [SerializeField] private TextMeshProUGUI shopPromptText;
    [SerializeField] private Transform playerTransform;

    [Header("Prompt Settings")]
    public Vector3 promptScreenPosition = new Vector3(960, 300, 0);
    public float popInDistance = 50f;
    public float popInDuration = 0.25f;

    private PlayerSystem player;
    private bool isShopOpen = false;
    private bool hasPoppedIn = false;

    void Awake()
    {
        Instance = this;

        if (shopPrompt == null) Debug.LogError("[ShopUI] shopPrompt not assigned!");
        if (shopScreen == null) Debug.LogError("[ShopUI] shopScreen not assigned!");
        if (shopPromptText == null) Debug.LogError("[ShopUI] shopPromptText not assigned!");

        shopPrompt?.SetActive(false);
        shopScreen?.SetActive(false);
    }

    void Start()
    {
        player = FindFirstObjectByType<PlayerSystem>();
        if (player != null)
            playerTransform = player.transform;

        Debug.Log($"[ShopUI] Initialized. Player found={player != null}");
    }

    void Update()
    {
        if (player == null)
        {
            player = FindFirstObjectByType<PlayerSystem>();
            return;
        }

        // Debug log the keypress
        if (Input.GetKeyDown(KeyCode.F))
            Debug.Log($"[ShopUI] F pressed — PromptActive={shopPrompt.activeSelf}, ShopOpen={isShopOpen}");

        // Open shop only when prompt is visible and not already open
        if (shopPrompt.activeSelf && !isShopOpen && Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log("[ShopUI] F pressed to OPEN shop.");
            OpenShop();
        }

        // Close shop only when open
        if (isShopOpen && (Input.GetKeyDown(KeyCode.Escape)))
        {
            Debug.Log("[ShopUI] F pressed to CLOSE shop.");
            CloseShop();
        }

        if (!isShopOpen) return;

        if (Input.GetKeyDown(KeyCode.I))
        {
            Debug.Log("[ShopUI] Trade: Coins to Gems");
            TradeCoinsForGems();
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            Debug.Log("[ShopUI] Trade: Coins to Stamina");
            TradeCoinsForStamina();
        }
    }

    // SHOP PROMPT HANDLING
    public void ShowPrompt()
    {
        Debug.Log("[ShopUI] ShowPrompt() called.");
        shopPrompt?.SetActive(true);
        shopPromptText.text = "SHOP 'F'";
        hasPoppedIn = false;

        shopPrompt.transform.position = promptScreenPosition;
        StartCoroutine(PopInPrompt());
    }

    public void HidePrompt()
    {
        Debug.Log("[ShopUI] HidePrompt() called.");
        shopPrompt?.SetActive(false);
    }

    private IEnumerator PopInPrompt()
    {
        hasPoppedIn = true;

        RectTransform rect = shopPrompt.GetComponent<RectTransform>();
        Vector3 startPos = promptScreenPosition + new Vector3(0, -popInDistance, 0);
        Vector3 endPos = promptScreenPosition;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.unscaledDeltaTime / popInDuration;
            float eased = Mathf.Sin(t * Mathf.PI * 0.5f);
            rect.position = Vector3.Lerp(startPos, endPos, eased);
            yield return null;
        }

        rect.position = endPos;
        Debug.Log("[ShopUI] Prompt pop-in animation completed.");
    }

    // SHOP OPEN / CLOSE
    private void OpenShop()
    {
        Debug.Log($"[ShopUI] OpenShop() called. Before changes: ShopPrompt={shopPrompt?.activeSelf}, ShopScreen={shopScreen?.activeSelf}");

        shopPrompt?.SetActive(false);
        isShopOpen = true;

        if (shopScreen != null)
        {
            shopScreen.SetActive(true);
            Debug.Log($"[ShopUI] ShopScreen.SetActive(true) executed. ActiveNow={shopScreen.activeSelf}");
        }
        else
        {
            Debug.LogError("[ShopUI] ShopScreen is NULL!");
        }

        if (ScreenDimmer.Instance != null)
        {
            Debug.Log("[ShopUI] Triggering dimmer fade-in...");
            ScreenDimmer.Instance.FadeIn(0.65f, 0.3f);
        }
        else
        {
            Debug.LogWarning("[ShopUI] ScreenDimmer.Instance missing!");
        }

        Time.timeScale = 0f;
    }

    private void CloseShop()
    {
        Debug.Log($"[ShopUI] CloseShop() called. ShopPrompt={shopPrompt?.activeSelf}, ShopScreen={shopScreen?.activeSelf}");

        isShopOpen = false;

        if (shopScreen != null)
        {
            shopScreen.SetActive(false);
            Debug.Log("[ShopUI] ShopScreen deactivated.");
        }

        if (ScreenDimmer.Instance != null)
        {
            Debug.Log("[ShopUI] Triggering dimmer fade-out...");
            ScreenDimmer.Instance.FadeOut(0.3f);
        }

        Time.timeScale = 1f;
        shopPrompt?.SetActive(true);

        Debug.Log("[ShopUI] Shop closed successfully.");
    }


    // TRADE LOGIC
    private void TradeCoinsForGems()
    {
        if (player.inventory.coins >= 10)
        {
            player.inventory.coins -= 10;
            player.inventory.gems += 1;
            Debug.Log("[ShopUI] Traded 10 Coins for 1 Gem.");
        }
        else
        {
            Debug.Log("[ShopUI] Not enough coins for Gem trade.");
        }
    }

    private void TradeCoinsForStamina()
    {
        if (player.inventory.coins >= 5)
        {
            player.inventory.coins -= 5;
            player.inventory.AddStamina(10);
            Debug.Log("[ShopUI] Traded 5 Coins for 10 Stamina.");
        }
        else
        {
            Debug.Log("[ShopUI] Not enough coins for Stamina trade.");
        }
    }
}
