using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScreenDimmer : MonoBehaviour
{
    public static ScreenDimmer Instance;
    public Image dimmerImage;

    void Awake()
    {
        Instance = this;
        dimmerImage.color = new Color(0, 0, 0, 0);
    }

    public void FadeIn(float targetAlpha = 0.65f, float duration = 0.3f)
    {
        gameObject.SetActive(true);
        StartCoroutine(FadeRoutine(targetAlpha, duration));
    }

    public void FadeOut(float duration = 0.3f)
    {
        StartCoroutine(FadeRoutine(0f, duration));
    }

    IEnumerator FadeRoutine(float targetAlpha, float duration)
    {
        float start = dimmerImage.color.a;
        float time = 0f;
        while (time < duration)
        {
            time += Time.unscaledDeltaTime;
            float a = Mathf.Lerp(start, targetAlpha, time / duration);
            dimmerImage.color = new Color(0, 0, 0, a);
            yield return null;
        }
        dimmerImage.color = new Color(0, 0, 0, targetAlpha);
        if (targetAlpha == 0) gameObject.SetActive(false);
    }
}