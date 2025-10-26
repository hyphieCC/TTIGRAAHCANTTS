using UnityEngine;
using TMPro;
using Managers;

public class FloorDisplayUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI floorText;

    private void Start()
    {
        if (floorText == null)
            floorText = GetComponent<TextMeshProUGUI>();

        UpdateFloorText(GameManager.Instance.currentFloor);
    }

    private void OnEnable()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnFloorChanged += UpdateFloorText;
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnFloorChanged -= UpdateFloorText;
    }

    private void UpdateFloorText(int floorNumber)
    {
        if (floorText != null)
            floorText.text = $"Floor {floorNumber}";
    }
}