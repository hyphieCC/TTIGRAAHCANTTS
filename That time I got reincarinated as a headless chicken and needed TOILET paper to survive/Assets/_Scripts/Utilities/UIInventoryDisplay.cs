using UnityEngine;
using TMPro;
using Systems;

public class UIInventoryDisplay : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI coinsText;
    public TextMeshProUGUI gemsText;
    public TextMeshProUGUI staminaText;

    private PlayerSystem player;

    void Start()
    {
        player = FindFirstObjectByType<PlayerSystem>();
    }

    void Update()
    {
        if (player == null) return;
        var inv = player.inventory;

        coinsText.text = $"Seeds: {inv.coins}";
        gemsText.text = $"Pebbles: {inv.gems}";
        staminaText.text = $"Stamina: {inv.stamina}";
    }
}