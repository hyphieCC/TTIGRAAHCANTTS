using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RoomSlotUI : MonoBehaviour
{
    public Image roomSprite;
    public TextMeshProUGUI roomNameText;
    public Image highlightBorder;

    [HideInInspector] public RoomDefinition roomData;

    public void Setup(RoomDefinition data)
    {
        roomData = data;
        roomSprite.sprite = data.spriteFromUp ?? data.spriteFromDown;
        roomNameText.text = data.roomName;
        SetHighlight(false);
    }

    public void SetHighlight(bool active)
    {
        if (highlightBorder != null)
            highlightBorder.enabled = active;
    }
}