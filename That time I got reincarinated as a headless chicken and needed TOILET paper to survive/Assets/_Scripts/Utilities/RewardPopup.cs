using UnityEngine;
using TMPro;

public class RewardPopup : MonoBehaviour
{
    public TextMeshProUGUI text;
    public float floatSpeed = 50f;
    public float fadeSpeed = 2f;
    private CanvasGroup canvasGroup;
    private RectTransform rect;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        canvasGroup = gameObject.AddComponent<CanvasGroup>();
        if (text == null) text = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        rect.anchoredPosition += Vector2.up * floatSpeed * Time.deltaTime;
        canvasGroup.alpha -= fadeSpeed * Time.deltaTime;
        if (canvasGroup.alpha <= 0)
            Destroy(gameObject);
    }

    public void Setup(string message, Color color)
    {
        text.text = message;
        text.color = color;
    }
}