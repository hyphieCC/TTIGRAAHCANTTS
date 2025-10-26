using UnityEngine;
using TMPro;

public class RewardPopupManager : MonoBehaviour
{
    public static RewardPopupManager Instance;
    public GameObject popupPrefab;
    public Canvas parentCanvas;

    void Awake()
    {
        Instance = this;
    }

    public void ShowPopup(string message, Color color, Vector3 worldPos)
    {
        if (popupPrefab == null || parentCanvas == null) return;

        // Convert world position to screen position
        Vector2 screenPos = Camera.main.WorldToScreenPoint(worldPos);
        var popupObj = Instantiate(popupPrefab, parentCanvas.transform);
        popupObj.GetComponent<RectTransform>().position = screenPos;

        var popup = popupObj.GetComponent<RewardPopup>();
        popup.Setup(message, color);
    }
}